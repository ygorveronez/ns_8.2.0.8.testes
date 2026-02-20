using System.Collections.Generic;

namespace Servicos.Global
{
    /// <summary>
    /// #51324 Controle de Container - OCR RIC - Modelo 11
    /// CTIL LOGISTICA LTDA
    /// </summary>
    public class ModeloRic_13 : IObjetoModeloRic
    {
        public List<string> IdentificadorDoModeloOCR => new List<string>()
        {
            "CTIL LOGISTICA LTDA",
            "CTIL LOG",
            "CTIL",
        };

        public List<string> DataDeColeta_ModeloOCR => new List<string>()
        {
            "DATA/DATE",
            "DATA",
            "DATE",
        };

        public List<string> Container_ModeloOCR => new List<string>()
        {
            "CONTÊINER",
            "CONTÉINER",
            "CONTÈINER",
            "CONTEINER",
        };

        public List<string> TipoContainer_ModeloOCR => new List<string>()
        {
            "TIPO/TYPE",
            "TIPO",
            "TYPE",
        };


        public List<string> TaraContainer_ModeloOCR => new List<string>()
        {
            "TARA",
            "TARE"
        };


        public List<string> ArmadorBooking_ModeloOCR => new List<string>()
        {
            "111111aaaaaa"
        };

        /// <summary>
        /// PODE VIR EM DUAS LINHAS
        /// </summary>
        public List<string> Transportadora_ModeloOCR => new List<string>()
        {
            "TRANSPORTADOR/TRUCK CO",
            "TRANSPORTADOR",
            "TRUCK CO",
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
            "LICENSE",
            "ICENSE",
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
