using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class CTe
    {
        [JsonProperty("protocoloIntegracaoCarga")]
        public int ProtocoloIntegracaoCarga { get; set; }

        [JsonProperty("protocoloIntegracaoPedido")]
        public string ProtocoloIntegracaoPedido { get; set; }

        [JsonProperty("protocoloIntegracaoCTEs")]
        public string ProtocoloIntegracaoCTEs { get; set; }

        [JsonProperty("chaveCTe")]
        public string ChaveCTe { get; set; }

        [JsonProperty("xmlCTe")]
        public string XmlCTe { get; set; }
    }
}
