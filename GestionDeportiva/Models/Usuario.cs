using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public bool? Activo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Empleado? Empleado { get; set; }

    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; set; } = new List<MovimientosInventario>();

    public virtual Role Role { get; set; } = null!;
}
