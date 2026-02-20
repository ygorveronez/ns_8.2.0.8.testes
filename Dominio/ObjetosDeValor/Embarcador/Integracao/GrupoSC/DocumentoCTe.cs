using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC
{
    public class DocumentoCTe
    {
        [JsonProperty("base64")]
        public string Base64CTe { get; set; }

        [JsonProperty("chave")]
        public string Chave { get; set; }

        [JsonProperty("protocoloCargaTMS")]
        public string ProtocoloCargaTMS { get; set; }

        [JsonProperty("protocoloCTeComplementadoTMS")]
        public string ProtocoloCTeComplementadoTMS { get; set; }

        [JsonProperty("protocoloCTeTMS")]
        public string ProtocoloCTeTMS { get; set; }

        [JsonProperty("numeroCTe")]
        public string NumeroCTe { get; set; }

        [JsonProperty("numeroCTeComplementadoTMS")]
        public string NumeroCTeComplementadoTMS { get; set; }

        [JsonProperty("numeroOC")]
        public string NumeroOC { get; set; }

        [JsonProperty("tabelaFrete")]
        public string TabelaFrete { get; set; }

        [JsonProperty("dataVigencia")]
        public string DataVigencia { get; set; }
    }
}
