namespace ECOP.AccesoDatos.Repositories.Interfaces;

public interface ITipoDocumentoRepositorio
{
    Task<IEnumerable<TipoDocumento>> ObtenerActivosAsync(CancellationToken ct = default);
    Task<TipoDocumento?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteAsync(int id, CancellationToken ct = default);
}