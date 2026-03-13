namespace ECOP.Models.Models;

/// <summary>
/// Producto del catálogo.
/// </summary>
public class Producto
{
    public int Id { get; set; }
    public required string Codigo { get; set; }
    public required string Descripcion { get; set; }
    public int UnidadMedidaId { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Stock { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaAlta { get; set; } = DateTime.Now;
    public DateTime? FechaModificacion { get; set; }


    public UnidadMedida? UnidadMedida { get; set; }
    public ICollection<DetallePedido> DetallePedidos { get; init; } = new List<DetallePedido>();
}
