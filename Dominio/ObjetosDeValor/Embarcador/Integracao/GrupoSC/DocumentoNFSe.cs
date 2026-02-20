using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC
{
    public class DocumentoNFSe
    {
        [JsonProperty("base64")]
        public string Base64 { get; set; }

        [JsonProperty("chave")]
        public string Chave { get; set; }

        [JsonProperty("protocoloCargaTMS")]
        public string ProtocoloCargaTMS { get; set; }

        [JsonProperty("protocoloNFSeComplementadoTMS")]
        public string ProtocoloNFSeComplementadoTMS { get; set; }

        [JsonProperty("protocoloNFSeTMS")]
        public string ProtocoloNFSeTMS { get; set; }

        [JsonProperty("numeroNFSe")]
        public string NumeroNFSe { get; set; }

        [JsonProperty("numeroNFSeComplementadoTMS")]
        public string NumeroNFSeComplementadoTMS { get; set; }

        [JsonProperty("numeroOC")]
        public string NumeroOC { get; set; }

        [JsonProperty("tabelaFrete")]
        public string TabelaFrete { get; set; }

        [JsonProperty("dataVigencia")]
        public string DataVigencia { get; set; }
    }
}
