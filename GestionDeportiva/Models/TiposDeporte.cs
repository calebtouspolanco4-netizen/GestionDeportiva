using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class TiposDeporte
{
    public int DeporteId { get; set; }

    public string Nombre { get; set; } = null!;

    public int? JugadoresPorEquipo { get; set; }

    public virtual ICollection<Escenario> Escenarios { get; set; } = new List<Escenario>();
}
