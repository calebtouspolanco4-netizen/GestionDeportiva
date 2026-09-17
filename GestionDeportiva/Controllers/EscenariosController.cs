using GestionDeportiva.Data;
using GestionDeportiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeportiva.Controllers
{
    public class EscenariosController : Controller
    {
        private readonly LajugadaDbContext _db;
        public EscenariosController(LajugadaDbContext db) => _db = db;

        // GET: /Escenarios
        public async Task<IActionResult> Index()
        {
            var escenarios = await _db.Escenarios
                .Include(e => e.Deporte)
                .Include(e => e.Estado)
                .Where(e => e.EsActivo == true)
                .OrderBy(e => e.Nombre)
                .ToListAsync();
            return View(escenarios);
        }

        // GET: /Escenarios/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var e = await _db.Escenarios
                .Include(x => x.Deporte)
                .Include(x => x.Estado)
                .FirstOrDefaultAsync(x => x.EscenarioId == id);
            if (e == null) return NotFound();
            return Json(new
            {
                e.EscenarioId, e.Nombre, deporte = e.Deporte.Nombre,
                estado = e.Estado.Nombre, e.PrecioPorHora,
                e.CapacidadPersonas, e.Descripcion, e.UrlImagen
            });
        }

        // GET: /Escenarios/Crear
        public async Task<IActionResult> Crear()
        {
            ViewBag.Deportes = await _db.TiposDeportes.ToListAsync();
            ViewBag.Estados  = await _db.EstadosEscenarios.ToListAsync();
            return PartialView("_FormEscenario", new Escenario());
        }

        // POST: /Escenarios/Crear
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Escenario e)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Deportes = await _db.TiposDeportes.ToListAsync();
                ViewBag.Estados  = await _db.EstadosEscenarios.ToListAsync();
                return PartialView("_FormEscenario", e);
            }
            e.EsActivo           = true;
            e.EstablecimientoId  = 1; // establecimiento por defecto
            _db.Escenarios.Add(e);
            await _db.SaveChangesAsync();
            return Json(new { ok = true, msg = "Escenario creado exitosamente." });
        }

        // GET: /Escenarios/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var e = await _db.Escenarios.FindAsync(id);
            if (e == null) return NotFound();
            ViewBag.Deportes = await _db.TiposDeportes.ToListAsync();
            ViewBag.Estados  = await _db.EstadosEscenarios.ToListAsync();
            return PartialView("_FormEscenario", e);
        }

        // POST: /Escenarios/Editar/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Escenario e)
        {
            if (id != e.EscenarioId) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Deportes = await _db.TiposDeportes.ToListAsync();
                ViewBag.Estados  = await _db.EstadosEscenarios.ToListAsync();
                return PartialView("_FormEscenario", e);
            }
            _db.Entry(e).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Json(new { ok = true, msg = "Escenario actualizado." });
        }

        // POST: /Escenarios/Eliminar/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var e = await _db.Escenarios.FindAsync(id);
            if (e == null) return NotFound();
            e.EsActivo = false; // soft-delete
            await _db.SaveChangesAsync();
            return Json(new { ok = true });
        }
    }
}
