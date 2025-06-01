using System.ComponentModel.DataAnnotations;

namespace WeeSe.Models.ViewModels
{
    public class CreaListinoViewModel
    {
        [Required(ErrorMessage = "Il nome del listino è obbligatorio")]
        [StringLength(255, ErrorMessage = "Il nome non può superare 255 caratteri")]
        [Display(Name = "Nome Listino")]
        public string NomeListino { get; set; } = string.Empty;

        [Display(Name = "Descrizione")]
        [StringLength(1000, ErrorMessage = "La descrizione non può superare 1000 caratteri")]
        public string? Descrizione { get; set; }

        [Required(ErrorMessage = "La data di validità è obbligatoria")]
        [Display(Name = "Valido Da")]
        [DataType(DataType.Date)]
        public DateTime DataValiditaDa { get; set; } = DateTime.Now;

        [Display(Name = "Valido Fino A")]
        [DataType(DataType.Date)]
        public DateTime? DataValiditaA { get; set; }

        [Required(ErrorMessage = "Lo stato è obbligatorio")]
        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;

        // Lista dei prodotti disponibili
        public List<ProdottoListinoViewModel> ProdottiDisponibili { get; set; } = new();

        // Prodotti selezionati per il listino
        public List<int> ProdottiSelezionati { get; set; } = new();

        // Prezzi per i prodotti selezionati
        public Dictionary<int, decimal> Prezzi { get; set; } = new();
    }

    public class ProdottoListinoViewModel
    {
        public int Id { get; set; }
        public string SNome { get; set; } = string.Empty;
        public string? TipoCalcolo { get; set; }
        public decimal? Prezzo { get; set; }
        public decimal? PrezzoV { get; set; }
        public decimal? PrezzoVV { get; set; }
        public int? LimiteAltezza { get; set; }
    }
}