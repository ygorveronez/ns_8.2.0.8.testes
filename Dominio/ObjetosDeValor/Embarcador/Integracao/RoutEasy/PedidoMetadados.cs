using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class PedidoMetadados
    {
        [JsonProperty(PropertyName = "protocolo")]
        public int Protocolo {  get; set; }
    }
}
