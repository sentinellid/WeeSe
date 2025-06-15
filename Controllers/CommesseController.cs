using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeeSe.Data;
using WeeSe.Models;
using WeeSe.Models.ViewModels;

namespace WeeSe.Controllers
{
    [Authorize]
    public class CommesseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CommesseController> _logger;

        public CommesseController(ApplicationDbContext context, ILogger<CommesseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Commesse
        public async Task<IActionResult> Index(CommesseFiltriViewModel? filtri = null)
        {
            try
            {
                filtri ??= new CommesseFiltriViewModel();

                var query = _context.Commesse
                    .Include(c => c.Preventivo)
                    .Include(c => c.Attivita)
                    .AsQueryable();

                // Applicazione filtri
                if (!string.IsNullOrEmpty(filtri.SearchTerm))
                {
                    query = query.Where(c =>
                        c.NumeroCommessa.Contains(filtri.SearchTerm) ||
                        c.Cliente.Contains(filtri.SearchTerm) ||
                        (c.Descrizione != null && c.Descrizione.Contains(filtri.SearchTerm)));
                }

                if (filtri.Stato.HasValue)
                    query = query.Where(c => c.Stato == filtri.Stato.Value);

                if (filtri.Priorita.HasValue)
                    query = query.Where(c => c.Priorita == filtri.Priorita.Value);

                if (!string.IsNullOrEmpty(filtri.Responsabile))
                    query = query.Where(c => c.Responsabile != null && c.Responsabile.Contains(filtri.Responsabile));

                if (filtri.DataDal.HasValue)
                    query = query.Where(c => c.DataInizio >= filtri.DataDal.Value);

                if (filtri.DataAl.HasValue)
                    query = query.Where(c => c.DataInizio <= filtri.DataAl.Value);

                if (filtri.SoloInRitardo)
                    query = query.Where(c => c.DataFinePrevista.HasValue && DateTime.Now > c.DataFinePrevista.Value && c.Stato != StatoCommessa.Completata);

                if (filtri.SoloUrgenti)
                    query = query.Where(c => c.Priorita == PrioritaCommessa.Urgente);

                // Ordinamento
                query = filtri.Ordinamento switch
                {
                    "numero" => query.OrderBy(c => c.NumeroCommessa),
                    "numero_desc" => query.OrderByDescending(c => c.NumeroCommessa),
                    "cliente" => query.OrderBy(c => c.Cliente),
                    "cliente_desc" => query.OrderByDescending(c => c.Cliente),
                    "data" => query.OrderBy(c => c.DataInizio),
                    "data_desc" => query.OrderByDescending(c => c.DataInizio),
                    "stato" => query.OrderBy(c => c.Stato),
                    "priorita" => query.OrderByDescending(c => c.Priorita),
                    "scadenza" => query.OrderBy(c => c.DataFinePrevista),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };

                var totalCount = await query.CountAsync();

                var commesse = await query
                    .Skip((filtri.PageIndex - 1) * filtri.PageSize)
                    .Take(filtri.PageSize)
                    .ToListAsync();

                var viewModel = new CommesseIndexViewModel
                {
                    Commesse = commesse,
                    Filtri = filtri,
                    TotalCount = totalCount,

                    // Statistiche rapide
                    StatisticheRapide = new StatisticheCommesseViewModel
                    {
                        TotaleCommesse = await _context.Commesse.CountAsync(),
                        CommesseAttive = await _context.Commesse.CountAsync(c =>
                            c.Stato == StatoCommessa.Nuova || c.Stato == StatoCommessa.InLavorazione),
                        CommesseInRitardo = await _context.Commesse.CountAsync(c =>
                            c.DataFinePrevista.HasValue && DateTime.Now > c.DataFinePrevista.Value && c.Stato != StatoCommessa.Completata),
                        CommesseUrgenti = await _context.Commesse.CountAsync(c =>
                            c.Priorita == PrioritaCommessa.Urgente && c.Stato != StatoCommessa.Completata),
                        FatturatoMese = await _context.Commesse
                            .Where(c => c.DataFineEffettiva.HasValue && c.DataFineEffettiva.Value.Month == DateTime.Now.Month && c.DataFineEffettiva.Value.Year == DateTime.Now.Year)
                            .SumAsync(c => c.ImportoFatturato)
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento delle commesse");
                TempData["Error"] = "Errore durante il caricamento delle commesse";
                return View(new CommesseIndexViewModel());
            }
        }

        // GET: Commesse/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var commessa = await _context.Commesse
                    .Include(c => c.Preventivo)
                    .Include(c => c.Attivita.OrderBy(a => a.Ordine))
                    .Include(c => c.Ordini)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (commessa == null) return NotFound();

                return View(commessa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della commessa {Id}", id);
                TempData["Error"] = "Errore durante il caricamento della commessa";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Commesse/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CommessaViewModel
            {
                NumeroCommessa = await GenerateNumeroCommessaAsync(),
                DataInizio = DateTime.Now,
                Responsabile = User.Identity?.Name
            };

            await LoadDropdownDataAsync(viewModel);
            return View(viewModel);
        }

        // POST: Commesse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommessaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var commessa = MapToCommessa(viewModel);
                    commessa.CreatedBy = User.Identity?.Name;
                    commessa.CreatedAt = DateTime.Now;

                    _context.Add(commessa);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Commessa {NumeroCommessa} creata con successo", commessa.NumeroCommessa);
                    TempData["Success"] = "Commessa creata con successo!";
                    return RedirectToAction(nameof(Details), new { id = commessa.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante la creazione della commessa");
                    TempData["Error"] = "Errore durante la creazione: " + ex.Message;
                }
            }

            await LoadDropdownDataAsync(viewModel);
            return View(viewModel);
        }

        // GET: Commesse/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var commessa = await _context.Commesse
                    .Include(c => c.Attivita.OrderBy(a => a.Ordine))
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (commessa == null) return NotFound();

                var viewModel = MapToViewModel(commessa);
                await LoadDropdownDataAsync(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della commessa per modifica {Id}", id);
                TempData["Error"] = "Errore durante il caricamento della commessa";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Commesse/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CommessaViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var commessa = await _context.Commesse
                        .Include(c => c.Attivita)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (commessa == null) return NotFound();

                    UpdateCommessaFromViewModel(commessa, viewModel);
                    commessa.UpdatedBy = User.Identity?.Name;
                    commessa.UpdatedAt = DateTime.Now;

                    _context.Update(commessa);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Commessa {Id} aggiornata con successo", id);
                    TempData["Success"] = "Commessa aggiornata con successo!";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommessaExists(viewModel.Id))
                        return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante l'aggiornamento della commessa {Id}", id);
                    TempData["Error"] = "Errore durante l'aggiornamento: " + ex.Message;
                }
            }

            await LoadDropdownDataAsync(viewModel);
            return View(viewModel);
        }

        // POST: Commesse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var commessa = await _context.Commesse.FindAsync(id);
                if (commessa != null)
                {
                    _context.Commesse.Remove(commessa);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Commessa {Id} eliminata con successo", id);
                    TempData["Success"] = "Commessa eliminata con successo!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione della commessa {Id}", id);
                TempData["Error"] = "Errore durante l'eliminazione: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Commesse/ChangeStatus/5
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, StatoCommessa nuovoStato)
        {
            try
            {
                var commessa = await _context.Commesse.FindAsync(id);
                if (commessa == null) return NotFound();

                var vecchioStato = commessa.Stato;
                commessa.Stato = nuovoStato;
                commessa.UpdatedBy = User.Identity?.Name;
                commessa.UpdatedAt = DateTime.Now;

                // Se completata, imposta data fine effettiva
                if (nuovoStato == StatoCommessa.Completata && !commessa.DataFineEffettiva.HasValue)
                {
                    commessa.DataFineEffettiva = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Stato commessa {Id} cambiato da {VecchioStato} a {NuovoStato}",
                    id, vecchioStato, nuovoStato);

                return Json(new { success = true, newStatus = nuovoStato.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio stato della commessa {Id}", id);
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Commesse/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var oggi = DateTime.Now;
                var inizioMese = new DateTime(oggi.Year, oggi.Month, 1);
                var inizioAnno = new DateTime(oggi.Year, 1, 1);

                var viewModel = new CommesseDashboardViewModel
                {
                    // Statistiche generali
                    TotaleCommesse = await _context.Commesse.CountAsync(),
                    CommesseAttive = await _context.Commesse.CountAsync(c =>
                        c.Stato == StatoCommessa.Nuova || c.Stato == StatoCommessa.InLavorazione),
                    CommesseInRitardo = await _context.Commesse.CountAsync(c =>
                        c.DataFinePrevista.HasValue && DateTime.Now > c.DataFinePrevista.Value && c.Stato != StatoCommessa.Completata),
                    CommesseUrgenti = await _context.Commesse.CountAsync(c =>
                        c.Priorita == PrioritaCommessa.Urgente && c.Stato != StatoCommessa.Completata),

                    // Commesse recenti
                    CommesseRecenti = await _context.Commesse
                        .Include(c => c.Preventivo)
                        .OrderByDescending(c => c.CreatedAt)
                        .Take(5)
                        .ToListAsync(),

                    // Commesse in scadenza
                    CommesseInScadenza = await _context.Commesse
                        .Where(c => c.DataFinePrevista.HasValue && c.DataFinePrevista.Value <= DateTime.Now.AddDays(7) && c.Stato != StatoCommessa.Completata)
                        .OrderBy(c => c.DataFinePrevista)
                        .Take(10)
                        .ToListAsync(),

                    // Attività in scadenza
                    AttivitaInScadenza = await _context.AttivitaCommesse
                        .Include(a => a.Commessa)
                        .Where(a => !a.Completata && a.DataPrevista.HasValue && a.DataPrevista.Value <= DateTime.Now.AddDays(3))
                        .OrderBy(a => a.DataPrevista)
                        .Take(10)
                        .ToListAsync(),

                    // Statistiche dettagliate
                    Statistiche = new StatisticheCommesseViewModel
                    {
                        TotaleCommesse = await _context.Commesse.CountAsync(),
                        CommesseAttive = await _context.Commesse.CountAsync(c =>
                            c.Stato == StatoCommessa.Nuova || c.Stato == StatoCommessa.InLavorazione),
                        CommesseInRitardo = await _context.Commesse.CountAsync(c =>
                            c.DataFinePrevista.HasValue && DateTime.Now > c.DataFinePrevista.Value && c.Stato != StatoCommessa.Completata),
                        CommesseUrgenti = await _context.Commesse.CountAsync(c =>
                            c.Priorita == PrioritaCommessa.Urgente && c.Stato != StatoCommessa.Completata),
                        FatturatoMese = await _context.Commesse
                            .Where(c => c.DataFineEffettiva >= inizioMese)
                            .SumAsync(c => c.ImportoFatturato),
                        FatturatoAnno = await _context.Commesse
                            .Where(c => c.DataFineEffettiva >= inizioAnno)
                            .SumAsync(c => c.ImportoFatturato),
                        AttivitaInScadenza = await _context.AttivitaCommesse
                            .CountAsync(a => !a.Completata && a.DataPrevista.HasValue && a.DataPrevista.Value <= DateTime.Now.AddDays(7))
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della dashboard commesse");
                TempData["Error"] = "Errore durante il caricamento della dashboard";
                return View(new CommesseDashboardViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchCommesse(string term)
        {
            try
            {
                var commesse = await _context.Commesse
                    .Where(c => c.NumeroCommessa.Contains(term) || c.Cliente.Contains(term))
                    .Select(c => new { id = c.Id, text = $"{c.NumeroCommessa} - {c.Cliente}" })
                    .Take(10)
                    .ToListAsync();

                return Json(commesse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la ricerca commesse");
                return Json(new List<object>());
            }
        }

        // Metodi privati helper
        private bool CommessaExists(int id)
        {
            return _context.Commesse.Any(e => e.Id == id);
        }

        private async Task<string> GenerateNumeroCommessaAsync()
        {
            var anno = DateTime.Now.Year;
            var ultimoNumero = await _context.Commesse
                .Where(c => c.NumeroCommessa.StartsWith($"COMM-{anno}-"))
                .CountAsync();

            return $"COMM-{anno}-{(ultimoNumero + 1):D3}";
        }

        private async Task LoadDropdownDataAsync(CommessaViewModel viewModel)
        {
            // Carica preventivi disponibili
            var preventivi = await _context.Preventivi
                .Where(p => p.Stato == StatoPreventivo.Confermato)
                .Select(p => new Preventivo { Id = p.Id, NumeroPreventivo = p.NumeroPreventivo, Cliente = p.Cliente })
                .ToListAsync();

            // Carica responsabili e tecnici dalle commesse esistenti
            var responsabili = await _context.Commesse
                .Where(c => !string.IsNullOrEmpty(c.Responsabile))
                .Select(c => c.Responsabile!)
                .Distinct()
                .ToListAsync();

            var tecnici = await _context.Commesse
                .Where(c => !string.IsNullOrEmpty(c.TecnicoAssegnato))
                .Select(c => c.TecnicoAssegnato!)
                .Distinct()
                .ToListAsync();

            viewModel.LoadDropdownData(preventivi, responsabili, tecnici);
        }

        private Commessa MapToCommessa(CommessaViewModel viewModel)
        {
            return new Commessa
            {
                NumeroCommessa = viewModel.NumeroCommessa,
                PreventivoId = viewModel.PreventivoId,
                Cliente = viewModel.Cliente,
                IndirizzoCliente = viewModel.IndirizzoCliente,
                Descrizione = viewModel.Descrizione,
                DataInizio = viewModel.DataInizio,
                DataFinePrevista = viewModel.DataFinePrevista,
                DataFineEffettiva = viewModel.DataFineEffettiva,
                Stato = viewModel.Stato,
                Priorita = viewModel.Priorita,
                ImportoTotale = viewModel.ImportoTotale,
                ImportoFatturato = viewModel.ImportoFatturato,
                Responsabile = viewModel.Responsabile,
                TecnicoAssegnato = viewModel.TecnicoAssegnato,
                Note = viewModel.Note,
                NoteInterne = viewModel.NoteInterne
            };
        }

        private CommessaViewModel MapToViewModel(Commessa commessa)
        {
            return new CommessaViewModel
            {
                Id = commessa.Id,
                NumeroCommessa = commessa.NumeroCommessa,
                PreventivoId = commessa.PreventivoId,
                Cliente = commessa.Cliente,
                IndirizzoCliente = commessa.IndirizzoCliente,
                Descrizione = commessa.Descrizione,
                DataInizio = commessa.DataInizio,
                DataFinePrevista = commessa.DataFinePrevista,
                DataFineEffettiva = commessa.DataFineEffettiva,
                Stato = commessa.Stato,
                Priorita = commessa.Priorita,
                ImportoTotale = commessa.ImportoTotale,
                ImportoFatturato = commessa.ImportoFatturato,
                Responsabile = commessa.Responsabile,
                TecnicoAssegnato = commessa.TecnicoAssegnato,
                Note = commessa.Note,
                NoteInterne = commessa.NoteInterne,
                CreatedAt = commessa.CreatedAt,
                CreatedBy = commessa.CreatedBy,
                UpdatedAt = commessa.UpdatedAt,
                UpdatedBy = commessa.UpdatedBy,
                Attivita = commessa.Attivita.Select(a => new AttivitaCommessaViewModel
                {
                    Id = a.Id,
                    CommessaId = a.CommessaId,
                    Descrizione = a.Descrizione,
                    DataPrevista = a.DataPrevista,
                    DataCompletamento = a.DataCompletamento,
                    Completata = a.Completata,
                    Responsabile = a.Responsabile,
                    Note = a.Note,
                    Ordine = a.Ordine,
                    InRitardo = a.InRitardo
                }).ToList()
            };
        }

        private void UpdateCommessaFromViewModel(Commessa commessa, CommessaViewModel viewModel)
        {
            commessa.Cliente = viewModel.Cliente;
            commessa.IndirizzoCliente = viewModel.IndirizzoCliente;
            commessa.Descrizione = viewModel.Descrizione;
            commessa.DataInizio = viewModel.DataInizio;
            commessa.DataFinePrevista = viewModel.DataFinePrevista;
            commessa.DataFineEffettiva = viewModel.DataFineEffettiva;
            commessa.Stato = viewModel.Stato;
            commessa.Priorita = viewModel.Priorita;
            commessa.ImportoTotale = viewModel.ImportoTotale;
            commessa.ImportoFatturato = viewModel.ImportoFatturato;
            commessa.Responsabile = viewModel.Responsabile;
            commessa.TecnicoAssegnato = viewModel.TecnicoAssegnato;
            commessa.Note = viewModel.Note;
            commessa.NoteInterne = viewModel.NoteInterne;
        }
    }
}