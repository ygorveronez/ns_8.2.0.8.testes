using System.Collections.Generic;

namespace Servicos.Global
{
    /// <summary>
    /// #51323 Controle de Container - OCR RIC - Modelo 10
    /// MARTINI MEAT S.A.
    /// </summary>
    public class ModeloRic_4 : IObjetoModeloRic
    {
        public List<string> IdentificadorDoModeloOCR => new List<string>()
        {
            "MARTINI MEAT",
            "MARTINI",
        };

        public List<string> DataDeColeta_ModeloOCR => new List<string>()
        {
            "DATA(DATE)",
        };

        public List<string> Container_ModeloOCR => new List<string>()
        {
            "CONTAINER",
            "CONTANER",
            "CONTAINE",
            "ONTAINER",
        };

        public List<string> TipoContainer_ModeloOCR => new List<string>()
        {
            "TIPO",
            "TIPD",
            "TPO",
            "TPD",
        };

        public List<string> TaraContainer_ModeloOCR => new List<string>()
        {
            "TARA",
            "IARA",
            "TABA",
            "TAPA",
        };

        public List<string> ArmadorBooking_ModeloOCR => new List<string>()
        {
            "BOOKING",
            "BOOKNG",
            "BDOKING",
            "BODKING",
            "BDOKNG",
            "BODKNG",
            "DOOKING",
            "8OOKING",
        };

        public List<string> Transportadora_ModeloOCR => new List<string>()
        {
            "TRANSPOR",
            "RANSPOR",
            "IRANSPOR",
            "RANSPOB",
            "RANSPOP",
        };

        public List<string> Motorista_ModeloOCR => new List<string>()
        {
            "MOTORISTA",
            "MOTORSTA",
            "MOIORISTA",
            "MOTORISIA",
            "OTORISTA",
            "OIORISTA",
            "OTORISIA",
        };

        public List<string> Placa_ModeloOCR => new List<string>()
        {
            "VEICULO",
            "VECULO",
            "VECUIO",
            "VEICUIO",
            "EICULO",
            "EICUIO",
        };

        public string DataDeColeta { get; set; }

        public string Container { get; set; }

        public string TipoContainer { get; set; }

        public string TaraContainer { get; set; }

        public string ArmadorBooking { get; set; }

        public string Transportadora { get; set; }

        public string Motorista { get; set; }

        public string Placa { get; set; }
    }
}
