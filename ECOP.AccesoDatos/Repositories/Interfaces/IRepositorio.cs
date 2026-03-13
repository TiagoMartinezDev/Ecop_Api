namespace ECOP.AccesoDatos.Repositories.Interfaces;

public interface IRepositorio<T> where T : class
{
    Task<IEnumerable<T>> ObtenerTodosAsync(CancellationToken ct = default);
    Task<T?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<T> CrearAsync(T entidad, CancellationToken ct = default);
    Task<T> ActualizarAsync(T entidad, CancellationToken ct = default);
    Task<bool> EliminarAsync(int id, CancellationToken ct = default);
}