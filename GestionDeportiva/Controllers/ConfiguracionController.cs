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
    public class ConfiguracionController : Controller
    {
        private readonly LajugadaDbContext _db;

        public ConfiguracionController(LajugadaDbContext db)
        {
            _db = db;
        }

        // GET: /Configuracion?tab=establecimiento
        public async Task<IActionResult> Index(string tab = "establecimiento", string? okMsg = null, string? errMsg = null)
        {
            var establecimiento = await _db.Establecimientos.FirstOrDefaultAsync(e => e.EstablecimientoId == 1)
                                  ?? await _db.Establecimientos.FirstOrDefaultAsync()
                                  ?? new Establecimiento
                                  {
                                      Nombre = "La Jugada - Canchas Sintéticas",
                                      Direccion = "Calle 72 # 45-30, Bogotá",
                                      Telefono = "601 234 5678",
                                      Activo = true
                                  };

            var deportesRaw = await _db.TiposDeportes
                .Include(d => d.Escenarios)
                .OrderBy(d => d.Nombre)
                .ToListAsync();

            var deportesVm = deportesRaw.Select(d => new DeporteConfigViewModel
            {
                DeporteId = d.DeporteId,
                Nombre = d.Nombre,
                JugadoresPorEquipo = d.JugadoresPorEquipo,
                TotalEscenarios = d.Escenarios.Count(e => e.EsActivo == true)
            }).ToList();

            var metodosRaw = await _db.MetodosPagos
                .Include(m => m.Pagos)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            var metodosVm = metodosRaw.Select(m => new MetodoPagoConfigViewModel
            {
                MetodoPagoId = m.MetodoPagoId,
                Nombre = m.Nombre,
                TotalPagosRegistrados = m.Pagos.Count
            }).ToList();

            var tarifasRaw = await _db.TarifasEspeciales
                .Include(t => t.Escenario)
                .OrderBy(t => t.Escenario.Nombre)
                .ThenBy(t => t.DiaSemana)
                .ToListAsync();

            var diasSemana = new[] { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };

            var tarifasVm = tarifasRaw.Select(t => new TarifaEspecialConfigViewModel
            {
                TarifaId = t.TarifaId,
                EscenarioId = t.EscenarioId,
                EscenarioNombre = t.Escenario?.Nombre ?? "",
                DiaSemana = t.DiaSemana,
                DiaSemanaNombre = (t.DiaSemana >= 0 && t.DiaSemana < 7) ? diasSemana[t.DiaSemana] : "Día " + t.DiaSemana,
                HoraInicio = t.HoraInicio,
                HoraFin = t.HoraFin,
                PrecioHora = t.PrecioHora,
                EsFestivo = t.EsFestivo
            }).ToList();

            var turnoActivo = await _db.TurnosCajas
                .Include(t => t.Empleado)
                .FirstOrDefaultAsync(t => t.Estado == "Abierto");

            var historialTurnos = await _db.TurnosCajas
                .Include(t => t.Empleado)
                .OrderByDescending(t => t.FechaApertura)
                .Take(10)
                .ToListAsync();

            var vm = new ConfiguracionViewModel
            {
                ActiveTab = tab,
                Establecimiento = establecimiento,
                Deportes = deportesVm,
                MetodosPago = metodosVm,
                TarifasEspeciales = tarifasVm,
                Escenarios = await _db.Escenarios.Where(e => e.EsActivo == true).OrderBy(e => e.Nombre).ToListAsync(),
                TurnoActivo = turnoActivo,
                HistorialTurnos = historialTurnos,
                MensajeExito = okMsg,
                MensajeError = errMsg
            };

            return View(vm);
        }

        // POST: /Configuracion/GuardarEstablecimiento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarEstablecimiento(Establecimiento model)
        {
            var est = await _db.Establecimientos.FirstOrDefaultAsync(e => e.EstablecimientoId == model.EstablecimientoId)
                      ?? await _db.Establecimientos.FirstOrDefaultAsync();

            if (est == null)
            {
                est = new Establecimiento();
                _db.Establecimientos.Add(est);
            }

            est.Nombre = model.Nombre;
            est.Direccion = model.Direccion;
            est.Telefono = model.Telefono;
            est.Activo = model.Activo;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "establecimiento", okMsg = "Datos del establecimiento actualizados correctamente." });
        }

        // POST: /Configuracion/CrearDeporte
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearDeporte(string nombre, int jugadores)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return RedirectToAction(nameof(Index), new { tab = "deportes", errMsg = "El nombre del deporte es obligatorio." });
            }

            var existe = await _db.TiposDeportes.AnyAsync(d => d.Nombre.ToLower() == nombre.Trim().ToLower());
            if (existe)
            {
                return RedirectToAction(nameof(Index), new { tab = "deportes", errMsg = "Ya existe un deporte registrado con ese nombre." });
            }

            _db.TiposDeportes.Add(new TiposDeporte
            {
                Nombre = nombre.Trim(),
                JugadoresPorEquipo = jugadores > 0 ? jugadores : 5
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "deportes", okMsg = "Nuevo deporte creado exitosamente." });
        }

        // POST: /Configuracion/EliminarDeporte/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarDeporte(int id)
        {
            var dep = await _db.TiposDeportes.Include(d => d.Escenarios).FirstOrDefaultAsync(d => d.DeporteId == id);
            if (dep == null) return NotFound();

            if (dep.Escenarios.Any())
            {
                return RedirectToAction(nameof(Index), new { tab = "deportes", errMsg = "No se puede eliminar el deporte porque tiene escenarios asociados." });
            }

            _db.TiposDeportes.Remove(dep);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "deportes", okMsg = "Deporte eliminado correctamente." });
        }

        // POST: /Configuracion/CrearMetodoPago
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearMetodoPago(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return RedirectToAction(nameof(Index), new { tab = "metodos", errMsg = "El nombre del método de pago es requerido." });
            }

            var existe = await _db.MetodosPagos.AnyAsync(m => m.Nombre.ToLower() == nombre.Trim().ToLower());
            if (existe)
            {
                return RedirectToAction(nameof(Index), new { tab = "metodos", errMsg = "Ya existe este método de pago." });
            }

            _db.MetodosPagos.Add(new MetodosPago { Nombre = nombre.Trim() });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "metodos", okMsg = "Método de pago registrado." });
        }

        // POST: /Configuracion/EliminarMetodoPago/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarMetodoPago(int id)
        {
            var m = await _db.MetodosPagos.Include(x => x.Pagos).FirstOrDefaultAsync(x => x.MetodoPagoId == id);
            if (m == null) return NotFound();

            if (m.Pagos.Any())
            {
                return RedirectToAction(nameof(Index), new { tab = "metodos", errMsg = "No se puede eliminar un método de pago con transacciones asociadas." });
            }

            _db.MetodosPagos.Remove(m);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "metodos", okMsg = "Método de pago eliminado." });
        }

        // POST: /Configuracion/CrearTarifaEspecial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearTarifaEspecial(int escenarioId, int diaSemana, string horaInicio, string horaFin, decimal precioHora, bool esFestivo = false)
        {
            if (!TimeOnly.TryParse(horaInicio, out var hIni) || !TimeOnly.TryParse(horaFin, out var hFin))
            {
                return RedirectToAction(nameof(Index), new { tab = "tarifas", errMsg = "Formato de horario inválido." });
            }

            if (precioHora <= 0)
            {
                return RedirectToAction(nameof(Index), new { tab = "tarifas", errMsg = "El precio por hora debe ser mayor a 0." });
            }

            var nueva = new TarifasEspeciale
            {
                EscenarioId = escenarioId,
                DiaSemana = diaSemana,
                HoraInicio = hIni,
                HoraFin = hFin,
                PrecioHora = precioHora,
                EsFestivo = esFestivo
            };

            _db.TarifasEspeciales.Add(nueva);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "tarifas", okMsg = "Tarifa especial creada exitosamente." });
        }

        // POST: /Configuracion/EliminarTarifaEspecial/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarTarifaEspecial(int id)
        {
            var t = await _db.TarifasEspeciales.FindAsync(id);
            if (t == null) return NotFound();

            _db.TarifasEspeciales.Remove(t);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "tarifas", okMsg = "Tarifa eliminada." });
        }

        // POST: /Configuracion/AbrirTurno
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AbrirTurno(decimal montoInicial)
        {
            var turnoAbierto = await _db.TurnosCajas.AnyAsync(t => t.Estado == "Abierto");
            if (turnoAbierto)
            {
                return RedirectToAction(nameof(Index), new { tab = "caja", errMsg = "Ya existe un turno de caja abierto." });
            }

            var empleado = await _db.Empleados.FirstOrDefaultAsync() ?? new Empleado { EmpleadoId = 1 };

            var nuevoTurno = new TurnosCaja
            {
                EmpleadoId = empleado.EmpleadoId,
                EstablecimientoId = 1,
                FechaApertura = DateTime.Now,
                MontoInicial = montoInicial,
                MontoEsperado = montoInicial,
                Estado = "Abierto"
            };

            _db.TurnosCajas.Add(nuevoTurno);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "caja", okMsg = "Turno de caja abierto correctamente." });
        }

        // POST: /Configuracion/CerrarTurno
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarTurno(int turnoId, decimal montoReal, string? observaciones)
        {
            var turno = await _db.TurnosCajas.FindAsync(turnoId);
            if (turno == null) return NotFound();

            var pagosTurno = await _db.Pagos.Where(p => p.TurnoId == turnoId).SumAsync(p => p.Monto);
            var montoEsperado = turno.MontoInicial + pagosTurno;

            turno.FechaCierre = DateTime.Now;
            turno.MontoReal = montoReal;
            turno.MontoEsperado = montoEsperado;
            turno.Observaciones = observaciones;
            turno.Estado = "Cerrado";

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "caja", okMsg = "Turno de caja cerrado con éxito." });
        }
    }
}

