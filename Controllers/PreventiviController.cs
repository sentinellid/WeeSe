using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeeSe.Data;
using WeeSe.Models;

namespace WeeSe.Controllers
{
    [Authorize]
    public class PreventiviController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PreventiviController> _logger;

        public PreventiviController(ApplicationDbContext context, ILogger<PreventiviController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Preventivi
        public async Task<IActionResult> Index()
        {
            try
            {
                var preventivi = await _context.Preventivi
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return View(preventivi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dei preventivi");
                TempData["Error"] = "Errore durante il caricamento dei preventivi";
                return View(new List<Preventivo>());
            }
        }

        // GET: Preventivi/Create
        public IActionResult Create()
        {
            var preventivo = new Preventivo
            {
                NumeroPreventivo = GenerateNumeroPreventivo(),
                Venditore = User.Identity?.Name,
                Data = DateTime.Now
            };
            return View(preventivo);
        }

        // POST: Preventivi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Preventivo preventivo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    preventivo.CreatedBy = User.Identity?.Name;
                    preventivo.CreatedAt = DateTime.Now;

                    _context.Add(preventivo);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Preventivo {NumeroPreventivo} creato con successo", preventivo.NumeroPreventivo);
                    TempData["Success"] = "Preventivo creato con successo!";
                    return RedirectToAction(nameof(Details), new { id = preventivo.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante la creazione del preventivo");
                    TempData["Error"] = "Errore durante la creazione del preventivo: " + ex.Message;
                }
            }

            return View(preventivo);
        }

        // GET: Preventivi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                var preventivo = await _context.Preventivi
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (preventivo == null)
                    return NotFound();

                return View(preventivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del preventivo {Id}", id);
                TempData["Error"] = "Errore durante il caricamento del preventivo";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Preventivi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                var preventivo = await _context.Preventivi.FindAsync(id);
                if (preventivo == null)
                    return NotFound();

                return View(preventivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del preventivo per modifica {Id}", id);
                TempData["Error"] = "Errore durante il caricamento del preventivo";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Preventivi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Preventivo preventivo)
        {
            if (id != preventivo.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    preventivo.UpdatedBy = User.Identity?.Name;
                    preventivo.UpdatedAt = DateTime.Now;

                    _context.Update(preventivo);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Preventivo {Id} aggiornato con successo", id);
                    TempData["Success"] = "Preventivo aggiornato con successo!";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreventivoExists(preventivo.Id))
                        return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante l'aggiornamento del preventivo {Id}", id);
                    TempData["Error"] = "Errore durante l'aggiornamento: " + ex.Message;
                }
            }

            return View(preventivo);
        }

        // POST: Preventivi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var preventivo = await _context.Preventivi.FindAsync(id);
                if (preventivo != null)
                {
                    _context.Preventivi.Remove(preventivo);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Preventivo {Id} eliminato con successo", id);
                    TempData["Success"] = "Preventivo eliminato con successo!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del preventivo {Id}", id);
                TempData["Error"] = "Errore durante l'eliminazione: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PreventivoExists(int id)
        {
            return _context.Preventivi.Any(e => e.Id == id);
        }

        private string GenerateNumeroPreventivo()
        {
            return $"PRV-{DateTime.Now:yyyyMMdd}-{DateTime.Now.Ticks.ToString()[^6..]}";
        }
    }
}