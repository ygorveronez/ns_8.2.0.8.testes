using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class Tripulacao
    {
        [JsonProperty("transportOperator")]
        public Transportadora OperadorTransporte { get; set; }

        [JsonProperty("transportOperatorType")]
        public Tipo TipoOperadorTransporte { get; set; }
    }
}
