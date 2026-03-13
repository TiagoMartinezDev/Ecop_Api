using ECOP.AccesoDatos.Data.EF;

namespace ECOP.AccesoDatos.Repositories.EF;

public class EFEstadoPedidoRepositorio(AppDbContext db) : IEstadoPedidoRepositorio
{
    public async Task<IEnumerable<EstadoPedido>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        var estados = await db.EstadosPedido
            .OrderBy(e => e.Id)
            .ToListAsync(ct);
        return estados;
    }

    public async Task<EstadoPedido?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        EstadoPedido? estado = await db.EstadosPedido.FindAsync([id], ct);
        return estado;
    }

    public async Task<EstadoPedido?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default)
    {
        EstadoPedido? estado = await db.EstadosPedido
            .FirstOrDefaultAsync(e => e.Codigo == codigo, ct);
        return estado;
    }
}

public class EFPedidoRepositorio(AppDbContext db) : IPedidoRepositorio
{
    private IQueryable<Pedido> ConIncludes()
    {
        var consulta = db.Pedidos
            .Include(p => p.Cliente)
                .ThenInclude(c => c!.TipoDocumento)
            .Include(p => p.Estado)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                    .ThenInclude(pr => pr!.UnidadMedida);
        return consulta;
    }

    public async Task<IEnumerable<Pedido>> ObtenerTodosAsync(
        int? clienteId = null, string? estadoCodigo = null, CancellationToken ct = default)
    {
        var p = ConIncludes();
        if (clienteId.HasValue)
            p = p.Where(ped => ped.ClienteId == clienteId.Value);
        if (!string.IsNullOrWhiteSpace(estadoCodigo))
            p = p.Where(ped => ped.Estado!.Codigo == estadoCodigo.ToUpperInvariant());
        return await p.OrderByDescending(ped => ped.FechaPedido).ToListAsync(ct);
    }

    public async Task<Pedido?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        Pedido? pedido = await ConIncludes().FirstOrDefaultAsync(p => p.Id == id, ct);
        return pedido;
    }

    public async Task<Pedido?> ObtenerPorNumeroAsync(string numeroPedido, CancellationToken ct)
    {
        Pedido? pedido = await ConIncludes().FirstOrDefaultAsync(p => p.NumeroPedido == numeroPedido, ct);
        return pedido;
    }

    public async Task<Pedido> CrearAsync(Pedido pedido, CancellationToken ct = default)
    {
        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync(ct);
        return await ConIncludes().FirstAsync(p => p.Id == pedido.Id, ct);
    }

    public async Task<bool> CambiarEstadoAsync(int pedidoId, int nuevoEstadoId, CancellationToken ct = default)
    {
        var filas = await db.Pedidos
            .Where(p => p.Id == pedidoId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.EstadoId, nuevoEstadoId)
                .SetProperty(p => p.FechaModificacion, DateTime.Now), ct);
        return filas > 0;
    }

    public async Task<int> PrefijoParaPedidoAsync(string prefijo, CancellationToken ct)
    {
        int total = await db.Pedidos
            .Where(p => p.NumeroPedido.StartsWith(prefijo))
            .CountAsync(ct);

        return total;
    }

    public async Task<bool> TienePedidosActivosAsync(int clienteId, CancellationToken ct)
    {
        var estadosActivos = new[] { "PENDIENTE", "CONFIRMADO", "EN_PROCESO" };

        bool existe = await db.Pedidos
            .AnyAsync(p => p.ClienteId == clienteId && estadosActivos.Contains(p.Estado!.Codigo), ct);

        return existe;
    }

    public async Task<Pedido> ActualizarAsync(Pedido pedido, CancellationToken ct = default)
    {
        // Eliminar detalles anteriores y reemplazar con los nuevos
        var detallesAnteriores = await db.DetallePedidos
            .Where(d => d.PedidoId == pedido.Id)
            .ToListAsync(ct);
        db.DetallePedidos.RemoveRange(detallesAnteriores);

        var cabecera = await db.Pedidos.FindAsync([pedido.Id], ct)
            ?? throw new InvalidOperationException("Pedido no encontrado.");

        cabecera.ClienteId = pedido.ClienteId;
        cabecera.TotalMonto = pedido.TotalMonto;
        cabecera.Observaciones = pedido.Observaciones;
        cabecera.FechaModificacion = DateTime.UtcNow;

        foreach (var d in pedido.Detalles)
            db.DetallePedidos.Add(new DetallePedido
            {
                PedidoId = pedido.Id,
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            });

        await db.SaveChangesAsync(ct);
        return await ConIncludes().FirstAsync(p => p.Id == pedido.Id, ct);
    }
}
