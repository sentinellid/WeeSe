using System.ComponentModel.DataAnnotations;
using System.Reflection;
using WeeSe.Models;

namespace WeeSe.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var displayAttribute = enumValue
                .GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.GetName() ?? enumValue.ToString();
        }

        public static string GetDisplayName(this PrioritaOrdine priorita)
        {
            return priorita switch
            {
                PrioritaOrdine.Bassa => "Bassa",
                PrioritaOrdine.Media => "Media",
                PrioritaOrdine.Alta => "Alta",
                PrioritaOrdine.Urgente => "Urgente",
                _ => priorita.ToString()
            };
        }

        public static string GetDisplayName(this StatoOrdine stato)
        {
            return stato switch
            {
                StatoOrdine.Nuovo => "Nuovo",
                StatoOrdine.Confermato => "Confermato",
                StatoOrdine.InProduzione => "In Produzione",
                StatoOrdine.Spedito => "Spedito",
                StatoOrdine.Consegnato => "Consegnato",
                StatoOrdine.Annullato => "Annullato",
                _ => stato.ToString()
            };
        }

        public static string GetDisplayName(this PrioritaCommessa priorita)
        {
            return priorita switch
            {
                PrioritaCommessa.Bassa => "Bassa",
                PrioritaCommessa.Media => "Media",
                PrioritaCommessa.Alta => "Alta",
                PrioritaCommessa.Urgente => "Urgente",
                _ => priorita.ToString()
            };
        }

        public static string GetDisplayName(this StatoCommessa stato)
        {
            return stato switch
            {
                StatoCommessa.Nuova => "Nuova",
                StatoCommessa.InLavorazione => "In Lavorazione",
                StatoCommessa.InPausa => "In Pausa",
                StatoCommessa.Completata => "Completata",
                StatoCommessa.Annullata => "Annullata",
                _ => stato.ToString()
            };
        }

        public static string GetCssClass(this PrioritaOrdine priorita)
        {
            return priorita switch
            {
                PrioritaOrdine.Bassa => "text-success",
                PrioritaOrdine.Media => "text-info",
                PrioritaOrdine.Alta => "text-warning",
                PrioritaOrdine.Urgente => "text-danger",
                _ => "text-muted"
            };
        }

        public static string GetIcon(this PrioritaOrdine priorita)
        {
            return priorita switch
            {
                PrioritaOrdine.Bassa => "fa-arrow-down",
                PrioritaOrdine.Media => "fa-minus",
                PrioritaOrdine.Alta => "fa-arrow-up",
                PrioritaOrdine.Urgente => "fa-fire",
                _ => "fa-minus"
            };
        }

        public static string GetCssClass(this StatoOrdine stato)
        {
            return stato switch
            {
                StatoOrdine.Nuovo => "badge bg-secondary",
                StatoOrdine.Confermato => "badge bg-primary",
                StatoOrdine.InProduzione => "badge bg-warning",
                StatoOrdine.Spedito => "badge bg-info",
                StatoOrdine.Consegnato => "badge bg-success",
                StatoOrdine.Annullato => "badge bg-danger",
                _ => "badge bg-secondary"
            };
        }
    }
}