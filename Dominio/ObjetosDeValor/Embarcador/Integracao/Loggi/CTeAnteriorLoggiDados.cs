using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi
{
    public class CTeAnteriorLoggiDados
    {
        [JsonProperty("valor_total")]
        public decimal ValorTotal { get; set; }

        [JsonProperty("pis")]
        public decimal Pis { get; set; }

        [JsonProperty("cofins")]
        public decimal Cofins { get; set; }

        [JsonProperty("gris")]
        public decimal Gris { get; set; }

        [JsonProperty("advalorem")]
        public decimal Advalorem { get; set; }

        [JsonProperty("icms")]
        public decimal Icms { get; set; }

        [JsonProperty("chave_cte")]
        public string ChaveCte { get; set; }

        [JsonProperty("data_emissao")]
        public string DataEmissao { get; set; }

        [JsonProperty("cnpj_emissor")]
        public string CnpjEmissor { get; set; }

        [JsonProperty("xml")]
        public string Xml { get; set; }
    }
}
