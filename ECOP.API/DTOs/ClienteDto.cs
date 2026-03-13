namespace ECOP.API.DTOs;

public record ClienteDto(
    int Id,
    string Nombre,
    string Apellido,
    string TipoDocumento,
    string NroDocumento,
    string? Email,
    string? Telefono,
    bool Activo,
    DateTime FechaAlta)
{
    public string NombreCompleto => $"{Nombre} {Apellido}";
}

public record ClienteCrearDto(
    string Nombre,
    string Apellido,
    int TipoDocumentoId,
    string NroDocumento,
    string? Email,
    string? Telefono);

public record ClienteActualizarDto(
    string Nombre,
    string Apellido,
    int TipoDocumentoId,
    string NroDocumento,
    bool Activo,
    string? Email,
    string? Telefono);

public record TipoDocumentoDto(int Id, string Codigo, string Descripcion, bool Activo);