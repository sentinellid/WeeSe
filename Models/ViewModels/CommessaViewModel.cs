using System.ComponentModel.DataAnnotations;

namespace WeeSe.Models.ViewModels
{
    public class CommessaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il numero commessa è richiesto")]
        [Display(Name = "Numero Commessa")]
        public string NumeroCommessa { get; set; } = string.Empty;

        [Display(Name = "Preventivo")]
        public int? PreventivoId { get; set; }

        [Required(ErrorMessage = "Il cliente è richiesto")]
        [Display(Name = "Cliente")]
        public string Cliente { get; set; } = string.Empty;

        [Display(Name = "Indirizzo Cliente")]
        public string? IndirizzoCliente { get; set; }

        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Required(ErrorMessage = "La data di inizio è richiesta")]
        [Display(Name = "Data Inizio")]
        [DataType(DataType.Date)]
        public DateTime DataInizio { get; set; } = DateTime.Now;

        [Display(Name = "Data Fine Prevista")]
        [DataType(DataType.Date)]
        public DateTime? DataFinePrevista { get; set; }

        [Display(Name = "Data Fine Effettiva")]
        [DataType(DataType.Date)]
        public DateTime? DataFineEffettiva { get; set; }

        [Display(Name = "Stato")]
        public StatoCommessa Stato { get; set; } = StatoCommessa.Nuova;

        [Display(Name = "Priorità")]
        public PrioritaCommessa Priorita { get; set; } = PrioritaCommessa.Media;

        [Display(Name = "Importo Totale")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal ImportoTotale { get; set; }

        [Display(Name = "Importo Fatturato")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal ImportoFatturato { get; set; }

        [Display(Name = "Responsabile")]
        public string? Responsabile { get; set; }

        [Display(Name = "Tecnico Assegnato")]
        public string? TecnicoAssegnato { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Display(Name = "Note Interne")]
        public string? NoteInterne { get; set; }

        // Metadati
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Attività
        public List<AttivitaCommessaViewModel> Attivita { get; set; } = new();

        // Liste per dropdown
        public List<Preventivo> PreventiviDisponibili { get; set; } = new();
        public List<string> ResponsabiliDisponibili { get; set; } = new();
        public List<string> TecniciDisponibili { get; set; } = new();

        public void LoadDropdownData(IEnumerable<Preventivo> preventivi,
                                   IEnumerable<string> responsabili,
                                   IEnumerable<string> tecnici)
        {
            PreventiviDisponibili = preventivi.ToList();
            ResponsabiliDisponibili = responsabili.ToList();
            TecniciDisponibili = tecnici.ToList();
        }
    }

    public class AttivitaCommessaViewModel
    {
        public int Id { get; set; }
        public int CommessaId { get; set; }

        [Required(ErrorMessage = "La descrizione è richiesta")]
        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        [Display(Name = "Data Prevista")]
        [DataType(DataType.Date)]
        public DateTime? DataPrevista { get; set; }

        [Display(Name = "Data Completamento")]
        [DataType(DataType.DateTime)]
        public DateTime? DataCompletamento { get; set; }

        [Display(Name = "Completata")]
        public bool Completata { get; set; }

        [Display(Name = "Responsabile")]
        public string? Responsabile { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Display(Name = "Ordine")]
        public int Ordine { get; set; }

        public bool InRitardo { get; set; }
    }

    public class CommesseIndexViewModel
    {
        public List<Commessa> Commesse { get; set; } = new();
        public CommesseFiltriViewModel Filtri { get; set; } = new();
        public int TotalCount { get; set; }
        public StatisticheCommesseViewModel StatisticheRapide { get; set; } = new();

        public bool HasPreviousPage => Filtri.PageIndex > 1;
        public bool HasNextPage => Filtri.PageIndex < TotalPages;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)Filtri.PageSize);
    }

    public class CommesseFiltriViewModel
    {
        public string? SearchTerm { get; set; }
        public StatoCommessa? Stato { get; set; }
        public PrioritaCommessa? Priorita { get; set; }
        public string? Responsabile { get; set; }
        public DateTime? DataDal { get; set; }
        public DateTime? DataAl { get; set; }
        public bool SoloInRitardo { get; set; }
        public bool SoloUrgenti { get; set; }
        public string Ordinamento { get; set; } = "data_desc";
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class StatisticheCommesseViewModel
    {
        public int TotaleCommesse { get; set; }
        public int CommesseAttive { get; set; }
        public int CommesseInRitardo { get; set; }
        public int CommesseUrgenti { get; set; }
        public decimal FatturatoMese { get; set; }
        public decimal FatturatoAnno { get; set; }
        public int AttivitaInScadenza { get; set; }
        public double TempoMedioCompletamento { get; set; }
    }

    public class CommesseDashboardViewModel
    {
        public int TotaleCommesse { get; set; }
        public int CommesseAttive { get; set; }
        public int CommesseInRitardo { get; set; }
        public int CommesseUrgenti { get; set; }
        public List<Commessa> CommesseRecenti { get; set; } = new();
        public List<Commessa> CommesseInScadenza { get; set; } = new();
        public List<AttivitaCommessa> AttivitaInScadenza { get; set; } = new();
        public StatisticheCommesseViewModel Statistiche { get; set; } = new();
    }
}