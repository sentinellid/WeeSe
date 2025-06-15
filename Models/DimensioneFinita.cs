using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeeSe.Models
{
    [Table("TBL_DimensioniFinite")]
    public class DimensioneFinita
    {
        public int Id { get; set; }

        [Required]
        public int PreventivoId { get; set; }

        [Required]
        [Display(Name = "Numero")]
        public int Numero { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Larghezza deve essere tra 1 e 10000 mm")]
        [Display(Name = "Larghezza L (mm)")]
        public int LarghezzaL { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Altezza deve essere tra 1 e 10000 mm")]
        [Display(Name = "Altezza H (mm)")]
        public int AltezzaH { get; set; }

        // Navigation property
        public Preventivo? Preventivo { get; set; }

        // Proprietà calcolata per l'area
        [Display(Name = "Area (m²)")]
        public decimal Area => (LarghezzaL * AltezzaH) / 1000000m;

        public string FinitureDisponibili { get; set; } = String.Empty;

        // Proprietà calcolata per descrizione
        public string Descrizione => $"{LarghezzaL} x {AltezzaH} mm ({Area:F2} m²)";
    }
}