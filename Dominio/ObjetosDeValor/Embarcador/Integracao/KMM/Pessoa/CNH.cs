using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class CNH
    {
        [JsonProperty(PropertyName = "numero_registro", Required = Required.Default)]
        public string NumeroRegistro { get; set; }

        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "categoria", Required = Required.Default)]
        public string Categoria { get; set; }

        [JsonProperty(PropertyName = "uf", Required = Required.Default)]
        public string UF { get; set; }

        [JsonProperty(PropertyName = "orgao_emissor", Required = Required.Default)]
        public string OrgaoEmissor { get; set; }

        [JsonProperty(PropertyName = "data_emissao", Required = Required.Default)]
        public string DataEmissao { get; set; }

        [JsonProperty(PropertyName = "data_validade", Required = Required.Default)]
        public string DataValidade { get; set; }

        [JsonProperty(PropertyName = "data_prim_habilitacao", Required = Required.Default)]
        public string DataPrimeiraHabilitacao { get; set; }

        [JsonProperty(PropertyName = "renach", Required = Required.Default)]
        public string Renach { get; set; }
    }
}
