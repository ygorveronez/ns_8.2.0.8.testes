using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.JJ
{
    public class ChamadoAnexo
    {
        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public string NomeArquivo { get; set; }

        [JsonProperty(PropertyName = "extension", Required = Required.Default)]
        public string Extensao { get; set; }

        [JsonProperty(PropertyName = "base64", Required = Required.Default)]
        public string Base64 { get; set; }
    }
}
