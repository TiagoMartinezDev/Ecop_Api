namespace ECOP.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TipoDocumento, TipoDocumentoDto>()
            .ConstructUsing(s => new(s.Id, s.Codigo, s.Descripcion, s.Activo));

        CreateMap<UnidadMedida, UnidadMedidaDto>()
            .ConstructUsing(s => new(s.Id, s.Codigo, s.Descripcion, s.Activo));

        CreateMap<EstadoPedido, EstadoPedidoDto>()
            .ConstructUsing(s => new(s.Id, s.Codigo, s.Descripcion));

        CreateMap<Cliente, ClienteDto>()
            .ConstructUsing((s, _) => new(
                s.Id, s.Nombre, s.Apellido,
                s.TipoDocumento != null ? s.TipoDocumento.Descripcion : string.Empty,
                s.NroDocumento, s.Email, s.Telefono,
                s.Activo, s.FechaAlta))
            .ForAllMembers(o => o.Ignore());

        CreateMap<ClienteCrearDto, Cliente>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Activo, o => o.MapFrom(_ => true))
            .ForMember(d => d.FechaAlta, o => o.MapFrom(_ => DateTime.Now))
            .ForMember(d => d.FechaModificacion, o => o.Ignore());

        CreateMap<ClienteActualizarDto, Cliente>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Activo, o => o.Ignore())
            .ForMember(d => d.FechaAlta, o => o.Ignore())
            .ForMember(d => d.FechaModificacion, o => o.Ignore());

        CreateMap<Producto, ProductoDto>()
            .ConstructUsing((s, _) => new(
                s.Id, s.Codigo, s.Descripcion,
                s.UnidadMedida != null ? s.UnidadMedida.Descripcion : string.Empty,
                s.PrecioUnitario, s.Stock, s.Activo, s.FechaAlta))
            .ForAllMembers(o => o.Ignore());

        CreateMap<ProductoCrearDto, Producto>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Activo, o => o.MapFrom(_ => true))
            .ForMember(d => d.FechaAlta, o => o.MapFrom(_ => DateTime.Now))
            .ForMember(d => d.FechaModificacion, o => o.Ignore());

        CreateMap<ProductoActualizarDto, Producto>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Codigo, o => o.Ignore())
            .ForMember(d => d.Activo, o => o.Ignore())
            .ForMember(d => d.FechaAlta, o => o.Ignore())
            .ForMember(d => d.FechaModificacion, o => o.Ignore());

        CreateMap<DetallePedido, DetallePedidoDto>()
            .ConstructUsing((s, _) => new(
                s.Id, s.ProductoId,
                s.Producto != null ? s.Producto.Codigo : string.Empty,
                s.Producto != null ? s.Producto.Descripcion : string.Empty,
                s.Producto?.UnidadMedida != null ? s.Producto.UnidadMedida.Descripcion : string.Empty,
                s.Cantidad, s.PrecioUnitario,
                s.Cantidad * s.PrecioUnitario))
            .ForAllMembers(o => o.Ignore());

        CreateMap<Pedido, PedidoDto>()
            .ConstructUsing((s, ctx) => new(
                s.Id, s.NumeroPedido, s.ClienteId,
                s.Cliente != null ? $"{s.Cliente.Nombre} {s.Cliente.Apellido}" : string.Empty,
                s.FechaPedido,
                s.Estado != null ? s.Estado.Descripcion : string.Empty,
                s.TotalMonto, s.Observaciones,
                ctx.Mapper.Map<List<DetallePedidoDto>>(s.Detalles)))
            .ForAllMembers(o => o.Ignore());
    }
}
