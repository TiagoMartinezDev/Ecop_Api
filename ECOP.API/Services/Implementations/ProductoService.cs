using ECOP.API.Services.Interfaces;

namespace ECOP.API.Services.Implementations;
public class ProductoService(IProductoRepositorio productoRepo, IUnidadMedidaRepositorio unidadRepo, IMapper mapper) : IProductoServicio
{
    public async Task<IEnumerable<ProductoDto>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        var productos = await productoRepo.ObtenerActivosAsync(ct);
        return mapper.Map<IEnumerable<ProductoDto>>(productos);
    }

    public async Task<ProductoDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        var producto = await productoRepo.ObtenerPorIdConDetalleAsync(id, ct);
        if (producto == null)
            return null;

        return mapper.Map<ProductoDto>(producto);
    }

    public async Task<ProductoDto?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default)
    {
        var producto = await productoRepo.ObtenerPorCodigoAsync(codigo, ct);
        if (producto == null)
            return null;

        return mapper.Map<ProductoDto>(producto);
    }

    public async Task<IEnumerable<UnidadMedidaDto>> ObtenerUnidadesMedidaAsync(CancellationToken ct = default)
    {
        var unidades = await unidadRepo.ObtenerActivasAsync(ct);
        return mapper.Map<IEnumerable<UnidadMedidaDto>>(unidades);
    }

    public async Task<(ProductoDto? Datos, string? Error)> CrearAsync(ProductoCrearDto dto, CancellationToken ct = default)
    {
        if (dto.PrecioUnitario <= 0)
            return (null, "El precio unitario debe ser mayor a cero.");

        if (!await unidadRepo.ExisteAsync(dto.UnidadMedidaId, ct))
            return (null, "Unidad de medida inválida.");

        if (await productoRepo.ExisteCodigoAsync(dto.Codigo, null, ct))
            return (null, $"Ya existe un producto con código '{dto.Codigo}'.");

        var producto = mapper.Map<Producto>(dto);
        return (mapper.Map<ProductoDto>(await productoRepo.CrearAsync(producto, ct)), null);
    }

    public async Task<(ProductoDto? Datos, string? Error)> ActualizarAsync(int id, ProductoActualizarDto dto, CancellationToken ct = default)
    {
        var producto = await productoRepo.ObtenerPorIdAsync(id, ct);
        if (producto is null || !producto.Activo)
            return (null, $"Producto con Id {id} no encontrado.");

        if (dto.PrecioUnitario <= 0)
            return (null, "El precio unitario debe ser mayor a cero.");

        if (!await unidadRepo.ExisteAsync(dto.UnidadMedidaId, ct))
            return (null, "Unidad de medida inválida.");

        mapper.Map(dto, producto);
        producto.FechaModificacion = DateTime.Now;
        return (mapper.Map<ProductoDto>(await productoRepo.ActualizarAsync(producto, ct)), null);
    }

    public async Task<(bool Ok, string? Error)> EliminarAsync(int id, CancellationToken ct = default)
    {
        var producto = await productoRepo.ObtenerPorIdAsync(id, ct);
        if (producto is null || !producto.Activo)
            return (false, $"Producto con Id {id} no encontrado.");

        var productoEliminado = await productoRepo.EliminarAsync(id, ct);
        return (productoEliminado, productoEliminado ? null : "No se pudo eliminar el producto.");
    }
}