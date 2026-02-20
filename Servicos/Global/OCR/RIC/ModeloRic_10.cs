using System.Collections.Generic;

namespace Servicos.Global
{
    /// <summary>
    /// #50946 Controle de Container - OCR RIC - Modelo 7
    /// Wilson.Sons
    /// </summary>
    public class ModeloRic_10 : IObjetoModeloRic
    {
        public List<string> IdentificadorDoModeloOCR => new List<string>()
        {
            "WILSON. SONS",
            "WILSON.SONS",
            "WILSON SONS",
            "WILSON S",
            "WILSON.S",
            "WILSON. S",
        };

        public List<string> DataDeColeta_ModeloOCR => new List<string>()
        {
            "DATA ENTRADA",
            "DATA E",
            "OATA E",
        };
        
        public List<string> Container_ModeloOCR => new List<string>()
        {
            "CONTAINER:",
            "CONTAINER",
        };

        public List<string> TipoContainer_ModeloOCR => new List<string>()
        {
            "TIPO",
            "TPO",
            "T PO",
            "TLPO",
        };


        /// <summary>
        /// ESSE MODELO NAO TEM TARA
        /// </summary>
        public List<string> TaraContainer_ModeloOCR => new List<string>();
        

        public List<string> ArmadorBooking_ModeloOCR => new List<string>()
        {
            "ARMADOR",
            "AIMADOR",
            "AIMADOI",
            "A MADOR",
            "A MADOI",
            "MADOR:",
        };

        /// <summary>
        /// PODE VIR EM DUAS LINHAS
        /// </summary>
        public List<string> Transportadora_ModeloOCR => new List<string>()
        {
            "TRANSPORTADORA",
            "IRANSPORTADORA",
            "LRANSPORTADORA",
            "SPORTADOR",
        };

        /// <summary>
        /// PODE VIR EM DUAS LINHAS
        /// </summary>
        public List<string> Motorista_ModeloOCR => new List<string>()
        {
            "MOTORISTA",
            "MOTOR STA",
        };

        public List<string> Placa_ModeloOCR => new List<string>()
        {
            "PLACAS",
            "PIACAS",
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
