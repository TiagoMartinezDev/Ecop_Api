namespace ECOP.Models.Models;

/// <summary>
/// Tipo de documento de identidad (CI, RUC, PAS, etc.).
/// </summary>
public class TipoDocumento
{
    public int Id { get; set; }
    public required string Codigo { get; set; }
    public required string Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaAlta { get; set; } = DateTime.Now;


    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
