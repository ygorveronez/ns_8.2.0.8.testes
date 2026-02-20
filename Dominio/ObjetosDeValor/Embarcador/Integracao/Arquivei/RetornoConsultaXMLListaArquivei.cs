using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Arquivei
{
    public class RetornoConsultaXmlListaArquivei
    {
        [JsonProperty(PropertyName = "access_key", Required = Required.Default)]
        public string ChaveAcessoDocumento { get; set; }

        [JsonProperty(PropertyName = "xml", Required = Required.Default)]
        public string XML { get; set; }
    }
}
