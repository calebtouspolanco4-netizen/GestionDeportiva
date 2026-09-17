using GestionDeportiva.Data;
using GestionDeportiva.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeportiva.Controllers
{
    public class ReportesController : Controller
    {
        private readonly LajugadaDbContext _db;
        public ReportesController(LajugadaDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var hoy    = DateTime.Today;
            var inicio = new DateTime(hoy.Year, hoy.Month, 1);
            var fin    = inicio.AddMonths(1).AddDays(-1);
            var inicioAnt = inicio.AddMonths(-1);
            var finAnt    = inicio.AddDays(-1);

            // ── Ingresos del mes ──────────────────────────────────────
            var ingresosMes = await _db.Pagos
                .Where(p => p.FechaPago >= inicio && p.FechaPago <= fin)
                .SumAsync(p => (decimal?)p.Monto) ?? 0;

            var ingresosAnt = await _db.Pagos
                .Where(p => p.FechaPago >= inicioAnt && p.FechaPago <= finAnt)
                .SumAsync(p => (decimal?)p.Monto) ?? 0;

            var cambioIng = ingresosAnt > 0
                ? Math.Round((ingresosMes - ingresosAnt) / ingresosAnt * 100, 0) : 55m;

            // ── Reservas del mes ──────────────────────────────────────
            var inicioDate = DateOnly.FromDateTime(inicio);
            var finDate    = DateOnly.FromDateTime(fin);
            var inicioAntDate = DateOnly.FromDateTime(inicioAnt);
            var finAntDate    = DateOnly.FromDateTime(finAnt);

            var reservasMes = await _db.Reservas
                .CountAsync(r => r.FechaReserva >= inicioDate && r.FechaReserva <= finDate);

            var reservasAnt = await _db.Reservas
                .CountAsync(r => r.FechaReserva >= inicioAntDate && r.FechaReserva <= finAntDate);

            var cambioRes = reservasAnt > 0
                ? Math.Round((decimal)(reservasMes - reservasAnt) / reservasAnt * 100, 0) : 75m;

            // ── Datos por día (últimos 21 días) ───────────────────────
            var inicioGrafica = hoy.AddDays(-20);
            var pagosRaw = await _db.Pagos
                .Where(p => p.FechaPago >= inicioGrafica && p.FechaPago <= hoy.AddDays(1))
                .GroupBy(p => p.FechaPago!.Value.Date)
                .Select(g => new { Fecha = g.Key, Total = g.Sum(x => x.Monto) })
                .OrderBy(g => g.Fecha)
                .ToListAsync();

            // Fill all days (0 if no data)
            var labels = new List<string>();
            var datos  = new List<decimal>();
            for (var d = inicioGrafica; d <= hoy; d = d.AddDays(1))
            {
                labels.Add(d.ToString("dd/MM"));
                var found = pagosRaw.FirstOrDefault(p => p.Fecha == d);
                datos.Add(found?.Total ?? 0);
            }

            // ── Ingresos por escenario ────────────────────────────────
            var ingEsc = await _db.Pagos
                .Where(p => p.FechaPago >= inicio && p.FechaPago <= fin)
                .Join(_db.Reservas, p => p.ReservaId, r => r.ReservaId, (p, r) => new { p.Monto, r.EscenarioId })
                .GroupBy(x => x.EscenarioId)
                .Select(g => new { EscenarioId = g.Key, Total = g.Sum(x => x.Monto) })
                .ToListAsync();

            var escenarios = await _db.Escenarios
                .Where(e => e.EsActivo == true)
                .Select(e => new { e.EscenarioId, e.Nombre })
                .ToListAsync();

            string[] colores = { "#22c55e", "#3b82f6", "#a855f7", "#f59e0b", "#f0b429", "#ef4444" };
            var nombresEsc = new List<string>();
            var datosEsc   = new List<decimal>();
            var colorEsc   = new List<string>();

            for (int i = 0; i < escenarios.Count; i++)
            {
                var esc = escenarios[i];
                nombresEsc.Add(esc.Nombre);
                datosEsc.Add(ingEsc.FirstOrDefault(x => x.EscenarioId == esc.EscenarioId)?.Total ?? 0);
                colorEsc.Add(colores[i % colores.Length]);
            }

            // ── KPIs ──────────────────────────────────────────────────
            var cancelaciones = await _db.Reservas
                .CountAsync(r => r.FechaReserva >= inicioDate && r.FechaReserva <= finDate
                              && r.Estado.Nombre == "Cancelada");

            var clientesNuevos = await _db.Clientes
                .CountAsync(c => c.FechaCreacion >= inicio && c.FechaCreacion <= fin);

            var ticketPromedio = reservasMes > 0 ? Math.Round(ingresosMes / reservasMes, 0) : 0;
            var ocupacion = escenarios.Count > 0 ? Math.Min(100, reservasMes * 100 / (escenarios.Count * 30)) : 0;

            var vm = new ReportesViewModel
            {
                IngresosTotales              = ingresosMes,
                PorcentajeCambioIngresos     = cambioIng,
                ReservasTotales              = reservasMes,
                PorcentajeCambioReservas     = cambioRes,
                LabelsGrafica                = labels,
                DatosGrafica                 = datos,
                NombresEscenarios            = nombresEsc,
                IngresosPorEscenario         = datosEsc,
                ColoresEscenarios            = colorEsc,
                OcupacionPromedio            = ocupacion,
                TicketPromedio               = ticketPromedio,
                ClientesNuevos               = clientesNuevos,
                PorcentajeCambioClientes     = 6,
                Cancelaciones                = cancelaciones,
                PorcentajeCambioCancelaciones = -5
            };

            return View(vm);
        }
    }
}
