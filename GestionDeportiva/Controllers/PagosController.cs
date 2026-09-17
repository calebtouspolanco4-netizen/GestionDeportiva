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
    public class PagosController : Controller
    {
        private readonly LajugadaDbContext _db;

        public PagosController(LajugadaDbContext db)
        {
            _db = db;
        }

        // GET: /Pagos
        public async Task<IActionResult> Index(
            string? busqueda,
            int? metodoPagoId,
            string? fechaDesde,
            string? fechaHasta,
            int pagina = 1,
            int porPagina = 10,
            bool registrar = false,
            int? reservaId = null)
        {
            var query = _db.Pagos
                .Include(p => p.MetodoPago)
                .Include(p => p.Reserva).ThenInclude(r => r.Cliente)
                .Include(p => p.Reserva).ThenInclude(r => r.Escenario)
                .AsQueryable();

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var b = busqueda.Trim().ToLower();
                query = query.Where(p =>
                    p.Reserva.Cliente.NombreCompleto.ToLower().Contains(b) ||
                    (p.ReferenciaTransaccion != null && p.ReferenciaTransaccion.ToLower().Contains(b)) ||
                    p.PagoId.ToString().Contains(b) ||
                    p.ReservaId.ToString().Contains(b));
            }

            // Filtro por método de pago
            if (metodoPagoId.HasValue && metodoPagoId.Value > 0)
            {
                query = query.Where(p => p.MetodoPagoId == metodoPagoId.Value);
            }

            // Filtro por rango de fechas
            if (!string.IsNullOrWhiteSpace(fechaDesde) && DateTime.TryParse(fechaDesde, out var fDesde))
            {
                query = query.Where(p => p.FechaPago >= fDesde.Date);
            }
            if (!string.IsNullOrWhiteSpace(fechaHasta) && DateTime.TryParse(fechaHasta, out var fHasta))
            {
                var fHastaFin = fHasta.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.FechaPago <= fHastaFin);
            }

            var totalRegistros = await query.CountAsync();

            var itemsRaw = await query
                .OrderByDescending(p => p.FechaPago)
                .ThenByDescending(p => p.PagoId)
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToListAsync();

            var pagosVm = itemsRaw.Select(p => new PagoItemViewModel
            {
                PagoId = p.PagoId,
                ReservaId = p.ReservaId,
                FechaPago = p.FechaPago ?? DateTime.MinValue,
                Monto = p.Monto,
                MetodoPagoId = p.MetodoPagoId,
                MetodoPagoNombre = p.MetodoPago?.Nombre ?? "Desconocido",
                ReferenciaTransaccion = p.ReferenciaTransaccion,
                ClienteNombre = p.Reserva?.Cliente?.NombreCompleto ?? "Sin cliente",
                ClienteTelefono = p.Reserva?.Cliente?.Telefono ?? "",
                EscenarioNombre = p.Reserva?.Escenario?.Nombre ?? "Sin escenario",
                FechaReserva = p.Reserva?.FechaReserva ?? DateOnly.MinValue,
                HoraInicio = p.Reserva?.HoraInicio ?? TimeOnly.MinValue,
                HoraFin = p.Reserva?.HoraFin ?? TimeOnly.MinValue,
                MontoTotalReserva = p.Reserva?.MontoTotal ?? 0
            }).ToList();

            // Reservas con saldo pendiente (para modal registrar pago)
            var reservasConSaldo = await _db.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Escenario)
                .Include(r => r.Pagos)
                .Where(r => r.EstadoId != 3) // no canceladas
                .OrderByDescending(r => r.FechaReserva)
                .ToListAsync();

            var reservasPendientesVm = reservasConSaldo
                .Select(r =>
                {
                    var pagado = r.Pagos.Sum(p => p.Monto);
                    var saldo = Math.Max(0, r.MontoTotal - pagado);
                    return new ReservaPendienteViewModel
                    {
                        ReservaId = r.ReservaId,
                        ClienteNombre = r.Cliente?.NombreCompleto ?? "",
                        EscenarioNombre = r.Escenario?.Nombre ?? "",
                        FechaReserva = r.FechaReserva,
                        HoraInicio = r.HoraInicio,
                        HoraFin = r.HoraFin,
                        MontoTotal = r.MontoTotal,
                        MontoPagado = pagado,
                        SaldoPendiente = saldo
                    };
                })
                .Where(r => r.SaldoPendiente > 0)
                .ToList();

            // Métricas
            var hoy = DateTime.Today;
            var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

            var pagosMes = await _db.Pagos
                .Where(p => p.FechaPago >= primerDiaMes && p.FechaPago <= ultimoDiaMes.AddDays(1).AddTicks(-1))
                .ToListAsync();

            var recaudadoMes = pagosMes.Sum(p => p.Monto);
            var transaccionesMes = pagosMes.Count;

            var recaudadoHoy = await _db.Pagos
                .Where(p => p.FechaPago.HasValue && p.FechaPago.Value.Date == hoy)
                .SumAsync(p => (decimal?)p.Monto) ?? 0;

            var saldoTotalCartera = reservasPendientesVm.Sum(r => r.SaldoPendiente);

            var vm = new PagosIndexViewModel
            {
                Pagos = pagosVm,
                MetodosPago = await _db.MetodosPagos.OrderBy(m => m.Nombre).ToListAsync(),
                ReservasPendientes = reservasPendientesVm,
                Busqueda = busqueda,
                MetodoPagoId = metodoPagoId,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                PaginaActual = pagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = Math.Max(1, (int)Math.Ceiling(totalRegistros / (double)porPagina)),
                PorPagina = porPagina,
                AbrirModalRegistrar = registrar,
                PreseleccionarReservaId = reservaId,
                TotalRecaudadoMes = recaudadoMes,
                TotalRecaudadoHoy = recaudadoHoy,
                TotalTransaccionesMes = transaccionesMes,
                SaldoTotalPendiente = saldoTotalCartera
            };

            return View(vm);
        }

        // GET: /Pagos/Registrar -> redirige a Index abriendo modal
        [HttpGet]
        public IActionResult Registrar(int? reservaId)
        {
            return RedirectToAction(nameof(Index), new { registrar = true, reservaId = reservaId });
        }

        // POST: /Pagos/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistrarPagoInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var primerError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                return Json(new { ok = false, msg = primerError ?? "Datos de pago inválidos." });
            }

            var reserva = await _db.Reservas
                .Include(r => r.Pagos)
                .FirstOrDefaultAsync(r => r.ReservaId == model.ReservaId);

            if (reserva == null)
            {
                return Json(new { ok = false, msg = "La reserva especificada no existe." });
            }

            var totalYaPagado = reserva.Pagos.Sum(p => p.Monto);
            var saldoPendiente = Math.Max(0, reserva.MontoTotal - totalYaPagado);

            if (model.Monto <= 0)
            {
                return Json(new { ok = false, msg = "El monto debe ser superior a cero." });
            }

            // Buscar turno activo de caja
            var turno = await _db.TurnosCajas.FirstOrDefaultAsync(t => t.Estado == "Abierto");

            var nuevoPago = new Pago
            {
                ReservaId = model.ReservaId,
                MetodoPagoId = model.MetodoPagoId,
                Monto = model.Monto,
                FechaPago = DateTime.Now,
                ReferenciaTransaccion = model.ReferenciaTransaccion,
                TurnoId = turno?.TurnoId
            };

            _db.Pagos.Add(nuevoPago);

            // Si con este pago se completa el saldo de la reserva, confirmar la reserva
            if ((totalYaPagado + model.Monto) >= reserva.MontoTotal && reserva.EstadoId == 2)
            {
                reserva.EstadoId = 1; // Confirmada
            }

            await _db.SaveChangesAsync();

            return Json(new
            {
                ok = true,
                msg = "Pago registrado exitosamente.",
                pagoId = nuevoPago.PagoId
            });
        }

        // GET: /Pagos/Comprobante/5
        [HttpGet]
        public async Task<IActionResult> Comprobante(int id)
        {
            var p = await _db.Pagos
                .Include(x => x.MetodoPago)
                .Include(x => x.Reserva).ThenInclude(r => r.Cliente)
                .Include(x => x.Reserva).ThenInclude(r => r.Escenario)
                .Include(x => x.Reserva).ThenInclude(r => r.Pagos)
                .FirstOrDefaultAsync(x => x.PagoId == id);

            if (p == null) return NotFound();

            var establecimiento = await _db.Establecimientos.FirstOrDefaultAsync()
                                  ?? new Establecimiento { Nombre = "La Jugada - Canchas Sintéticas", Direccion = "Bogotá, Colombia", Telefono = "601 234 5678" };

            var totalPagadoReserva = p.Reserva.Pagos.Sum(x => x.Monto);

            var vm = new ComprobantePagoViewModel
            {
                PagoId = p.PagoId,
                FechaPago = p.FechaPago ?? DateTime.Now,
                MontoPagado = p.Monto,
                MetodoPago = p.MetodoPago.Nombre,
                ReferenciaTransaccion = p.ReferenciaTransaccion,
                ReservaId = p.ReservaId,
                ClienteNombre = p.Reserva.Cliente.NombreCompleto,
                ClienteTelefono = p.Reserva.Cliente.Telefono,
                EscenarioNombre = p.Reserva.Escenario.Nombre,
                FechaReserva = p.Reserva.FechaReserva,
                HoraInicio = p.Reserva.HoraInicio,
                HoraFin = p.Reserva.HoraFin,
                MontoTotalReserva = p.Reserva.MontoTotal,
                TotalPagadoReserva = totalPagadoReserva,
                EstablecimientoNombre = establecimiento.Nombre,
                EstablecimientoDireccion = establecimiento.Direccion ?? "Principal",
                EstablecimientoTelefono = establecimiento.Telefono ?? ""
            };

            return Json(new
            {
                pagoId = vm.PagoId,
                fechaPago = vm.FechaPago.ToString("dd/MM/yyyy hh:mm tt"),
                montoPagado = vm.MontoPagado,
                metodoPago = vm.MetodoPago,
                referencia = vm.ReferenciaTransaccion ?? "—",
                reservaId = vm.ReservaId,
                clienteNombre = vm.ClienteNombre,
                clienteTelefono = vm.ClienteTelefono,
                escenarioNombre = vm.EscenarioNombre,
                fechaReserva = vm.FechaReserva.ToString("dd/MM/yyyy"),
                horario = $"{vm.HoraInicio:HH:mm} - {vm.HoraFin:HH:mm}",
                montoTotalReserva = vm.MontoTotalReserva,
                totalPagadoReserva = vm.TotalPagadoReserva,
                saldoRestante = vm.SaldoRestante,
                establecimientoNombre = vm.EstablecimientoNombre,
                establecimientoDireccion = vm.EstablecimientoDireccion,
                establecimientoTelefono = vm.EstablecimientoTelefono
            });
        }

        // POST: /Pagos/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var p = await _db.Pagos.Include(x => x.Reserva).FirstOrDefaultAsync(x => x.PagoId == id);
            if (p == null) return NotFound();

            var reserva = p.Reserva;
            _db.Pagos.Remove(p);
            await _db.SaveChangesAsync();

            // Reevaluar saldo de la reserva
            var pagosRestantes = await _db.Pagos.Where(x => x.ReservaId == reserva.ReservaId).SumAsync(x => x.Monto);
            if (pagosRestantes < reserva.MontoTotal && reserva.EstadoId == 1)
            {
                // Si ya no está totalmente pagada y era pendiente originalmente, puede volver a pendiente
                reserva.EstadoId = 2; // Pendiente
                await _db.SaveChangesAsync();
            }

            return Json(new { ok = true, msg = "Pago anulado exitosamente." });
        }
    }
}

