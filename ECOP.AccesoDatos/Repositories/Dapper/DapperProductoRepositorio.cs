using ECOP.AccesoDatos.Data.Dapper;

namespace ECOP.AccesoDatos.Repositories.Dapper;

public class DapperProductoRepositorio(IDapperConnectionFactory factory) : IProductoRepositorio
{
    private const string SelectBase = """
        SELECT
            p.Id, p.Codigo, p.Descripcion, p.UnidadMedidaId,
            p.PrecioUnitario, p.Stock, p.Activo, p.FechaAlta, p.FechaModificacion,
            um.Id AS UmId, um.Codigo AS UmCodigo,
            um.Descripcion AS UmDescripcion, um.Activo AS UmActivo
        FROM Productos p
        JOIN UnidadesMedida um ON um.Id = p.UnidadMedidaId
        """;

    private static Producto Map(dynamic r) => new()
    {
        Id                = (int)r.Id,
        Codigo            = (string)r.Codigo,
        Descripcion       = (string)r.Descripcion,
        UnidadMedidaId    = (int)r.UnidadMedidaId,
        PrecioUnitario    = (decimal)r.PrecioUnitario,
        Stock             = (int)r.Stock,
        Activo            = (bool)r.Activo,
        FechaAlta         = (DateTime)r.FechaAlta,
        FechaModificacion = (DateTime?)r.FechaModificacion,
        UnidadMedida = new UnidadMedida
        {
            Id = (int)r.UmId, Codigo = (string)r.UmCodigo,
            Descripcion = (string)r.UmDescripcion, Activo = (bool)r.UmActivo
        }
    };

    public async Task<IEnumerable<Producto>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync($"{SelectBase} ORDER BY p.Codigo")).Select(r => (Producto)Map(r));
    }

    public async Task<IEnumerable<Producto>> ObtenerActivosAsync(CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync($"{SelectBase} WHERE p.Activo = 1 ORDER BY p.Codigo")).Select(r => (Producto)Map(r));
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync($"{SelectBase} WHERE p.Id = @Id", new { Id = id });
        return row is null ? null : (Producto)Map(row);
    }

    public async Task<Producto?> ObtenerPorIdConDetalleAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync($"{SelectBase} WHERE p.Id = @Id", new { Id = id });
        return row is null ? null : (Producto)Map(row);
    }

    public async Task<Producto?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync($"{SelectBase} WHERE p.Codigo = @Codigo AND p.Activo = 1", new { Codigo = codigo });
        return row is null ? null : (Producto)Map(row);
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, int? excluirId = null, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Productos WHERE Codigo = @Codigo AND (@ExcluirId IS NULL OR Id <> @ExcluirId)",
            new { Codigo = codigo, ExcluirId = excluirId }) > 0;
    }

    public async Task<IEnumerable<Producto>> ObtenerPorIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync($"{SelectBase} WHERE p.Id IN @Ids AND p.Activo = 1", new { Ids = ids }))
               .Select(r => (Producto)Map(r));
    }

    public async Task AjustarStockAsync(int productoId, int delta, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        await conn.ExecuteAsync("UPDATE Productos SET Stock = Stock + @Delta WHERE Id = @Id",
            new { Delta = delta, Id = productoId });
    }

    public async Task<Producto> CrearAsync(Producto e, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        e.Id = await conn.ExecuteScalarAsync<int>("""
            INSERT INTO Productos (Codigo, Descripcion, UnidadMedidaId, PrecioUnitario, Stock, Activo, FechaAlta)
            VALUES (@Codigo, @Descripcion, @UnidadMedidaId, @PrecioUnitario, @Stock, 1, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """, e);
        return (await ObtenerPorIdConDetalleAsync(e.Id, ct))!;
    }

    public async Task<Producto> ActualizarAsync(Producto e, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        await conn.ExecuteAsync("""
            UPDATE Productos SET
                Descripcion = @Descripcion, UnidadMedidaId = @UnidadMedidaId,
                PrecioUnitario = @PrecioUnitario, Stock = @Stock, FechaModificacion = GETDATE()
            WHERE Id = @Id
            """, e);
        return (await ObtenerPorIdConDetalleAsync(e.Id, ct))!;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteAsync(
            "UPDATE Productos SET Activo = 0, FechaModificacion = GETDATE() WHERE Id = @Id AND Activo = 1",
            new { Id = id }) > 0;
    }
}

public class DapperUnidadMedidaRepositorio(IDapperConnectionFactory factory) : IUnidadMedidaRepositorio
{
    private static UnidadMedida Map(dynamic r) => new()
        { Id = (int)r.Id, Codigo = (string)r.Codigo, Descripcion = (string)r.Descripcion, Activo = (bool)r.Activo };

    public async Task<IEnumerable<UnidadMedida>> ObtenerActivasAsync(CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync(
            "SELECT Id, Codigo, Descripcion, Activo FROM UnidadesMedida WHERE Activo = 1 ORDER BY Descripcion"))
            .Select(r => (UnidadMedida)Map(r));
    }

    public async Task<UnidadMedida?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync(
            "SELECT Id, Codigo, Descripcion, Activo FROM UnidadesMedida WHERE Id = @Id", new { Id = id });
        return row is null ? null : (UnidadMedida)Map(row);
    }

    public async Task<bool> ExisteAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM UnidadesMedida WHERE Id = @Id AND Activo = 1", new { Id = id }) > 0;
    }
}
