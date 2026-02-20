using Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Requisicao;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Retorno
{
    public class InputRetorno : CancelamentoCarga
    {
        [JsonProperty(PropertyName = "__metadata")]
        public Metadata Metadata { get; set; }
    }
}
