using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Requisicao
{
    public class InputRequisicao
    {
        [JsonProperty(PropertyName = "Input")]
        public CancelamentoCarga CancelamentoCarga { get; set; }
    }
}
