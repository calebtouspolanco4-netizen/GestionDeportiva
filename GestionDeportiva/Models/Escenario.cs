using System;
using System.Collections.Generic;

namespace GestionDeportiva.Models;

public partial class Escenario
{
    public int EscenarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public int DeporteId { get; set; }

    public int EstadoId { get; set; }

    public decimal PrecioPorHora { get; set; }

    public string? UrlImagen { get; set; }

    public int? CapacidadPersonas { get; set; }

    public string? Descripcion { get; set; }

    public int EstablecimientoId { get; set; }

    public bool? EsActivo { get; set; }

    public virtual TiposDeporte Deporte { get; set; } = null!;

    public virtual Establecimiento Establecimiento { get; set; } = null!;

    public virtual EstadosEscenario Estado { get; set; } = null!;

    public virtual ICollection<MantenimientosEscenario> MantenimientosEscenarios { get; set; } = new List<MantenimientosEscenario>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<TarifasEspeciale> TarifasEspeciales { get; set; } = new List<TarifasEspeciale>();
}
