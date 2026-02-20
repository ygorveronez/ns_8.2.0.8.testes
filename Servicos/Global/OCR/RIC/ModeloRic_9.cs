using System.Collections.Generic;

namespace Servicos.Global
{
    /// <summary>
    /// #50945 Controle de Container - OCR RIC - Modelo 6
    /// #51324 Controle de Container - OCR RIC - Modelo 12
    /// MERCOTAINER RIO GRANDE
    /// </summary>
    public class ModeloRic_9 : IObjetoModeloRic
    {
        public List<string> IdentificadorDoModeloOCR => new List<string>()
        {
            "MERCOTAINER - RIO GRANDE",
            "MERCOTAINER",
            "MERCOTAINE",
        };

        public List<string> DataDeColeta_ModeloOCR => new List<string>()
        {
            "DATA/DATE",
            "DATE",
            "DATA"
        };

        public List<string> Container_ModeloOCR => new List<string>()
        {
            "CONTÈINER",
            "CONTÊINER",
            "CONTÉINER",
            "CONTEINER",
        };

        public List<string> TipoContainer_ModeloOCR => new List<string>()
        {
            "ISO CODE",
            "SO CODE",
        };

        public List<string> TaraContainer_ModeloOCR => new List<string>()
        {
            "TARA/TARE",
            "TARA/TAR",
            "ARA/TARE",
            "TARE",
            "TARA",
        };

        public List<string> ArmadorBooking_ModeloOCR => new List<string>()
        {
            "ENTREGUE POR/DELIVERED BY",
            "ENTREGUE POR/DELIVERY BY",
            "ENTREGUE POR/DELIVERY",
            "NTREGUE POR/DELIVERED BY",
            "ENTREGUE POR",
            "DELIVERED BY",
            "DELIVERED",
        };

        public List<string> Transportadora_ModeloOCR => new List<string>()
        {
            "TRANSPORTADOR/TRUCK CO.",
            "TRANSPORTADOR/TRUCK CO",
            "ANSPORTADOR/TRUCK CO",
            "ANSPORTADOR/TRUCK CO",
            "TRANSPORTADOR/TRUCK",
            "TRUCK CO",
            "TRANSPORTADOR",
            "TRANSPORTADO",
            "RANSPORTADOR"
        };
        
        public List<string> Motorista_ModeloOCR => new List<string>()
        {
            "PLACA/LICENSE",
            "LACA/LICENSE",
            "LICENSE",
            "PLACA"
        };

        public List<string> Placa_ModeloOCR => new List<string>() {
            "PLACA/LICENSE",
            "LACA/LICENSE",
            "LICENSE",
            "PLACA"
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
