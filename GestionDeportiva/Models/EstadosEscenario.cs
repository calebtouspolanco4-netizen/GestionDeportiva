using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class EstadosEscenario
{
    public int EstadoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Escenario> Escenarios { get; set; } = new List<Escenario>();
}
