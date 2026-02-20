using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LBC
{
    public sealed class RetornoIntegracaoSituacao
    {
        [JsonProperty("entities")]
        public RetornoIntegracaoItem[] Itens { get; set; }

        [JsonProperty("message")]
        public string Mensagem { get; set; }

        [JsonProperty("status")]
        public int Codigo { get; set; }
    }
}
