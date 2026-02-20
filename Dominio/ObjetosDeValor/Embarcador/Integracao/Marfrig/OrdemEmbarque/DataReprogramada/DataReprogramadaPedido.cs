using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class DataReprogramadaPedido
    {
        [JsonProperty(PropertyName = "numeroPedido", Required = Required.Always)]
        public string NumeroPedido { get; set; }

        [JsonProperty(PropertyName = "protocoloTMSPedido", Required = Required.Always)]
        public int ProtocoloPedido { get; set; }

        [JsonProperty(PropertyName = "dataReprogramada", Required = Required.AllowNull)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss", true })]
        public DateTime? DataReprogramada { get; set; }
    }
}
