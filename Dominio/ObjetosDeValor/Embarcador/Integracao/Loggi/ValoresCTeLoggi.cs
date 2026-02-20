using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi
{

    public class ValoresCTeLoggi
    {
        [JsonProperty("valor_total")]
        public float valor_total { get; set; }
        [JsonProperty("pis")]
        public float pis { get; set; }
        [JsonProperty("cofins")]
        public float cofins { get; set; }
        [JsonProperty("gris")]
        public float gris { get; set; }
        [JsonProperty("advalorem")]
        public float advalorem { get; set; }
        [JsonProperty("icms")]
        public float icms { get; set; }
        [JsonProperty("chave_cte")]
        public string chave_cte { get; set; }
        [JsonProperty("data_emissao")]
        public string data_emissao { get; set; }
        [JsonProperty("cnpj_emissor")]
        public string cnpj_emissor { get; set; }
        [JsonProperty("xml")]
        public string xml { get; set; }
    }
}
