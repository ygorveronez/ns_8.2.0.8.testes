using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart
{
    public class RetornoConsultaStatus
    {
        [JsonProperty(PropertyName = "codigo", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "mensagem", Required = Required.Default)]
        public string Mensagem { get; set; }
    }
}
