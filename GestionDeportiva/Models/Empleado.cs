using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class Empleado
{
    public int EmpleadoId { get; set; }

    public int UsuarioId { get; set; }

    public string DocumentoIdentidad { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string Cargo { get; set; } = null!;

    public DateOnly? FechaIngreso { get; set; }

    public virtual ICollection<AuditoriaTransaccione> AuditoriaTransacciones { get; set; } = new List<AuditoriaTransaccione>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<TurnosCaja> TurnosCajas { get; set; } = new List<TurnosCaja>();

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
