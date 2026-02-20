using System.Collections.Generic;

namespace Servicos.Global
{
    /// <summary>
    /// #50947 Controle de Container - OCR RIC - Modelo 8
    /// SAGA CONTAINERS
    /// </summary>
    public class ModeloRic_11 : IObjetoModeloRic
    {
        public List<string> IdentificadorDoModeloOCR => new List<string>()
        {
            "SAGA CONTAINERS",
            "SACA CONTAINERS",
            "SAGA C",
            "SACA C",
        };

        public List<string> DataDeColeta_ModeloOCR => new List<string>()
        {
            "DATE",
            "DALE",
            "DAIE",
            "OATE",
            "OALE",
        };

        public List<string> Container_ModeloOCR => new List<string>()
        {
            "OUT -",
            "OUT",
        };

        public List<string> TipoContainer_ModeloOCR => new List<string>()
        {
            "TYPE",
            "IYPE",
            "LYPE",
        };

        public List<string> TaraContainer_ModeloOCR => new List<string>()
        {
            "TARE",
            "IARE",
        };


        public List<string> ArmadorBooking_ModeloOCR => new List<string>()
        {
            "OWNER:",
            "OWNER",
            "0WNER",
        };

        /// <summary>
        /// PODE VIR EM DUAS LINHAS
        /// </summary>
        public List<string> Transportadora_ModeloOCR => new List<string>()
        {
            "TRUCKING CO.",
            "TRUCKING CO",
        };

        /// <summary>
        /// PODE VIR EM DUAS LINHAS
        /// </summary>
        public List<string> Motorista_ModeloOCR => new List<string>()
        {
            "DRIVER"
        };

        public List<string> Placa_ModeloOCR => new List<string>()
        {
            "TRUCK LICENCE",
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
