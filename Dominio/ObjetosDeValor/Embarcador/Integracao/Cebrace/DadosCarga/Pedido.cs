using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga
{
    public class Pedido
    {
        [JsonProperty(PropertyName = "protocolo", Required = Required.Default)]
        public string Protocolo { get; set; }

        [JsonProperty(PropertyName = "destinatario", Required = Required.Default)]
        public string Destinatario { get; set; }
    }
}