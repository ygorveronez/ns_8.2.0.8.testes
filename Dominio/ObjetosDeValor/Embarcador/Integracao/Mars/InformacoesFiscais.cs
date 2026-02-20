using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class InformacoesFiscais
    {
        [JsonProperty("taxSituation")]
        public string SituacaoTributaria { get; set; }

        [JsonProperty("calculationBasis")]
        public string BaseCalculo { get; set; }

        [JsonProperty("icmsRate")]
        public string AliquotaIcms { get; set; }

        [JsonProperty("icmsValue")]
        public string ValorIcms { get; set; }
    }
}
