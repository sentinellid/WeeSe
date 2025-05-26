using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeeSe.Data;
using WeeSe.Models;
using WeeSe.Models.ViewModels;

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
            var preventivi = await _context.Preventivi
                .Include(p => p.Dimensioni)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(preventivi); // ✅ Questa View usa List<Preventivo>, non ViewModel
        }

        // GET: Preventivi/Details/5  
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var preventivo = await _context.Preventivi
                .Include(p => p.Dimensioni.OrderBy(d => d.Numero))
                .FirstOrDefaultAsync(m => m.Id == id);

            if (preventivo == null) return NotFound();

            return View(preventivo); // ✅ Questa View usa Preventivo, non ViewModel
        }

        // GET: Preventivi/Create ⚠️ QUESTA USA VIEWMODEL
        public IActionResult Create()
        {
            var viewModel = new PreventivoViewModel
            {
                NumeroPreventivo = GenerateNumeroPreventivo(),
                Venditore = User.Identity?.Name,
                Data = DateTime.Now,
                // ✅ Inizializza con 3 dimensioni vuote
                Dimensioni = new List<DimensioneViewModel>
                {
                    new DimensioneViewModel(),
                    new DimensioneViewModel(),
                    new DimensioneViewModel()
                }
            };

            return View(viewModel); // ✅ PASSA IL VIEWMODEL, NON IL MODEL
        }

        // POST: Preventivi/Create ⚠️ QUESTA USA VIEWMODEL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PreventivoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // ✅ CONVERTI VIEWMODEL → MODEL
                    var preventivo = MapViewModelToModel(viewModel);
                    preventivo.CreatedBy = User.Identity?.Name;
                    preventivo.CreatedAt = DateTime.Now;
                    preventivo.Stato = StatoPreventivo.Bozza;

                    _context.Preventivi.Add(preventivo);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("✅ Preventivo {NumeroPreventivo} creato con successo!", preventivo.NumeroPreventivo);
                    TempData["Success"] = $"✅ Preventivo {preventivo.NumeroPreventivo} creato con successo!";

                    return RedirectToAction(nameof(Details), new { id = preventivo.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Errore durante la creazione del preventivo");
                    TempData["Error"] = "❌ Errore durante il salvataggio: " + ex.Message;
                }
            }
            else
            {
                TempData["Warning"] = "⚠️ Controlla i dati inseriti";
            }

            return View(viewModel); // ✅ RITORNA IL VIEWMODEL IN CASO DI ERRORE
        }

        // GET: Preventivi/Edit/5 ⚠️ QUESTA USA VIEWMODEL
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var preventivo = await _context.Preventivi
                .Include(p => p.Dimensioni.OrderBy(d => d.Numero))
                .FirstOrDefaultAsync(p => p.Id == id);

            if (preventivo == null) return NotFound();

            // ✅ CONVERTI MODEL → VIEWMODEL
            var viewModel = MapModelToViewModel(preventivo);

            return View(viewModel); // ✅ PASSA IL VIEWMODEL
        }

        // POST: Preventivi/Edit/5 ⚠️ QUESTA USA VIEWMODEL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PreventivoViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var preventivo = await _context.Preventivi
                        .Include(p => p.Dimensioni)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (preventivo == null) return NotFound();

                    // ✅ AGGIORNA IL MODEL CON I DATI DAL VIEWMODEL
                    UpdateModelFromViewModel(preventivo, viewModel);
                    preventivo.UpdatedBy = User.Identity?.Name;
                    preventivo.UpdatedAt = DateTime.Now;

                    // ✅ Rimuovi dimensioni esistenti
                    _context.DimensioniFinite.RemoveRange(preventivo.Dimensioni);

                    // ✅ Aggiungi nuove dimensioni valide
                    int numeroDimensione = 1;
                    foreach (var dim in viewModel.Dimensioni.Where(d => d.LarghezzaL > 0 && d.AltezzaH > 0))
                    {
                        preventivo.Dimensioni.Add(new DimensioneFinita
                        {
                            Numero = numeroDimensione++,
                            LarghezzaL = dim.LarghezzaL,
                            AltezzaH = dim.AltezzaH,
                            PreventivoId = preventivo.Id
                        });
                    }

                    _context.Update(preventivo);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "✅ Preventivo aggiornato con successo!";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante l'aggiornamento del preventivo {Id}", id);
                    TempData["Error"] = "❌ Errore durante l'aggiornamento: " + ex.Message;
                }
            }

            return View(viewModel); // ✅ RITORNA IL VIEWMODEL IN CASO DI ERRORE
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
                    TempData["Success"] = "✅ Preventivo eliminato con successo!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del preventivo {Id}", id);
                TempData["Error"] = "❌ Errore durante l'eliminazione: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ METODI DI MAPPING PRIVATI
        private PreventivoViewModel MapModelToViewModel(Preventivo preventivo)
        {
            var viewModel = new PreventivoViewModel
            {
                Id = preventivo.Id,
                Cliente = preventivo.Cliente,
                Indirizzo = preventivo.Indirizzo,
                Data = preventivo.Data,
                Venditore = preventivo.Venditore,
                NumeroPreventivo = preventivo.NumeroPreventivo,
                RiferimentoOrdine = preventivo.RiferimentoOrdine,
                Finitura = preventivo.Finitura,
                Vetro = preventivo.Vetro,
                FinituraVetro = preventivo.FinituraVetro,
                NumeroVie = preventivo.NumeroVie,
                SistemaChiusura = preventivo.SistemaChiusura,
                ConfigurazioneSinistra = preventivo.ConfigurazioneSinistra,
                ConfigurazioneDestra = preventivo.ConfigurazioneDestra,
                AperturaCentrale = preventivo.AperturaCentrale,
                VaschettaTrascinamento = preventivo.VaschettaTrascinamento,
                Tappo = preventivo.Tappo,
                TrasportoImballo = preventivo.TrasportoImballo,
                Note = preventivo.Note,
                Stato = preventivo.Stato,
                Dimensioni = preventivo.Dimensioni?.Select(d => new DimensioneViewModel
                {
                    Id = d.Id,
                    LarghezzaL = d.LarghezzaL,
                    AltezzaH = d.AltezzaH
                }).ToList() ?? new List<DimensioneViewModel>()
            };

            // ✅ Assicurati che ci siano almeno 3 dimensioni per il form
            while (viewModel.Dimensioni.Count < 3)
            {
                viewModel.Dimensioni.Add(new DimensioneViewModel());
            }

            return viewModel;
        }

        private Preventivo MapViewModelToModel(PreventivoViewModel viewModel)
        {
            var preventivo = new Preventivo
            {
                Cliente = viewModel.Cliente,
                Indirizzo = viewModel.Indirizzo,
                Data = viewModel.Data,
                Venditore = viewModel.Venditore,
                NumeroPreventivo = viewModel.NumeroPreventivo,
                RiferimentoOrdine = viewModel.RiferimentoOrdine,
                Finitura = viewModel.Finitura,
                Vetro = viewModel.Vetro,
                FinituraVetro = viewModel.FinituraVetro,
                NumeroVie = viewModel.NumeroVie,
                SistemaChiusura = viewModel.SistemaChiusura,
                ConfigurazioneSinistra = viewModel.ConfigurazioneSinistra,
                ConfigurazioneDestra = viewModel.ConfigurazioneDestra,
                AperturaCentrale = viewModel.AperturaCentrale,
                VaschettaTrascinamento = viewModel.VaschettaTrascinamento,
                Tappo = viewModel.Tappo,
                TrasportoImballo = viewModel.TrasportoImballo,
                Note = viewModel.Note,
                Stato = viewModel.Stato
            };

            // ✅ Aggiungi dimensioni valide
            int numeroDimensione = 1;
            foreach (var dim in viewModel.Dimensioni.Where(d => d.LarghezzaL > 0 && d.AltezzaH > 0))
            {
                preventivo.Dimensioni.Add(new DimensioneFinita
                {
                    Numero = numeroDimensione++,
                    LarghezzaL = dim.LarghezzaL,
                    AltezzaH = dim.AltezzaH
                });
            }

            return preventivo;
        }

        private void UpdateModelFromViewModel(Preventivo preventivo, PreventivoViewModel viewModel)
        {
            preventivo.Cliente = viewModel.Cliente;
            preventivo.Indirizzo = viewModel.Indirizzo;
            preventivo.Data = viewModel.Data;
            preventivo.Venditore = viewModel.Venditore;
            preventivo.RiferimentoOrdine = viewModel.RiferimentoOrdine;
            preventivo.Finitura = viewModel.Finitura;
            preventivo.Vetro = viewModel.Vetro;
            preventivo.FinituraVetro = viewModel.FinituraVetro;
            preventivo.NumeroVie = viewModel.NumeroVie;
            preventivo.SistemaChiusura = viewModel.SistemaChiusura;
            preventivo.ConfigurazioneSinistra = viewModel.ConfigurazioneSinistra;
            preventivo.ConfigurazioneDestra = viewModel.ConfigurazioneDestra;
            preventivo.AperturaCentrale = viewModel.AperturaCentrale;
            preventivo.VaschettaTrascinamento = viewModel.VaschettaTrascinamento;
            preventivo.Tappo = viewModel.Tappo;
            preventivo.TrasportoImballo = viewModel.TrasportoImballo;
            preventivo.Note = viewModel.Note;
            preventivo.Stato = viewModel.Stato;
        }

        private string GenerateNumeroPreventivo()
        {
            return $"PRV-{DateTime.Now:yyyyMMdd}-{DateTime.Now.Ticks.ToString()[^6..]}";
        }

        // ================================
        // 🎯 ACTION PER ACCETTARE PREVENTIVO
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accetta(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Trova e verifica preventivo
                var preventivo = await _context.Preventivi
                    .Include(p => p.Cliente)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (preventivo == null)
                    return Json(new { success = false, error = "Preventivo non trovato" });

                // 2️⃣ Aggiorna preventivo
                preventivo.Stato = StatoPreventivo.Confermato;
                preventivo.UpdatedBy = User.Identity?.Name;
                preventivo.UpdatedAt = DateTime.Now;

                // 3️⃣ 🚀 CREA ORDINE AUTOMATICAMENTE
                var numeroOrdine = await GeneraNumeroOrdineAsync();
                var ordine = new Ordine
                {
                    PreventivoId = preventivo.Id,
                    NumeroOrdine = numeroOrdine,
                    Cliente = preventivo.Cliente,
                    //IndirizzoSpedizione = preventivo.Cliente.,
                    Descrizione = $"Ordine generato da preventivo {preventivo.NumeroPreventivo}",
                    DataOrdine = DateTime.Now,
                    DataConsegnaRichiesta = DateTime.Now.AddDays(15), // Default 15 giorni
                    Stato = StatoOrdine.Confermato, // 🎯 STATO INIZIALE
                    Priorita = PrioritaOrdine.Media,
                    ImportoTotale = preventivo.ImportoTotale,
                    Responsabile = User.Identity?.Name,
                    Note = $"Generato automaticamente dal preventivo {preventivo.NumeroPreventivo}",
                    CreatedBy = User.Identity?.Name,
                    CreatedAt = DateTime.Now
                };

                _context.Ordini.Add(ordine);
                await _context.SaveChangesAsync(); // 💾 Salva per ottenere ordine.Id

                // 4️⃣ 🚀 CREA COMMESSA AUTOMATICAMENTE  
                var numeroCommessa = await GeneraNumeroCommessaAsync();
                var commessa = new Commessa
                {
                    OrdineId = ordine.Id, // 🔗 Link all'ordine
                    PreventivoId = preventivo.Id, // 🔗 Link al preventivo
                    NumeroCommessa = numeroCommessa,
                    Descrizione = $"Commessa per ordine {ordine.NumeroOrdine}",
                    DataInizio = DateTime.Now,
                    DataFinePrevista = DateTime.Now.AddDays(10), // Default 10 giorni lavorazione
                    Stato = StatoCommessa.Nuova, // 🎯 STATO INIZIALE
                    Priorita = PrioritaCommessa.Media,
                    ImportoTotale = preventivo.ImportoTotale,
                    Responsabile = User.Identity?.Name,
                    Note = $"Generata automaticamente dall'ordine {ordine.NumeroOrdine}",
                    CreatedBy = User.Identity?.Name,
                    CreatedAt = DateTime.Now
                };

                _context.Commesse.Add(commessa);

                // 5️⃣ 💾 SALVA TUTTO
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 6️⃣ 📝 LOG
                _logger?.LogInformation("✅ Preventivo {PreventivoId} accettato. Creati Ordine {OrdineId} e Commessa {CommessaId}",
                    preventivo.Id, ordine.Id, commessa.Id);

                // 7️⃣ 🎉 RISPOSTA JSON
                return Json(new
                {
                    success = true,
                    message = "Preventivo accettato con successo!",
                    numeroOrdine = numeroOrdine,
                    numeroCommessa = numeroCommessa,
                    ordineId = ordine.Id,
                    commessaId = commessa.Id,
                    redirectUrl = Url.Action("Details", "Ordini", new { id = ordine.Id })
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger?.LogError(ex, "❌ Errore durante l'accettazione del preventivo {Id}", id);
                return Json(new
                {
                    success = false,
                    error = $"Errore durante l'accettazione: {ex.Message}"
                });
            }
        }

        // ================================
        // 🎯 ACTION PER CAMBIO STATO NORMALE
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, StatoPreventivo nuovoStato)
        {
            try
            {
                var preventivo = await _context.Preventivi.FindAsync(id);

                if (preventivo == null)
                    return Json(new { success = false, error = "Preventivo non trovato" });

                // Validazioni cambio stato
                if (!IsValidStateTransition(preventivo.Stato, nuovoStato))
                {
                    return Json(new
                    {
                        success = false,
                        error = $"Cambio stato non valido da {preventivo.Stato} a {nuovoStato}"
                    });
                }

                // Aggiorna stato
                preventivo.Stato = nuovoStato;
                preventivo.UpdatedBy = User.Identity?.Name;
                preventivo.UpdatedAt = DateTime.Now;

                // Date specifiche per alcuni stati

                preventivo.Data = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger?.LogInformation("Stato preventivo {PreventivoId} cambiato da {StatoVecchio} a {StatoNuovo}",
                    id, preventivo.Stato, nuovoStato);

                return Json(new
                {
                    success = true,
                    message = $"Stato aggiornato a: {nuovoStato}",
                    nuovoStato = nuovoStato.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Errore durante il cambio stato preventivo {Id}", id);
                return Json(new
                {
                    success = false,
                    error = $"Errore durante il cambio stato: {ex.Message}"
                });
            }
        }

        // ================================
        // 🛠️ METODI HELPER
        // ================================
        private async Task<string> GeneraNumeroOrdineAsync()
        {
            var anno = DateTime.Now.Year;
            var ultimoNumero = await _context.Ordini
                .Where(o => o.NumeroOrdine.StartsWith($"ORD-{anno}"))
                .CountAsync();
            return $"ORD-{anno}-{(ultimoNumero + 1):D3}";
        }

        private async Task<string> GeneraNumeroCommessaAsync()
        {
            var anno = DateTime.Now.Year;
            var ultimoNumero = await _context.Commesse
                .Where(c => c.NumeroCommessa.StartsWith($"COM-{anno}"))
                .CountAsync();
            return $"COM-{anno}-{(ultimoNumero + 1):D3}";
        }

        private static bool IsValidStateTransition(StatoPreventivo statoCorrente, StatoPreventivo nuovoStato)
        {
            return statoCorrente switch
            {
                // Da BOZZA puoi andare a CONFERMATO o ANNULLATO
                StatoPreventivo.Bozza => nuovoStato is StatoPreventivo.Confermato or StatoPreventivo.Annullato,

                // Stati finali - NON possono più cambiare
                StatoPreventivo.Confermato => false,
                StatoPreventivo.Annullato => false,

                _ => false
            };
        }
    }
}