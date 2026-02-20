using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoFecharCarga
{
    public class ConfirmacaoFecharCarga
    {
        [JsonProperty("numeroCarregamento")]
        public string NumeroCarregamento { get; set; }

        [JsonProperty("objeto")]
        public string ProtocoloIntegracao { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("dataRetorno")]
        public string DataRetorno { get; set; }

        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }

        [JsonProperty("codigoMensagem")]
        public string CodigoMensagem { get; set; }
    }
}
