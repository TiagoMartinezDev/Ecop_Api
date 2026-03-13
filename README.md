# ECOP — API Pedidos de Mercaderías

API en **.NET 9 / C# 13** para la administración completa de clientes, productos y pedidos de mercaderías.

---
## Requisitos

| Componente | Versión |
|---|---|
| .NET SDK | 9.0 |
| Visual Studio | 2022 v17.8+ |
| SQL Server Express | 2019 o 2022 |
---



## ⚙ Configuración inicial

### 1. Cadena de conexión

Editá `ECOP.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SERVIDOR\\INSTANCIA;Initial Catalog=ECOP_PedidosMercaderias;User ID=sa;Password=TU_PASSWORD;Encrypt=True;TrustServerCertificate=True"
  }
}
```

### 2. Elegir proveedor de datos

En `appsettings.json` cambiar esta línea dependiendo al proveedor de datos:

```json
"DataProvider": "EntityFramework"   
"DataProvider": "Dapper"            
```

Con un solo cambio el sistema cambia de implementación sin tocar código.

---

## 🗄 Creación de la base de datos

Elegir **una sola** de las opciones:

### Opción A — Migraciones de EF Core ✅ Recomendada

Abrir la **Package Manager Console** en Visual Studio (`Herramientas → NuGet → Package Manager Console`) y ejecutá:

```powershell
Add-Migration InitialMigration -Project ECOP.AccesoDatos -StartupProject ECOP.API
Update-Database -Project ECOP.AccesoDatos -StartupProject ECOP.API
```

EF Core creará la base de datos y todas las tablas automáticamente.


### Opción B — Scripts SQL manuales

Ejecutá en SSMS en este orden:

```
Database/01_CrearTablas.sql    → crea la base de datos y todas las tablas
Database/02_DatosPrueba.sql    → inserta datos de prueba
```

> **Si usás la Opción B y luego querés usar migraciones**, ejecutá esto para que EF no intente recrear las tablas:
> ```sql
> INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
> VALUES ('InitialMigration', '9.0.4');
> ```

---

### URLs disponibles

| Recurso | URL |
|---|---|
| Swagger UI | `https://localhost:7001/swagger` |
| Clientes | `https://localhost:7001/api/clientes` |
| Productos | `https://localhost:7001/api/productos` |
| Pedidos | `https://localhost:7001/api/pedidos` |

---

## 🔌 Endpoints

### Clientes `/api/clientes`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/clientes` | Listar activos |
| GET | `/api/clientes/{id}` | Por Id |
| GET | `/api/clientes/tipos-documento` | Tipos de documento |
| POST | `/api/clientes` | Crear |
| PUT | `/api/clientes/{id}` | Actualizar |
| DELETE | `/api/clientes/{id}` | Baja lógica |

### Productos `/api/productos`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/productos` | Listar activos |
| GET | `/api/productos/{id}` | Por Id |
| GET | `/api/productos/codigo/{codigo}` | Por código |
| GET | `/api/productos/unidades-medida` | Unidades de medida |
| POST | `/api/productos` | Crear |
| PUT | `/api/productos/{id}` | Actualizar |
| DELETE | `/api/productos/{id}` | Baja lógica |

### Pedidos `/api/pedidos`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/pedidos` | Listar (filtros: `?clienteId=&estadoCodigo=`) |
| GET | `/api/pedidos/{id}` | Por Id |
| GET | `/api/pedidos/numero/{nro}` | Por número |
| GET | `/api/pedidos/estados` | Estados disponibles |
| POST | `/api/pedidos` | Crear pedido |
| PUT | `/api/pedidos/{id}` | Actualiza pedido |
| PATCH | `/api/pedidos/{id}/estado` | Cambiar estado |
| DELETE | `/api/pedidos/{id}` | Cancelar |

---

## 🏗 Estructura de la solución

```
ECOP.sln
├── ECOP.Models/                        → Entidades dependencias externas
│   └── Models/
│
├── ECOP.AccesoDatos/                   → Capa de acceso a datos
│   ├── Data/
│   │   ├── EF/AppDbContext.cs          → DbContext EF Core 9
│   │   └── Dapper/DapperConnectionFactory.cs
│   └── Repositories/
│       ├── Interfaces/
│       ├── EF/                         → Implementaciones con EF Core 9
│       └── Dapper/                     → Implementaciones con Dapper
│
├── ECOP.API/
│   ├── Controllers/
│   ├── Services/
│   ├── DTOs/
│   ├── Mappings/
│   ├── Program.cs
│   └── appsettings.json
│
└── Database/
    ├── InsertDatos.sql
    └── Tablas.sql
```


---

## ⚠ Notas importantes

**Soft delete** — Los Clientes y Productos no se eliminan físicamente de la BD. El campo `Activo = false` los marca como inactivos y dejan de aparecer en las consultas.

**Stock** — Al crear un pedido el stock se descuenta automáticamente. Al cancelarlo se restituye.