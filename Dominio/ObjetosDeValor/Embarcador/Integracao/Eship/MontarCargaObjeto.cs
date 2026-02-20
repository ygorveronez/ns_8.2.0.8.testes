
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class MontarCargaObjeto
    {
        [JsonProperty(PropertyName = "codigoObjeto", Required = Required.Default)]
        public string NumeroPedidoEmbarcador { get; set; }
    }
}
