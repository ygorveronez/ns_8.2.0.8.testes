using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Havan
{
    public class EnvioOcorrencia
    {
        [JsonProperty(PropertyName = "dataOcorrencia", Required = Required.Default)]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "protocoloPedido", Required = Required.Default)]
        public int ProtocoloPedido { get; set; }

        [JsonProperty(PropertyName = "codigoIntegracao", Required = Required.Default)]
        public string CodigoIntegracao { get; set; }

        [JsonProperty(PropertyName = "descricao", Required = Required.Default)]
        public string Descricao { get; set; }
    }
}
