using ECOP.AccesoDatos.Data.Dapper;
using ECOP.AccesoDatos.Data.EF;
using ECOP.AccesoDatos.Repositories.Dapper;
using ECOP.AccesoDatos.Repositories.EF;
using ECOP.AccesoDatos.Repositories.Interfaces;
using ECOP.API.Services.Implementations;
using ECOP.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Verificar que proveedor de datos usar (Dapper o Entity Framework)
var proveedor  = builder.Configuration.GetValue<string>("DataProvider") ?? "EntityFramework";
var usarDapper = proveedor.Equals("Dapper", StringComparison.OrdinalIgnoreCase);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(maxRetryCount: 3)));

builder.Services.AddSingleton<IDapperConnectionFactory, DapperConnectionFactory>();

if (usarDapper)
{
    builder.Services.AddScoped<IClienteRepositorio, DapperClienteRepositorio>();
    builder.Services.AddScoped<ITipoDocumentoRepositorio, DapperTipoDocumentoRepositorio>();
    builder.Services.AddScoped<IProductoRepositorio, DapperProductoRepositorio>();
    builder.Services.AddScoped<IUnidadMedidaRepositorio, DapperUnidadMedidaRepositorio>();
    builder.Services.AddScoped<IEstadoPedidoRepositorio, DapperEstadoPedidoRepositorio>();
    builder.Services.AddScoped<IPedidoRepositorio, DapperPedidoRepositorio>();
}
else
{
    builder.Services.AddScoped<IClienteRepositorio, EFClienteRepositorio>();
    builder.Services.AddScoped<ITipoDocumentoRepositorio, EFTipoDocumentoRepositorio>();
    builder.Services.AddScoped<IProductoRepositorio, EFProductoRepositorio>();
    builder.Services.AddScoped<IUnidadMedidaRepositorio, EFUnidadMedidaRepositorio>();
    builder.Services.AddScoped<IEstadoPedidoRepositorio, EFEstadoPedidoRepositorio>();
    builder.Services.AddScoped<IPedidoRepositorio, EFPedidoRepositorio>();
}

builder.Services.AddScoped<IClienteServicio, ClienteService>();
builder.Services.AddScoped<IProductoServicio, ProductoService>();
builder.Services.AddScoped<IPedidoServicio, PedidoService>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "ECOP — API Pedidos de Mercaderías",
        Version     = "v1",
        Description = $"Proveedor de datos activo: **{proveedor}**"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        opt.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", $"ECOP API v1 [{proveedor}]");
    opt.RoutePrefix   = string.Empty;
    opt.DocumentTitle = "ECOP — Swagger UI";
});

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
