using System;
using System.Collections.Generic;
using GestionDeportiva.Models;

namespace GestionDeportiva.ViewModels
{
    public class ConfiguracionViewModel
    {
        public string ActiveTab { get; set; } = "establecimiento";
        public Establecimiento Establecimiento { get; set; } = new();
        public List<DeporteConfigViewModel> Deportes { get; set; } = new();
        public List<MetodoPagoConfigViewModel> MetodosPago { get; set; } = new();
        public List<TarifaEspecialConfigViewModel> TarifasEspeciales { get; set; } = new();
        public List<Escenario> Escenarios { get; set; } = new();
        public TurnosCaja? TurnoActivo { get; set; }
        public List<TurnosCaja> HistorialTurnos { get; set; } = new();

        public string? MensajeExito { get; set; }
        public string? MensajeError { get; set; }
    }

    public class DeporteConfigViewModel
    {
        public int DeporteId { get; set; }
        public string Nombre { get; set; } = "";
        public int? JugadoresPorEquipo { get; set; }
        public int TotalEscenarios { get; set; }
    }

    public class MetodoPagoConfigViewModel
    {
        public int MetodoPagoId { get; set; }
        public string Nombre { get; set; } = "";
        public int TotalPagosRegistrados { get; set; }
    }

    public class TarifaEspecialConfigViewModel
    {
        public int TarifaId { get; set; }
        public int EscenarioId { get; set; }
        public string EscenarioNombre { get; set; } = "";
        public int DiaSemana { get; set; }
        public string DiaSemanaNombre { get; set; } = "";
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public decimal PrecioHora { get; set; }
        public bool? EsFestivo { get; set; }
    }
}

