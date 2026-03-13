using ECOP.API.Services.Interfaces;
using ECOP.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECOP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductosController(IProductoServicio servicio) : ControllerBase
{
    /// <summary>Lista todos los productos activos</summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(CancellationToken ct)
    {
        IEnumerable<ProductoDto> productos = await servicio.ObtenerTodosAsync(ct);
        var respuesta = ApiRespuesta<IEnumerable<ProductoDto>>.Ok(productos);

        return Ok(respuesta);
    }

    /// <summary>Obtiene un producto por Id</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken ct)
    {
        var dto = await servicio.ObtenerPorIdAsync(id, ct);
        return dto is null
            ? NotFound(ApiRespuesta<ProductoDto>.Error($"Producto {id} no encontrado."))
            : Ok(ApiRespuesta<ProductoDto>.Ok(dto));
    }

    /// <summary>Obtiene un producto por código</summary>
    [HttpGet("codigo/{codigo}")]
    public async Task<IActionResult> ObtenerPorCodigo(string codigo, CancellationToken ct)
    {
        var dto = await servicio.ObtenerPorCodigoAsync(codigo, ct);
        return dto is null
            ? NotFound(ApiRespuesta<ProductoDto>.Error($"Producto '{codigo}' no encontrado."))
            : Ok(ApiRespuesta<ProductoDto>.Ok(dto));
    }

    /// <summary>Lista las unidades de medida disponibles</summary>
    [HttpGet("unidades-medida")]
    public async Task<IActionResult> ObtenerUnidadesMedida(CancellationToken ct)
    {
        var unidades = await servicio.ObtenerUnidadesMedidaAsync(ct);
        var respuesta = ApiRespuesta<IEnumerable<UnidadMedidaDto>>.Ok(unidades);

        return Ok(respuesta);
    }

    /// <summary>Crea un nuevo producto</summary>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ProductoCrearDto dto, CancellationToken ct)
    {
        var (datos, error) = await servicio.CrearAsync(dto, ct);
        return error is not null
            ? BadRequest(ApiRespuesta<ProductoDto>.Error(error))
            : CreatedAtAction(nameof(ObtenerPorId), new { id = datos!.Id },
                ApiRespuesta<ProductoDto>.Ok(datos, "Producto creado exitosamente."));
    }

    /// <summary>Actualiza un producto existente</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ProductoActualizarDto dto, CancellationToken ct)
    {
        var (datos, error) = await servicio.ActualizarAsync(id, dto, ct);
        if (error is null) return Ok(ApiRespuesta<ProductoDto>.Ok(datos!, "Producto actualizado."));
        return datos is null
            ? NotFound(ApiRespuesta<ProductoDto>.Error(error))
            : BadRequest(ApiRespuesta<ProductoDto>.Error(error));
    }

    /// <summary>Baja lógica de un producto</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        var (ok, error) = await servicio.EliminarAsync(id, ct);
        return ok
            ? Ok(ApiRespuesta<object>.Ok(new { Id = id }, "Producto eliminado."))
            : NotFound(ApiRespuesta<object>.Error(error!));
    }
}