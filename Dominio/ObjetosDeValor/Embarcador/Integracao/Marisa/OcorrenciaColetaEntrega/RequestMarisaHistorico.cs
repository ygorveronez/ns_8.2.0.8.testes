using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa
{
    public class RequestMarisaHistorico
    {
        [JsonProperty("extra")]
        public RequestMarisaExtra RequestMarisaExtra;

        [JsonProperty("shipper_provider_state")]
        public string StatusFornecedorRemetente;

        [JsonProperty("esprinter_message")]
        public string MensagemSprinter;

        [JsonProperty("request_hash")]
        public string HashRequest;

        [JsonProperty("provider_message")]
        public string MensagemFornecedor;

        [JsonProperty("shipment_order_volume_state_history")]
        public string HistoricoStatusVolumePedidoRemessa;

        [JsonProperty("created_iso")]
        public string DataEvento;

        [JsonProperty("request_origin")]
        public string OrigemRequest;

        [JsonProperty("created")]
        public string CriadoEm;

        [JsonProperty("location")]
        public RequestMarisaLocalidadeCliente RequestMarisaLocalidadeCliente;

        [JsonProperty("attachments")]
        public RequestMarisaAnexo Anexos;

        [JsonProperty("shipment_order_volume_state_localized")]
        public string StatusVolumePedidoRemessaLocalizado;

        [JsonProperty("shipment_order_volume_state")]
        public string MacroStatus;

        [JsonProperty("provider_state")]
        public string CodigoMicroStatus;

        [JsonProperty("EventDate")]
        public string DataDeEvento;

        [JsonProperty("tracking_state")]
        public string StatusTracking;

        [JsonProperty("shipment_order_volume_id")]
        public string IdVolumeRemessaPedido;

        [JsonProperty("event_date_iso")]
        public string DataEventoIso;

        [JsonProperty("shipment_volume_micro_state")]
        public RequestMarisaMicroStatusRemessaPedido RequestMarisaMicroStatusRemessaPedido;
    }
}