using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class TarifasEspeciale
{
    public int TarifaId { get; set; }

    public int EscenarioId { get; set; }

    public int DiaSemana { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public decimal PrecioHora { get; set; }

    public bool? EsFestivo { get; set; }

    public virtual Escenario Escenario { get; set; } = null!;
}
