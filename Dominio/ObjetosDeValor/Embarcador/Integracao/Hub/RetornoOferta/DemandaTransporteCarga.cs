using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class DemandaTransporteCarga
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cargoId")]
        public string IdCarga { get; set; }
    }
}
