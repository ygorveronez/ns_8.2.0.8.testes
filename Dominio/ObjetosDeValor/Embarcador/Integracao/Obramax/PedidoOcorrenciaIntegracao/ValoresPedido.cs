using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class ValoresPedido
    {
        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("data")]
        public DadosIntegracaoPedido DadosIntegracao { get; set; }
    }
}
