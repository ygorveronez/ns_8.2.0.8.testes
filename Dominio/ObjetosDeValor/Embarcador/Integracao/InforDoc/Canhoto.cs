using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.InforDoc
{
    public class Canhoto
    {
        [JsonProperty(PropertyName = "chave", Required = Required.Always)]
        public string Chave { get; set; }

        [JsonProperty(PropertyName = "nomeArquivo", Required = Required.Always)]
        public string NomeArquivo { get; set; }

        [JsonProperty(PropertyName = "base64", Required = Required.Always)]
        public string Base64 { get; set; }
    }
}
