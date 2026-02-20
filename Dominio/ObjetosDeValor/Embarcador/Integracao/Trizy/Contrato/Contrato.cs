using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Contrato
    {
        [JsonProperty(PropertyName = "contrato", Required = Required.Default)]
        public ContratoDetalhes ContratoDetalhes { get; set; }
    }
}
