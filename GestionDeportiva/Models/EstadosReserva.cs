using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class EstadosReserva
{
    public int EstadoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
