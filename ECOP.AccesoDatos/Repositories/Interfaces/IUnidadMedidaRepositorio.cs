namespace ECOP.AccesoDatos.Repositories.Interfaces;

public interface IUnidadMedidaRepositorio
{
    Task<IEnumerable<UnidadMedida>> ObtenerActivasAsync(CancellationToken ct = default);
    Task<UnidadMedida?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteAsync(int id, CancellationToken ct = default);
}