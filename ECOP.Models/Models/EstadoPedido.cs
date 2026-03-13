namespace ECOP.Models.Models;

/// <summary>
/// Estado de un pedido (PENDIENTE, CONFIRMADO, EN_PROCESO, DESPACHADO, ENTREGADO, CANCELADO).
/// </summary>
public class EstadoPedido
{
    public int Id { get; set; }
    public required string Codigo { get; set; }
    public required string Descripcion { get; set; }


    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}