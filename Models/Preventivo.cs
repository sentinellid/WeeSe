using System.ComponentModel.DataAnnotations;

namespace WeeSe.Models
{
    public class Preventivo
    {
        public int Id { get; set; }

        // Header
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

        // Dimensioni (3 sezioni)
        [Display(Name = "Dimensione 1 - Larghezza (mm)")]
        public int? Dimensione1_Larghezza { get; set; }

        [Display(Name = "Dimensione 1 - Altezza (mm)")]
        public int? Dimensione1_Altezza { get; set; }

        [Display(Name = "Dimensione 2 - Larghezza (mm)")]
        public int? Dimensione2_Larghezza { get; set; }

        [Display(Name = "Dimensione 2 - Altezza (mm)")]
        public int? Dimensione2_Altezza { get; set; }

        [Display(Name = "Dimensione 3 - Larghezza (mm)")]
        public int? Dimensione3_Larghezza { get; set; }

        [Display(Name = "Dimensione 3 - Altezza (mm)")]
        public int? Dimensione3_Altezza { get; set; }

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

        // Metadati
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Stato del preventivo
        [Display(Name = "Stato")]
        public StatoPreventivo Stato { get; set; } = StatoPreventivo.Bozza;
    }

    public enum StatoPreventivo
    {
        Bozza,
        Inviato,
        Approvato,
        Rifiutato,
        Confermato,
        Annullato
    }
}