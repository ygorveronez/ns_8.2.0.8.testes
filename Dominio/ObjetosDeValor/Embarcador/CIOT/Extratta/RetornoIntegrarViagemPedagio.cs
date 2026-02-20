using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class RetornoIntegrarViagemPedagio
    {
        [JsonProperty(PropertyName = "Status", Required = Required.Default)]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "Mensagem", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "Valor", Required = Required.Default)]
        public decimal? Valor { get; set; }

        [JsonProperty(PropertyName = "ProtocoloRequisicao", Required = Required.Default)]
        public string ProtocoloRequisicao { get; set; }

        [JsonProperty(PropertyName = "ProtocoloProcessamento", Required = Required.Default)]
        public string ProtocoloProcessamento { get; set; }
    }
}
