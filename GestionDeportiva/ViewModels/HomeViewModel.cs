namespace GestionDeportiva.ViewModels
{
    public class HomeViewModel
    {
        public int ReservasHoy { get; set; }
        public decimal PorcentajeCambioReservas { get; set; }
        public int ClientesActivos { get; set; }
        public int EscenariosDisponibles { get; set; }
        public int TotalEscenarios { get; set; }
        public decimal IngresosHoy { get; set; }
        public decimal PorcentajeCambioIngresos { get; set; }
        public Models.Escenario? EscenarioDestacado { get; set; }
        public int ReservasConfirmadas { get; set; }
        public int ReservasPendientes { get; set; }
        public int ReservasCanceladas { get; set; }
        public int ReservasDisponibles { get; set; }
        public List<ReservaDelDiaViewModel> ReservasDelDia { get; set; } = new();
    }

    public class ReservaDelDiaViewModel
    {
        public int ReservaId { get; set; }
        public string HoraInicio { get; set; } = "";
        public string HoraFin { get; set; } = "";
        public string NombreEscenario { get; set; } = "";
        public string NombreCliente { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string EstadoNombre { get; set; } = "";
    }
}
