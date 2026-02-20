using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAP
{
    public class IntegracaoEnvioDadosCarga
    {
        [JsonProperty("MultiRoute")]
        public string RotaMulti { get; set; }

        [JsonProperty("LoadProtocol")]
        public string ProtocoloCarga { get; set; }

        [JsonProperty("Process")]
        public string Processo { get; set; }

        [JsonProperty("Carrier")]
        public string Transportadora { get; set; }

        [JsonProperty("Vehicle")]
        public Veiculo Veiculo { get; set; }

        [JsonProperty("Driver")]
        public Motorista Motorista { get; set; }

        [JsonProperty("FreightValue")]
        public string ValorFrete { get; set; }

        [JsonProperty("KM")]
        public string Quilometragem { get; set; }

        [JsonProperty("DeliveryTotal")]
        public string TotalEntregas { get; set; }

        [JsonProperty("Item")]
        public List<ItemEntrega> Itens { get; set; }
    }   
}
