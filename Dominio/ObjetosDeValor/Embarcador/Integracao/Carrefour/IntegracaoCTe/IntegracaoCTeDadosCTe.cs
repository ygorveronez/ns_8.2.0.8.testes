using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.IntegracaoCTe
{
    public sealed class IntegracaoCTeDadosCTe
    {
        [JsonProperty(PropertyName = "dacte", Required = Required.Always)]
        public string Chave { get; set; }

        [JsonProperty(PropertyName = "cnpjDestinatario", Required = Required.Always)]
        public string CpfCnpjDestinatario { get; set; }

        [JsonProperty(PropertyName = "liberacaoParaPagamentoAutomatico", Required = Required.Always)]
        public bool LiberacaoParaPagamentoAutomatico { get; set; }

        [JsonProperty(PropertyName = "tipoTomador", Required = Required.Always)]
        public string TipoTomador { get; set; }

        [JsonProperty(PropertyName = "usuarioBusca", Required = Required.Always)]
        public string Usuario {
            get { return "multEmb"; }
        }

        [JsonProperty(PropertyName = "xmlCTe", Required = Required.Always)]
        public string Xml { get; set; }
    }
}
