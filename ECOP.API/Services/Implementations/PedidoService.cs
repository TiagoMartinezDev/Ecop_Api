using ECOP.API.DTOs;
using ECOP.API.Services.Interfaces;

namespace ECOP.API.Services.Implementations;
public class PedidoService(
    IPedidoRepositorio       pedidoRepo,
    IClienteRepositorio      clienteRepo,
    IProductoRepositorio     productoRepo,
    IEstadoPedidoRepositorio estadoRepo,
    IMapper                  mapper) : IPedidoServicio
{
    public async Task<IEnumerable<PedidoDto>> ObtenerTodosAsync(int? clienteId, string? estadoCodigo, CancellationToken ct = default)
    {
        var pedidos = await pedidoRepo.ObtenerTodosAsync(clienteId, estadoCodigo, ct);
        return mapper.Map<IEnumerable<PedidoDto>>(pedidos);
    }

    public async Task<PedidoDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        var pedido = await pedidoRepo.ObtenerPorIdAsync(id, ct);
        if(pedido is null) 
            return null;

        return mapper.Map<PedidoDto>(pedido);
    }

    public async Task<PedidoDto?> ObtenerPorNumeroAsync(string numeroPedido, CancellationToken ct = default)
    {
        var pedido = await pedidoRepo.ObtenerPorNumeroAsync(numeroPedido, ct);
        if(pedido is null) 
            return null;

        return mapper.Map<PedidoDto>(pedido);
    }

    public async Task<IEnumerable<EstadoPedidoDto>> ObtenerEstadosAsync(CancellationToken ct = default)
    {
        var estados = await estadoRepo.ObtenerTodosAsync(ct);
        return mapper.Map<IEnumerable<EstadoPedidoDto>>(estados);
    }

    public async Task<(PedidoDto? Datos, string? Error, List<string>? Errores)> CrearAsync(
        PedidoCrearDto dto, CancellationToken ct = default)
    {
        var cliente = await clienteRepo.ObtenerPorIdAsync(dto.ClienteId, ct);
        if (cliente is null || !cliente.Activo)
            return (null, $"Cliente con Id {dto.ClienteId} no encontrado.", null);

        if (dto.Detalles == null || dto.Detalles.Count <= 0)
            return (null, "El pedido debe tener al menos un producto.", null);

        //Validar productos duplicados en el mismo pedido
        var duplicados = dto.Detalles
            .GroupBy(d => d.ProductoId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicados.Count > 0)
            return (null, $"Productos duplicados en el pedido: {string.Join(", ", duplicados)}.", null);

        //Validar productos
        var productos = (await productoRepo.ObtenerPorIdsAsync(dto.Detalles.Select(d => d.ProductoId), ct)).ToList();
        var noEncontrados = dto.Detalles.Select(d => d.ProductoId).Except(productos.Select(p => p.Id)).ToList();
        if (noEncontrados.Count > 0)
            return (null, $"Productos no encontrados: {string.Join(", ", noEncontrados)}.", null);

        //Validar cantidades y stock por línea
        List<string> errores = new();
        foreach (var det in dto.Detalles)
        {
            if (det.Cantidad <= 0) 
            { 
                errores.Add($"Cantidad inválida en producto {det.ProductoId}.");
                continue; 
            }
            var prod = productos.First(p => p.Id == det.ProductoId);
            if (prod.Stock < det.Cantidad)
                errores.Add($"Stock insuficiente para '{prod.Descripcion}'. Disponible: {prod.Stock}, solicitado: {det.Cantidad}.");
        }
        if (errores.Count > 0) return (null, "Errores en los detalles del pedido.", errores);

        var estadoPendiente = await estadoRepo.ObtenerPorCodigoAsync("PENDIENTE", ct)
            ?? throw new InvalidOperationException("Estado PENDIENTE no encontrado en la base de datos.");

        var prefijo = DateTime.Now.ToString("MMyyyy");
        var secuencia = await pedidoRepo.PrefijoParaPedidoAsync(prefijo, ct) + 1;

        var detalles = dto.Detalles.Select(d =>
        {
            var prod = productos.First(p => p.Id == d.ProductoId);
            return new DetallePedido
            {
                ProductoId     = d.ProductoId,
                Cantidad       = d.Cantidad,
                PrecioUnitario = prod.PrecioUnitario
            };
        }).ToList();

        var pedido = new Pedido
        {
            NumeroPedido  = $"{prefijo}{secuencia:D5}",
            ClienteId     = dto.ClienteId,
            EstadoId      = estadoPendiente.Id,
            TotalMonto    = detalles.Sum(d => d.Cantidad * d.PrecioUnitario),
            Observaciones = dto.Observaciones,
            Detalles      = detalles
        };

        var pedidoCreado = await pedidoRepo.CrearAsync(pedido, ct);

        foreach (var det in detalles)
            await productoRepo.AjustarStockAsync(det.ProductoId, -det.Cantidad, ct);

        return (mapper.Map<PedidoDto>(pedidoCreado), null, null);
    }

    public async Task<(PedidoDto? Datos, string? Error)> CambiarEstadoAsync(
        int id, PedidoCambioEstadoDto dto, CancellationToken ct = default)
    {
        var pedido = await pedidoRepo.ObtenerPorIdAsync(id, ct);
        if (pedido is null)
            return (null, $"Pedido con Id {id} no encontrado.");

        if (pedido.Estado?.Codigo is "CANCELADO" or "ENTREGADO")
            return (null, $"No se puede modificar un pedido en estado '{pedido.Estado.Descripcion}'.");

        var nuevoEstado = await estadoRepo.ObtenerPorIdAsync(dto.EstadoId, ct);
        if (nuevoEstado is null)
            return (null, "Estado de pedido inválido.");

        await pedidoRepo.CambiarEstadoAsync(id, dto.EstadoId, ct);

        if (nuevoEstado.Codigo == "CANCELADO" && pedido.Estado?.Codigo != "CANCELADO")
            foreach (var det in pedido.Detalles)
                await productoRepo.AjustarStockAsync(det.ProductoId, +det.Cantidad, ct);

        if (pedido.Estado?.Codigo == "CANCELADO" && nuevoEstado.Codigo != "CANCELADO")
            foreach (var det in pedido.Detalles)
                await productoRepo.AjustarStockAsync(det.ProductoId, -det.Cantidad, ct);

        var pedidoActualizado = await pedidoRepo.ObtenerPorIdAsync(id, ct);
        return (mapper.Map<PedidoDto>(pedidoActualizado), null);
    }

    public async Task<(PedidoDto? Datos, string? Error, List<string>? Errores)> ActualizarAsync(
        int id, PedidoActualizarDto dto, CancellationToken ct = default)
    {
        var pedido = await pedidoRepo.ObtenerPorIdAsync(id, ct);
        if (pedido is null)
            return (null, $"Pedido con Id {id} no encontrado.", null);

        if (pedido.Estado?.Codigo is "CANCELADO" or "ENTREGADO")
            return (null, $"No se puede editar un pedido en estado '{pedido.Estado.Descripcion}'.", null);

        var cliente = await clienteRepo.ObtenerPorIdAsync(dto.ClienteId, ct);
        if (cliente is null || !cliente.Activo)
            return (null, $"Cliente con Id {dto.ClienteId} no encontrado.", null);

        if (dto.Detalles is not { Count: > 0 })
            return (null, "El pedido debe tener al menos un producto.", null);

        var duplicados = dto.Detalles
            .GroupBy(d => d.ProductoId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicados.Count > 0)
            return (null, $"Productos duplicados en el pedido: {string.Join(", ", duplicados)}.", null);

        var productos = (await productoRepo.ObtenerPorIdsAsync(dto.Detalles.Select(d => d.ProductoId), ct)).ToList();
        var noEncontrados = dto.Detalles.Select(d => d.ProductoId).Except(productos.Select(p => p.Id)).ToList();
        if (noEncontrados.Count > 0)
            return (null, $"Productos no encontrados: {string.Join(", ", noEncontrados)}.", null);

        List<string> errores = [];
        foreach (var det in dto.Detalles)
        {
            if (det.Cantidad <= 0) { errores.Add($"Cantidad inválida en producto {det.ProductoId}."); continue; }
            var prod = productos.First(p => p.Id == det.ProductoId);
            if (prod.Stock < det.Cantidad)
                errores.Add($"Stock insuficiente para '{prod.Descripcion}'. Disponible: {prod.Stock}, solicitado: {det.Cantidad}.");
        }
        if (errores.Count > 0) return (null, "Errores en los detalles del pedido.", errores);

        var detalles = dto.Detalles.Select(d =>
        {
            var prod = productos.First(p => p.Id == d.ProductoId);
            return new DetallePedido
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = prod.PrecioUnitario
            };
        }).ToList();

        var pedidoActualizado = new Pedido
        {
            Id = id,
            NumeroPedido = pedido.NumeroPedido,
            ClienteId = dto.ClienteId,
            TotalMonto = detalles.Sum(d => d.Cantidad * d.PrecioUnitario),
            Observaciones = dto.Observaciones,
            Detalles = detalles
        };

        foreach (var det in pedido.Detalles)
            await productoRepo.AjustarStockAsync(det.ProductoId, +det.Cantidad, ct);

        var resultado = await pedidoRepo.ActualizarAsync(pedidoActualizado, ct);

        foreach (var det in detalles)
            await productoRepo.AjustarStockAsync(det.ProductoId, -det.Cantidad, ct);

        return (mapper.Map<PedidoDto>(resultado), null, null);
    }

}