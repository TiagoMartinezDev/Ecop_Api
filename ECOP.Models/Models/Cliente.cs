namespace ECOP.Models.Models;

/// <summary>
/// Cliente del sistema.
/// </summary>
public class Cliente
{
    public int Id{ get; set; }
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
    public int TipoDocumentoId { get; set; }
    public required string NroDocumento { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaAlta{ get; set; } = DateTime.Now;
    public DateTime? FechaModificacion { get; set; }
    

    public TipoDocumento? TipoDocumento { get; set; }
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public string NombreCompleto => $"{Nombre} {Apellido}";
}
