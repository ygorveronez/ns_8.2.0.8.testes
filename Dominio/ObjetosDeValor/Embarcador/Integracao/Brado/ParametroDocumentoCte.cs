using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Brado
{
    public class ParametroDocumentoCte
    {
        [JsonProperty("pedido_id")]
        public string CodigoPedido { get; set; }

        [JsonProperty("arquivo")]
        public string ArquivoXML { get; set; }   
    }
}
