
USE master;
GO


BEGIN
    CREATE TABLE TiposDocumento (
        Id          INT           NOT NULL IDENTITY(1,1),
        Codigo      NVARCHAR(10)  NOT NULL,
        Descripcion NVARCHAR(50)  NOT NULL,
        Activo      BIT           NOT NULL DEFAULT 1,
        FechaAlta   DATETIME2     NOT NULL,
        CONSTRAINT PK_TiposDocumento PRIMARY KEY (Id)
    );

    CREATE UNIQUE INDEX IX_TiposDocumento_Codigo ON TiposDocumento (Codigo);

    PRINT 'Tabla TiposDocumento creada.';
END
GO

BEGIN
    CREATE TABLE Clientes (
        Id                INT           NOT NULL IDENTITY(1,1),
        Nombre            NVARCHAR(100) NOT NULL,
        Apellido          NVARCHAR(100) NOT NULL,
        TipoDocumentoId   INT           NOT NULL,
        NroDocumento      NVARCHAR(20)  NOT NULL,
        Email             NVARCHAR(150) NULL,
        Telefono          NVARCHAR(20)  NULL,
        Activo            BIT           NOT NULL DEFAULT 1,
        FechaAlta         DATETIME2     NOT NULL,
        FechaModificacion DATETIME2     NULL,
        CONSTRAINT PK_Clientes PRIMARY KEY (Id),
        CONSTRAINT FK_Clientes_TiposDocumento FOREIGN KEY (TipoDocumentoId)
            REFERENCES TiposDocumento (Id) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX IX_Clientes_TipoDocumentoId_NroDocumento
        ON Clientes (TipoDocumentoId, NroDocumento);

    PRINT 'Tabla Clientes creada.';
END
GO

BEGIN
    CREATE TABLE UnidadesMedida (
        Id          INT          NOT NULL IDENTITY(1,1),
        Codigo      NVARCHAR(10) NOT NULL,
        Descripcion NVARCHAR(50) NOT NULL,
        Activo      BIT          NOT NULL DEFAULT 1,
        CONSTRAINT PK_UnidadesMedida PRIMARY KEY (Id)
    );

    CREATE UNIQUE INDEX IX_UnidadesMedida_Codigo ON UnidadesMedida (Codigo);

    PRINT 'Tabla UnidadesMedida creada.';
END
GO

BEGIN
    CREATE TABLE Productos (
        Id                INT            NOT NULL IDENTITY(1,1),
        Codigo            NVARCHAR(20)   NOT NULL,
        Descripcion       NVARCHAR(200)  NOT NULL,
        UnidadMedidaId    INT            NOT NULL,
        PrecioUnitario    DECIMAL(18,2)  NOT NULL,
        Stock             INT            NOT NULL DEFAULT 0,
        Activo            BIT            NOT NULL DEFAULT 1,
        FechaAlta         DATETIME2      NOT NULL,
        FechaModificacion DATETIME2      NULL,
        CONSTRAINT PK_Productos PRIMARY KEY (Id),
        CONSTRAINT FK_Productos_UnidadesMedida FOREIGN KEY (UnidadMedidaId)
            REFERENCES UnidadesMedida (Id) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX IX_Productos_Codigo ON Productos (Codigo);

    PRINT 'Tabla Productos creada.';
END
GO

BEGIN
    CREATE TABLE EstadosPedido (
        Id          INT          NOT NULL IDENTITY(1,1),
        Codigo      NVARCHAR(20) NOT NULL,
        Descripcion NVARCHAR(50) NOT NULL,
        CONSTRAINT PK_EstadosPedido PRIMARY KEY (Id)
    );

    CREATE UNIQUE INDEX IX_EstadosPedido_Codigo ON EstadosPedido (Codigo);

    PRINT 'Tabla EstadosPedido creada.';
END
GO

BEGIN
    CREATE TABLE Pedidos (
        Id                INT           NOT NULL IDENTITY(1,1),
        NumeroPedido      NVARCHAR(20)  NOT NULL,
        ClienteId         INT           NOT NULL,
        FechaPedido       DATETIME2     NOT NULL,
        EstadoId          INT           NOT NULL,
        TotalMonto        DECIMAL(18,2) NOT NULL,
        Observaciones     NVARCHAR(500) NULL,
        FechaModificacion DATETIME2     NULL,
        CONSTRAINT PK_Pedidos PRIMARY KEY (Id),
        CONSTRAINT FK_Pedidos_Clientes FOREIGN KEY (ClienteId)
            REFERENCES Clientes (Id) ON DELETE NO ACTION,
        CONSTRAINT FK_Pedidos_EstadosPedido FOREIGN KEY (EstadoId)
            REFERENCES EstadosPedido (Id) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX IX_Pedidos_NumeroPedido ON Pedidos (NumeroPedido);

    PRINT 'Tabla Pedidos creada.';
END
GO

BEGIN
    CREATE TABLE DetallePedidos (
        Id             INT           NOT NULL IDENTITY(1,1),
        PedidoId       INT           NOT NULL,
        ProductoId     INT           NOT NULL,
        Cantidad       INT           NOT NULL,
        PrecioUnitario DECIMAL(18,2) NOT NULL,
        CONSTRAINT PK_DetallePedidos PRIMARY KEY (Id),
        CONSTRAINT FK_DetallePedidos_Pedidos FOREIGN KEY (PedidoId)
            REFERENCES Pedidos (Id) ON DELETE CASCADE,
        CONSTRAINT FK_DetallePedidos_Productos FOREIGN KEY (ProductoId)
            REFERENCES Productos (Id) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX IX_DetallePedidos_PedidoId_ProductoId
        ON DetallePedidos (PedidoId, ProductoId);

    PRINT 'Tabla DetallePedidos creada.';
END
GO

BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL,
        ProductVersion NVARCHAR(32)  NOT NULL,
        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );

    PRINT 'Tabla __EFMigrationsHistory creada.';
END
GO
