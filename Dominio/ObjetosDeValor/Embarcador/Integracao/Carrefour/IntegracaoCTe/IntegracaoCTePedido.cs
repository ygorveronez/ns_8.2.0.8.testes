using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.IntegracaoCTe
{
    public sealed class IntegracaoCTePedido
    {
        [JsonProperty(PropertyName = "protocoloPedido", Required = Required.Always)]
        public int ProtocoloPedido { get; set; }

        [JsonProperty(PropertyName = "dadosCTe", Required = Required.Always)]
        public List<IntegracaoCTeDadosCTe> CTes { get; set; }
    }
}
