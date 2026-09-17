using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class Cliente
{
    public int ClienteId { get; set; }

    public int? UsuarioId { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public bool? TieneWhatsapp { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool? EsActivo { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual Usuario? Usuario { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
