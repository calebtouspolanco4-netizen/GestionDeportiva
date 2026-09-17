using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class HistorialNotificacione
{
    public int NotificacionId { get; set; }

    public int ReservaId { get; set; }

    public string TipoCanal { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public string? Estado { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public virtual Reserva Reserva { get; set; } = null!;
}
