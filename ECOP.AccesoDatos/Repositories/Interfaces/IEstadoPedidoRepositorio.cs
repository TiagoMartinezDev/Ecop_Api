namespace ECOP.AccesoDatos.Repositories.Interfaces;

public interface IEstadoPedidoRepositorio
{
    Task<IEnumerable<EstadoPedido>> ObtenerTodosAsync(CancellationToken ct = default);
    Task<EstadoPedido?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<EstadoPedido?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default);
}