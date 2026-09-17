using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class Establecimiento
{
    public int EstablecimientoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Escenario> Escenarios { get; set; } = new List<Escenario>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<TurnosCaja> TurnosCajas { get; set; } = new List<TurnosCaja>();
}
