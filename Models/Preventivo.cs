using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeeSe.Models
{
    [Table("TBL_Preventivi")]
    public class Preventivo
    {
        public int Id { get; set; }

        [Display(Name = "Sistema tutto vetro weese scorrevole")]
        public string SistemaDescrizione { get; set; } = "Sistema tutto vetro weese scorrevole";

        // Sezione Cliente
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

        [Display(Name = "Preventivo")]
        public string NumeroPreventivo { get; set; } = string.Empty;

        [Display(Name = "Rif. Ordine")]
        public string? RiferimentoOrdine { get; set; }

        public virtual ICollection<DimensioneFinita> Dimensioni { get; set; } = new List<DimensioneFinita>();

        // Finitura
        [Display(Name = "Finitura")]
        public string? Finitura { get; set; }

        // Vetro
        [Display(Name = "Vetro")]
        public string? Vetro { get; set; }

        [Display(Name = "Finitura Vetro")]
        public string? FinituraVetro { get; set; }

        // Numero vie
        [Display(Name = "Numero vie")]
        public int NumeroVie { get; set; } = 2;

        // Sistema di chiusura
        [Display(Name = "Sistema di chiusura")]
        public string? SistemaChiusura { get; set; }

        // Configurazione aperture
        [Display(Name = "Sinistra")]
        public string? ConfigurazioneSinistra { get; set; }

        [Display(Name = "Destra")]
        public string? ConfigurazioneDestra { get; set; }

        [Display(Name = "Apertura centrale")]
        public string? AperturaCentrale { get; set; }

        // Vaschetta di trascinamento
        [Display(Name = "Vaschetta di trascinamento")]
        public string? VaschettaTrascinamento { get; set; }

        // Tappo
        [Display(Name = "Tappo")]
        public string? Tappo { get; set; }

        // Trasporto e imballo
        [Display(Name = "Trasporto e imballo")]
        public string? TrasportoImballo { get; set; }

        // Note
        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Display(Name = "Firma per accettazione")]
        public string? FirmaAccettazione { get; set; }

        // Metadati
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Totali
        [Display(Name = "Subtotale")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Subtotale { get; set; }

        [Display(Name = "IVA")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Iva { get; set; }

        [Display(Name = "Totale")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Totale { get; set; }

        // Stato del preventivo
        [Display(Name = "Stato")]
        public StatoPreventivo Stato { get; set; } = StatoPreventivo.Bozza;

        public decimal AreaTotale => Dimensioni?.Sum(d => d.Area) ?? 0;
        public int NumeroDimensioni => Dimensioni?.Count ?? 0;

        public decimal ImportoTotale;

        // Calcolo automatico dei totali
        public void CalcolaTotali()
        {
            // Qui implementerai la logica di calcolo
            // Subtotale = calcolo base sui componenti
            // Iva = Subtotale * 0.22m (22%)
            // Totale = Subtotale + Iva
        }
    }

    public enum StatoPreventivo
    {
        Bozza,
        Confermato,
        Annullato     // ← Questo invece di "Scaduto"
    }
}