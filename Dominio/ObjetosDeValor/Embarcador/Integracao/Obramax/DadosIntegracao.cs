using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class DadosIntegracao
    {
        [JsonProperty("Cabecalho")]
        public string OperacaoCarga { get; set; }

        [JsonProperty("NomeMotorista")]
        public string NomeMotorista { get; set; }

        [JsonProperty("NumeroCarga")]
        public string NumeroCarga { get; set; }

        [JsonProperty("Placa")]
        public string Placa { get; set; }

        [JsonProperty("ProtocoloCarga")]
        public string ProtocoloCarga { get; set; }

        [JsonProperty("Remessa")]
        public List<Remessa> Remessa { get; set; }

        [JsonProperty("Tipotransporte")]
        public string Tipotransporte { get; set; }

        [JsonProperty("Tipoveiculo")]
        public string Tipoveiculo { get; set; }

        [JsonProperty("Transportadora")]
        public List<Transportadora> Transportadora { get; set; }

        [JsonProperty("Operation")]
        public string Operation { get; set; }
    }
}
