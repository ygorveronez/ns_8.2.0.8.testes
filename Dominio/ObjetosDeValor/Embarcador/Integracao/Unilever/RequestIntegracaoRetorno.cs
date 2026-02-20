using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class RequestIntegracaoRetorno
    {
        [JsonProperty("occurrence")]
        public Ocorrencia Ocorrencia { get; set; }

        [JsonProperty("vehiclePlate")]
        public string PlacaVeiculo { get; set; }

        [JsonProperty("shipment")]
        public Envio Envio { get; set; }
    }
}
