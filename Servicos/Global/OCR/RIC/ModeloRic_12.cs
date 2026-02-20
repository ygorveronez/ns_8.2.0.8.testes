using System.Collections.Generic;

namespace Servicos.Global
{
    /// <summary>
    /// #51322 Controle de Container - OCR RIC - Modelo 9
    /// ROGERIO PHILIPPI CIA LTDA
    /// </summary>
    public class ModeloRic_12 : IObjetoModeloRic
    {
        
        public List<string> IdentificadorDoModeloOCR => new List<string>()
        {
            "ROGERIO PHILIPPI CIA LTDA",
            "ROGERIO PHILIPPI",
            "ROGERIO P",
        };

        public List<string> DataDeColeta_ModeloOCR => new List<string>()
        {
            "DATA",
            "OATA",
        };

        public List<string> Container_ModeloOCR => new List<string>()
        {
            "CONTAINER",
        };

        public List<string> TipoContainer_ModeloOCR => new List<string>()
        {
            "TIPO",
            "TPO",
        };


        public List<string> TaraContainer_ModeloOCR => new List<string>()
        {
            "TARA"
        };


        public List<string> ArmadorBooking_ModeloOCR => new List<string>()
        {
            "ARMADOR"
        };

        /// <summary>
        /// PODE VIR EM DUAS LINHAS
        /// </summary>
        public List<string> Transportadora_ModeloOCR => new List<string>()
        {
            "TRANSPORT.",
            "TRANSPORT",
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
            "PLACA",
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
