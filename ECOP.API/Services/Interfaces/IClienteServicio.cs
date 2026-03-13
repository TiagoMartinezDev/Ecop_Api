namespace ECOP.API.Services.Interfaces;

public interface IClienteServicio
{
    Task<IEnumerable<ClienteDto>> ObtenerTodosAsync(CancellationToken ct = default);
    Task<ClienteDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<TipoDocumentoDto>> ObtenerTiposDocumentoAsync(CancellationToken ct = default);
    Task<(ClienteDto? Datos, string? Error)> CrearAsync(ClienteCrearDto dto, CancellationToken ct = default);
    Task<(ClienteDto? Datos, string? Error)> ActualizarAsync(int id, ClienteActualizarDto dto, CancellationToken ct = default);
    Task<(bool Ok, string? Error)> EliminarAsync(int id, CancellationToken ct = default);
}