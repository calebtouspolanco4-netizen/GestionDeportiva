namespace GestionDeportiva.ViewModels
{
    public class ClientesIndexViewModel
    {
        public List<Models.VistaClientesResuman> Clientes { get; set; } = new();
        public string? Busqueda { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int PorPagina { get; set; } = 10;
    }
}
