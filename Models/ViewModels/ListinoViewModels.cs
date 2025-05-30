// ViewModels/ListinoViewModels.cs
using System.ComponentModel.DataAnnotations;
using WeeSe.Models;

namespace WeeSe.ViewModels
{
    public class ListinoIndexViewModel
    {
        public List<ListinoListItem> Listini { get; set; }
        public string SearchTerm { get; set; }
        // Paginazione
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // Ordinamento
        public string SortBy { get; set; } = "NomeListino";
        public string SortOrder { get; set; } = "asc";

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class ListinoListItem
    {
        public int IDListino { get; set; }
        public string NomeListino { get; set; }
        public decimal? PerTrasporto { get; set; }
        public decimal? CostoCapra { get; set; }
        public decimal? PerImballo { get; set; }
    }

    public class ListinoEditViewModel
    {
        public Listino Listino { get; set; }
        public List<ListinoProdottoEditItem> Prodotti { get; set; }
    }

    public class ListinoProdottoEditItem
    {
        public int Id { get; set; }
        public int IDListino { get; set; }
        public int IDTipoProdotto { get; set; }
        public string NomeListino { get; set; } = string.Empty;
        public decimal? PerTrasporto { get; set; }
        public decimal? CostoCapra { get; set; }
        public decimal? PerImballo { get; set; }
        public decimal? Prezzo { get; set; }
        public decimal? PrezzoV { get; set; }
        public decimal? PrezzoVV { get; set; }
        public string TipoCalcolo { get; set; } = string.Empty;
        public decimal? LimiteAltezza { get; set; }
        public string SNome { get; set; } = string.Empty;
    }

}