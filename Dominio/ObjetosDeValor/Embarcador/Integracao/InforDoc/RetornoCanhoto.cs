using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.InforDoc
{
    public class RetornoCanhoto
    {
        [JsonProperty(PropertyName = "Success", Required = Required.Default)]
        public bool Sucesso { get; set; }

        [JsonProperty(PropertyName = "Message", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "Detail", Required = Required.Default)]
        public string Detalhe { get; set; }

        [JsonProperty(PropertyName = "URLComprovante", Required = Required.Default)]
        public string URLComprovante { get; set; }
    }
}
