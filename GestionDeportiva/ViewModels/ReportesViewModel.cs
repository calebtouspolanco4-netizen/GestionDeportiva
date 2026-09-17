namespace GestionDeportiva.ViewModels
{
    public class ReportesViewModel
    {
        public decimal IngresosTotales { get; set; }
        public decimal PorcentajeCambioIngresos { get; set; }
        public int ReservasTotales { get; set; }
        public decimal PorcentajeCambioReservas { get; set; }
        public List<string> LabelsGrafica { get; set; } = new();
        public List<decimal> DatosGrafica { get; set; } = new();
        public List<string> NombresEscenarios { get; set; } = new();
        public List<decimal> IngresosPorEscenario { get; set; } = new();
        public List<string> ColoresEscenarios { get; set; } = new();
        public int OcupacionPromedio { get; set; }
        public decimal TicketPromedio { get; set; }
        public int ClientesNuevos { get; set; }
        public decimal PorcentajeCambioClientes { get; set; }
        public int Cancelaciones { get; set; }
        public decimal PorcentajeCambioCancelaciones { get; set; }
    }
}
