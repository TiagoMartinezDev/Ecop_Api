using ECOP.AccesoDatos.Data.EF;

namespace ECOP.AccesoDatos.Repositories.EF;

public class EFClienteRepositorio(AppDbContext db) : IClienteRepositorio
{
    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        var listaCompleta = await db.Clientes
            .Include(c => c.TipoDocumento)
            .ToListAsync(ct);
        return listaCompleta;
    }

    public async Task<IEnumerable<Cliente>> ObtenerActivosAsync(CancellationToken ct = default)
    {
        var clientes = await db.Clientes
            .Include(c => c.TipoDocumento)
            .Where(c => c.Activo)
            .OrderBy(c => c.Apellido)
            .ThenBy(c => c.Nombre)
            .ToListAsync(ct);
        return clientes;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        Cliente? cliente = await db.Clientes.FindAsync([id], ct);
        return cliente;
    }

    public async Task<Cliente?> ObtenerPorIdConDetalleAsync(int id, CancellationToken ct = default)
    {
        Cliente? cliente = await db.Clientes
            .Include(c => c.TipoDocumento)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        return cliente;
    }

    public async Task<bool> ExisteDocumentoAsync(int tipoDocId, string nroDoc, int? excluirId = null, CancellationToken ct = default)
    {
        bool existe = await db.Clientes.AnyAsync(c =>
            c.TipoDocumentoId == tipoDocId &&
            c.NroDocumento == nroDoc &&
            (excluirId == null || c.Id != excluirId), ct);
        return existe;
    }

    public async Task<Cliente> CrearAsync(Cliente entidad, CancellationToken ct = default)
    {
        db.Clientes.Add(entidad);
        await db.SaveChangesAsync(ct);
        await db.Entry(entidad).Reference(c => c.TipoDocumento).LoadAsync(ct);
        return entidad;
    }

    public async Task<Cliente> ActualizarAsync(Cliente entidad, CancellationToken ct = default)
    {
        db.Clientes.Update(entidad);
        await db.SaveChangesAsync(ct);
        await db.Entry(entidad).Reference(c => c.TipoDocumento).LoadAsync(ct);
        return entidad;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken ct = default)
    {
        var filas = await db.Clientes
            .Where(c => c.Id == id && c.Activo)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.Activo, false)
                .SetProperty(c => c.FechaModificacion, DateTime.Now), ct);
        return filas > 0;
    }
}

public class EFTipoDocumentoRepositorio(AppDbContext db) : ITipoDocumentoRepositorio
{
    public async Task<IEnumerable<TipoDocumento>> ObtenerActivosAsync(CancellationToken ct = default)
    {
        var resultados = await db.TiposDocumento
            .Where(t => t.Activo)
            .OrderBy(t => t.Descripcion)
            .ToListAsync(ct);
        return resultados;
    }

    public async Task<TipoDocumento?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        TipoDocumento? tipo = await db.TiposDocumento.FindAsync([id], ct);
        return tipo;
    }

    public async Task<bool> ExisteAsync(int id, CancellationToken ct = default)
    {
        bool existe = await db.TiposDocumento.AnyAsync(t => t.Id == id, ct);
        return existe;
    }
}
