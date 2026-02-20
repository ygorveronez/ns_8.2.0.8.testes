using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LBC
{
    public sealed class RetornoIntegracaoItem
    {
        [JsonProperty(PropertyName = "intEntityId")]
        public string CodigoIntegracao { get; set; }

        [JsonProperty(PropertyName = "entityId")]
        public int Codigo { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Mensagem { get; set; }
    }
}
