using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class Pago
{
    public int PagoId { get; set; }

    public int ReservaId { get; set; }

    public int? TurnoId { get; set; }

    public int MetodoPagoId { get; set; }

    public decimal Monto { get; set; }

    public string? ReferenciaTransaccion { get; set; }

    public DateTime? FechaPago { get; set; }

    public virtual MetodosPago MetodoPago { get; set; } = null!;

    public virtual Reserva Reserva { get; set; } = null!;

    public virtual TurnosCaja? Turno { get; set; }
}
