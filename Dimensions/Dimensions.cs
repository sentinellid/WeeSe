namespace WeeSe.Dimensions
{
    public class Dimensions
    {
        public List<string> FinitureDisponibili { get; set; } = new()
        {
            "bianco 9010",
            "avorio 1013",
            "marrone 8017",
            "ox argento",
            "antracite DK-702"
        };

        public List<string> VetriDisponibili { get; set; } = new()
        {
            "temp. 10mm",
            "55.2 stratificato",
            "55.2 temp/ingl"
        };

        public List<string> FinitureVetroDisponibili { get; set; } = new()
        {
            "chiaro",
            "extra chiaro",
            "fumé",
            "satinato"
        };

        public List<string> SistemiChiusuraDisponibili { get; set; } = new()
        {
            "paletto",
            "pedalina",
            "blocco pedalina",
            "serratura"
        };

        public List<string> VaschetteTrascinamentoDisponibili { get; set; } = new()
        {
            "adesiva trasparente - singola",
            "adesiva trasparente - doppia",
            "acciaio - doppia"
        };

        public List<string> TappiDisponibili { get; set; } = new()
        {
            "metallo",
            "stillicidio"
        };

        public List<string> TrasportiDisponibili { get; set; } = new()
        {
            "ritiro presso ns sede",
            "spedizione con cavalletto"
        };

    }
}
