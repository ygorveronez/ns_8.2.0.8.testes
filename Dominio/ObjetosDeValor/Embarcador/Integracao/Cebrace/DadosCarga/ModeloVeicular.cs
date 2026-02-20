using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga
{
    public class ModeloVeicular
    {
        [JsonProperty(PropertyName = "CodigoIntegracao", Required = Required.Default)]
        public string CodigoIntegracao { get; set; }
    }
}