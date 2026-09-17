using GestionDeportiva.Data;
using GestionDeportiva.Models;
using GestionDeportiva.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeportiva.Controllers
{
    public class ClientesController : Controller
    {
        private readonly LajugadaDbContext _db;
        public ClientesController(LajugadaDbContext db) => _db = db;

        // GET: /Clientes?busqueda=...&pagina=1&porPagina=10
        public async Task<IActionResult> Index(string? busqueda, int pagina = 1, int porPagina = 10)
        {
            var query = _db.VistaClientesResumen.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
                query = query.Where(c =>
                    c.Cliente.Contains(busqueda) ||
                    c.Contacto.Contains(busqueda));

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.TotalReservas)
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToListAsync();

            var vm = new ClientesIndexViewModel
            {
                Clientes       = items,
                Busqueda       = busqueda,
                PaginaActual   = pagina,
                TotalRegistros = total,
                TotalPaginas   = (int)Math.Ceiling(total / (double)porPagina),
                PorPagina      = porPagina
            };
            return View(vm);
        }

        // GET: /Clientes/Crear
        public IActionResult Crear() => PartialView("_FormCliente", new Cliente());

        // POST: /Clientes/Crear
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Cliente c)
        {
            if (!ModelState.IsValid) return PartialView("_FormCliente", c);
            c.FechaCreacion = DateTime.Now;
            c.EsActivo      = true;
            c.TieneWhatsapp ??= true;
            _db.Clientes.Add(c);
            await _db.SaveChangesAsync();
            return Json(new { ok = true, msg = "Cliente creado exitosamente." });
        }

        // POST: /Clientes/Eliminar/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var c = await _db.Clientes.FindAsync(id);
            if (c == null) return NotFound();
            c.EsActivo = false;
            await _db.SaveChangesAsync();
            return Json(new { ok = true });
        }
    }
}
