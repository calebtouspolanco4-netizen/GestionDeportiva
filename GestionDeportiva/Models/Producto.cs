using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class Producto
{
    public int ProductoId { get; set; }

    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal? PrecioVenta { get; set; }

    public decimal? PrecioAlquiler { get; set; }

    public int StockActual { get; set; }

    public int? StockMinimo { get; set; }

    public bool? EsAlquilable { get; set; }

    public bool? Activo { get; set; }

    public int EstablecimientoId { get; set; }

    public virtual ICollection<AlquileresReserva> AlquileresReservas { get; set; } = new List<AlquileresReserva>();

    public virtual CategoriasProducto Categoria { get; set; } = null!;

    public virtual ICollection<DetallesVentum> DetallesVenta { get; set; } = new List<DetallesVentum>();

    public virtual Establecimiento Establecimiento { get; set; } = null!;

    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; set; } = new List<MovimientosInventario>();
}
