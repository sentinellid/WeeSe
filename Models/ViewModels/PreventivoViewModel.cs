using System.ComponentModel.DataAnnotations;

namespace WeeSe.Models.ViewModels
{
    public class PreventivoViewModel
    {
        public int Id { get; set; }

        // ✅ SEZIONE CLIENTE
        [Required(ErrorMessage = "Il cliente è obbligatorio")]
        [Display(Name = "Cliente")]
        public string Cliente { get; set; } = string.Empty;

        [Display(Name = "Indirizzo")]
        public string? Indirizzo { get; set; }

        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; } = DateTime.Now;

        [Display(Name = "Venditore")]
        public string? Venditore { get; set; }

        [Display(Name = "Preventivo N.")]
        public string NumeroPreventivo { get; set; } = string.Empty;

        [Display(Name = "Rif. Ordine")]
        public string? RiferimentoOrdine { get; set; }

        // ✅ DIMENSIONI DINAMICHE
        public List<DimensioneViewModel> Dimensioni { get; set; } = new();

        // ✅ CONFIGURAZIONE PRODOTTO
        [Display(Name = "Finitura")]
        public string? Finitura { get; set; }

        [Display(Name = "Vetro")]
        public string? Vetro { get; set; }

        [Display(Name = "Finitura Vetro")]
        public string? FinituraVetro { get; set; }

        [Display(Name = "Numero vie")]
        [Range(2, 6, ErrorMessage = "Il numero di vie deve essere tra 2 e 6")]
        public int NumeroVie { get; set; } = 2;

        [Display(Name = "Sistema di chiusura")]
        public string? SistemaChiusura { get; set; }

        [Display(Name = "Configurazione Sinistra")]
        public string? ConfigurazioneSinistra { get; set; }

        [Display(Name = "Configurazione Destra")]
        public string? ConfigurazioneDestra { get; set; }

        [Display(Name = "Apertura centrale")]
        public string? AperturaCentrale { get; set; }

        [Display(Name = "Vaschetta di trascinamento")]
        public string? VaschettaTrascinamento { get; set; }

        [Display(Name = "Tappo")]
        public string? Tappo { get; set; }

        [Display(Name = "Trasporto e imballo")]
        public string? TrasportoImballo { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Display(Name = "Stato")]
        public StatoPreventivo Stato { get; set; } = StatoPreventivo.Bozza;

        // ✅ PROPRIETÀ CALCOLATE
        public decimal AreaTotale => Dimensioni.Sum(d => d.Area);
        public int NumeroDimensioni => Dimensioni.Count;

        // ✅ LISTE PER DROPDOWN
        public List<string> FinitureDisponibili { get; set; } = new()
        {
            "bianco 9010",
            "avorio 1013",
            "marrone 8017",
            "ox argento",
            "antracite DK-702"
        };

        public List<string> VetriDisponibili { get; set; } = new()
        {
            "temp. 10mm",
            "55.2 stratificato",
            "55.2 temp/ingl"
        };

        public List<string> FinitureVetroDisponibili { get; set; } = new()
        {
            "chiaro",
            "extra chiaro",
            "fumé",
            "satinato"
        };

        public List<string> SistemiChiusuraDisponibili { get; set; } = new()
        {
            "paletto",
            "pedalina",
            "blocco pedalina",
            "serratura"
        };

        public List<string> VaschetteTrascinamentoDisponibili { get; set; } = new()
        {
            "adesiva trasparente - singola",
            "adesiva trasparente - doppia",
            "acciaio - doppia"
        };

        public List<string> TappiDisponibili { get; set; } = new()
        {
            "metallo",
            "stillicidio"
        };

        public List<string> TrasportiDisponibili { get; set; } = new()
        {
            "ritiro presso ns sede",
            "spedizione con cavalletto"
        };
    }

    // ✅ VIEWMODEL PER SINGOLA DIMENSIONE
    public class DimensioneViewModel
    {
        public int Id { get; set; }

        [Range(1, 10000, ErrorMessage = "Larghezza deve essere tra 1 e 10000 mm")]
        [Display(Name = "Larghezza (mm)")]
        public int LarghezzaL { get; set; }

        [Range(1, 10000, ErrorMessage = "Altezza deve essere tra 1 e 10000 mm")]
        [Display(Name = "Altezza (mm)")]
        public int AltezzaH { get; set; }

        // Proprietà calcolata per l'area
        public decimal Area => LarghezzaL > 0 && AltezzaH > 0 ? (LarghezzaL * AltezzaH) / 1000000m : 0;

        // Descrizione leggibile
        public string Descrizione => $"{LarghezzaL} x {AltezzaH} mm ({Area:F2} m²)";
    }

    // ✅ VIEWMODEL PER LISTA PREVENTIVI
    public class PreventivoListViewModel
    {
        public List<Preventivo> Preventivi { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public StatoPreventivo? FiltroStato { get; set; }

        // Proprietà per paginazione
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        // Range di pagine da mostrare
        public IEnumerable<int> PageNumbers
        {
            get
            {
                var start = Math.Max(1, PageIndex - 2);
                var end = Math.Min(TotalPages, PageIndex + 2);
                return Enumerable.Range(start, end - start + 1);
            }
        }
    }

    // ✅ VIEWMODEL PER DETTAGLI PREVENTIVO
    public class PreventivoDetailsViewModel
    {
        public Preventivo Preventivo { get; set; } = new();
        public List<DimensioneFinita> Dimensioni { get; set; } = new();
        public bool CanEdit { get; set; } = true;
        public bool CanDelete { get; set; } = true;
        public string? ReturnUrl { get; set; }
    }
}