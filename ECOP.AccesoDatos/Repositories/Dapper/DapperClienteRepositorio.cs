using ECOP.AccesoDatos.Data.Dapper;

namespace ECOP.AccesoDatos.Repositories.Dapper;

public class DapperClienteRepositorio(IDapperConnectionFactory factory) : IClienteRepositorio
{
    private const string SelectClientesBase = """
        SELECT
            c.Id,
            c.Nombre, 
            c.Apellido, 
            c.TipoDocumentoId, 
            c.NroDocumento,
            c.Email, 
            c.Telefono, 
            c.Activo, 
            c.FechaAlta, 
            c.FechaModificacion,
            td.Id AS DocId,
            td.Codigo AS DocCodigo,
            td.Descripcion AS DocDescripcion, 
            td.Activo AS DocActivo,
            td.FechaAlta AS DocFechaAlta
        FROM Clientes c
        JOIN TiposDocumento td ON td.Id = c.TipoDocumentoId
        """;

    private static Cliente Map(dynamic row) => new()
    {
        Id                = (int)row.Id,
        Nombre            = (string)row.Nombre,
        Apellido          = (string)row.Apellido,
        TipoDocumentoId   = (int)row.TipoDocumentoId,
        NroDocumento      = (string)row.NroDocumento,
        Email             = (string?)row.Email,
        Telefono          = (string?)row.Telefono,
        Activo            = (bool)row.Activo,
        FechaAlta         = (DateTime)row.FechaAlta,
        FechaModificacion = (DateTime?)row.FechaModificacion,
        TipoDocumento = new TipoDocumento
        {
            Id          = (int)row.DocId,
            Codigo      = (string)row.DocCodigo,
            Descripcion = (string)row.DocDescripcion,
            Activo      = (bool)row.DocActivo,
            FechaAlta   = (DateTime)row.DocFechaAlta
        }
    };

    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync($"{SelectClientesBase} ORDER BY c.Apellido, c.Nombre")).Select(r => (Cliente)Map(r));
    }

    public async Task<IEnumerable<Cliente>> ObtenerActivosAsync(CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync($"{SelectClientesBase} WHERE c.Activo = 1 ORDER BY c.Apellido, c.Nombre")).Select(r => (Cliente)Map(r));
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync($"{SelectClientesBase} WHERE c.Id = @Id", new { Id = id });

        if (row == null) 
            return null;

        return (Cliente)Map(row);
    }

    public async Task<Cliente?> ObtenerPorIdConDetalleAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync($"{SelectClientesBase} WHERE c.Id = @Id AND c.Activo = 1", new { Id = id });
        if (row == null)
            return null;

        return (Cliente)Map(row);
    }

    public async Task<bool> ExisteDocumentoAsync(int tipoDocId, string nroDoc, int? excluirId = null, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>("""
            SELECT COUNT(1) FROM Clientes
            WHERE TipoDocumentoId = @TipoDocId
              AND NroDocumento    = @NroDoc
              AND (@ExcluirId IS NULL OR Id <> @ExcluirId)
            """, new { TipoDocId = tipoDocId, NroDoc = nroDoc, ExcluirId = excluirId }) > 0;
    }

    public async Task<Cliente> CrearAsync(Cliente e, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        e.Id = await conn.ExecuteScalarAsync<int>("""
            INSERT INTO Clientes (Nombre, Apellido, TipoDocumentoId, NroDocumento, Email, Telefono, Activo, FechaAlta)
            VALUES (@Nombre, @Apellido, @TipoDocumentoId, @NroDocumento, @Email, @Telefono, 1, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """, e);
        return (await ObtenerPorIdConDetalleAsync(e.Id, ct))!;
    }

    public async Task<Cliente> ActualizarAsync(Cliente e, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        await conn.ExecuteAsync("""
            UPDATE Clientes SET
                Nombre            = @Nombre,
                Apellido          = @Apellido,
                TipoDocumentoId   = @TipoDocumentoId,
                NroDocumento      = @NroDocumento,
                Email             = @Email,
                Telefono          = @Telefono,
                FechaModificacion = GETDATE()
            WHERE Id = @Id
            """, e);
        return (await ObtenerPorIdConDetalleAsync(e.Id, ct))!;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteAsync(
            "UPDATE Clientes SET Activo = 0, FechaModificacion = GETDATE() WHERE Id = @Id AND Activo = 1",
            new { Id = id }) > 0;
    }
}

public class DapperTipoDocumentoRepositorio(IDapperConnectionFactory factory) : ITipoDocumentoRepositorio
{
    private static TipoDocumento Map(dynamic r) => new()
    {
        Id = (int)r.Id, Codigo = (string)r.Codigo,
        Descripcion = (string)r.Descripcion, Activo = (bool)r.Activo, FechaAlta = (DateTime)r.FechaAlta
    };

    public async Task<IEnumerable<TipoDocumento>> ObtenerActivosAsync(CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync(
            "SELECT Id, Codigo, Descripcion, Activo, FechaAlta FROM TiposDocumento WHERE Activo = 1 ORDER BY Descripcion"))
            .Select(r => (TipoDocumento)Map(r));
    }

    public async Task<TipoDocumento?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync(
            "SELECT Id, Codigo, Descripcion, Activo, FechaAlta FROM TiposDocumento WHERE Id = @Id", new { Id = id });
        if (row == null)
            return null;

        return (TipoDocumento)Map(row);
    }

    public async Task<bool> ExisteAsync(int id, CancellationToken ct = default)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM TiposDocumento WHERE Id = @Id AND Activo = 1", new { Id = id }) > 0;
    }
}
