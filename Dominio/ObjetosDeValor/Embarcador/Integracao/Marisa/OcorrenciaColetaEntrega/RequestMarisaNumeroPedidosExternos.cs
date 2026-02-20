using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa
{
    public class RequestMarisaNumeroPedidosExternos
    {
        [JsonProperty("sales")]
        public string Pedidos;
    }
}