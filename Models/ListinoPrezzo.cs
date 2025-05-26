using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeeSe.Models
{
    [Table("TBL_ListiniPrezzi")]
    public class ListinoPrezzo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Codice { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Descrizione { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,4)")]
        public decimal PrezzoUnitario { get; set; }

        // ... resto dei campi
    }
}