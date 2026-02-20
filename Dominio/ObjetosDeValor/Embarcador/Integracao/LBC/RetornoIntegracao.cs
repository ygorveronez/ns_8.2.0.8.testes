using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LBC
{
    public sealed class RetornoIntegracao
    {
        [JsonProperty("data")]
        public RetornoIntegracaoSituacao[] Situacoes { get; set; }
    }
}
