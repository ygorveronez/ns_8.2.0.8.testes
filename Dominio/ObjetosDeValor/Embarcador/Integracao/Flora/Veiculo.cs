using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Flora
{
    public class Veiculo
    {
        [JsonProperty("temVeiculo")]
        public int TemVeiculo { get; set; }

        [JsonProperty("CdVeiculo")]
        public string Placa { get; set; }

        [JsonProperty("PlacaPrincipal")]
        public string PlacaPrincipal { get; set; }

        [JsonProperty("CdTransportadora")]
        public string CodigoIntegracaoTransportadora { get; set; }

        [JsonProperty("TransportadoraSubContratada")]
        public string TransportadoraSubContratada { get; set; }

        [JsonProperty("CdMotorista")]
        public string CodigoIntegracaoMotorista { get; set; }

        [JsonProperty("PrevisaoColeta")]
        public string PrevisaoColeta { get; set; }

        [JsonProperty("KmTotal")]
        public decimal KmTotal { get; set; }

        [JsonProperty("VeiculoAtrelado")]
        public List<VeiculoAtrelado> VeiculoAtrelado { get; set; }

        [JsonProperty("VeiculoEvento")]
        public List<VeiculoEvento> VeiculoEvento { get; set; }

        [JsonProperty("VeiculoInformacao")]
        public List<VeiculoInformacao> VeiculoInformacao { get; set; }
    }
}
