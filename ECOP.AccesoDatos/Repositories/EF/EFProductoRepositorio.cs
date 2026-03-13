using ECOP.AccesoDatos.Data.EF;

namespace ECOP.AccesoDatos.Repositories.EF;

public class EFProductoRepositorio(AppDbContext db) : IProductoRepositorio
{
    public async Task<IEnumerable<Producto>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        var productos = await db.Productos
            .Include(p => p.UnidadMedida)
            .ToListAsync(ct);
        return productos;
    }

    public async Task<IEnumerable<Producto>> ObtenerActivosAsync(CancellationToken ct = default)
    {
        var productos = await db.Productos
            .Include(p => p.UnidadMedida)
            .Where(p => p.Activo)
            .OrderBy(p => p.Codigo)
            .ToListAsync(ct);
        return productos;
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        Producto? producto = await db.Productos.FindAsync([id], ct);
        return producto;
    }

    public async Task<Producto?> ObtenerPorIdConDetalleAsync(int id, CancellationToken ct = default)
    {
        Producto? producto = await db.Productos
            .Include(p => p.UnidadMedida)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return producto;
    }

    public async Task<Producto?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default)
    {
        Producto? producto = await db.Productos
            .Include(p => p.UnidadMedida)
            .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Activo, ct);
        return producto;
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, int? excluirId = null, CancellationToken ct = default)
    {
        bool existe = await db.Productos.AnyAsync(p =>
            p.Codigo == codigo && (excluirId == null || p.Id != excluirId), ct);
        return existe;
    }

    public async Task<IEnumerable<Producto>> ObtenerPorIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var productos = await db.Productos
            .Where(p => ids.Contains(p.Id) && p.Activo)
            .ToListAsync(ct);
        return productos;
    }

    public async Task AjustarStockAsync(int productoId, int delta, CancellationToken ct = default)
    {
        await db.Productos
            .Where(p => p.Id == productoId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock + delta), ct);
    }

    public async Task<Producto> CrearAsync(Producto entidad, CancellationToken ct = default)
    {
        db.Productos.Add(entidad);
        await db.SaveChangesAsync(ct);
        await db.Entry(entidad).Reference(p => p.UnidadMedida).LoadAsync(ct);
        return entidad;
    }

    public async Task<Producto> ActualizarAsync(Producto entidad, CancellationToken ct = default)
    {
        db.Productos.Update(entidad);
        await db.SaveChangesAsync(ct);
        await db.Entry(entidad).Reference(p => p.UnidadMedida).LoadAsync(ct);
        return entidad;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken ct = default)
    {
        var filas = await db.Productos
            .Where(p => p.Id == id && p.Activo)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Activo,            false)
                .SetProperty(p => p.FechaModificacion, DateTime.Now), ct);
        return filas > 0;
    }
}

public class EFUnidadMedidaRepositorio(AppDbContext db) : IUnidadMedidaRepositorio
{
    public async Task<IEnumerable<UnidadMedida>> ObtenerActivasAsync(CancellationToken ct = default)
    {
        var lista = await db.UnidadesMedida
            .Where(u => u.Activo)
            .OrderBy(u => u.Descripcion)
            .ToListAsync(ct);

        return lista;
    }

    public async Task<UnidadMedida?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        UnidadMedida? unidad = await db.UnidadesMedida.FindAsync([id], ct);
        return unidad;
    }

    public async Task<bool> ExisteAsync(int id, CancellationToken ct = default)
    {
        bool existe = await db.UnidadesMedida.AnyAsync(u => u.Id == id && u.Activo, ct);
        return existe;
    }
}
