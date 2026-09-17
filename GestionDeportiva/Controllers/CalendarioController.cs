using GestionDeportiva.Data;
using GestionDeportiva.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeportiva.Controllers
{
    public class CalendarioController : Controller
    {
        private readonly LajugadaDbContext _db;
        public CalendarioController(LajugadaDbContext db) => _db = db;

        // GET: /Calendario?semana=0  (0 = semana actual, -1 = anterior, +1 = siguiente)
        public async Task<IActionResult> Index(int semana = 0)
        {
            // Calcular inicio de la semana (Lunes)
            var hoy   = DateTime.Today;
            var diff  = (int)hoy.DayOfWeek - 1; // Monday = 1 in DayOfWeek
            if (diff < 0) diff = 6;              // Sunday fix
            var lunes = DateOnly.FromDateTime(hoy.AddDays(-diff + semana * 7));
            var domingo = lunes.AddDays(6);

            var reservas = await _db.Reservas
                .Where(r => r.FechaReserva >= lunes && r.FechaReserva <= domingo)
                .Include(r => r.Cliente)
                .Include(r => r.Escenario)
                .Include(r => r.Estado)
                .OrderBy(r => r.FechaReserva).ThenBy(r => r.HoraInicio)
                .ToListAsync();

            var vm = new CalendarioViewModel
            {
                FechaInicio    = lunes,
                FechaFin       = domingo,
                DiasDelaSemana = Enumerable.Range(0, 7).Select(i => lunes.AddDays(i)).ToList(),
                Reservas       = reservas.Select(r =>
                {
                    // Day of week: 0=Lun ... 6=Dom
                    var dow = ((int)r.FechaReserva.DayOfWeek + 6) % 7;
                    return new ReservaCalendarioViewModel
                    {
                        ReservaId       = r.ReservaId,
                        NombreCliente   = r.Cliente.NombreCompleto,
                        NombreEscenario = r.Escenario.Nombre,
                        Fecha           = r.FechaReserva,
                        HoraInicio      = r.HoraInicio.ToString("HH:mm"),
                        HoraFin         = r.HoraFin.ToString("HH:mm"),
                        HoraInicioInt   = r.HoraInicio.Hour,
                        HoraFinInt      = r.HoraFin.Hour,
                        Estado          = r.Estado.Nombre.ToLower(),
                        DiaSemana       = dow
                    };
                }).ToList()
            };

            ViewBag.SemanaOffset = semana;
            return View(vm);
        }
    }
}
