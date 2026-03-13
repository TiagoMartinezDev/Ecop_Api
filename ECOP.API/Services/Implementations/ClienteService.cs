using ECOP.API.Services.Interfaces;

namespace ECOP.API.Services.Implementations;
public class ClienteService(IClienteRepositorio clienteRepo, ITipoDocumentoRepositorio tipoDocRepo, IMapper mapper) : IClienteServicio
{
    public async Task<IEnumerable<ClienteDto>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        var clientesActivos = await clienteRepo.ObtenerTodosAsync(ct);
        return mapper.Map<IEnumerable<ClienteDto>>(clientesActivos); ;
    }

    public async Task<ClienteDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        var cliente = await clienteRepo.ObtenerPorIdConDetalleAsync(id, ct);

        if (cliente is null)
            return null;

        return mapper.Map<ClienteDto>(cliente);
    }

    public async Task<IEnumerable<TipoDocumentoDto>> ObtenerTiposDocumentoAsync(CancellationToken ct = default)
    {
        var docActivos = await tipoDocRepo.ObtenerActivosAsync(ct);
        return mapper.Map<IEnumerable<TipoDocumentoDto>>(docActivos); ;
    }

    public async Task<(ClienteDto? Datos, string? Error)> CrearAsync(ClienteCrearDto dto, CancellationToken ct = default)
    {
        if (!await tipoDocRepo.ExisteAsync(dto.TipoDocumentoId, ct))
            return (null, "Tipo de documento inválido.");

        if (await clienteRepo.ExisteDocumentoAsync(dto.TipoDocumentoId, dto.NroDocumento, null, ct))
            return (null, "Ya existe un cliente registrado con ese documento.");

        var entidad  = mapper.Map<Cliente>(dto);
        var creado   = await clienteRepo.CrearAsync(entidad, ct);
        return (mapper.Map<ClienteDto>(creado), null);
    }

    public async Task<(ClienteDto? Datos, string? Error)> ActualizarAsync(int id, ClienteActualizarDto dto, CancellationToken ct = default)
    {
        var cliente = await clienteRepo.ObtenerPorIdAsync(id, ct);
        if (cliente is null)
            return (null, $"Cliente con Id {id} no encontrado.");

        if (!await tipoDocRepo.ExisteAsync(dto.TipoDocumentoId, ct))
            return (null, "Tipo de documento inválido.");

        if (await clienteRepo.ExisteDocumentoAsync(dto.TipoDocumentoId, dto.NroDocumento, id, ct))
            return (null, "Ya existe otro cliente con ese documento.");

        mapper.Map(dto, cliente);
        cliente.FechaModificacion = DateTime.Now;
        var clienteActualizado = await clienteRepo.ActualizarAsync(cliente, ct);
        return (mapper.Map<ClienteDto>(clienteActualizado), null);
    }

    public async Task<(bool Ok, string? Error)> EliminarAsync(int id, CancellationToken ct = default)
    {
        var cliente = await clienteRepo.ObtenerPorIdAsync(id, ct);
        if (cliente is null || !cliente.Activo)
            return (false, $"Cliente con Id {id} no encontrado o desactivo.");

        var clienteEliminado = await clienteRepo.EliminarAsync(id, ct);
        return (clienteEliminado, clienteEliminado ? null : "No se pudo eliminar el cliente.");
    }
}