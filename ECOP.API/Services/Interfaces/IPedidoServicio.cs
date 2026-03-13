namespace ECOP.API.Services.Interfaces;

public interface IPedidoServicio
{
    Task<IEnumerable<PedidoDto>> ObtenerTodosAsync(int? clienteId, string? estadoCodigo, CancellationToken ct = default);
    Task<PedidoDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<PedidoDto?> ObtenerPorNumeroAsync(string numeroPedido, CancellationToken ct = default);
    Task<IEnumerable<EstadoPedidoDto>> ObtenerEstadosAsync(CancellationToken ct = default);
    Task<(PedidoDto? Datos, string? Error, List<string>? Errores)> CrearAsync(PedidoCrearDto dto, CancellationToken ct = default);
    Task<(PedidoDto? Datos, string? Error)> CambiarEstadoAsync(int id, PedidoCambioEstadoDto dto, CancellationToken ct = default);
    Task<(PedidoDto? Datos, string? Error, List<string>? Errores)> ActualizarAsync(int id, PedidoActualizarDto dto, CancellationToken ct = default);
}