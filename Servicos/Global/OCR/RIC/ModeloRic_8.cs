using System;
using System.Collections.Generic;

namespace Servicos.Global
{
    public class ModeloRic_8 : IObjetoModeloRic
    {
        public List<string> IdentificadorDoModeloOCR => new List<string>()
        {
            "CONEXAO MARITIMA",
            "CONEXÃO MARITIMA",
            "CONEXÃO MARÍTIMA",
            "CONEXAO MARITIMA SERV.",
            "MARITIMA SERV",
        };
        public List<string> DataDeColeta_ModeloOCR => new List<string>()
        {
            "DATA DE EMISSAO",
            "DATA DE EMISSÃO",
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
            "COD. ISO",
            "COD ISO",
            "COD IS"
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
            "ABMADOR",
            "ARMADOB",
            "ARMAOOR",
            "ARMAOOB",
            "ABMAOOR",
            "ABMAOOB",
            "ARMADO",
            "RMADOR"
        };

        public List<string> Transportadora_ModeloOCR => new List<string>()
        {
            "TRANSPORTADOR",
            "TRANSPORTADO",
            "RANSPORTADOR",
            "TRANSPOTADOR",
            "TRANSPOTA"
        };

        public List<string> Motorista_ModeloOCR => new List<string>()
        {
            "MOTORISTA",
            "MOTORIST",
            "OTORISTA",
            "MOTORIS",
            "MOTORIST0",
            "M0T0RISTA",
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
        public string Container { get; set; }
        public string TipoContainer { get; set; }
        public string TaraContainer { get; set; }
        public string ArmadorBooking { get; set; }
        public string Transportadora { get; set; }
        public string Motorista { get; set; }
        public string Placa { get; set; }

        List<string> IObjetoModeloRic.Container_ModeloOCR => throw new NotImplementedException();
    }
}
