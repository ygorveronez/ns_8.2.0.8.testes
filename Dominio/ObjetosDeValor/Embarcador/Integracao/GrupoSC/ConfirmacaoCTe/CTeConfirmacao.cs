using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoCTe
{
    public class CTeConfirmacao
    {
        [JsonProperty("protocoloCTe")]
        public string ProtocoloCTe { get; set; }

        [JsonProperty("evento")]
        public string Evento { get; set; }

        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        [JsonProperty("dataHora")]
        public string DataHora { get; set; }
    }
}
