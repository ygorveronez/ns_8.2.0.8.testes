using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa
{
    public class RequestMarisaMicroStatusRemessaPedido
    {
        [JsonProperty("id")]
        public string CodigoMicroStatus;

        [JsonProperty("code")]
        public string Codigo;

        [JsonProperty("description")]
        public string Descricao;

        [JsonProperty("default_name")]
        public string DescricaoMicroStatus;

        [JsonProperty("shipment_volume_state_localized")]
        public string StatusVolumeRemessaLocalizado;

        [JsonProperty("shipment_volume_state")]
        public string MacroStatus;

        [JsonProperty("shipment_volume_state_source_id")]
        public string IdStatusVolumeRemessaOrigem;

        [JsonProperty("i18n_name")]
        public string EmNome;

        [JsonProperty("shipment_order_volume_state_id")]
        public string IdStatusVolumePedidoRemessa;

        [JsonProperty("name")]
        public string DescricaoMicroStatusNome;
    }
}