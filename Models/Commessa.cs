using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeeSe.Models
{
    [Table("TBL_Commesse")]
    public class Commessa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Numero Commessa")]
        public string NumeroCommessa { get; set; } = string.Empty;

        [Display(Name = "Preventivo")]
        public int? PreventivoId { get; set; }

        [ForeignKey("PreventivoId")]
        public virtual Preventivo? Preventivo { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Cliente")]
        public string Cliente { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Indirizzo Cliente")]
        public string? IndirizzoCliente { get; set; }

        [StringLength(1000)]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

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
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportoTotale { get; set; }

        [Display(Name = "Importo Fatturato")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportoFatturato { get; set; }

        [StringLength(100)]
        [Display(Name = "Responsabile")]
        public string? Responsabile { get; set; }

        [StringLength(100)]
        [Display(Name = "Tecnico Assegnato")]
        public string? TecnicoAssegnato { get; set; }

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

        // Navigation Properties
        public virtual ICollection<AttivitaCommessa> Attivita { get; set; } = new List<AttivitaCommessa>();
        public virtual ICollection<Ordine> Ordini { get; set; } = new List<Ordine>();

        // Proprietà calcolate
        [NotMapped]
        public bool InRitardo => DataFinePrevista.HasValue && DateTime.Now > DataFinePrevista.Value && Stato != StatoCommessa.Completata;

        [NotMapped]
        public int PercentualeCompletamento
        {
            get
            {
                if (!Attivita.Any()) return 0;
                var completate = Attivita.Count(a => a.Completata);
                return (int)Math.Round((double)completate / Attivita.Count * 100);
            }
        }

        [NotMapped]
        public string CssClassStato => Stato switch
        {
            StatoCommessa.Nuova => "badge bg-secondary",
            StatoCommessa.InLavorazione => "badge bg-primary",
            StatoCommessa.InPausa => "badge bg-warning",
            StatoCommessa.Completata => "badge bg-success",
            StatoCommessa.Annullata => "badge bg-danger",
            _ => "badge bg-secondary"
        };

        [NotMapped]
        public string CssClassPriorita => Priorita switch
        {
            PrioritaCommessa.Bassa => "text-success",
            PrioritaCommessa.Media => "text-info",
            PrioritaCommessa.Alta => "text-warning",
            PrioritaCommessa.Urgente => "text-danger",
            _ => "text-muted"
        };
    }

    public enum StatoCommessa
    {
        [Display(Name = "Nuova")]
        Nuova = 0,
        [Display(Name = "In Lavorazione")]
        InLavorazione = 1,
        [Display(Name = "In Pausa")]
        InPausa = 2,
        [Display(Name = "Completata")]
        Completata = 3,
        [Display(Name = "Annullata")]
        Annullata = 4
    }

    public enum PrioritaCommessa
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
}
