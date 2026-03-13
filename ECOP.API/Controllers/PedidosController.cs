using ECOP.API.Services.Interfaces;
using ECOP.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECOP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PedidosController(IPedidoServicio servicio) : ControllerBase
{
    /// <summary>Lista pedidos — filtros opcionales: clienteId, estadoCodigo</summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(int? clienteId, string? estadoCodigo, CancellationToken ct)
    {
        var pedidos = await servicio.ObtenerTodosAsync(clienteId, estadoCodigo, ct);
        var respuesta = ApiRespuesta<IEnumerable<PedidoDto>>.Ok(pedidos);

        return Ok(respuesta);
    }

    /// <summary>Obtiene un pedido por Id</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken ct)
    {
        var dto = await servicio.ObtenerPorIdAsync(id, ct);
        return dto is null
            ? NotFound(ApiRespuesta<PedidoDto>.Error($"Pedido {id} no encontrado."))
            : Ok(ApiRespuesta<PedidoDto>.Ok(dto));
    }

    /// <summary>Obtiene un pedido por número</summary>
    [HttpGet("numero/{numeroPedido}")]
    public async Task<IActionResult> ObtenerPorNumero(string numeroPedido, CancellationToken ct)
    {
        var dto = await servicio.ObtenerPorNumeroAsync(numeroPedido, ct);
        return dto is null
            ? NotFound(ApiRespuesta<PedidoDto>.Error($"Pedido '{numeroPedido}' no encontrado."))
            : Ok(ApiRespuesta<PedidoDto>.Ok(dto));
    }

    /// <summary>Lista los estados de pedido disponibles</summary>
    [HttpGet("estados")]
    public async Task<IActionResult> ObtenerEstados(CancellationToken ct)
    {
        var listaEstados = await servicio.ObtenerEstadosAsync(ct);
        var respuesta = ApiRespuesta<IEnumerable<EstadoPedidoDto>>.Ok(listaEstados);

        return Ok(respuesta);
    }

    /// <summary>Crea un nuevo pedido de mercaderías</summary>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] PedidoCrearDto dto, CancellationToken ct)
    {
        var (datos, error, errores) = await servicio.CrearAsync(dto, ct);
        return error is not null
            ? BadRequest(ApiRespuesta<PedidoDto>.Error(error, errores))
            : CreatedAtAction(nameof(ObtenerPorId), new { id = datos!.Id },
                ApiRespuesta<PedidoDto>.Ok(datos, $"Pedido {datos.NumeroPedido} creado exitosamente."));
    }

    /// <summary>Cambia el estado de un pedido</summary>
    [HttpPatch("{id:int}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] PedidoCambioEstadoDto dto, CancellationToken ct)
    {
        var (datos, error) = await servicio.CambiarEstadoAsync(id, dto, ct);
        if (error is null) 
            return Ok(ApiRespuesta<PedidoDto>.Ok(datos!, "Estado actualizado."));
        
        return datos is null
            ? NotFound(ApiRespuesta<PedidoDto>.Error(error))
            : BadRequest(ApiRespuesta<PedidoDto>.Error(error));
    }

    /// <summary>Cancela un pedido (cambia estado a CANCELADO = Id 6)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancelar(int id, CancellationToken ct)
    {
        var dto = new PedidoCambioEstadoDto(6);
        return await CambiarEstado(id, dto, ct);
    }

    /// <summary>Actualiza cliente, observaciones y detalles de un pedido (no permitido en estado CANCELADO o ENTREGADO)</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] PedidoActualizarDto dto, CancellationToken ct)
    {
        var (datos, error, errores) = await servicio.ActualizarAsync(id, dto, ct);
        if (error is null) 
            return Ok(ApiRespuesta<PedidoDto>.Ok(datos!, $"Pedido {datos!.NumeroPedido} actualizado exitosamente."));
        
        return datos is null
            ? NotFound(ApiRespuesta<PedidoDto>.Error(error, errores))
            : BadRequest(ApiRespuesta<PedidoDto>.Error(error, errores));
    }
}