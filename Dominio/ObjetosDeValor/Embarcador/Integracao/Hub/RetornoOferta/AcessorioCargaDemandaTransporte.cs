using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class AcessorioCargaDemandaTransporte
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cargoAccessoryId")]
        public string IdAcessorioCarga { get; set; }
    }
}
