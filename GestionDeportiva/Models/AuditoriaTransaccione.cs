using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class AuditoriaTransaccione
{
    public int AuditoriaId { get; set; }

    public int EmpleadoId { get; set; }

    public string TipoAccion { get; set; } = null!;

    public string TablaAfectada { get; set; } = null!;

    public int RegistroId { get; set; }

    public string? Detalles { get; set; }

    public DateTime? FechaAccion { get; set; }

    public virtual Empleado Empleado { get; set; } = null!;
}
