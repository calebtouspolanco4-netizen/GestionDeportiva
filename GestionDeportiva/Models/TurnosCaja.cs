using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class TurnosCaja
{
    public int TurnoId { get; set; }

    public int EmpleadoId { get; set; }

    public DateTime? FechaApertura { get; set; }

    public DateTime? FechaCierre { get; set; }

    public decimal MontoInicial { get; set; }

    public decimal? MontoEsperado { get; set; }

    public decimal? MontoReal { get; set; }

    public decimal? Diferencia { get; set; }

    public string? Estado { get; set; }

    public string? Observaciones { get; set; }

    public int EstablecimientoId { get; set; }

    public virtual Empleado Empleado { get; set; } = null!;

    public virtual Establecimiento Establecimiento { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
