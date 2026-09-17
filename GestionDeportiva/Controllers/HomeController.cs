using GestionDeportiva.Data;
using GestionDeportiva.Models;
using GestionDeportiva.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GestionDeportiva.Controllers
{
    public class HomeController : Controller
    {
        private readonly LajugadaDbContext _db;

        public HomeController(LajugadaDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var hoy  = DateOnly.FromDateTime(DateTime.Today);
            var ayer = hoy.AddDays(-1);

            // ── Reservas del día ──────────────────────────────────────
            var reservasHoy = await _db.Reservas
                .Where(r => r.FechaReserva == hoy)
                .Include(r => r.Cliente)
                .Include(r => r.Escenario)
                .Include(r => r.Estado)
                .OrderBy(r => r.HoraInicio)
                .ToListAsync();

            var reservasAyer = await _db.Reservas.CountAsync(r => r.FechaReserva == ayer);
            var cambioRes = reservasAyer > 0
                ? Math.Round((decimal)(reservasHoy.Count - reservasAyer) / reservasAyer * 100, 0)
                : 20m;

            // ── Clientes activos ──────────────────────────────────────
            var clientesActivos = await _db.Clientes.CountAsync(c => c.EsActivo == true);

            // ── Escenarios ────────────────────────────────────────────
            var escenarios = await _db.Escenarios
                .Include(e => e.Estado)
                .Include(e => e.Deporte)
                .Where(e => e.EsActivo == true)
                .ToListAsync();
            var escenariosDisponibles = escenarios.Count(e => e.Estado.Nombre == "Disponible");

            // ── Ingresos de hoy ───────────────────────────────────────
            var ingresosHoy = await _db.Pagos
                .Where(p => p.FechaPago.HasValue && p.FechaPago.Value.Date == DateTime.Today)
                .SumAsync(p => (decimal?)p.Monto) ?? 0;

            var ingresosAyer = await _db.Pagos
                .Where(p => p.FechaPago.HasValue && p.FechaPago.Value.Date == DateTime.Today.AddDays(-1))
                .SumAsync(p => (decimal?)p.Monto) ?? 0;
            var cambioIng = ingresosAyer > 0
                ? Math.Round((ingresosHoy - ingresosAyer) / ingresosAyer * 100, 0)
                : 18m;

            // ── Resumen del día ───────────────────────────────────────
            var confirmadas = reservasHoy.Count(r => r.Estado.Nombre == "Confirmada");
            var pendientes  = reservasHoy.Count(r => r.Estado.Nombre == "Pendiente");
            var canceladas  = reservasHoy.Count(r => r.Estado.Nombre == "Cancelada");
            var disponibles = Math.Max(0, escenariosDisponibles - confirmadas - pendientes);

            // ── Escenario destacado ───────────────────────────────────
            var destacado = escenarios.FirstOrDefault(e => e.Estado.Nombre == "Disponible")
                         ?? escenarios.FirstOrDefault();

            var vm = new HomeViewModel
            {
                ReservasHoy              = reservasHoy.Count,
                PorcentajeCambioReservas = cambioRes,
                ClientesActivos          = clientesActivos,
                EscenariosDisponibles    = escenariosDisponibles,
                TotalEscenarios          = escenarios.Count,
                IngresosHoy              = ingresosHoy,
                PorcentajeCambioIngresos = cambioIng,
                EscenarioDestacado       = destacado,
                ReservasConfirmadas      = confirmadas,
                ReservasPendientes       = pendientes,
                ReservasCanceladas       = canceladas,
                ReservasDisponibles      = disponibles,
                ReservasDelDia           = reservasHoy.Select(r => new ReservaDelDiaViewModel
                {
                    ReservaId      = r.ReservaId,
                    HoraInicio     = r.HoraInicio.ToString("HH:mm"),
                    HoraFin        = r.HoraFin.ToString("HH:mm"),
                    NombreEscenario = r.Escenario.Nombre,
                    NombreCliente  = r.Cliente.NombreCompleto,
                    Telefono       = r.Cliente.Telefono,
                    EstadoNombre   = r.Estado.Nombre
                }).ToList()
            };

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
