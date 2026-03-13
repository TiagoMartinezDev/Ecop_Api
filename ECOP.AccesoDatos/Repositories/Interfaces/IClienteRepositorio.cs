namespace ECOP.AccesoDatos.Repositories.Interfaces;

public interface IClienteRepositorio : IRepositorio<Cliente>
{
    Task<IEnumerable<Cliente>> ObtenerActivosAsync(CancellationToken ct = default);
    Task<Cliente?> ObtenerPorIdConDetalleAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteDocumentoAsync(int tipoDocId, string nroDoc, int? excluirId = null, CancellationToken ct = default);
}