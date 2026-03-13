namespace ECOP.Models.Models;

/// <summary>
/// Unidad de medida para productos (UN, KG, LT, MT, etc.).
/// </summary>
public class UnidadMedida
{
    public int Id { get; set; }
    public required string Codigo { get; set; }
    public required string Descripcion { get; set; }
    public bool Activo { get; set; } = true;


    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
