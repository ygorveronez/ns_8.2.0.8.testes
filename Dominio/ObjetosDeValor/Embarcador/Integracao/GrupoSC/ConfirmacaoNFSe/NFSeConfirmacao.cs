using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoNFSe
{
    public class NFSeConfirmacao
    {
        [JsonProperty("protocoloNFSe")]
        public string ProtocoloNFSe { get; set; }

        [JsonProperty("evento")]
        public string Evento { get; set; }

        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        [JsonProperty("dataHora")]
        public string DataHora { get; set; }
    }
}
