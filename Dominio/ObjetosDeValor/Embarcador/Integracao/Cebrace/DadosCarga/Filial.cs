using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga
{
    public class Filial
    {
        [JsonProperty(PropertyName = "codigoIntegracao", Required = Required.Default)]
        public string CodigoIntegracao { get; set; }
    }
}