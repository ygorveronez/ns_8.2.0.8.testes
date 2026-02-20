using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public class DadosEnvioDemanda
    {
        [JsonProperty("transportDemand")]
        public DemandaTransporte DemandaTransporte { get; set; }
    }
}
