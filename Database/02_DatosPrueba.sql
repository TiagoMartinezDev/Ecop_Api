USE master;
GO

--TiposDocumento
BEGIN
    SET IDENTITY_INSERT TiposDocumento ON;
    INSERT INTO TiposDocumento (Id, Codigo, Descripcion, Activo, FechaAlta) VALUES
        (1, 'CI',  'Cédula de Identidad', 1, GETDATE()),
        (2, 'RUC', 'RUC',                 1, GETDATE()),
        (3, 'PAS', 'Pasaporte',           1, GETDATE()),
        (4, 'DNI', 'Documento Nacional',  1, GETDATE());
    SET IDENTITY_INSERT TiposDocumento OFF;
END
GO

--UnidadesMedida
BEGIN
    SET IDENTITY_INSERT UnidadesMedida ON;
    INSERT INTO UnidadesMedida (Id, Codigo, Descripcion, Activo) VALUES
        (1, 'UN',  'Unidad',    1),
        (2, 'KG',  'Kilogramo', 1),
        (3, 'LT',  'Litro',     1),
        (4, 'MT',  'Metro',     1),
        (5, 'CJ',  'Caja',      1),
        (6, 'BL',  'Bolsa',     1),
        (7, 'PQ',  'Paquete',   1);
    SET IDENTITY_INSERT UnidadesMedida OFF;
END
GO

--EstadosPedido
BEGIN
    SET IDENTITY_INSERT EstadosPedido ON;
    INSERT INTO EstadosPedido (Id, Codigo, Descripcion) VALUES
        (1, 'PENDIENTE',  'Pendiente'),
        (2, 'CONFIRMADO', 'Confirmado'),
        (3, 'EN_PROCESO', 'En Proceso'),
        (4, 'DESPACHADO', 'Despachado'),
        (5, 'ENTREGADO',  'Entregado'),
        (6, 'CANCELADO',  'Cancelado');
    SET IDENTITY_INSERT EstadosPedido OFF;
END
GO

--Clientes
BEGIN
    SET IDENTITY_INSERT Clientes ON;
    INSERT INTO Clientes (Id, Nombre, Apellido, TipoDocumentoId, NroDocumento, Email, Telefono, Activo, FechaAlta) VALUES
        (1,  'Carlos',       'Rodríguez', 1, '3456789',    'carlos.rodriguez@gmail.com',    '0981-111222', 1, GETDATE()),
        (2,  'María',        'González',  1, '4567890',    'maria.gonzalez@hotmail.com',    '0982-222333', 1, GETDATE()),
        (3,  'Luis',         'Martínez',  1, '5678901',    'luis.martinez@yahoo.com',       '0983-333444', 1, GETDATE()),
        (4,  'Ana',          'Pérez',     1, '6789012',    'ana.perez@gmail.com',           '0984-444555', 1, GETDATE()),
        (5,  'Roberto',      'Sánchez',   2, '80012345-1', 'roberto.sanchez@empresa.com',   '021-555666',  1, GETDATE()),
        (6,  'Laura',        'Fernández', 1, '7890123',    'laura.fernandez@gmail.com',     '0985-555666', 1, GETDATE()),
        (7,  'Diego',        'López',     1, '8901234',    'diego.lopez@outlook.com',       '0986-666777', 1, GETDATE()),
        (8,  'Sofía',        'Ramírez',   1, '9012345',    'sofia.ramirez@gmail.com',       '0987-777888', 1, GETDATE()),
        (9,  'Distribuidora','Norte',     2, '90034567-2', 'compras@distribuidoranorte.com','021-888999',  1, GETDATE()),
        (10, 'Jorge',        'Benítez',   1, '1234567',    'jorge.benitez@gmail.com',       '0988-888999', 1, GETDATE());
    SET IDENTITY_INSERT Clientes OFF;
END
GO

-- Productos
BEGIN
    SET IDENTITY_INSERT Productos ON;
    INSERT INTO Productos (Id, Codigo, Descripcion, UnidadMedidaId, PrecioUnitario, Stock, Activo, FechaAlta) VALUES
        (1,  'ACE-001', 'Aceite de Soja 900ml',       3,  12500.00, 150, 1, GETDATE()),
        (2,  'ARR-001', 'Arroz Largo Fino 1kg',        2,   8900.00, 300, 1, GETDATE()),
        (3,  'AZU-001', 'Azúcar Blanca 1kg',           2,   7500.00, 250, 1, GETDATE()),
        (4,  'HAR-001', 'Harina de Trigo 1kg',         2,   6800.00, 200, 1, GETDATE()),
        (5,  'FID-001', 'Fideos Spaghetti 500g',       7,   5500.00, 180, 1, GETDATE()),
        (6,  'FID-002', 'Fideos Mostachol 500g',       7,   5500.00, 160, 1, GETDATE()),
        (7,  'SAL-001', 'Sal Fina 1kg',                2,   3200.00, 400, 1, GETDATE()),
        (8,  'MAI-001', 'Maizena 500g',                7,   9800.00, 120, 1, GETDATE()),
        (9,  'GAL-001', 'Galletitas Surtidas x100u',   5,  18500.00,  80, 1, GETDATE()),
        (10, 'LEC-001', 'Leche Entera 1L',             3,   7200.00, 200, 1, GETDATE()),
        (11, 'LEC-002', 'Leche Descremada 1L',         3,   7800.00, 150, 1, GETDATE()),
        (12, 'YOG-001', 'Yogur Natural 200g',          1,   4500.00, 100, 1, GETDATE()),
        (13, 'QUE-001', 'Queso Paraguay 1kg',          2,  45000.00,  60, 1, GETDATE()),
        (14, 'MAN-001', 'Manteca 200g',                1,  12000.00,  90, 1, GETDATE()),
        (15, 'JAB-001', 'Jabón en Polvo 1kg',          6,  15500.00, 120, 1, GETDATE()),
        (16, 'JAB-002', 'Jabón de Tocador x3u',        7,   8900.00, 150, 1, GETDATE()),
        (17, 'LAV-001', 'Lavandina 1L',                3,   6500.00, 130, 1, GETDATE()),
        (18, 'DET-001', 'Detergente Líquido 500ml',    3,   9200.00, 110, 1, GETDATE()),
        (19, 'PAP-001', 'Papel Higiénico x4u',         7,  12800.00, 200, 1, GETDATE()),
        (20, 'PAP-002', 'Papel de Cocina x2u',         7,   8500.00, 160, 1, GETDATE());
    SET IDENTITY_INSERT Productos OFF;
END
GO

--Pedidos
BEGIN
    SET IDENTITY_INSERT Pedidos ON;
    INSERT INTO Pedidos (Id, NumeroPedido, ClienteId, FechaPedido, EstadoId, TotalMonto, Observaciones) VALUES
        (1,  '202601000001', 1,  DATEADD(DAY, -20, GETDATE()), 5,  91800.00,  'Entrega en domicilio'),
        (2,  '202601000002', 2,  DATEADD(DAY, -18, GETDATE()), 5,  54500.00,  NULL),
        (3,  '202602000001', 3,  DATEADD(DAY, -15, GETDATE()), 4,  78200.00,  'Llamar antes de entregar'),
        (4,  '202602000002', 5,  DATEADD(DAY, -12, GETDATE()), 3, 185000.00,  'Pedido corporativo mensual'),
        (5,  '202602000003', 4,  DATEADD(DAY, -10, GETDATE()), 2,  47500.00,  NULL),
        (6,  '202603000001', 6,  DATEADD(DAY,  -7, GETDATE()), 2,  62400.00,  NULL),
        (7,  '202603000002', 9,  DATEADD(DAY,  -5, GETDATE()), 3, 320000.00,  'Pedido mayorista'),
        (8,  '202603000003', 7,  DATEADD(DAY,  -3, GETDATE()), 1,  38700.00,  NULL),
        (9,  '202603000004', 8,  DATEADD(DAY,  -2, GETDATE()), 1,  55200.00,  NULL),
        (10, '202603000005', 10, DATEADD(DAY,  -1, GETDATE()), 6,  29000.00,  'Cancelado por el cliente');
    SET IDENTITY_INSERT Pedidos OFF;
END
GO

--DetallePedidos
BEGIN
    SET IDENTITY_INSERT DetallePedidos ON;
    INSERT INTO DetallePedidos (Id, PedidoId, ProductoId, Cantidad, PrecioUnitario) VALUES
        (1,  1,  1,  3,  12500.00),
        (2,  1,  2,  5,   8900.00),
        (3,  1,  3,  2,   7500.00),
        (4,  2,  5,  4,   5500.00),
        (5,  2,  7,  3,   3200.00),
        (6,  2, 10,  3,   7200.00),
        (7,  3, 13,  1,  45000.00),
        (8,  3, 14,  2,  12000.00),
        (9,  3, 12,  2,   4500.00),
        (10, 4,  1, 10,  12500.00),
        (11, 4,  2, 10,   8900.00),
        (12, 5, 15,  2,  15500.00),
        (13, 5, 19,  1,  12800.00),
        (14, 5, 18,  2,   9200.00),
        (15, 6,  4,  5,   6800.00),
        (16, 6,  8,  2,   9800.00),
        (17, 6,  9,  1,  18500.00),
        (18, 7,  1, 20,  12500.00),
        (19, 7,  2, 10,   8900.00),
        (20, 7, 15,  5,  15500.00),
        (21, 8, 16,  2,   8900.00),
        (22, 8, 17,  3,   6500.00),
        (23, 9, 10,  4,   7200.00),
        (24, 9, 11,  2,   7800.00),
        (25, 9, 12,  3,   4500.00),
        (26, 10, 6,  2,   5500.00),
        (27, 10, 7,  3,   3200.00);
    SET IDENTITY_INSERT DetallePedidos OFF;
END
GO