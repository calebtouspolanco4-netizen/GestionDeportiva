using System;
using System.Collections.Generic;
using GestionDeportiva.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionDeportiva.Data;

public partial class LajugadaDbContext : DbContext
{
    public LajugadaDbContext()
    {
    }

    public LajugadaDbContext(DbContextOptions<LajugadaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AlquileresReserva> AlquileresReservas { get; set; }

    public virtual DbSet<AuditoriaTransaccione> AuditoriaTransacciones { get; set; }

    public virtual DbSet<CategoriasProducto> CategoriasProductos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<DetallesVentum> DetallesVenta { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<Escenario> Escenarios { get; set; }

    public virtual DbSet<Establecimiento> Establecimientos { get; set; }

    public virtual DbSet<EstadosEscenario> EstadosEscenarios { get; set; }

    public virtual DbSet<EstadosReserva> EstadosReservas { get; set; }

    public virtual DbSet<HistorialNotificacione> HistorialNotificaciones { get; set; }

    public virtual DbSet<MantenimientosEscenario> MantenimientosEscenarios { get; set; }

    public virtual DbSet<MetodosPago> MetodosPagos { get; set; }

    public virtual DbSet<MovimientosInventario> MovimientosInventarios { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TarifasEspeciale> TarifasEspeciales { get; set; }

    public virtual DbSet<TiposDeporte> TiposDeportes { get; set; }

    public virtual DbSet<TurnosCaja> TurnosCajas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    public virtual DbSet<VistaClientesResuman> VistaClientesResumen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LajugadaDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlquileresReserva>(entity =>
        {
            entity.HasKey(e => e.AlquilerId).HasName("PK__Alquiler__16CCCDF9BB9E440F");

            entity.ToTable("AlquileresReserva", tb =>
                {
                    tb.HasTrigger("TR_DescontarStockAlquiler");
                    tb.HasTrigger("TR_DevolverStockAlquiler");
                });

            entity.Property(e => e.AlquilerId).HasColumnName("alquiler_id");
            entity.Property(e => e.Cantidad)
                .HasDefaultValue(1)
                .HasColumnName("cantidad");
            entity.Property(e => e.Devuelto)
                .HasDefaultValue(false)
                .HasColumnName("devuelto");
            entity.Property(e => e.PrecioAlquiler)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_alquiler");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");
            entity.Property(e => e.ReservaId).HasColumnName("reserva_id");

            entity.HasOne(d => d.Producto).WithMany(p => p.AlquileresReservas)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Alquileres_Productos");

            entity.HasOne(d => d.Reserva).WithMany(p => p.AlquileresReservas)
                .HasForeignKey(d => d.ReservaId)
                .HasConstraintName("FK_Alquileres_Reservas");
        });

        modelBuilder.Entity<AuditoriaTransaccione>(entity =>
        {
            entity.HasKey(e => e.AuditoriaId).HasName("PK__Auditori__BA8431F91B3336B3");

            entity.Property(e => e.AuditoriaId).HasColumnName("auditoria_id");
            entity.Property(e => e.Detalles)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("detalles");
            entity.Property(e => e.EmpleadoId).HasColumnName("empleado_id");
            entity.Property(e => e.FechaAccion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_accion");
            entity.Property(e => e.RegistroId).HasColumnName("registro_id");
            entity.Property(e => e.TablaAfectada)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tabla_afectada");
            entity.Property(e => e.TipoAccion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo_accion");

            entity.HasOne(d => d.Empleado).WithMany(p => p.AuditoriaTransacciones)
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Auditoria_Empleados");
        });

        modelBuilder.Entity<CategoriasProducto>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__DB875A4F4AE79E7C");

            entity.ToTable("CategoriasProducto");

            entity.HasIndex(e => e.Nombre, "UQ__Categori__72AFBCC68007CFAF").IsUnique();

            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PK__Clientes__47E34D642534EC68");

            entity.HasIndex(e => e.UsuarioId, "UQ__Clientes__2ED7D2AE0D0C1F35").IsUnique();

            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.EsActivo)
                .HasDefaultValue(true)
                .HasColumnName("es_activo");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("nombre_completo");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.TieneWhatsapp)
                .HasDefaultValue(true)
                .HasColumnName("tiene_whatsapp");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithOne(p => p.Cliente)
                .HasForeignKey<Cliente>(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Clientes_Usuarios");
        });

        modelBuilder.Entity<DetallesVentum>(entity =>
        {
            entity.HasKey(e => e.DetalleId).HasName("PK__Detalles__91B12E70F1E02998");

            entity.ToTable(tb => tb.HasTrigger("TR_DescontarStockVenta"));

            entity.Property(e => e.DetalleId).HasColumnName("detalle_id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");
            entity.Property(e => e.Subtotal)
                .HasComputedColumnSql("([cantidad]*[precio_unitario])", false)
                .HasColumnType("decimal(21, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.VentaId).HasColumnName("venta_id");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetallesVenta)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallesVenta_Productos");

            entity.HasOne(d => d.Venta).WithMany(p => p.DetallesVenta)
                .HasForeignKey(d => d.VentaId)
                .HasConstraintName("FK_DetallesVenta_Ventas");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.EmpleadoId).HasName("PK__Empleado__6FBB65FD2737A96E");

            entity.HasIndex(e => e.DocumentoIdentidad, "UQ__Empleado__1A03B13F312E130F").IsUnique();

            entity.HasIndex(e => e.UsuarioId, "UQ__Empleado__2ED7D2AEC731157C").IsUnique();

            entity.Property(e => e.EmpleadoId).HasColumnName("empleado_id");
            entity.Property(e => e.Cargo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cargo");
            entity.Property(e => e.DocumentoIdentidad)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("documento_identidad");
            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_ingreso");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("nombre_completo");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithOne(p => p.Empleado)
                .HasForeignKey<Empleado>(d => d.UsuarioId)
                .HasConstraintName("FK_Empleados_Usuarios");
        });

        modelBuilder.Entity<Escenario>(entity =>
        {
            entity.HasKey(e => e.EscenarioId).HasName("PK__Escenari__C24A33561B246273");

            entity.Property(e => e.EscenarioId).HasColumnName("escenario_id");
            entity.Property(e => e.CapacidadPersonas).HasColumnName("capacidad_personas");
            entity.Property(e => e.DeporteId).HasColumnName("deporte_id");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo)
                .HasDefaultValue(true)
                .HasColumnName("es_activo");
            entity.Property(e => e.EstablecimientoId)
                .HasDefaultValue(1)
                .HasColumnName("establecimiento_id");
            entity.Property(e => e.EstadoId).HasColumnName("estado_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioPorHora)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_por_hora");
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("url_imagen");

            entity.HasOne(d => d.Deporte).WithMany(p => p.Escenarios)
                .HasForeignKey(d => d.DeporteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Escenarios_TiposDeporte");

            entity.HasOne(d => d.Establecimiento).WithMany(p => p.Escenarios)
                .HasForeignKey(d => d.EstablecimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Escenario__estab__37703C52");

            entity.HasOne(d => d.Estado).WithMany(p => p.Escenarios)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Escenarios_EstadosEscenario");
        });

        modelBuilder.Entity<Establecimiento>(entity =>
        {
            entity.HasKey(e => e.EstablecimientoId).HasName("PK__Establec__2D139C28D46EC748");

            entity.Property(e => e.EstablecimientoId).HasColumnName("establecimiento_id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<EstadosEscenario>(entity =>
        {
            entity.HasKey(e => e.EstadoId).HasName("PK__EstadosE__053774EF0CAF7009");

            entity.ToTable("EstadosEscenario");

            entity.HasIndex(e => e.Nombre, "UQ__EstadosE__72AFBCC619129142").IsUnique();

            entity.Property(e => e.EstadoId).HasColumnName("estado_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadosReserva>(entity =>
        {
            entity.HasKey(e => e.EstadoId).HasName("PK__EstadosR__053774EF7159EF4A");

            entity.ToTable("EstadosReserva");

            entity.HasIndex(e => e.Nombre, "UQ__EstadosR__72AFBCC61BF2E0C2").IsUnique();

            entity.Property(e => e.EstadoId).HasColumnName("estado_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<HistorialNotificacione>(entity =>
        {
            entity.HasKey(e => e.NotificacionId).HasName("PK__Historia__8A87EB31D28EB545");

            entity.Property(e => e.NotificacionId).HasColumnName("notificacion_id");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Enviado")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_envio");
            entity.Property(e => e.Mensaje)
                .HasColumnType("text")
                .HasColumnName("mensaje");
            entity.Property(e => e.ReservaId).HasColumnName("reserva_id");
            entity.Property(e => e.TipoCanal)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipo_canal");

            entity.HasOne(d => d.Reserva).WithMany(p => p.HistorialNotificaciones)
                .HasForeignKey(d => d.ReservaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__reser__4B7734FF");
        });

        modelBuilder.Entity<MantenimientosEscenario>(entity =>
        {
            entity.HasKey(e => e.MantenimientoId).HasName("PK__Mantenim__520AB6518BBD577B");

            entity.ToTable("MantenimientosEscenario");

            entity.Property(e => e.MantenimientoId).HasColumnName("mantenimiento_id");
            entity.Property(e => e.EscenarioId).HasColumnName("escenario_id");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.Motivo)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("motivo");
            entity.Property(e => e.RealizadoPor)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("realizado_por");

            entity.HasOne(d => d.Escenario).WithMany(p => p.MantenimientosEscenarios)
                .HasForeignKey(d => d.EscenarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Mantenimi__escen__41EDCAC5");
        });

        modelBuilder.Entity<MetodosPago>(entity =>
        {
            entity.HasKey(e => e.MetodoPagoId).HasName("PK__MetodosP__DBF39997C902EE59");

            entity.ToTable("MetodosPago");

            entity.HasIndex(e => e.Nombre, "UQ__MetodosP__72AFBCC654B6F07C").IsUnique();

            entity.Property(e => e.MetodoPagoId).HasColumnName("metodo_pago_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<MovimientosInventario>(entity =>
        {
            entity.HasKey(e => e.MovimientoId).HasName("PK__Movimien__A87EF0E58B46B10D");

            entity.ToTable("MovimientosInventario");

            entity.Property(e => e.MovimientoId).HasColumnName("movimiento_id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_movimiento");
            entity.Property(e => e.Motivo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("motivo");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipo_movimiento");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Producto).WithMany(p => p.MovimientosInventarios)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movimient__produ__45BE5BA9");

            entity.HasOne(d => d.Usuario).WithMany(p => p.MovimientosInventarios)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movimient__usuar__46B27FE2");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.PagoId).HasName("PK__Pagos__FFF0A58E2BCBC8FC");

            entity.HasIndex(e => e.ReservaId, "IX_Pagos_Reserva");

            entity.Property(e => e.PagoId).HasColumnName("pago_id");
            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago");
            entity.Property(e => e.MetodoPagoId).HasColumnName("metodo_pago_id");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.ReferenciaTransaccion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("referencia_transaccion");
            entity.Property(e => e.ReservaId).HasColumnName("reserva_id");
            entity.Property(e => e.TurnoId).HasColumnName("turno_id");

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.MetodoPagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pagos_MetodosPago");

            entity.HasOne(d => d.Reserva).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.ReservaId)
                .HasConstraintName("FK_Pagos_Reservas");

            entity.HasOne(d => d.Turno).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.TurnoId)
                .HasConstraintName("FK_Pagos_TurnosCaja");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__FB5CEEEC450C7F2F");

            entity.HasIndex(e => e.CategoriaId, "IX_Productos_Categoria");

            entity.Property(e => e.ProductoId).HasColumnName("producto_id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsAlquilable)
                .HasDefaultValue(false)
                .HasColumnName("es_alquilable");
            entity.Property(e => e.EstablecimientoId)
                .HasDefaultValue(1)
                .HasColumnName("establecimiento_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioAlquiler)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_alquiler");
            entity.Property(e => e.PrecioVenta)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_venta");
            entity.Property(e => e.StockActual).HasColumnName("stock_actual");
            entity.Property(e => e.StockMinimo)
                .HasDefaultValue(5)
                .HasColumnName("stock_minimo");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Categorias");

            entity.HasOne(d => d.Establecimiento).WithMany(p => p.Productos)
                .HasForeignKey(d => d.EstablecimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Productos__estab__395884C4");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.ReservaId).HasName("PK__Reservas__F1437E48A28DA475");

            entity.HasIndex(e => e.ClienteId, "IX_Reservas_Cliente");

            entity.HasIndex(e => new { e.FechaReserva, e.HoraInicio, e.HoraFin }, "IX_Reservas_FechaHora");

            entity.Property(e => e.ReservaId).HasColumnName("reserva_id");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.EmpleadoId).HasColumnName("empleado_id");
            entity.Property(e => e.EscenarioId).HasColumnName("escenario_id");
            entity.Property(e => e.EstadoId).HasColumnName("estado_id");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaReserva).HasColumnName("fecha_reserva");
            entity.Property(e => e.HoraFin).HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_total");
            entity.Property(e => e.Notas)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("notas");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservas_Clientes");

            entity.HasOne(d => d.Empleado).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.EmpleadoId)
                .HasConstraintName("FK_Reservas_Empleados");

            entity.HasOne(d => d.Escenario).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.EscenarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservas_Escenarios");

            entity.HasOne(d => d.Estado).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservas_EstadosReserva");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC83AC61FE");

            entity.HasIndex(e => e.Nombre, "UQ__Roles__72AFBCC685B2767D").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TarifasEspeciale>(entity =>
        {
            entity.HasKey(e => e.TarifaId).HasName("PK__TarifasE__0A71ABAD9985DED8");

            entity.Property(e => e.TarifaId).HasColumnName("tarifa_id");
            entity.Property(e => e.DiaSemana).HasColumnName("dia_semana");
            entity.Property(e => e.EsFestivo)
                .HasDefaultValue(false)
                .HasColumnName("es_festivo");
            entity.Property(e => e.EscenarioId).HasColumnName("escenario_id");
            entity.Property(e => e.HoraFin).HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            entity.Property(e => e.PrecioHora)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_hora");

            entity.HasOne(d => d.Escenario).WithMany(p => p.TarifasEspeciales)
                .HasForeignKey(d => d.EscenarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TarifasEs__escen__3F115E1A");
        });

        modelBuilder.Entity<TiposDeporte>(entity =>
        {
            entity.HasKey(e => e.DeporteId).HasName("PK__TiposDep__57BEAF8CF55CC8AE");

            entity.ToTable("TiposDeporte");

            entity.HasIndex(e => e.Nombre, "UQ__TiposDep__72AFBCC682C9F7F8").IsUnique();

            entity.Property(e => e.DeporteId).HasColumnName("deporte_id");
            entity.Property(e => e.JugadoresPorEquipo).HasColumnName("jugadores_por_equipo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TurnosCaja>(entity =>
        {
            entity.HasKey(e => e.TurnoId).HasName("PK__TurnosCa__8E611ADF2EF41AD4");

            entity.ToTable("TurnosCaja");

            entity.HasIndex(e => new { e.EmpleadoId, e.Estado }, "IX_TurnosCaja_Empleado");

            entity.Property(e => e.TurnoId).HasColumnName("turno_id");
            entity.Property(e => e.Diferencia)
                .HasComputedColumnSql("([monto_real]-[monto_esperado])", false)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("diferencia");
            entity.Property(e => e.EmpleadoId).HasColumnName("empleado_id");
            entity.Property(e => e.EstablecimientoId)
                .HasDefaultValue(1)
                .HasColumnName("establecimiento_id");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Abierto")
                .HasColumnName("estado");
            entity.Property(e => e.FechaApertura)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_apertura");
            entity.Property(e => e.FechaCierre)
                .HasColumnType("datetime")
                .HasColumnName("fecha_cierre");
            entity.Property(e => e.MontoEsperado)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_esperado");
            entity.Property(e => e.MontoInicial)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_inicial");
            entity.Property(e => e.MontoReal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_real");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("observaciones");

            entity.HasOne(d => d.Empleado).WithMany(p => p.TurnosCajas)
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TurnosCaja_Empleados");

            entity.HasOne(d => d.Establecimiento).WithMany(p => p.TurnosCajas)
                .HasForeignKey(d => d.EstablecimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TurnosCaj__estab__3B40CD36");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2ED7D2AFE8193EBE");

            entity.HasIndex(e => e.Email, "UQ__Usuarios__AB6E6164B8A5B82E").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.VentaId).HasName("PK__Ventas__B1350809FB34ACD0");

            entity.Property(e => e.VentaId).HasColumnName("venta_id");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.EmpleadoId).HasColumnName("empleado_id");
            entity.Property(e => e.FechaVenta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_venta");
            entity.Property(e => e.MetodoPagoId).HasColumnName("metodo_pago_id");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_total");
            entity.Property(e => e.TurnoId).HasColumnName("turno_id");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Venta)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK_Ventas_Clientes");

            entity.HasOne(d => d.Empleado).WithMany(p => p.Venta)
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ventas_Empleados");

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Venta)
                .HasForeignKey(d => d.MetodoPagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ventas_MetodosPago");

            entity.HasOne(d => d.Turno).WithMany(p => p.Venta)
                .HasForeignKey(d => d.TurnoId)
                .HasConstraintName("FK_Ventas_TurnosCaja");
        });

        modelBuilder.Entity<VistaClientesResuman>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vista_Clientes_Resumen");

            entity.Property(e => e.Cliente)
                .HasMaxLength(120)
                .IsUnicode(false);
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.Contacto)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TotalGastado)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("Total_Gastado");
            entity.Property(e => e.TotalReservas).HasColumnName("Total_Reservas");
            entity.Property(e => e.UltimaReserva).HasColumnName("Ultima_Reserva");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
