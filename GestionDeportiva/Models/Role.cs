using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
