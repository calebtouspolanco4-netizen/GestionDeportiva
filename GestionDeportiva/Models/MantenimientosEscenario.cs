using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class MantenimientosEscenario
{
    public int MantenimientoId { get; set; }

    public int EscenarioId { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public string Motivo { get; set; } = null!;

    public string? RealizadoPor { get; set; }

    public virtual Escenario Escenario { get; set; } = null!;
}
