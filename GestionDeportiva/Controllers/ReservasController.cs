using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionDeportiva.Data;
using GestionDeportiva.Models;
using GestionDeportiva.ViewModels;

namespace GestionDeportiva.Controllers
{
    public class ReservasController : Controller
    {
        private readonly LajugadaDbContext _db;

        public ReservasController(LajugadaDbContext db)
        {
            _db = db;
        }

        // GET: /Reservas
        public async Task<IActionResult> Index(
            string? busqueda,
            int? escenarioId,
            int? estadoId,
            string? fecha,
            int pagina = 1,
            int porPagina = 10,
            bool crear = false)
        {
            var query = _db.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Escenario).ThenInclude(e => e.Deporte)
                .Include(r => r.Estado)
                .Include(r => r.Pagos)
                .AsQueryable();

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var b = busqueda.Trim().ToLower();
                query = query.Where(r =>
                    r.Cliente.NombreCompleto.ToLower().Contains(b) ||
                    r.Cliente.Telefono.Contains(b) ||
                    r.ReservaId.ToString().Contains(b));
            }

            // Filtro por escenario
            if (escenarioId.HasValue && escenarioId.Value > 0)
            {
                query = query.Where(r => r.EscenarioId == escenarioId.Value);
            }

            // Filtro por estado
            if (estadoId.HasValue && estadoId.Value > 0)
            {
                query = query.Where(r => r.EstadoId == estadoId.Value);
            }

            // Filtro por fecha
            if (!string.IsNullOrWhiteSpace(fecha) && DateOnly.TryParse(fecha, out var fDate))
            {
                query = query.Where(r => r.FechaReserva == fDate);
            }

            var totalRegistros = await query.CountAsync();

            var itemsRaw = await query
                .OrderByDescending(r => r.FechaReserva)
                .ThenByDescending(r => r.HoraInicio)
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToListAsync();

            var reservasVm = itemsRaw.Select(r =>
            {
                var duracionHoras = (r.HoraFin.ToTimeSpan() - r.HoraInicio.ToTimeSpan()).TotalHours;
                var totalPagado = r.Pagos.Sum(p => p.Monto);

                return new ReservaItemViewModel
                {
                    ReservaId = r.ReservaId,
                    ClienteId = r.ClienteId,
                    ClienteNombre = r.Cliente?.NombreCompleto ?? "Sin cliente",
                    ClienteTelefono = r.Cliente?.Telefono ?? "",
                    TieneWhatsapp = r.Cliente?.TieneWhatsapp ?? false,
                    EscenarioId = r.EscenarioId,
                    EscenarioNombre = r.Escenario?.Nombre ?? "Sin escenario",
                    DeporteNombre = r.Escenario?.Deporte?.Nombre ?? "",
                    FechaReserva = r.FechaReserva,
                    HoraInicio = r.HoraInicio,
                    HoraFin = r.HoraFin,
                    DuracionHoras = Math.Round(duracionHoras, 1),
                    MontoTotal = r.MontoTotal,
                    MontoPagado = totalPagado,
                    EstadoId = r.EstadoId,
                    EstadoNombre = r.Estado?.Nombre ?? "Desconocido",
                    Notas = r.Notas,
                    FechaCreacion = r.FechaCreacion
                };
            }).ToList();

            // Métricas globales
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var todasReservas = await _db.Reservas.Include(r => r.Estado).ToListAsync();
            var total = todasReservas.Count;
            var confirmadas = todasReservas.Count(r => r.Estado.Nombre == "Confirmada");
            var pendientes = todasReservas.Count(r => r.Estado.Nombre == "Pendiente");
            var canceladas = todasReservas.Count(r => r.Estado.Nombre == "Cancelada");
            var completadas = todasReservas.Count(r => r.Estado.Nombre == "Completada");

            var vm = new ReservasIndexViewModel
            {
                Reservas = reservasVm,
                Escenarios = await _db.Escenarios.Where(e => e.EsActivo == true).OrderBy(e => e.Nombre).ToListAsync(),
                Clientes = await _db.Clientes.Where(c => c.EsActivo == true).OrderBy(c => c.NombreCompleto).ToListAsync(),
                Estados = await _db.EstadosReservas.OrderBy(e => e.EstadoId).ToListAsync(),
                MetodosPago = await _db.MetodosPagos.OrderBy(m => m.Nombre).ToListAsync(),
                Busqueda = busqueda,
                EscenarioId = escenarioId,
                EstadoId = estadoId,
                Fecha = fecha,
                PaginaActual = pagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = Math.Max(1, (int)Math.Ceiling(totalRegistros / (double)porPagina)),
                PorPagina = porPagina,
                TotalReservas = total,
                ReservasConfirmadas = confirmadas,
                ReservasPendientes = pendientes,
                ReservasCanceladas = canceladas,
                ReservasCompletadas = completadas
            };

            ViewBag.AbrirModalCrear = crear;
            return View(vm);
        }

        // GET: /Reservas/Crear -> redirige a Index con parámetro para abrir modal
        [HttpGet]
        public IActionResult Crear()
        {
            return RedirectToAction(nameof(Index), new { crear = true });
        }

        // POST: /Reservas/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearReservaInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var primerError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                return Json(new { ok = false, msg = primerError ?? "Datos inválidos." });
            }

            if (model.HoraFin <= model.HoraInicio)
            {
                return Json(new { ok = false, msg = "La hora de fin debe ser posterior a la hora de inicio." });
            }

            var escenario = await _db.Escenarios.FindAsync(model.EscenarioId);
            if (escenario == null)
            {
                return Json(new { ok = false, msg = "El escenario seleccionado no existe." });
            }

            // 1. Verificación de conflicto de horario (Solapamiento)
            // Existe solapamiento si: model.HoraInicio < r.HoraFin Y model.HoraFin > r.HoraInicio
            var existeConflicto = await _db.Reservas
                .AnyAsync(r =>
                    r.EscenarioId == model.EscenarioId &&
                    r.FechaReserva == model.FechaReserva &&
                    r.EstadoId != 3 && // No canceladas
                    (model.HoraInicio < r.HoraFin && model.HoraFin > r.HoraInicio));

            if (existeConflicto)
            {
                return Json(new
                {
                    ok = false,
                    msg = $"Conflicto de horario: El escenario ya se encuentra reservado en ese rango para la fecha {model.FechaReserva:dd/MM/yyyy}."
                });
            }

            // 2. Calcular monto total si no vino especificado
            var duracionHoras = (model.HoraFin.ToTimeSpan() - model.HoraInicio.ToTimeSpan()).TotalHours;
            var montoCalculado = (decimal)duracionHoras * escenario.PrecioPorHora;
            var montoTotal = model.MontoTotal > 0 ? model.MontoTotal : montoCalculado;

            // Determinar estado inicial:
            // Si registra pago inicial completo -> Confirmada (1)
            // Si registra abono parcial -> Confirmada (1) o Pendiente (2) según pago
            // Si no registra pago -> Pendiente (2)
            int estadoInicial = 2; // Pendiente
            if (model.RegistrarPagoInicial && model.MontoAbono.HasValue && model.MontoAbono.Value > 0)
            {
                if (model.MontoAbono.Value >= montoTotal)
                    estadoInicial = 1; // Confirmada si pagó total
                else
                    estadoInicial = 1; // También confirmada con abono
            }

            var nuevaReserva = new Reserva
            {
                ClienteId = model.ClienteId,
                EscenarioId = model.EscenarioId,
                FechaReserva = model.FechaReserva,
                HoraInicio = model.HoraInicio,
                HoraFin = model.HoraFin,
                MontoTotal = montoTotal,
                EstadoId = estadoInicial,
                Notas = model.Notas,
                FechaCreacion = DateTime.Now,
                EmpleadoId = 1 // Administrador por defecto
            };

            _db.Reservas.Add(nuevaReserva);
            await _db.SaveChangesAsync();

            // 3. Registrar abono inicial si se especificó
            if (model.RegistrarPagoInicial && model.MontoAbono.HasValue && model.MontoAbono.Value > 0 && model.MetodoPagoId.HasValue)
            {
                // Buscar turno abierto o turno 1
                var turno = await _db.TurnosCajas.FirstOrDefaultAsync(t => t.Estado == "Abierto");

                var pago = new Pago
                {
                    ReservaId = nuevaReserva.ReservaId,
                    MetodoPagoId = model.MetodoPagoId.Value,
                    Monto = model.MontoAbono.Value,
                    FechaPago = DateTime.Now,
                    ReferenciaTransaccion = model.ReferenciaPago ?? "Abono inicial",
                    TurnoId = turno?.TurnoId
                };

                _db.Pagos.Add(pago);
                await _db.SaveChangesAsync();
            }

            return Json(new { ok = true, msg = "Reserva creada exitosamente.", reservaId = nuevaReserva.ReservaId });
        }

        // GET: /Reservas/Detalle/5
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var r = await _db.Reservas
                .Include(x => x.Cliente)
                .Include(x => x.Escenario).ThenInclude(e => e.Deporte)
                .Include(x => x.Estado)
                .Include(x => x.Pagos).ThenInclude(p => p.MetodoPago)
                .FirstOrDefaultAsync(x => x.ReservaId == id);

            if (r == null) return NotFound();

            var totalPagado = r.Pagos.Sum(p => p.Monto);
            var saldo = Math.Max(0, r.MontoTotal - totalPagado);

            return Json(new
            {
                reservaId = r.ReservaId,
                cliente = r.Cliente.NombreCompleto,
                telefono = r.Cliente.Telefono,
                tieneWhatsapp = r.Cliente.TieneWhatsapp ?? false,
                escenario = r.Escenario.Nombre,
                deporte = r.Escenario.Deporte?.Nombre ?? "",
                precioPorHora = r.Escenario.PrecioPorHora,
                fecha = r.FechaReserva.ToString("dd/MM/yyyy"),
                horaInicio = r.HoraInicio.ToString("HH:mm"),
                horaFin = r.HoraFin.ToString("HH:mm"),
                duracionHoras = Math.Round((r.HoraFin.ToTimeSpan() - r.HoraInicio.ToTimeSpan()).TotalHours, 1),
                montoTotal = r.MontoTotal,
                totalPagado = totalPagado,
                saldoPendiente = saldo,
                estadoId = r.EstadoId,
                estadoNombre = r.Estado.Nombre,
                notas = r.Notas ?? "Sin observaciones",
                fechaCreacion = r.FechaCreacion?.ToString("dd/MM/yyyy hh:mm tt") ?? "",
                pagos = r.Pagos.OrderByDescending(p => p.FechaPago).Select(p => new
                {
                    pagoId = p.PagoId,
                    monto = p.Monto,
                    metodo = p.MetodoPago.Nombre,
                    fecha = p.FechaPago?.ToString("dd/MM/yyyy hh:mm tt") ?? "",
                    referencia = p.ReferenciaTransaccion ?? "—"
                }).ToList()
            });
        }

        // POST: /Reservas/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, int nuevoEstadoId)
        {
            var r = await _db.Reservas.FindAsync(id);
            if (r == null) return NotFound();

            r.EstadoId = nuevoEstadoId;
            await _db.SaveChangesAsync();

            return Json(new { ok = true, msg = "Estado de reserva actualizado con éxito." });
        }

        // POST: /Reservas/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var r = await _db.Reservas.Include(x => x.Pagos).FirstOrDefaultAsync(x => x.ReservaId == id);
            if (r == null) return NotFound();

            // Si tiene pagos asociados, se cancela para mantener consistencia contable
            if (r.Pagos.Any())
            {
                r.EstadoId = 3; // Cancelada
                await _db.SaveChangesAsync();
                return Json(new { ok = true, msg = "La reserva tiene pagos registrados. Ha sido marcada como Cancelada." });
            }

            _db.Reservas.Remove(r);
            await _db.SaveChangesAsync();
            return Json(new { ok = true, msg = "Reserva eliminada exitosamente." });
        }
    }
}

