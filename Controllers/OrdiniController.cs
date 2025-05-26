using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeeSe.Data;
using WeeSe.Models;
using WeeSe.Models.ViewModels;

namespace WeeSe.Controllers
{
    [Authorize]
    public class OrdiniController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdiniController> _logger;

        public OrdiniController(ApplicationDbContext context, ILogger<OrdiniController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Ordini
        public async Task<IActionResult> Index(OrdiniFiltriViewModel? filtri = null)
        {
            try
            {
                filtri ??= new OrdiniFiltriViewModel();

                var query = _context.Ordini
                    .Include(o => o.Commessa)
                    .Include(o => o.Preventivo)
                    .AsQueryable();

                // Applicazione filtri
                if (!string.IsNullOrEmpty(filtri.SearchTerm))
                {
                    query = query.Where(o =>
                        o.NumeroOrdine.Contains(filtri.SearchTerm) ||
                        o.Cliente.Contains(filtri.SearchTerm) ||
                        (o.Descrizione != null && o.Descrizione.Contains(filtri.SearchTerm)) ||
                        (o.NumeroTracking != null && o.NumeroTracking.Contains(filtri.SearchTerm)));
                }

                if (filtri.Stato.HasValue)
                    query = query.Where(o => o.Stato == filtri.Stato.Value);

                if (filtri.Priorita.HasValue)
                    query = query.Where(o => o.Priorita == filtri.Priorita.Value);

                if (!string.IsNullOrEmpty(filtri.Responsabile))
                    query = query.Where(o => o.Responsabile != null && o.Responsabile.Contains(filtri.Responsabile));

                if (!string.IsNullOrEmpty(filtri.Fornitore))
                    query = query.Where(o => o.Fornitore != null && o.Fornitore.Contains(filtri.Fornitore));

                if (filtri.DataDal.HasValue)
                    query = query.Where(o => o.DataOrdine >= filtri.DataDal.Value);

                if (filtri.DataAl.HasValue)
                    query = query.Where(o => o.DataOrdine <= filtri.DataAl.Value);

                if (filtri.SoloInRitardo)
                    query = query.Where(o => o.DataConsegnaRichiesta.HasValue && DateTime.Now > o.DataConsegnaRichiesta.Value && o.Stato != StatoOrdine.Consegnato);

                if (filtri.SoloUrgenti)
                    query = query.Where(o => o.Priorita == PrioritaOrdine.Urgente);

                if (filtri.SoloNonPagati)
                    query = query.Where(o => o.ImportoPagato < o.ImportoTotale);

                // Ordinamento
                query = filtri.Ordinamento switch
                {
                    "numero" => query.OrderBy(o => o.NumeroOrdine),
                    "numero_desc" => query.OrderByDescending(o => o.NumeroOrdine),
                    "cliente" => query.OrderBy(o => o.Cliente),
                    "cliente_desc" => query.OrderByDescending(o => o.Cliente),
                    "data" => query.OrderBy(o => o.DataOrdine),
                    "data_desc" => query.OrderByDescending(o => o.DataOrdine),
                    "stato" => query.OrderBy(o => o.Stato),
                    "priorita" => query.OrderByDescending(o => o.Priorita),
                    "consegna" => query.OrderBy(o => o.DataConsegnaRichiesta),
                    "importo" => query.OrderByDescending(o => o.ImportoTotale),
                    _ => query.OrderByDescending(o => o.CreatedAt)
                };

                var totalCount = await query.CountAsync();

                var ordini = await query
                    .Skip((filtri.PageIndex - 1) * filtri.PageSize)
                    .Take(filtri.PageSize)
                    .ToListAsync();

                var viewModel = new OrdiniIndexViewModel
                {
                    Ordini = ordini,
                    Filtri = filtri,
                    TotalCount = totalCount,

                    // Statistiche rapide
                    StatisticheRapide = new StatisticheOrdiniViewModel
                    {
                        TotaleOrdini = await _context.Ordini.CountAsync(),
                        OrdiniAttivi = await _context.Ordini.CountAsync(o =>
                            o.Stato != StatoOrdine.Consegnato && o.Stato != StatoOrdine.Annullato),
                        OrdiniInRitardo = await _context.Ordini.CountAsync(o =>
                            o.DataConsegnaRichiesta.HasValue && DateTime.Now > o.DataConsegnaRichiesta.Value && o.Stato != StatoOrdine.Consegnato),
                        OrdiniUrgenti = await _context.Ordini.CountAsync(o =>
                            o.Priorita == PrioritaOrdine.Urgente && o.Stato != StatoOrdine.Consegnato),
                        FatturatoMese = await _context.Ordini
                            .Where(o => o.DataOrdine.Month == DateTime.Now.Month && o.DataOrdine.Year == DateTime.Now.Year)
                            .SumAsync(o => o.ImportoTotale),
                        ImportoDaPagare = await _context.Ordini
                            .Where(o => o.ImportoPagato < o.ImportoTotale)
                            .SumAsync(o => o.ImportoTotale - o.ImportoPagato),
                        OrdiniSpediti = await _context.Ordini.CountAsync(o => o.Stato == StatoOrdine.Spedito),
                        OrdiniInProduzione = await _context.Ordini.CountAsync(o => o.Stato == StatoOrdine.InProduzione)
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento degli ordini");
                TempData["Error"] = "Errore durante il caricamento degli ordini";
                return View(new OrdiniIndexViewModel());
            }
        }

        // GET: Ordini/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var ordine = await _context.Ordini
                    .Include(o => o.Commessa)
                    .Include(o => o.Preventivo)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (ordine == null) return NotFound();

                return View(ordine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dell'ordine {Id}", id);
                TempData["Error"] = "Errore durante il caricamento dell'ordine";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Ordini/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new OrdineViewModel
            {
                NumeroOrdine = await GenerateNumeroOrdineAsync(),
                DataOrdine = DateTime.Now,
                Responsabile = User.Identity?.Name
            };

            await LoadDropdownDataAsync(viewModel);
            return View(viewModel);
        }

        // POST: Ordini/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrdineViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var ordine = MapToOrdine(viewModel);
                    ordine.CreatedBy = User.Identity?.Name;
                    ordine.CreatedAt = DateTime.Now;

                    _context.Add(ordine);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Ordine {NumeroOrdine} creato con successo", ordine.NumeroOrdine);
                    TempData["Success"] = "Ordine creato con successo!";
                    return RedirectToAction(nameof(Details), new { id = ordine.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante la creazione dell'ordine");
                    TempData["Error"] = "Errore durante la creazione: " + ex.Message;
                }
            }

            await LoadDropdownDataAsync(viewModel);
            return View(viewModel);
        }

        // GET: Ordini/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var ordine = await _context.Ordini.FirstOrDefaultAsync(o => o.Id == id);
                if (ordine == null) return NotFound();

                var viewModel = MapToViewModel(ordine);
                await LoadDropdownDataAsync(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dell'ordine per modifica {Id}", id);
                TempData["Error"] = "Errore durante il caricamento dell'ordine";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Ordini/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrdineViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var ordine = await _context.Ordini.FirstOrDefaultAsync(o => o.Id == id);
                    if (ordine == null) return NotFound();

                    UpdateOrdineFromViewModel(ordine, viewModel);
                    ordine.UpdatedBy = User.Identity?.Name;
                    ordine.UpdatedAt = DateTime.Now;

                    _context.Update(ordine);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Ordine {Id} aggiornato con successo", id);
                    TempData["Success"] = "Ordine aggiornato con successo!";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdineExists(viewModel.Id))
                        return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante l'aggiornamento dell'ordine {Id}", id);
                    TempData["Error"] = "Errore durante l'aggiornamento: " + ex.Message;
                }
            }

            await LoadDropdownDataAsync(viewModel);
            return View(viewModel);
        }

        // POST: Ordini/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ordine = await _context.Ordini.FindAsync(id);
                if (ordine != null)
                {
                    _context.Ordini.Remove(ordine);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Ordine {Id} eliminato con successo", id);
                    TempData["Success"] = "Ordine eliminato con successo!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione dell'ordine {Id}", id);
                TempData["Error"] = "Errore durante l'eliminazione: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Ordini/ChangeStatus/5
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, StatoOrdine nuovoStato)
        {
            try
            {
                var ordine = await _context.Ordini.FindAsync(id);
                if (ordine == null) return NotFound();

                var vecchioStato = ordine.Stato;
                ordine.Stato = nuovoStato;
                ordine.UpdatedBy = User.Identity?.Name;
                ordine.UpdatedAt = DateTime.Now;

                // Se consegnato, imposta data consegna effettiva
                if (nuovoStato == StatoOrdine.Consegnato && !ordine.DataConsegnaEffettiva.HasValue)
                {
                    ordine.DataConsegnaEffettiva = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Stato ordine {Id} cambiato da {VecchioStato} a {NuovoStato}",
                    id, vecchioStato, nuovoStato);

                return Json(new { success = true, newStatus = nuovoStato.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio stato dell'ordine {Id}", id);
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Ordini/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var oggi = DateTime.Now;
                var inizioMese = new DateTime(oggi.Year, oggi.Month, 1);

                var viewModel = new OrdiniDashboardViewModel
                {
                    TotaleOrdini = await _context.Ordini.CountAsync(),
                    OrdiniAttivi = await _context.Ordini.CountAsync(o =>
                        o.Stato != StatoOrdine.Consegnato && o.Stato != StatoOrdine.Annullato),
                    OrdiniInRitardo = await _context.Ordini.CountAsync(o =>
                        o.DataConsegnaRichiesta.HasValue && DateTime.Now > o.DataConsegnaRichiesta.Value && o.Stato != StatoOrdine.Consegnato),
                    OrdiniUrgenti = await _context.Ordini.CountAsync(o =>
                        o.Priorita == PrioritaOrdine.Urgente && o.Stato != StatoOrdine.Consegnato),

                    OrdiniRecenti = await _context.Ordini
                        .Include(o => o.Commessa)
                        .Include(o => o.Preventivo)
                        .OrderByDescending(o => o.CreatedAt)
                        .Take(5)
                        .ToListAsync(),

                    OrdiniInScadenza = await _context.Ordini
                        .Where(o => o.DataConsegnaRichiesta.HasValue && o.DataConsegnaRichiesta.Value <= DateTime.Now.AddDays(7) && o.Stato != StatoOrdine.Consegnato)
                        .OrderBy(o => o.DataConsegnaRichiesta)
                        .Take(10)
                        .ToListAsync(),

                    OrdiniDaSpedire = await _context.Ordini
                        .Where(o => o.Stato == StatoOrdine.InProduzione)
                        .OrderBy(o => o.DataConsegnaRichiesta)
                        .Take(10)
                        .ToListAsync(),

                    Statistiche = new StatisticheOrdiniViewModel
                    {
                        TotaleOrdini = await _context.Ordini.CountAsync(),
                        OrdiniAttivi = await _context.Ordini.CountAsync(o =>
                            o.Stato != StatoOrdine.Consegnato && o.Stato != StatoOrdine.Annullato),
                        OrdiniInRitardo = await _context.Ordini.CountAsync(o =>
                            o.DataConsegnaRichiesta.HasValue && DateTime.Now > o.DataConsegnaRichiesta.Value && o.Stato != StatoOrdine.Consegnato),
                        OrdiniUrgenti = await _context.Ordini.CountAsync(o =>
                            o.Priorita == PrioritaOrdine.Urgente && o.Stato != StatoOrdine.Consegnato),
                        FatturatoMese = await _context.Ordini
                            .Where(o => o.DataOrdine >= inizioMese)
                            .SumAsync(o => o.ImportoTotale),
                        ImportoDaPagare = await _context.Ordini
                            .Where(o => o.ImportoPagato < o.ImportoTotale)
                            .SumAsync(o => o.ImportoTotale - o.ImportoPagato),
                        OrdiniSpediti = await _context.Ordini.CountAsync(o => o.Stato == StatoOrdine.Spedito),
                        OrdiniInProduzione = await _context.Ordini.CountAsync(o => o.Stato == StatoOrdine.InProduzione)
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della dashboard ordini");
                TempData["Error"] = "Errore durante il caricamento della dashboard";
                return View(new OrdiniDashboardViewModel());
            }
        }

        // API per ricerca rapida
        [HttpGet]
        public async Task<IActionResult> SearchOrdini(string term)
        {
            try
            {
                var ordini = await _context.Ordini
                    .Where(o => o.NumeroOrdine.Contains(term) || o.Cliente.Contains(term))
                    .Select(o => new { id = o.Id, text = $"{o.NumeroOrdine} - {o.Cliente}" })
                    .Take(10)
                    .ToListAsync();

                return Json(ordini);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la ricerca ordini");
                return Json(new List<object>());
            }
        }

        // Metodi privati helper
        private bool OrdineExists(int id)
        {
            return _context.Ordini.Any(e => e.Id == id);
        }

        private async Task<string> GenerateNumeroOrdineAsync()
        {
            var anno = DateTime.Now.Year;
            var ultimoNumero = await _context.Ordini
                .Where(o => o.NumeroOrdine.StartsWith($"ORD-{anno}-"))
                .CountAsync();

            return $"ORD-{anno}-{(ultimoNumero + 1):D3}";
        }

        private async Task LoadDropdownDataAsync(OrdineViewModel viewModel)
        {
            var commesse = await _context.Commesse
                .Where(c => c.Stato != StatoCommessa.Completata && c.Stato != StatoCommessa.Annullata)
                .Select(c => new Commessa { Id = c.Id, NumeroCommessa = c.NumeroCommessa, Cliente = c.Cliente })
                .ToListAsync();

            var preventivi = await _context.Preventivi
                .Where(p => p.Stato == StatoPreventivo.Approvato || p.Stato == StatoPreventivo.Confermato)
                .Select(p => new Preventivo { Id = p.Id, NumeroPreventivo = p.NumeroPreventivo, Cliente = p.Cliente })
                .ToListAsync();

            var responsabili = await _context.Ordini
                .Where(o => !string.IsNullOrEmpty(o.Responsabile))
                .Select(o => o.Responsabile!)
                .Distinct()
                .ToListAsync();

            var fornitori = await _context.Ordini
                .Where(o => !string.IsNullOrEmpty(o.Fornitore))
                .Select(o => o.Fornitore!)
                .Distinct()
                .ToListAsync();

            viewModel.LoadDropdownData(commesse, preventivi, responsabili, fornitori);
        }

        private Ordine MapToOrdine(OrdineViewModel viewModel)
        {
            return new Ordine
            {
                NumeroOrdine = viewModel.NumeroOrdine,
                CommessaId = viewModel.CommessaId,
                PreventivoId = viewModel.PreventivoId,
                Cliente = viewModel.Cliente,
                IndirizzoSpedizione = viewModel.IndirizzoSpedizione,
                Descrizione = viewModel.Descrizione,
                DataOrdine = viewModel.DataOrdine,
                DataConsegnaRichiesta = viewModel.DataConsegnaRichiesta,
                DataConsegnaEffettiva = viewModel.DataConsegnaEffettiva,
                Stato = viewModel.Stato,
                Priorita = viewModel.Priorita,
                ImportoTotale = viewModel.ImportoTotale,
                ImportoPagato = viewModel.ImportoPagato,
                Responsabile = viewModel.Responsabile,
                Fornitore = viewModel.Fornitore,
                NumeroTracking = viewModel.NumeroTracking,
                Note = viewModel.Note,
                NoteInterne = viewModel.NoteInterne
            };
        }

        private OrdineViewModel MapToViewModel(Ordine ordine)
        {
            return new OrdineViewModel
            {
                Id = ordine.Id,
                NumeroOrdine = ordine.NumeroOrdine,
                CommessaId = ordine.CommessaId,
                PreventivoId = ordine.PreventivoId,
                Cliente = ordine.Cliente,
                IndirizzoSpedizione = ordine.IndirizzoSpedizione,
                Descrizione = ordine.Descrizione,
                DataOrdine = ordine.DataOrdine,
                DataConsegnaRichiesta = ordine.DataConsegnaRichiesta,
                DataConsegnaEffettiva = ordine.DataConsegnaEffettiva,
                Stato = ordine.Stato,
                Priorita = ordine.Priorita,
                ImportoTotale = ordine.ImportoTotale,
                ImportoPagato = ordine.ImportoPagato,
                Responsabile = ordine.Responsabile,
                Fornitore = ordine.Fornitore,
                NumeroTracking = ordine.NumeroTracking,
                Note = ordine.Note,
                NoteInterne = ordine.NoteInterne,
                CreatedAt = ordine.CreatedAt,
                CreatedBy = ordine.CreatedBy,
                UpdatedAt = ordine.UpdatedAt,
                UpdatedBy = ordine.UpdatedBy
            };
        }

        private void UpdateOrdineFromViewModel(Ordine ordine, OrdineViewModel viewModel)
        {
            ordine.CommessaId = viewModel.CommessaId;
            ordine.PreventivoId = viewModel.PreventivoId;
            ordine.Cliente = viewModel.Cliente;
            ordine.IndirizzoSpedizione = viewModel.IndirizzoSpedizione;
            ordine.Descrizione = viewModel.Descrizione;
            ordine.DataOrdine = viewModel.DataOrdine;
            ordine.DataConsegnaRichiesta = viewModel.DataConsegnaRichiesta;
            ordine.DataConsegnaEffettiva = viewModel.DataConsegnaEffettiva;
            ordine.Stato = viewModel.Stato;
            ordine.Priorita = viewModel.Priorita;
            ordine.ImportoTotale = viewModel.ImportoTotale;
            ordine.ImportoPagato = viewModel.ImportoPagato;
            ordine.Responsabile = viewModel.Responsabile;
            ordine.Fornitore = viewModel.Fornitore;
            ordine.NumeroTracking = viewModel.NumeroTracking;
            ordine.Note = viewModel.Note;
            ordine.NoteInterne = viewModel.NoteInterne;
        }
    }
}