namespace ECOP.Models.Models;

/// <summary>
/// Línea de detalle de un pedido.
/// </summary>
public class DetallePedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }


    public decimal Subtotal => Cantidad * PrecioUnitario;
    public Pedido? Pedido { get; set; }
    public Producto? Producto { get; set; }
}
