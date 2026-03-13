namespace ECOP.Models.Models;

/// <summary>
/// Cabecera de un pedido de mercaderías.
/// </summary>
public class Pedido
{
    public int Id { get; set; }
    public required string NumeroPedido { get; set; }
    public int ClienteId { get; set; }
    public DateTime FechaPedido { get; set; } = DateTime.Now;
    public int EstadoId { get; set; }
    public decimal TotalMonto { get; set; }
    public string? Observaciones { get; set; }
    public DateTime? FechaModificacion { get; set; }


    public Cliente? Cliente { get; set; }
    public EstadoPedido? Estado { get; set; }
    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
}
