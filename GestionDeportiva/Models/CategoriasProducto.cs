using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class CategoriasProducto
{
    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
