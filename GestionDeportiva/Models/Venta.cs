using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class Venta
{
    public int VentaId { get; set; }

    public int? ClienteId { get; set; }

    public int EmpleadoId { get; set; }

    public int? TurnoId { get; set; }

    public int MetodoPagoId { get; set; }

    public decimal MontoTotal { get; set; }

    public DateTime? FechaVenta { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual ICollection<DetallesVentum> DetallesVenta { get; set; } = new List<DetallesVentum>();

    public virtual Empleado Empleado { get; set; } = null!;

    public virtual MetodosPago MetodoPago { get; set; } = null!;

    public virtual TurnosCaja? Turno { get; set; }
}
