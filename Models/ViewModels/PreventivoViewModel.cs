using System.ComponentModel.DataAnnotations;
using WeeSe.Dimensions;

namespace WeeSe.Models.ViewModels
{
    public class PreventivoViewModel
    {
        public int Id { get; set; }

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

        public List<DimensioneViewModel> Dimensioni { get; set; } = new();

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

        public decimal AreaTotale => Dimensioni.Sum(d => d.Area);
        public int NumeroDimensioni => Dimensioni.Count;

        public Dimensions.Dimensions? Dimensions { get; set; } = new Dimensions.Dimensions();

    }

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

    public class PreventivoDetailsViewModel
    {
        public Preventivo Preventivo { get; set; } = new();
        public List<DimensioneFinita> Dimensioni { get; set; } = new();
        public bool CanEdit { get; set; } = true;
        public bool CanDelete { get; set; } = true;
        public string? ReturnUrl { get; set; }
    }
}