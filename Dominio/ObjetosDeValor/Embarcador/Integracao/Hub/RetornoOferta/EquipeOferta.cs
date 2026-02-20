using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class EquipeOferta
    {
        [JsonProperty("transportOperator")]
        public OperadorTransporte OperadorTransporte { get; set; }
    }
}
