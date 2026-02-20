using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCarga.Requisicao
{
    public class InputRequisicao
    {
        [JsonProperty(PropertyName = "Input")]
        public Carga Carga { get; set; }
    }
}
