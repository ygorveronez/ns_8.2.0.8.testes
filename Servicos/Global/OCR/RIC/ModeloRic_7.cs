using System.Collections.Generic;

namespace Servicos.Global
{
    public class ModeloRic_7 : IObjetoModeloRic
    {
        public List<string> IdentificadorDoModeloOCR => new List<string>()
        {
            "MEDLOG - MEDITERRANEAN LOGISTICA",
            "MEDLOG - MEDITERRANEAN",
            "MEDLOG",
            "MEDITERRANEAN"
        };

        public List<string> DataDeColeta_ModeloOCR => new List<string>()
        {
            "DATA",
            "DAT",
            "ATA",
            "OATA",
            "DATO",
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
            "IPO",
        };

        public List<string> TaraContainer_ModeloOCR => new List<string>()
        {
            "TARA",
            "IARA",
            "TABA",
             "ARA",
            "TAPA"

        };

        public List<string> ArmadorBooking_ModeloOCR => new List<string>()
        {
            "LESSOR",
            "LESSO"
        };

        public List<string> Transportadora_ModeloOCR => new List<string>()
        {
            "TRANSPOR.",
            "TRANSPOR",
            "RANSPOR",
            "IRANSPOR",
            "RANSPOB",
            "RANSPOP"
        };

        public List<string> Motorista_ModeloOCR => new List<string>()
        {
            "MOTORISTA",
            "MOTORSTA",
            "MOIORISTA",
            "MOTORISIA",
            "OTORISTA",
            "OIORISTA",
            "OTORISIA"
        };

        public List<string> Placa_ModeloOCR => new List<string>()
        {
            "PLACA",
            "PIACA",
            "PLAGA",
            "PIAGA",
            "LACA",
             "PLAC",
        };

        public string DataDeColeta { get; set; }
        public string HoradeColeta { get; set; }

        public string Container { get; set; }

        public string TipoContainer { get; set; }

        public string TaraContainer { get; set; }

        public string ArmadorBooking { get; set; }

        public string Transportadora { get; set; }

        public string Motorista { get; set; }

        public string Placa { get; set; }
    }
}


