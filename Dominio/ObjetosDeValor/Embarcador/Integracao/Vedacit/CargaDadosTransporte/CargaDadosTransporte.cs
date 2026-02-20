using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte
{
    public class CargaDadosTransporte
    {
        /// <summary>
        /// Uma lista de objetos que representam os pedidos
        /// </summary>
        [JsonProperty(PropertyName = "pedidos", Order = 0)]
        public List<Pedido> Pedidos { get; set; }
    }
}
