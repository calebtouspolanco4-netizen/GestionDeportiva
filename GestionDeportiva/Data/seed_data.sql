-- ============================================================
-- La Jugada — Script de Datos de Prueba
-- Ejecutar en orden sobre la BD LaJugadaDB
-- ============================================================

USE [LaJugadaDB];
GO

-- Limpiar datos existentes (en orden por FK)
DELETE FROM [dbo].[Pagos];
DELETE FROM [dbo].[HistorialNotificaciones];
DELETE FROM [dbo].[AlquileresReserva];
DELETE FROM [dbo].[Reservas];
DELETE FROM [dbo].[AuditoriaTransacciones];
DELETE FROM [dbo].[TurnosCaja];
DELETE FROM [dbo].[TarifasEspeciales];
DELETE FROM [dbo].[MantenimientosEscenario];
DELETE FROM [dbo].[Escenarios];
DELETE FROM [dbo].[Clientes];
DELETE FROM [dbo].[Empleados];
DELETE FROM [dbo].[Usuarios];
DELETE FROM [dbo].[DetallesVenta];
DELETE FROM [dbo].[Ventas];
DELETE FROM [dbo].[MovimientosInventario];
DELETE FROM [dbo].[Productos];
DELETE FROM [dbo].[CategoriasProducto];
DELETE FROM [dbo].[MetodosPago];
DELETE FROM [dbo].[EstadosReserva];
DELETE FROM [dbo].[EstadosEscenario];
DELETE FROM [dbo].[TiposDeporte];
DELETE FROM [dbo].[Establecimientos];
DELETE FROM [dbo].[Roles];
GO

-- Reset identities
DBCC CHECKIDENT ('Roles',              RESEED, 0);
DBCC CHECKIDENT ('Establecimientos',   RESEED, 0);
DBCC CHECKIDENT ('TiposDeporte',       RESEED, 0);
DBCC CHECKIDENT ('EstadosEscenario',   RESEED, 0);
DBCC CHECKIDENT ('EstadosReserva',     RESEED, 0);
DBCC CHECKIDENT ('MetodosPago',        RESEED, 0);
DBCC CHECKIDENT ('CategoriasProducto', RESEED, 0);
DBCC CHECKIDENT ('Usuarios',           RESEED, 0);
DBCC CHECKIDENT ('Empleados',          RESEED, 0);
DBCC CHECKIDENT ('Clientes',           RESEED, 0);
DBCC CHECKIDENT ('Escenarios',         RESEED, 0);
DBCC CHECKIDENT ('Reservas',           RESEED, 0);
DBCC CHECKIDENT ('Pagos',              RESEED, 0);
DBCC CHECKIDENT ('TurnosCaja',         RESEED, 0);
DBCC CHECKIDENT ('Productos',          RESEED, 0);
GO

-- ============================================================
-- 1. ROLES
-- ============================================================
INSERT INTO [dbo].[Roles] ([nombre]) VALUES
  ('Administrador'),
  ('Empleado'),
  ('Cliente');
GO

-- ============================================================
-- 2. ESTABLECIMIENTOS
-- ============================================================
INSERT INTO [dbo].[Establecimientos] ([nombre], [direccion], [telefono], [activo]) VALUES
  ('La Jugada - Sede Principal', 'Calle 72 # 45-30, Bogotá', '601 234 5678', 1);
GO

-- ============================================================
-- 3. TIPOS DE DEPORTE
-- ============================================================
INSERT INTO [dbo].[TiposDeporte] ([nombre], [jugadores_por_equipo]) VALUES
  ('Fútbol 11', 11),
  ('Fútbol 7',   7),
  ('Fútbol 8',   8),
  ('Fútbol 5',   5),
  ('Microfútbol', 5);
GO

-- ============================================================
-- 4. ESTADOS DE ESCENARIO
-- ============================================================
INSERT INTO [dbo].[EstadosEscenario] ([nombre]) VALUES
  ('Disponible'),
  ('Mantenimiento'),
  ('Ocupado'),
  ('Cerrado');
GO

-- ============================================================
-- 5. ESTADOS DE RESERVA
-- ============================================================
INSERT INTO [dbo].[EstadosReserva] ([nombre]) VALUES
  ('Confirmada'),
  ('Pendiente'),
  ('Cancelada'),
  ('Completada');
GO

-- ============================================================
-- 6. MÉTODOS DE PAGO
-- ============================================================
INSERT INTO [dbo].[MetodosPago] ([nombre]) VALUES
  ('Efectivo'),
  ('Transferencia'),
  ('Tarjeta débito'),
  ('Tarjeta crédito'),
  ('Nequi'),
  ('Daviplata');
GO

-- ============================================================
-- 7. CATEGORÍAS DE PRODUCTO
-- ============================================================
INSERT INTO [dbo].[CategoriasProducto] ([nombre]) VALUES
  ('Equipamiento deportivo'),
  ('Alquiler de implementos'),
  ('Bebidas'),
  ('Snacks');
GO

-- ============================================================
-- 8. USUARIOS (password_hash = "Admin123!" hasheado simulado)
-- ============================================================
INSERT INTO [dbo].[Usuarios] ([email], [password_hash], [role_id], [activo]) VALUES
  ('admin@lajugada.com',    'AQAAAAIAAYagAAAAEHx...hash1', 1, 1),  -- Administrador
  ('empleado1@lajugada.com','AQAAAAIAAYagAAAAEHx...hash2', 2, 1),  -- Empleado
  ('empleado2@lajugada.com','AQAAAAIAAYagAAAAEHx...hash3', 2, 1),
  ('juan.perez@gmail.com',  'AQAAAAIAAYagAAAAEHx...hash4', 3, 1),  -- Clientes
  ('carlos.gomez@gmail.com','AQAAAAIAAYagAAAAEHx...hash5', 3, 1),
  ('maria.rodriguez@gmail.com','AQAAAAIAAYagAAAAEHx...hash6', 3, 1),
  ('luis.martinez@gmail.com',  'AQAAAAIAAYagAAAAEHx...hash7', 3, 1),
  ('andres.andrez@gmail.com',  'AQAAAAIAAYagAAAAEHx...hash8', 3, 1),
  ('diego.ramirez@gmail.com',  'AQAAAAIAAYagAAAAEHx...hash9', 3, 1),
  ('sofia.torres@gmail.com',   'AQAAAAIAAYagAAAAEHx...hash10',3, 1),
  ('nicolas.vargas@gmail.com', 'AQAAAAIAAYagAAAAEHx...hash11',3, 1),
  ('valentina.ruiz@gmail.com', 'AQAAAAIAAYagAAAAEHx...hash12',3, 1),
  ('camilo.herrera@gmail.com', 'AQAAAAIAAYagAAAAEHx...hash13',3, 1),
  ('paula.moreno@gmail.com',   'AQAAAAIAAYagAAAAEHx...hash14',3, 1),
  ('sergio.castillo@gmail.com','AQAAAAIAAYagAAAAEHx...hash15',3, 1);
GO

-- ============================================================
-- 9. EMPLEADOS
-- ============================================================
INSERT INTO [dbo].[Empleados] ([usuario_id], [documento_identidad], [nombre_completo], [cargo]) VALUES
  (1, '79845623', 'Carlos Alberto López',  'Administrador'),
  (2, '52741896', 'Andrea Patricia Gómez', 'Cajero'),
  (3, '1020345678','Jhon Fredy Mora',      'Auxiliar');
GO

-- ============================================================
-- 10. ESCENARIOS (6 canchas como en el diseño)
-- ============================================================
INSERT INTO [dbo].[Escenarios]
  ([nombre], [deporte_id], [estado_id], [precio_por_hora], [url_imagen], [capacidad_personas], [descripcion], [establecimiento_id], [es_activo])
VALUES
  ('Cancha Sintética 1', 1, 1, 70000, NULL, 22, 'Cancha de fútbol 11 con iluminación LED y pasto sintético de última generación.', 1, 1),
  ('Cancha Sintética 2', 1, 1, 70000, NULL, 22, 'Cancha de fútbol 11 con tribuna techada para espectadores.', 1, 1),
  ('Cancha Sintética 3', 2, 1, 55000, NULL, 14, 'Cancha de fútbol 7 ideal para partidos rápidos y entrenamientos.', 1, 1),
  ('Cancha Sintética 4', 2, 1, 55000, NULL, 14, 'Cancha de fútbol 7 con cerramiento total y vestiarios.', 1, 1),
  ('Cancha Sintética 5', 3, 1, 60000, NULL, 16, 'Cancha de fútbol 8 multipropósito con pasto sintético 3G.', 1, 1),
  ('Cancha Sintética 6', 3, 2, 60000, NULL, 16, 'Cancha de fútbol 8 — actualmente en mantenimiento de pasto.', 1, 1);
GO

-- ============================================================
-- 11. TARIFAS ESPECIALES (fines de semana)
-- ============================================================
INSERT INTO [dbo].[TarifasEspeciales] ([escenario_id], [dia_semana], [hora_inicio], [hora_fin], [precio_hora], [es_festivo]) VALUES
  (1, 6, '08:00', '22:00', 85000, 0),  -- Cancha 1 Sábado
  (1, 0, '08:00', '22:00', 85000, 0),  -- Cancha 1 Domingo
  (2, 6, '08:00', '22:00', 85000, 0),
  (2, 0, '08:00', '22:00', 85000, 0),
  (3, 6, '08:00', '22:00', 65000, 0),
  (3, 0, '08:00', '22:00', 65000, 0);
GO

-- ============================================================
-- 12. CLIENTES
-- ============================================================
INSERT INTO [dbo].[Clientes] ([usuario_id], [nombre_completo], [telefono], [tiene_whatsapp], [es_activo]) VALUES
  (4,  'Juan Pérez',          '300 123 4567', 1, 1),
  (5,  'Carlos Gómez',        '301 234 5678', 1, 1),
  (6,  'María Rodríguez',     '301 234 5678', 1, 1),
  (7,  'Luis Martínez',       '302 345 6799', 1, 1),
  (8,  'Andrés Andrez',       '304 367 6872', 1, 1),
  (9,  'Diego Ramírez',       '305 670 8912', 1, 1),
  (10, 'Sofía Torres',        '316 789 0123', 1, 1),
  (11, 'Nicolás Vargas',      '320 890 1234', 1, 1),
  (12, 'Valentina Ruiz',      '311 901 2345', 1, 1),
  (13, 'Camilo Herrera',      '315 012 3456', 1, 1),
  (14, 'Paula Moreno',        '318 123 4567', 1, 1),
  (15, 'Sergio Castillo',     '303 456 5876', 1, 1);
GO

-- Clientes sin usuario (registros directos en caja)
INSERT INTO [dbo].[Clientes] ([usuario_id], [nombre_completo], [telefono], [tiene_whatsapp], [es_activo]) VALUES
  (NULL, 'Fernando Flores',   '303 457 6690', 1, 1),
  (NULL, 'Mara Rivera',       '317 234 5678', 0, 1),
  (NULL, 'Roberto Castro',    '312 345 6789', 1, 1);
GO

-- ============================================================
-- 13. RESERVAS — Semana actual (Lun–Dom)
--     Ajusta las fechas según el día real de hoy
-- ============================================================
DECLARE @hoy    DATE = CAST(GETDATE() AS DATE);
DECLARE @lunes  DATE = DATEADD(day, 1 - CASE DATEPART(dw,@hoy) WHEN 1 THEN 7 ELSE DATEPART(dw,@hoy)-1 END, @hoy);

-- Lunes
INSERT INTO [dbo].[Reservas] ([cliente_id],[escenario_id],[empleado_id],[fecha_reserva],[hora_inicio],[hora_fin],[estado_id],[monto_total]) VALUES
  (1, 1, 2, @lunes, '08:00', '10:00', 1, 140000),
  (4, 2, 2, @lunes, '13:00', '14:00', 1, 70000),
  (7, 3, 2, @lunes, '18:00', '19:00', 1, 55000);

-- Martes
INSERT INTO [dbo].[Reservas] ([cliente_id],[escenario_id],[empleado_id],[fecha_reserva],[hora_inicio],[hora_fin],[estado_id],[monto_total]) VALUES
  (2, 1, 2, DATEADD(day,1,@lunes), '10:00', '11:00', 1, 70000),
  (6, 4, 2, DATEADD(day,1,@lunes), '18:00', '19:00', 1, 55000);

-- Miércoles
INSERT INTO [dbo].[Reservas] ([cliente_id],[escenario_id],[empleado_id],[fecha_reserva],[hora_inicio],[hora_fin],[estado_id],[monto_total]) VALUES
  (3, 2, 2, DATEADD(day,2,@lunes), '09:00', '10:00', 1, 70000),
  (9, 5, 2, DATEADD(day,2,@lunes), '15:00', '16:00', 2, 60000);

-- Jueves
INSERT INTO [dbo].[Reservas] ([cliente_id],[escenario_id],[empleado_id],[fecha_reserva],[hora_inicio],[hora_fin],[estado_id],[monto_total]) VALUES
  (5, 2, 2, DATEADD(day,3,@lunes), '10:00', '11:00', 2, 70000),
  (8, 3, 2, DATEADD(day,3,@lunes), '11:00', '12:00', 3, 55000),
  (11,5, 2, DATEADD(day,3,@lunes), '11:00', '12:00', 2, 60000);

-- Viernes
INSERT INTO [dbo].[Reservas] ([cliente_id],[escenario_id],[empleado_id],[fecha_reserva],[hora_inicio],[hora_fin],[estado_id],[monto_total]) VALUES
  (1, 1, 2, DATEADD(day,4,@lunes), '08:00', '10:00', 1, 140000),
  (10,4, 2, DATEADD(day,4,@lunes), '14:00', '15:00', 1, 55000),
  (12,2, 2, DATEADD(day,4,@lunes), '11:00', '12:00', 3, 70000);

-- Sábado
INSERT INTO [dbo].[Reservas] ([cliente_id],[escenario_id],[empleado_id],[fecha_reserva],[hora_inicio],[hora_fin],[estado_id],[monto_total]) VALUES
  (6, 1, 2, DATEADD(day,5,@lunes), '09:00', '10:00', 1, 85000),
  (3, 2, 2, DATEADD(day,5,@lunes), '14:00', '15:00', 1, 85000),
  (9, 3, 2, DATEADD(day,5,@lunes), '00:00', '01:00', 1, 65000);

-- Domingo
INSERT INTO [dbo].[Reservas] ([cliente_id],[escenario_id],[empleado_id],[fecha_reserva],[hora_inicio],[hora_fin],[estado_id],[monto_total]) VALUES
  (7, 1, 2, DATEADD(day,6,@lunes), '10:00', '11:00', 1, 85000),
  (2, 2, 2, DATEADD(day,6,@lunes), '15:00', '16:00', 2, 85000);
GO

-- ============================================================
-- 14. RESERVAS — Mes actual (datos para gráficas de reportes)
-- ============================================================
DECLARE @inicioMes DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

-- Semanas anteriores del mes
INSERT INTO [dbo].[Reservas] ([cliente_id],[escenario_id],[empleado_id],[fecha_reserva],[hora_inicio],[hora_fin],[estado_id],[monto_total]) VALUES
  (1, 1, 2, DATEADD(day,  0,@inicioMes), '08:00','10:00', 1, 140000),
  (2, 2, 2, DATEADD(day,  0,@inicioMes), '10:00','11:00', 1,  70000),
  (3, 3, 2, DATEADD(day,  1,@inicioMes), '09:00','10:00', 4,  55000),
  (4, 4, 2, DATEADD(day,  1,@inicioMes), '11:00','12:00', 1,  55000),
  (5, 1, 2, DATEADD(day,  2,@inicioMes), '14:00','16:00', 1, 140000),
  (6, 2, 2, DATEADD(day,  2,@inicioMes), '08:00','09:00', 1,  70000),
  (7, 5, 2, DATEADD(day,  3,@inicioMes), '10:00','11:00', 4,  60000),
  (8, 3, 2, DATEADD(day,  3,@inicioMes), '18:00','19:00', 1,  55000),
  (9, 1, 2, DATEADD(day,  4,@inicioMes), '08:00','10:00', 1, 140000),
  (10,2, 2, DATEADD(day,  4,@inicioMes), '13:00','14:00', 1,  70000),
  (11,4, 2, DATEADD(day,  5,@inicioMes), '09:00','10:00', 1,  55000),
  (12,5, 2, DATEADD(day,  5,@inicioMes), '16:00','17:00', 4,  60000),
  (1, 2, 2, DATEADD(day,  7,@inicioMes), '10:00','12:00', 1, 140000),
  (3, 1, 2, DATEADD(day,  7,@inicioMes), '08:00','09:00', 1,  70000),
  (5, 3, 2, DATEADD(day,  8,@inicioMes), '14:00','15:00', 1,  55000),
  (2, 4, 2, DATEADD(day,  8,@inicioMes), '11:00','12:00', 3,  55000),
  (6, 1, 2, DATEADD(day,  9,@inicioMes), '18:00','20:00', 1, 140000),
  (8, 2, 2, DATEADD(day,  9,@inicioMes), '08:00','09:00', 1,  70000),
  (4, 5, 2, DATEADD(day, 10,@inicioMes), '10:00','11:00', 4,  60000),
  (7, 3, 2, DATEADD(day, 10,@inicioMes), '16:00','17:00', 1,  55000),
  (9, 1, 2, DATEADD(day, 11,@inicioMes), '09:00','10:00', 1,  85000),
  (11,2, 2, DATEADD(day, 11,@inicioMes), '14:00','15:00', 1,  85000),
  (1, 4, 2, DATEADD(day, 12,@inicioMes), '10:00','11:00', 1,  65000),
  (3, 1, 2, DATEADD(day, 14,@inicioMes), '08:00','10:00', 1, 140000),
  (5, 2, 2, DATEADD(day, 14,@inicioMes), '11:00','12:00', 1,  70000),
  (2, 3, 2, DATEADD(day, 15,@inicioMes), '13:00','14:00', 4,  55000),
  (6, 5, 2, DATEADD(day, 15,@inicioMes), '15:00','16:00', 1,  60000),
  (8, 1, 2, DATEADD(day, 16,@inicioMes), '08:00','09:00', 1,  70000),
  (10,2, 2, DATEADD(day, 16,@inicioMes), '10:00','11:00', 3,  70000),
  (12,4, 2, DATEADD(day, 17,@inicioMes), '14:00','15:00', 1,  55000),
  (4, 1, 2, DATEADD(day, 18,@inicioMes), '09:00','11:00', 1, 140000),
  (7, 3, 2, DATEADD(day, 18,@inicioMes), '16:00','17:00', 4,  55000),
  (9, 2, 2, DATEADD(day, 19,@inicioMes), '11:00','12:00', 1,  70000);
GO

-- ============================================================
-- 15. TURNO DE CAJA (para asociar pagos)
-- ============================================================
INSERT INTO [dbo].[TurnosCaja] ([empleado_id],[monto_inicial],[estado],[establecimiento_id]) VALUES
  (2, 200000, 'Abierto', 1);
GO

-- ============================================================
-- 16. PAGOS — Asociados a las reservas del mes
--     (confirmar reservas con estado_id = 1 o 4)
-- ============================================================
-- Pagos semana actual
INSERT INTO [dbo].[Pagos] ([reserva_id],[turno_id],[metodo_pago_id],[monto],[fecha_pago])
SELECT r.reserva_id, 1, 
  CASE (r.reserva_id % 6) + 1 WHEN 1 THEN 1 WHEN 2 THEN 2 WHEN 3 THEN 5 WHEN 4 THEN 3 WHEN 5 THEN 6 ELSE 1 END,
  r.monto_total,
  CAST(r.fecha_reserva AS DATETIME) + CAST(r.hora_inicio AS DATETIME)
FROM [dbo].[Reservas] r
WHERE r.estado_id IN (1, 4)
  AND r.reserva_id NOT IN (SELECT reserva_id FROM [dbo].[Pagos]);
GO

-- ============================================================
-- 17. PRODUCTOS (alquiler de chalecos, balones, etc.)
-- ============================================================
INSERT INTO [dbo].[Productos]
  ([categoria_id],[nombre],[descripcion],[precio_venta],[precio_alquiler],[stock_actual],[stock_minimo],[es_alquilable],[activo],[establecimiento_id])
VALUES
  (2, 'Balón de fútbol',    'Balón profesional #5',     15000, 5000,  10, 3, 1, 1, 1),
  (2, 'Chalecos verdes',    'Set x10 chalecos fluorescentes', NULL, 8000, 15, 5, 1, 1, 1),
  (2, 'Chalecos amarillos', 'Set x10 chalecos amarillos',     NULL, 8000, 15, 5, 1, 1, 1),
  (2, 'Arquería completa',  'Guantes + rodilleras + gorra',   NULL, 12000, 4, 2, 1, 1, 1),
  (1, 'Peto protector',     'Peto acolchado para arquero',    35000, NULL,  6, 2, 0, 1, 1),
  (3, 'Agua Cristal 600ml', 'Agua embotellada 600ml',          2500, NULL, 48, 10, 0, 1, 1),
  (3, 'Gatorade',           'Bebida hidratante 500ml',         4000, NULL, 36,  8, 0, 1, 1),
  (4, 'Barra proteína',     'Barra de proteína 60g',           6000, NULL, 20,  5, 0, 1, 1);
GO

-- ============================================================
-- 18. MANTENIMIENTO (Cancha 6)
-- ============================================================
INSERT INTO [dbo].[MantenimientosEscenario]
  ([escenario_id],[fecha_inicio],[fecha_fin],[motivo],[realizado_por])
VALUES
  (6, DATEADD(day,-3,GETDATE()), DATEADD(day,4,GETDATE()),
   'Reemplazo de pasto sintético y pintura de líneas', 'Sinteticas Colombia S.A.S.');
GO

PRINT '==========================================================';
PRINT ' Datos de prueba insertados correctamente en LaJugadaDB';
PRINT '==========================================================';
GO
