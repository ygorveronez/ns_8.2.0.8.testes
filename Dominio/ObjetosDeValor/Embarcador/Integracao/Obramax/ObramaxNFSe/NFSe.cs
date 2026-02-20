using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ObramaxNFSe
{
    public class NFSe
    {
        [JsonProperty("protocoloIntegracaoCarga")]
        public int ProtocoloIntegracaoCarga { get; set; }

        [JsonProperty("protocoloIntegracaoPedido")]
        public string ProtocoloIntegracaoPedido { get; set; }

        [JsonProperty("protocoloIntegracaoNFSS")]
        public string ProtocoloIntegracaoNFSe { get; set; }

        [JsonProperty("chaveNFs")]
        public string ChaveNFs { get; set; }

        [JsonProperty("xmlNFs")]
        public string XmlNFs { get; set; }

        [JsonProperty("cnpjTransportador")]
        public string CnpjTransportador { get; set; }

        [JsonProperty("numeroNFS")]
        public string NumeroNFS { get; set; }

        [JsonProperty("serieNFS")]
        public string SerieNFS { get; set; }

        [JsonProperty("valorNFS")]
        public string ValorNFS { get; set; }

        [JsonProperty("digitoVerificadorNFS")]
        public string DigitoVerificadorNFS { get; set; }

        [JsonProperty("dataEmissaoNFS")]
        public string DataEmissaoNFS { get; set; }
    }
}
