using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class DetalhesCargaCancelamento
    {
        [JsonProperty("companyCode")]
        public string Atividade { get; set; }

        [JsonProperty("plantCode")]
        public string CodigoIntegracaoFilial { get; set; }

        [JsonProperty("event")]
        public string Evento { get { return "transportOrderCancellation"; } }

        [JsonProperty("transportOrderId")]
        public string CodigoCargaEmbarcador { get; set; }

        [JsonProperty("protocolNumber")]
        public string ProtocoloCarga { get; set; }

        [JsonProperty("issuingCarrier")]
        public string CodigoIntegracaoTransportador { get; set; }

        [JsonProperty("plannedEventStartDate")]
        public string DataEnvioCancelamentoPlanejado { get; set; }

        [JsonProperty("plannedEventEndDate")]
        public string DataCancelamentoPlanejado { get; set; }

        [JsonProperty("actualEventStartDate")]
        public string DataEnvioCancelamentoEfetuado { get; set; }

        [JsonProperty("actualEventEndDate")]
        public string DataCancelamentoEfetuado { get; set; }

        [JsonProperty("reasonForEvent")]
        public string MotivoCancelamento { get; set; }

        [JsonProperty("eventStatus")]
        public string SituacaoDoCancelamento { get { return "Cancelled"; } }

        [JsonProperty("deliveriesData")]
        public List<PedidoCancelamento> Pedidos { get; set; }
    }
}
