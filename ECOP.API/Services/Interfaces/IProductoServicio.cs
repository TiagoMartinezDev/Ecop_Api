namespace ECOP.API.Services.Interfaces;

public interface IProductoServicio
{
    Task<IEnumerable<ProductoDto>> ObtenerTodosAsync(CancellationToken ct = default);
    Task<ProductoDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<ProductoDto?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default);
    Task<IEnumerable<UnidadMedidaDto>> ObtenerUnidadesMedidaAsync(CancellationToken ct = default);
    Task<(ProductoDto? Datos, string? Error)> CrearAsync(ProductoCrearDto dto, CancellationToken ct = default);
    Task<(ProductoDto? Datos, string? Error)> ActualizarAsync(int id, ProductoActualizarDto dto, CancellationToken ct = default);
    Task<(bool Ok, string? Error)> EliminarAsync(int id, CancellationToken ct = default);
}