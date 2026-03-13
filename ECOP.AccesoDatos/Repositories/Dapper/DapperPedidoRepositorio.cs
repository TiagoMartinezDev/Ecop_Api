using ECOP.AccesoDatos.Data.Dapper;

namespace ECOP.AccesoDatos.Repositories.Dapper;

public class DapperEstadoPedidoRepositorio(IDapperConnectionFactory factory) : IEstadoPedidoRepositorio
{
    private static EstadoPedido Map(dynamic r) =>
        new() { Id = (int)r.Id, Codigo = (string)r.Codigo, Descripcion = (string)r.Descripcion };

    public async Task<IEnumerable<EstadoPedido>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync("SELECT Id, Codigo, Descripcion FROM EstadosPedido ORDER BY Id"))
               .Select(r => (EstadoPedido)Map(r));
    }

    public async Task<EstadoPedido?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync(
            "SELECT Id, Codigo, Descripcion FROM EstadosPedido WHERE Id = @Id", new { Id = id });
        if (row == null)
            return null;

        return  (EstadoPedido)Map(row);
    }

    public async Task<EstadoPedido?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync(
            "SELECT Id, Codigo, Descripcion FROM EstadosPedido WHERE Codigo = @Codigo", new { Codigo = codigo });
        if (row == null)
            return null;

        return (EstadoPedido)Map(row);
    }
}

public class DapperPedidoRepositorio(IDapperConnectionFactory factory) : IPedidoRepositorio
{
    private const string SelectPedido = """
        SELECT
            p.Id, p.NumeroPedido, p.ClienteId, p.FechaPedido,
            p.EstadoId, p.TotalMonto, p.Observaciones, p.FechaModificacion,
            c.Id AS CliId, c.Nombre AS CliNombre, c.Apellido AS CliApellido,
            c.TipoDocumentoId AS CliTdId, c.NroDocumento AS CliDoc,
            c.Email AS CliEmail, c.Telefono AS CliTel,
            c.Activo AS CliActivo, c.FechaAlta AS CliFechaAlta,
            td.Id AS TdId, td.Codigo AS TdCodigo, td.Descripcion AS TdDesc,
            td.Activo AS TdActivo, td.FechaAlta AS TdFechaAlta,
            e.Id AS EstId, e.Codigo AS EstCodigo, e.Descripcion AS EstDesc
        FROM Pedidos p
        JOIN Clientes c ON c.Id = p.ClienteId
        JOIN TiposDocumento td ON td.Id = c.TipoDocumentoId
        JOIN EstadosPedido e ON e.Id = p.EstadoId
        """;

    private const string SelectDetalle = """
        SELECT
            d.Id, d.PedidoId, d.ProductoId, d.Cantidad, d.PrecioUnitario,
            pr.Id AS PrId, pr.Codigo AS PrCodigo, pr.Descripcion AS PrDesc,
            pr.PrecioUnitario AS PrPrecio, pr.Stock AS PrStock, pr.Activo AS PrActivo,
            um.Id AS UmId, um.Codigo AS UmCodigo, um.Descripcion AS UmDesc, um.Activo AS UmActivo
        FROM DetallePedidos d
        JOIN Productos pr ON pr.Id = d.ProductoId
        JOIN UnidadesMedida um ON um.Id = pr.UnidadMedidaId
        """;

    private static Pedido MapPedido(dynamic r) => new()
    {
        Id = (int)r.Id, NumeroPedido = (string)r.NumeroPedido,
        ClienteId = (int)r.ClienteId, FechaPedido = (DateTime)r.FechaPedido,
        EstadoId = (int)r.EstadoId, TotalMonto = (decimal)r.TotalMonto,
        Observaciones = (string?)r.Observaciones, FechaModificacion = (DateTime?)r.FechaModificacion,
        Cliente = new Cliente
        {
            Id = (int)r.CliId, Nombre = (string)r.CliNombre, Apellido = (string)r.CliApellido,
            TipoDocumentoId = (int)r.CliTdId, NroDocumento = (string)r.CliDoc,
            Email = (string?)r.CliEmail, Telefono = (string?)r.CliTel,
            Activo = (bool)r.CliActivo, FechaAlta = (DateTime)r.CliFechaAlta,
            TipoDocumento = new TipoDocumento
            {
                Id = (int)r.TdId, Codigo = (string)r.TdCodigo, Descripcion = (string)r.TdDesc,
                Activo = (bool)r.TdActivo, FechaAlta = (DateTime)r.TdFechaAlta
            }
        },
        Estado = new EstadoPedido { Id = (int)r.EstId, Codigo = (string)r.EstCodigo, Descripcion = (string)r.EstDesc }
    };

    private static DetallePedido MapDetalle(dynamic r) => new()
    {
        Id = (int)r.Id, PedidoId = (int)r.PedidoId, ProductoId = (int)r.ProductoId,
        Cantidad = (int)r.Cantidad, PrecioUnitario = (decimal)r.PrecioUnitario,
        Producto = new Producto
        {
            Id = (int)r.PrId, Codigo = (string)r.PrCodigo, Descripcion = (string)r.PrDesc,
            PrecioUnitario = (decimal)r.PrPrecio, Stock = (int)r.PrStock, Activo = (bool)r.PrActivo,
            UnidadMedidaId = (int)r.UmId,
            UnidadMedida = new UnidadMedida
                { Id = (int)r.UmId, Codigo = (string)r.UmCodigo, Descripcion = (string)r.UmDesc, Activo = (bool)r.UmActivo }
        }
    };

    private async Task<List<DetallePedido>> CargarDetallesAsync(IDbConnection conn, IEnumerable<int> ids)
    {
        var rows = await conn.QueryAsync($"{SelectDetalle} WHERE d.PedidoId IN @Ids ORDER BY d.Id", new { Ids = ids });
        return rows.Select(r => (DetallePedido)MapDetalle(r)).ToList();
    }

    public async Task<IEnumerable<Pedido>> ObtenerTodosAsync(
        int? clienteId = null, string? estadoCodigo = null, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var filtros = new List<string>();
        if (clienteId.HasValue) filtros.Add("p.ClienteId = @ClienteId");
        if (!string.IsNullOrWhiteSpace(estadoCodigo)) filtros.Add("e.Codigo = @EstadoCodigo");

        var where = filtros.Count > 0 ? $"WHERE {string.Join(" AND ", filtros)}" : string.Empty;
        var pedidos = (await conn.QueryAsync(
            $"{SelectPedido} {where} ORDER BY p.FechaPedido DESC",
            new { ClienteId = clienteId, EstadoCodigo = estadoCodigo?.ToUpperInvariant() }))
            .Select(r => (Pedido)MapPedido(r)).ToList();

        if (pedidos.Count > 0)
        {
            var detalles = await CargarDetallesAsync(conn, pedidos.Select(p => p.Id));
            foreach (var p in pedidos)
                ((List<DetallePedido>)p.Detalles).AddRange(detalles.Where(d => d.PedidoId == p.Id));
        }
        return pedidos;
    }

    public async Task<Pedido?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync($"{SelectPedido} WHERE p.Id = @Id", new { Id = id });
        if (row is null) return null;
        var pedido = (Pedido)MapPedido(row);
        ((List<DetallePedido>)pedido.Detalles).AddRange(await CargarDetallesAsync(conn, [pedido.Id]));
        return pedido;
    }

    public async Task<Pedido?> ObtenerPorNumeroAsync(string numeroPedido, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync($"{SelectPedido} WHERE p.NumeroPedido = @NumeroPedido", new { NumeroPedido = numeroPedido });
        if (row is null) return null;
        var pedido = (Pedido)MapPedido(row);
        ((List<DetallePedido>)pedido.Detalles).AddRange(await CargarDetallesAsync(conn, [pedido.Id]));
        return pedido;
    }

    public async Task<Pedido> CrearAsync(Pedido pedido, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            pedido.Id = await conn.ExecuteScalarAsync<int>("""
                INSERT INTO Pedidos (NumeroPedido, ClienteId, FechaPedido, EstadoId, TotalMonto, Observaciones)
                VALUES (@NumeroPedido, @ClienteId, GETDATE(), @EstadoId, @TotalMonto, @Observaciones);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """, pedido, tx);

            foreach (var det in pedido.Detalles)
            {
                det.PedidoId = pedido.Id;
                await conn.ExecuteAsync("""
                    INSERT INTO DetallePedidos (PedidoId, ProductoId, Cantidad, PrecioUnitario)
                    VALUES (@PedidoId, @ProductoId, @Cantidad, @PrecioUnitario);
                    """, det, tx);
                await conn.ExecuteAsync(
                    "UPDATE Productos SET Stock = Stock - @Cantidad WHERE Id = @ProductoId",
                    new { det.Cantidad, det.ProductoId }, tx);
            }
            tx.Commit();
        }
        catch { tx.Rollback(); throw; }

        return (await ObtenerPorIdAsync(pedido.Id, ct))!;
    }

    public async Task<bool> CambiarEstadoAsync(int pedidoId, int nuevoEstadoId, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            var nuevoCodigo = await conn.ExecuteScalarAsync<string>(
                "SELECT Codigo FROM EstadosPedido WHERE Id = @Id", new { Id = nuevoEstadoId }, tx) ?? "";

            if (nuevoCodigo == "CANCELADO")
            {
                var detalles = await conn.QueryAsync(
                    "SELECT ProductoId, Cantidad FROM DetallePedidos WHERE PedidoId = @Id", new { Id = pedidoId }, tx);
                foreach (var d in detalles)
                    await conn.ExecuteAsync(
                        "UPDATE Productos SET Stock = Stock + @Cantidad WHERE Id = @ProductoId",
                        new { d.Cantidad, d.ProductoId }, tx);
            }

            var filas = await conn.ExecuteAsync(
                "UPDATE Pedidos SET EstadoId = @EstadoId, FechaModificacion = GETDATE() WHERE Id = @Id",
                new { EstadoId = nuevoEstadoId, Id = pedidoId }, tx);
            tx.Commit();
            return filas > 0;
        }
        catch { tx.Rollback(); throw; }
    }

    public async Task<int> PrefijoParaPedidoAsync(string prefijo, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Pedidos WHERE NumeroPedido LIKE @Prefijo + '%'", new { Prefijo = prefijo });
    }

    public async Task<bool> TienePedidosActivosAsync(int clienteId, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>("""
            SELECT COUNT(1) FROM Pedidos p
            JOIN EstadosPedido e ON e.Id = p.EstadoId
            WHERE p.ClienteId = @ClienteId
              AND e.Codigo IN ('PENDIENTE', 'CONFIRMADO', 'EN_PROCESO')
            """, new { ClienteId = clienteId }) > 0;
    }

    public async Task<Pedido> ActualizarAsync(Pedido pedido, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            await conn.ExecuteAsync(
                "DELETE FROM DetallePedidos WHERE PedidoId = @Id",
                new { pedido.Id }, tx);

            await conn.ExecuteAsync("""
                UPDATE Pedidos SET
                    ClienteId         = @ClienteId,
                    TotalMonto        = @TotalMonto,
                    Observaciones     = @Observaciones,
                    FechaModificacion = GETDATE()
                WHERE Id = @Id
                """, new { pedido.ClienteId, pedido.TotalMonto, pedido.Observaciones, pedido.Id }, tx);

            foreach (var d in pedido.Detalles)
                await conn.ExecuteAsync(
                    "INSERT INTO DetallePedidos (PedidoId, ProductoId, Cantidad, PrecioUnitario) VALUES (@PedidoId, @ProductoId, @Cantidad, @PrecioUnitario)",
                    new { PedidoId = pedido.Id, d.ProductoId, d.Cantidad, d.PrecioUnitario }, tx);

            tx.Commit();
            return await ObtenerPorIdAsync(pedido.Id, ct) ?? pedido;
        }
        catch { tx.Rollback(); throw; }
    }
}
