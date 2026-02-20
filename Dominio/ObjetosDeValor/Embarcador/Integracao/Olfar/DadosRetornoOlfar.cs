using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar
{
    public class DadosRetornoOlfar
    {
        [JsonProperty("Retorno")]
        public string Retorno { get; set; }

        [JsonProperty("MsgRet")]
        public string MensagemRetorno { get; set; }
    }
}
