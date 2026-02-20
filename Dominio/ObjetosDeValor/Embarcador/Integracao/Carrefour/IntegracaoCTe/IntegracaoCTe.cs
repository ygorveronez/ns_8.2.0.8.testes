using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.IntegracaoCTe
{
    public sealed class IntegracaoCTe
    {
        [JsonProperty(PropertyName = "numeroRomaneio", Required = Required.Always)]
        public string NumeroCarga { get; set; }

        [JsonProperty(PropertyName = "NumeroTotalDeRegistros", Required = Required.Always)]
        public int NumeroTotalCTes { get; set; }

        [JsonProperty(PropertyName = "FLAG_Aguarda_Recebimento")]
        public bool AguardaRecebimento { get; set; }

        [JsonProperty(PropertyName = "tipoOperacao", Required = Required.Always)]
        public string TipoOperacao { get; set; }

        [JsonProperty(PropertyName = "protocoloDeCarga", Required = Required.Always)]
        public int ProtocoloCarga { get; set; }

        [JsonProperty(PropertyName = "protocolosPedido", Required = Required.Always)]
        public List<IntegracaoCTePedido> Pedidos { get; set; }
    }
}
