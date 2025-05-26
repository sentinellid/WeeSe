using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeeSe.Models
{
    [Table("TBL_Ordini")]
    public class Ordine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Numero Ordine")]
        public string NumeroOrdine { get; set; } = string.Empty;

        [Display(Name = "Commessa")]
        public int? CommessaId { get; set; }

        [ForeignKey("CommessaId")]
        public virtual Commessa? Commessa { get; set; }

        [Display(Name = "Preventivo")]
        public int? PreventivoId { get; set; }

        [ForeignKey("PreventivoId")]
        public virtual Preventivo? Preventivo { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Cliente")]
        public string Cliente { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Indirizzo Spedizione")]
        public string? IndirizzoSpedizione { get; set; }

        [StringLength(1000)]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

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
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportoTotale { get; set; }

        [Display(Name = "Importo Pagato")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportoPagato { get; set; }

        [StringLength(100)]
        [Display(Name = "Responsabile")]
        public string? Responsabile { get; set; }

        [StringLength(100)]
        [Display(Name = "Fornitore")]
        public string? Fornitore { get; set; }

        [StringLength(100)]
        [Display(Name = "Numero Tracking")]
        public string? NumeroTracking { get; set; }

        [StringLength(2000)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        [StringLength(2000)]
        [Display(Name = "Note Interne")]
        public string? NoteInterne { get; set; }

        // Metadati
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Proprietà calcolate
        [NotMapped]
        public bool InRitardo => DataConsegnaRichiesta.HasValue && DateTime.Now > DataConsegnaRichiesta.Value && Stato != StatoOrdine.Consegnato;

        [NotMapped]
        public decimal ImportoResiduo => ImportoTotale - ImportoPagato;

        [NotMapped]
        public bool PagamentoCompleto => ImportoPagato >= ImportoTotale;

        [NotMapped]
        public string CssClassStato => Stato switch
        {
            StatoOrdine.Nuovo => "badge bg-secondary",
            StatoOrdine.Confermato => "badge bg-primary",
            StatoOrdine.InProduzione => "badge bg-warning",
            StatoOrdine.Spedito => "badge bg-info",
            StatoOrdine.Consegnato => "badge bg-success",
            StatoOrdine.Annullato => "badge bg-danger",
            _ => "badge bg-secondary"
        };

        [NotMapped]
        public string CssClassPriorita => Priorita switch
        {
            PrioritaOrdine.Bassa => "text-success",
            PrioritaOrdine.Media => "text-info",
            PrioritaOrdine.Alta => "text-warning",
            PrioritaOrdine.Urgente => "text-danger",
            _ => "text-muted"
        };
    }

    public enum PrioritaOrdine
    {
        [Display(Name = "Bassa")]
        Bassa = 0,

        [Display(Name = "Media")]
        Media = 1,

        [Display(Name = "Alta")]
        Alta = 2,

        [Display(Name = "Urgente")]
        Urgente = 3
    }

    public enum StatoOrdine
    {
        [Display(Name = "Nuovo")]
        Nuovo = 0,

        [Display(Name = "Confermato")]
        Confermato = 1,

        [Display(Name = "In Produzione")]
        InProduzione = 2,

        [Display(Name = "Spedito")]
        Spedito = 3,

        [Display(Name = "Consegnato")]
        Consegnato = 4,

        [Display(Name = "Annullato")]
        Annullato = 5
    }
}
