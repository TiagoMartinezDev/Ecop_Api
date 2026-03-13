namespace ECOP.API.DTOs;

public record ApiRespuesta<T>
{
    public bool Exitoso { get; init; }
    public string Mensaje { get; init; } = string.Empty;
    public T? Datos { get; init; }
    public List<string>? Errores { get; init; }

    public static ApiRespuesta<T> Ok(T datos, string mensaje = "Operación exitosa") =>
        new() { Exitoso = true, Mensaje = mensaje, Datos = datos };

    public static ApiRespuesta<T> Error(string mensaje, List<string>? errores = null) =>
        new() { Exitoso = false, Mensaje = mensaje, Errores = errores };
}