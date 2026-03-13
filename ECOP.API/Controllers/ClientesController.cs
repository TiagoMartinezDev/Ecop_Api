using ECOP.API.Services.Interfaces;
using ECOP.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECOP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientesController(IClienteServicio servicio) : ControllerBase
{
    /// <summary>Lista todos los clientes</summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var clientes = await servicio.ObtenerTodosAsync();
        return Ok(clientes);
    }

    /// <summary>Obtiene un cliente por Id</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken ct)
    {
        var dto = await servicio.ObtenerPorIdAsync(id, ct);
        if (dto is null)
            return NotFound(ApiRespuesta<ClienteDto>.Error($"Cliente {id} no encontrado."));
        
        return Ok(ApiRespuesta<ClienteDto>.Ok(dto));
    }

    /// <summary>Lista los tipos de documento disponibles</summary>
    [HttpGet("tipos-documento")]
    public async Task<IActionResult> ObtenerTiposDocumento()
    {
        var tipos = await servicio.ObtenerTiposDocumentoAsync();
        return Ok(tipos);
    }

    /// <summary>Crea un nuevo cliente</summary>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ClienteCrearDto dto, CancellationToken ct)
    {
        var (datos, error) = await servicio.CrearAsync(dto, ct);
        return error is not null
            ? BadRequest(ApiRespuesta<ClienteDto>.Error(error))
            : CreatedAtAction(nameof(ObtenerPorId), new { id = datos!.Id },
                ApiRespuesta<ClienteDto>.Ok(datos, "Cliente creado exitosamente."));
    }

    /// <summary>Actualiza un cliente existente</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ClienteActualizarDto dto, CancellationToken ct)
    {
        var (datos, error) = await servicio.ActualizarAsync(id, dto, ct);
        if (error is null)
            return Ok(ApiRespuesta<ClienteDto>.Ok(datos!, "Cliente actualizado."));
        
        return datos is null
            ? NotFound(ApiRespuesta<ClienteDto>.Error(error))
            : BadRequest(ApiRespuesta<ClienteDto>.Error(error));
    }

    /// <summary>Baja lógica de un cliente</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        var (ok, error) = await servicio.EliminarAsync(id, ct);
        return ok
            ? Ok(ApiRespuesta<object>.Ok(new { Id = id }, "Cliente eliminado."))
            : NotFound(ApiRespuesta<object>.Error(error!));
    }
}