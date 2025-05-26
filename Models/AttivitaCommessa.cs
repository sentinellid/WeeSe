using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeeSe.Models
{
    [Table("TBL_AttivitaCommesse")]
    public class AttivitaCommessa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CommessaId { get; set; }

        [ForeignKey("CommessaId")]
        public virtual Commessa Commessa { get; set; } = null!;

        [Required]
        [StringLength(500)]
        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        [Display(Name = "Data Prevista")]
        [DataType(DataType.Date)]
        public DateTime? DataPrevista { get; set; }

        [Display(Name = "Data Completamento")]
        [DataType(DataType.DateTime)]
        public DateTime? DataCompletamento { get; set; }

        [Display(Name = "Completata")]
        public bool Completata { get; set; } = false;

        [StringLength(100)]
        [Display(Name = "Responsabile")]
        public string? Responsabile { get; set; }

        [StringLength(1000)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Display(Name = "Ordine")]
        public int Ordine { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }

        [NotMapped]
        public bool InRitardo => DataPrevista.HasValue && DateTime.Now > DataPrevista.Value && !Completata;
    }
}