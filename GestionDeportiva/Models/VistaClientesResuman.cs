using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class VistaClientesResuman
{
    public int ClienteId { get; set; }

    public string Cliente { get; set; } = null!;

    public string Contacto { get; set; } = null!;

    public bool? WhatsApp { get; set; }

    public int? TotalReservas { get; set; }

    public decimal TotalGastado { get; set; }

    public DateOnly? UltimaReserva { get; set; }
}
