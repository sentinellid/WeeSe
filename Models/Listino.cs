// Models/Listino.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeeSe.Models
{
    [Table("TBL_Listino")]
    public class Listino
    {
        [Key]
        public int IDListino { get; set; }

        [Required(ErrorMessage = "Il nome del listino è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
        [Display(Name = "Nome Listino")]
        public string NomeListino { get; set; } = String.Empty;

        [Display(Name = "% Trasporto")]
        [Range(0, 100, ErrorMessage = "La percentuale deve essere tra 0 e 100")]
        public decimal? PerTrasporto { get; set; }

        [Display(Name = "Costo Capra")]
        [DataType(DataType.Currency)]
        public decimal? CostoCapra { get; set; }

        [Display(Name = "% Imballo")]
        [Range(0, 100, ErrorMessage = "La percentuale deve essere tra 0 e 100")]
        public decimal? PerImballo { get; set; }

        // Navigazione
        public virtual ICollection<ListinoProdotto> ListinoProdotti { get; set; }
    }

    [Table("TBL_LISTINO_PRODOTTI")]
    public class ListinoProdotto
    {
        [Key]
        public int IDListino { get; set; }
        public int IDTipoProdotto { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Prezzo { get; set; }

        [DataType(DataType.Currency)]
        public decimal? PrezzoV { get; set; }

        [DataType(DataType.Currency)]
        public decimal? PrezzoVV { get; set; }

        [StringLength(50)]
        public string TipoCalcolo { get; set; } = String.Empty;

        public int? LimiteAltezza { get; set; }

        // Navigazione
        [ForeignKey("IDListino")]
        public virtual Listino Listino { get; set; }

        [ForeignKey("IDTipoProdotto")]
        public virtual Prodotto Prodotto { get; set; }
    }

    [Table("TBL_PRODOTTO")]
    public class Prodotto
    {
        [Key]
        public int IDTipoProdotto { get; set; }

        [StringLength(100)]
        public string SNome { get; set; }

        // Altri campi del prodotto...
    }
}