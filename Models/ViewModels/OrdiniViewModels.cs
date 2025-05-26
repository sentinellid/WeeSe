using System.ComponentModel.DataAnnotations;
using WeeSe.Models;
namespace WeeSe.Models.ViewModels
{
    public class OrdineViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il numero ordine è richiesto")]
        [Display(Name = "Numero Ordine")]
        public string NumeroOrdine { get; set; } = string.Empty;

        [Display(Name = "Commessa")]
        public int? CommessaId { get; set; }

        [Display(Name = "Preventivo")]
        public int? PreventivoId { get; set; }

        [Required(ErrorMessage = "Il cliente è richiesto")]
        [Display(Name = "Cliente")]
        public string Cliente { get; set; } = string.Empty;

        [Display(Name = "Indirizzo Spedizione")]
        public string? IndirizzoSpedizione { get; set; }

        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Required(ErrorMessage = "La data ordine è richiesta")]
        [Display(Name = "Data Ordine")]
        [DataType(DataType.Date)]
        public DateTime DataOrdine { get; set; } = DateTime.Now;

        [Display(Name = "Data Consegna Richiesta")]
        [DataType(DataType.Date)]
        public DateTime? DataConsegnaRichiesta { get; set; }

        [Display(Name = "Data Consegna Effettiva")]
        [DataType(DataType.Date)]
        public DateTime? DataConsegnaEffettiva { get; set; }

        [Display(Name = "Stato")]
        public StatoOrdine Stato { get; set; } = StatoOrdine.Nuovo;

        [Display(Name = "Priorità")]
        public PrioritaOrdine Priorita { get; set; } = PrioritaOrdine.Media;

        [Display(Name = "Importo Totale")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal ImportoTotale { get; set; }

        [Display(Name = "Importo Pagato")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal ImportoPagato { get; set; }

        [Display(Name = "Responsabile")]
        public string? Responsabile { get; set; }

        [Display(Name = "Fornitore")]
        public string? Fornitore { get; set; }

        [Display(Name = "Numero Tracking")]
        public string? NumeroTracking { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Display(Name = "Note Interne")]
        public string? NoteInterne { get; set; }

        // Metadati
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Liste per dropdown
        public List<Commessa> CommesseDisponibili { get; set; } = new();
        public List<Preventivo> PreventiviDisponibili { get; set; } = new();
        public List<string> ResponsabiliDisponibili { get; set; } = new();
        public List<string> FornitoriDisponibili { get; set; } = new();

        public void LoadDropdownData(IEnumerable<Commessa> commesse,
                                   IEnumerable<Preventivo> preventivi,
                                   IEnumerable<string> responsabili,
                                   IEnumerable<string> fornitori)
        {
            CommesseDisponibili = commesse.ToList();
            PreventiviDisponibili = preventivi.ToList();
            ResponsabiliDisponibili = responsabili.ToList();
            FornitoriDisponibili = fornitori.ToList();
        }
    }

    public class OrdiniIndexViewModel
    {
        public List<Ordine> Ordini { get; set; } = new();
        public OrdiniFiltriViewModel Filtri { get; set; } = new();
        public int TotalCount { get; set; }
        public StatisticheOrdiniViewModel StatisticheRapide { get; set; } = new();

        public bool HasPreviousPage => Filtri.PageIndex > 1;
        public bool HasNextPage => Filtri.PageIndex < TotalPages;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)Filtri.PageSize);
    }

    public class OrdiniFiltriViewModel
    {
        public string? SearchTerm { get; set; }
        public StatoOrdine? Stato { get; set; }
        public PrioritaOrdine? Priorita { get; set; }
        public string? Responsabile { get; set; }
        public string? Fornitore { get; set; }
        public DateTime? DataDal { get; set; }
        public DateTime? DataAl { get; set; }
        public bool SoloInRitardo { get; set; }
        public bool SoloUrgenti { get; set; }
        public bool SoloNonPagati { get; set; }
        public string Ordinamento { get; set; } = "data_desc";
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class StatisticheOrdiniViewModel
    {
        public int TotaleOrdini { get; set; }
        public int OrdiniAttivi { get; set; }
        public int OrdiniInRitardo { get; set; }
        public int OrdiniUrgenti { get; set; }
        public decimal FatturatoMese { get; set; }
        public decimal ImportoDaPagare { get; set; }
        public int OrdiniSpediti { get; set; }
        public int OrdiniInProduzione { get; set; }
    }

    public class OrdiniDashboardViewModel
    {
        public int TotaleOrdini { get; set; }
        public int OrdiniAttivi { get; set; }
        public int OrdiniInRitardo { get; set; }
        public int OrdiniUrgenti { get; set; }
        public List<Ordine> OrdiniRecenti { get; set; } = new();
        public List<Ordine> OrdiniInScadenza { get; set; } = new();
        public List<Ordine> OrdiniDaSpedire { get; set; } = new();
        public StatisticheOrdiniViewModel Statistiche { get; set; } = new();
    }
}