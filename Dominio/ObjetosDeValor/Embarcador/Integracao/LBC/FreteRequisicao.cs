using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LBC
{
    public sealed class FreteRequisicao
    {
        [JsonProperty("data")]
        public FreteRequisicaoItem[] Itens { get; set; }
    }
}
