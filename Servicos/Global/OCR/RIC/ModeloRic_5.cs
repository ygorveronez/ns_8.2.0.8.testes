using System.Collections.Generic;

namespace Servicos.Global
{
    /// <summary>
    /// Concordia Logistica
    /// Ziran Paranagua
    /// TRANSTEC WORLD Terminais e Logística Ltda
    /// Apmt
    /// #50916 Controle de Container - OCR RIC - Modelo 1
    /// </summary>
    public class ModeloRic_5 : IObjetoModeloRic
    {
        public List<string> IdentificadorDoModeloOCR => new List<string>()
        {
            "CONCORDIA LOGISTICA",
            "CONCÓRDIA",
            "CONCORDIA",
            "APMT",
            "RETROPORTUARIOS",
            "ZIRAN",
            "TRANSTEC WORLD",
            "ANSTEC WORL",
            "TRANSTEC",
            "WORLD TERMINAIS",
            "WORLD TERMINA",
            "WORLO TERMINA",
        };

        public List<string> DataDeColeta_ModeloOCR => new List<string>()
        {
            "DATA/HORA",
            "DATA HORA",
            "DATA/HDRA",
            "OATA/HORA",
            "DATA/HOR",
            "ATA/HORA",
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
            "ARMADOR",
            "CLIENTE",
            "ABMADOR",
            "ARMADOB",
            "ARMAOOR",
            "ARMAOOB",
            "ABMAOOR",
            "ABMAOOB",
            "ARMADO",
            "RMADOR",
            "LIENTE"
        };

        public List<string> Transportadora_ModeloOCR => new List<string>()
        {
            "TRANSPORT.",
            "RANSPORT",
            "TRANSPORT",
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
            "VEICULO",
            "PIACA",
            "PLAGA",
            "PIAGA",
            "LACA",
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
