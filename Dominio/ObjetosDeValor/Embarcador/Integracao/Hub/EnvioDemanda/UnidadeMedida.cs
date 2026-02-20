using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class UnidadeMedida
    {
        [JsonProperty("measurementUnit")]
        public Tipo Unidade { get; set; }

        [JsonProperty("capacity")]
        public decimal Capacidade { get; set; }
    }
}
