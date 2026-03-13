namespace ECOP.API.DTOs;

public record EstadoPedidoDto(int Id, string Codigo, string Descripcion);

public record DetallePedidoDto(
    int     Id,
    int     ProductoId,
    string  CodigoProducto,
    string  DescripcionProducto,
    string  UnidadMedida,
    int     Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);

public record PedidoDto(
    int      Id,
    string   NumeroPedido,
    int      ClienteId,
    string   NombreCliente,
    DateTime FechaPedido,
    string   Estado,
    decimal  TotalMonto,
    string?  Observaciones,
    List<DetallePedidoDto> Detalles);

public record DetallePedidoCrearDto(int ProductoId, int Cantidad);

public record PedidoCrearDto(
    int                         ClienteId,
    string?                     Observaciones,
    List<DetallePedidoCrearDto> Detalles);

public record PedidoCambioEstadoDto(int EstadoId);

public record DetallePedidoActualizarDto(int ProductoId, int Cantidad);
public record PedidoActualizarDto(
    int ClienteId,
    string? Observaciones,
    List<DetallePedidoActualizarDto> Detalles);