namespace ECOP.AccesoDatos.Repositories.Interfaces;

public interface IProductoRepositorio : IRepositorio<Producto>
{
    Task<IEnumerable<Producto>> ObtenerActivosAsync(CancellationToken ct = default);
    Task<Producto?> ObtenerPorIdConDetalleAsync(int id, CancellationToken ct = default);
    Task<Producto?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default);
    Task<bool> ExisteCodigoAsync(string codigo, int? excluirId = null, CancellationToken ct = default);
    Task<IEnumerable<Producto>> ObtenerPorIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
    Task AjustarStockAsync(int productoId, int delta, CancellationToken ct = default);
}