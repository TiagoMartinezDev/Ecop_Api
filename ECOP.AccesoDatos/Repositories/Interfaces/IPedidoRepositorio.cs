namespace ECOP.AccesoDatos.Repositories.Interfaces;

public interface IPedidoRepositorio
{
    Task<IEnumerable<Pedido>> ObtenerTodosAsync(int? clienteId = null, string? estadoCodigo = null, CancellationToken ct = default);
    Task<Pedido?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<Pedido?> ObtenerPorNumeroAsync(string numeroPedido, CancellationToken ct = default);
    Task<Pedido> CrearAsync(Pedido pedido, CancellationToken ct = default);
    Task<bool> CambiarEstadoAsync(int pedidoId, int nuevoEstadoId, CancellationToken ct = default);
    Task<int> PrefijoParaPedidoAsync(string prefijo, CancellationToken ct = default);
    Task<bool> TienePedidosActivosAsync(int clienteId, CancellationToken ct = default);
    Task<Pedido> ActualizarAsync(Pedido pedido, CancellationToken ct = default);
}