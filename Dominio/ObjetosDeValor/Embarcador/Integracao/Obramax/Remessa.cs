using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class Remessa
    {
        [JsonProperty("Remessa")]
        public string RemessaCarga { get; set; }

        [JsonProperty("Protocolo")]
        public string Protocolo { get; set; }

        [JsonProperty("ChaveNota")]
        public string ChaveNota { get; set; }

        [JsonProperty("PedidoVTEX")]
        public string NumeroPedido { get; set; }
        
    }
}
