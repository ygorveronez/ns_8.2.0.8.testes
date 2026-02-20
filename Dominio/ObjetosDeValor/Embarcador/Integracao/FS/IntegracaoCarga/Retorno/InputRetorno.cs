using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCarga.Retorno
{
    public class InputRetorno : Requisicao.Carga
    {
        [JsonProperty(PropertyName = "__metadata")]
        public Metadata Metadata { get; set; }
    }
}
