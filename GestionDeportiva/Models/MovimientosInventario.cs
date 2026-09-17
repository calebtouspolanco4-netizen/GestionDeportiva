using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class MovimientosInventario
{
    public int MovimientoId { get; set; }

    public int ProductoId { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int Cantidad { get; set; }

    public string? Motivo { get; set; }

    public int UsuarioId { get; set; }

    public DateTime? FechaMovimiento { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
