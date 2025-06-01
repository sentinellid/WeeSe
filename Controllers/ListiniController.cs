// Controllers/ListiniController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeeSe.Data;
using WeeSe.Models;
using WeeSe.ViewModels;

namespace WeeSe.Controllers
{
    [Authorize]
    public class ListiniController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListiniController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var query = _context.Listini.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(l => l.NomeListino.Contains(searchTerm));
            }


            var listini = await query
                .Select(l => new ListinoListItem
                {
                    IDListino = l.IDListino,
                    NomeListino = l.NomeListino,
                    PerTrasporto = l.PerTrasporto,
                    CostoCapra = l.CostoCapra,
                    PerImballo = l.PerImballo
                })
                .OrderBy(l => l.NomeListino)
                .ToListAsync();

            var viewModel = new ListinoIndexViewModel
            {
                Listini = listini,
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var listino = await _context.Listini.FindAsync(id);
            if (listino == null)
            {
                return NotFound();
            }

            return View(listino);
        }

        public IActionResult Create()
        {
            return View(new Listino());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Listino listino)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Listini.Add(listino);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Listino creato con successo!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Errore durante la creazione del listino.";
                }
            }

            return View(listino);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var listino = await _context.Listini.FindAsync(id);
            if (listino == null)
            {
                return NotFound();
            }

            // Query per i prodotti associati
            var prodotti = await (from l in _context.Listini
                                  where l.IDListino == id
                                  join lp in _context.Set<ListinoProdotto>() on l.IDListino equals lp.IDListino
                                  join p in _context.Set<Prodotto>() on lp.IDTipoProdotto equals p.IDTipoProdotto
                                  select new ListinoProdottoEditItem
                                  {
                                      IDListino = l.IDListino,
                                      IDTipoProdotto = lp.IDTipoProdotto,
                                      NomeListino = l.NomeListino,
                                      PerTrasporto = l.PerTrasporto,
                                      CostoCapra = l.CostoCapra,
                                      PerImballo = l.PerImballo,
                                      Prezzo = lp.Prezzo,
                                      PrezzoV = lp.PrezzoV,
                                      PrezzoVV = lp.PrezzoVV,
                                      TipoCalcolo = lp.TipoCalcolo,
                                      LimiteAltezza = lp.LimiteAltezza,
                                      SNome = p.SNome
                                  }).ToListAsync();

            var viewModel = new ListinoEditViewModel
            {
                Listino = listino,
                Prodotti = prodotti
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateListino(Listino listino)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Listini.Update(listino);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Listino aggiornato con successo!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Errore durante l'aggiornamento del listino." });
                }
            }

            return Json(new { success = false, message = "Dati non validi." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProdotto([FromBody] ListinoProdottoEditItem item)
        {
            try
            {
                var listinoProdotto = await _context.Set<ListinoProdotto>().FindAsync(item.Id);
                if (listinoProdotto != null)
                {
                    listinoProdotto.Prezzo = item.Prezzo;
                    listinoProdotto.PrezzoV = item.PrezzoV;
                    listinoProdotto.PrezzoVV = item.PrezzoVV;
                    listinoProdotto.TipoCalcolo = item.TipoCalcolo;
                    listinoProdotto.LimiteAltezza = item.LimiteAltezza;

                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Prodotto aggiornato con successo!" });
                }

                return Json(new { success = false, message = "Prodotto non trovato." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Errore durante l'aggiornamento." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var listino = await _context.Listini.FindAsync(id);
                if (listino != null)
                {
                    _context.Listini.Remove(listino);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Listino eliminato con successo!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Listino non trovato.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Errore durante l'eliminazione del listino.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}