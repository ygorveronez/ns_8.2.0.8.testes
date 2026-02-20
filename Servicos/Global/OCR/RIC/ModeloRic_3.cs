using System.Collections.Generic;

namespace Servicos.Global
{
    /// <summary>
    /// #50944 Controle de Container - OCR RIC - Modelo 5
    /// Lechman Terminais
    /// </summary>
    public class ModeloRic_3 : IObjetoModeloRic
    {
        public List<string> IdentificadorDoModeloOCR => new List<string>()
        {
            "LECHMAN TERMINAIS",
            "LECHMAN",
            "ESTRADA VELHA",
            "ESTRADA V",
            "INTERCHANGE OUT",
        };

        public List<string> DataDeColeta_ModeloOCR => new List<string>()
        {
            "DATE",
            "OATE",
            "DAIE",
            "OAIE",
        };

        /// <summary>
        /// A linha depois de Date é o container
        /// </summary>
        public List<string> Container_ModeloOCR => new List<string>();

        /// <summary>
        /// O Tipo está logo depois do Container
        /// </summary>
        public List<string> TipoContainer_ModeloOCR => new List<string>();

        public List<string> TaraContainer_ModeloOCR => new List<string>()
        {
            "TARE",
            "IARE",
            "TABE",
            "TAPE",
        };

        /*public List<string> ArmadorBooking_ModeloOCR => new List<string>()
        {
            "BOOKING",
            "BOOKINC",
            "DOOKING",
            "DOOKINC",
        };*/
        public List<string> ArmadorBooking_ModeloOCR => new List<string>()
        {
            "OWNER:",
            "OWNER",
            "0WNER",
        };

        public List<string> Transportadora_ModeloOCR => new List<string>()
        {
            "CARRIER",
            "CARRIEB",
            "CARRER",
            "ARRIER",
            "ARRIE",
        };

        public List<string> Motorista_ModeloOCR => new List<string>()
        {
            "DRIVER",
            "ORIVER",
            "DRIVEB",
            "ORIVEB",
            "DRVER",
        };

        public List<string> Placa_ModeloOCR => new List<string>()
        {
            "PLATE",
            "PIATE",
            "PLAIE",
            "PIAIE",
            "LATE",
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
