using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta
{
    public class RetornoConsultaCustoPedagioRota
    {
        [JsonProperty(PropertyName = "Status", Required = Required.Default)]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "Mensagem", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "CustoTotal", Required = Required.Default)]
        public decimal CustoTotal { get; set; }

        [JsonProperty(PropertyName = "CustoTotalTAG", Required = Required.Default)]
        public decimal CustoTotalTAG { get; set; }

        [JsonProperty(PropertyName = "IdentificadorHistorico", Required = Required.Default)]
        public string IdentificadorHistorico { get; set; }
    }
}
