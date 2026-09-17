namespace GestionDeportiva.ViewModels
{
    public class CalendarioViewModel
    {
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public List<ReservaCalendarioViewModel> Reservas { get; set; } = new();
        public List<DateOnly> DiasDelaSemana { get; set; } = new();
    }

    public class ReservaCalendarioViewModel
    {
        public int ReservaId { get; set; }
        public string NombreCliente { get; set; } = "";
        public string NombreEscenario { get; set; } = "";
        public DateOnly Fecha { get; set; }
        public string HoraInicio { get; set; } = "";
        public string HoraFin { get; set; } = "";
        public int HoraInicioInt { get; set; }
        public int HoraFinInt { get; set; }
        public string Estado { get; set; } = "";
        public int DiaSemana { get; set; } // 0=Lun ... 6=Dom
    }
}
