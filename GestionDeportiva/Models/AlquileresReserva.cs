using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class AlquileresReserva
{
    public int AlquilerId { get; set; }

    public int ReservaId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioAlquiler { get; set; }

    public bool? Devuelto { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Reserva Reserva { get; set; } = null!;
}
