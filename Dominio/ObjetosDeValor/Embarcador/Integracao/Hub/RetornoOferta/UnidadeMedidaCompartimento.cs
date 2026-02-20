using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class UnidadeMedidaCompartimento
    {
        [JsonProperty("measurementUnit")]
        public Tipo UnidadeMedida { get; set; }

        [JsonProperty("capacity")]
        public decimal Capacidade { get; set; }
    }
}
