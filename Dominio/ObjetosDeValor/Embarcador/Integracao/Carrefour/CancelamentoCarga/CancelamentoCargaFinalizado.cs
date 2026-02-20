using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.CancelamentoCarga
{
    public sealed class CancelamentoCargaFinalizado
    {
        [JsonProperty(PropertyName = "numeroRomaneio", Required = Required.Always)]
        public string CodigoCargaEmbarcador { get; set; }

        [JsonProperty(PropertyName = "protocoloCarga", Required = Required.Always)]
        public string ProtocoloCarga { get; set; }

        [JsonProperty(PropertyName = "mensagemErro", Required = Required.AllowNull)]
        public string MensagemErro
        {
            get
            {
                return "";
            }
        }

        [JsonProperty(PropertyName = "resultadoOperacao", Required = Required.Always)]
        public string ResultadoOperacao
        {
            get
            {
                return "1";
            }
        }
    }
}
