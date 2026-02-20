using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class CarteiraIdentidade
    {
        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "orgao_emissor", Required = Required.Default)]
        public string OrgaoEmissor { get; set; }

        [JsonProperty(PropertyName = "data_emissao", Required = Required.Default)]
        public string DataEmissao { get; set; }

        [JsonProperty(PropertyName = "uf", Required = Required.Default)]
        public string UF { get; set; }
    }
}
