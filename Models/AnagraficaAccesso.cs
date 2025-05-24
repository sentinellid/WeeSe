using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeeSe.Models
{
    [Table("TBL_Anagrafica_Accesso")]
    public class AnagraficaAccesso
    {
        [Key]
        public int IDAnagraficaAccesso { get; set; }
        
        [Required]
        [StringLength(100)]
        public string sUtente { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string sPassword { get; set; } = string.Empty;
        
        public bool bAttivo { get; set; }
        
        public int IDRuolo { get; set; }
        
        public Guid? GuidReset { get; set; }
    }
}
