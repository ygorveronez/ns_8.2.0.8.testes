using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class OrdemEmbarque
    {
        [JsonProperty(PropertyName = "cabecalho", Required = Required.Always)]
        public OrdemEmbarqueCabecalho Cabecalho { get; set; }

        [JsonProperty(PropertyName = "frete", Required = Required.Always)]
        public OrdemEmbarqueFrete Frete { get; set; }

        [JsonProperty(PropertyName = "pedidos", Required = Required.Always)]
        public List<OrdemEmbarquePedido> Pedidos { get; set; }
    }
}
