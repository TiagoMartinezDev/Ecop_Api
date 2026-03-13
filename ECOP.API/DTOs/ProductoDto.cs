namespace ECOP.API.DTOs;

public record ProductoDto(
    int Id,
    string Codigo, 
    string Descripcion, 
    string UnidadMedida, 
    decimal PrecioUnitario,
    int Stock, 
    bool Activo, 
    DateTime FechaAlta);

public record ProductoCrearDto(
    string Codigo, 
    string Descripcion, 
    int UnidadMedidaId, 
    decimal PrecioUnitario, 
    int Stock = 0);

public record ProductoActualizarDto(
    string Descripcion, 
    int UnidadMedidaId, 
    decimal PrecioUnitario, 
    int Stock);

public record UnidadMedidaDto(int Id, string Codigo, string Descripcion, bool Activo);