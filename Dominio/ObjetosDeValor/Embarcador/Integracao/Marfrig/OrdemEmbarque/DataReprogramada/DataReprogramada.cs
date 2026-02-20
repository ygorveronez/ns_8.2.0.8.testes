using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class DataReprogramada
    {
        [JsonProperty(PropertyName = "pedidos", Required = Required.Always)]
        public List<DataReprogramadaPedido> Pedidos { get; set; }
    }
}
