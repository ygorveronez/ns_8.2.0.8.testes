using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga
{
    public class Transportador
    {
        [JsonProperty(PropertyName = "codigoIntegracao", Required = Required.Default)]
        public string CodigoIntegracao { get; set; }

        [JsonProperty(PropertyName = "remetente", Required = Required.Default)]
        public string Remetente { get; set; }
    }
}