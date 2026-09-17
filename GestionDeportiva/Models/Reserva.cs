using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class Reserva
{
    public int ReservaId { get; set; }

    public int ClienteId { get; set; }

    public int EscenarioId { get; set; }

    public int? EmpleadoId { get; set; }

    public DateOnly FechaReserva { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public int EstadoId { get; set; }

    public decimal MontoTotal { get; set; }

    public string? Notas { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<AlquileresReserva> AlquileresReservas { get; set; } = new List<AlquileresReserva>();

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Empleado? Empleado { get; set; }

    public virtual Escenario Escenario { get; set; } = null!;

    public virtual EstadosReserva Estado { get; set; } = null!;

    public virtual ICollection<HistorialNotificacione> HistorialNotificaciones { get; set; } = new List<HistorialNotificacione>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
