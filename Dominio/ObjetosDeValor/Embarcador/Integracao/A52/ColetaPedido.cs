using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52
{
    public class ColetaPedido
    {
        [JsonProperty(PropertyName = "cd_tipo_operacao", Required = Required.Default)]
        public string TipoOperacao { get; set; }

        [JsonProperty(PropertyName = "cd_veiculo", Required = Required.Default)]
        public string Veiculo { get; set; }

        [JsonProperty(PropertyName = "cd_motorista", Required = Required.Default)]
        public string Motorista { get; set; }

        [JsonProperty(PropertyName = "cd_carreta", Required = Required.Default)]
        public string Carreta { get; set; }

        [JsonProperty(PropertyName = "cd_remetente", Required = Required.Default)]
        public string Remetente { get; set; }

        [JsonProperty(PropertyName = "cd_destinatario", Required = Required.Default)]
        public string Destinatario { get; set; }

        [JsonProperty(PropertyName = "cd_expedidor", Required = Required.Default)]
        public string Expedidor { get; set; }

        [JsonProperty(PropertyName = "cd_recebedor", Required = Required.Default)]
        public string Recebedor { get; set; }

        [JsonProperty(PropertyName = "dt_ini_prev", Required = Required.Default)]
        public string DataInicioPrevisao { get; set; }

        [JsonProperty(PropertyName = "dt_fim_prev", Required = Required.Default)]
        public string DataFimPrevisao { get; set; }

        [JsonProperty(PropertyName = "vl_volume", Required = Required.Default)]
        public int Volume { get; set; }

        [JsonProperty(PropertyName = "vl_valor", Required = Required.Default)]
        public decimal Valor { get; set; }

        [JsonProperty(PropertyName = "vl_peso_nfe", Required = Required.Default)]
        public decimal PesoNFe { get; set; }

        [JsonProperty(PropertyName = "cd_status", Required = Required.Default)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "ds_obs", Required = Required.Default)]
        public string Observacao { get; set; }

        [JsonProperty(PropertyName = "cd_embarcador", Required = Required.Default)]
        public string Embarcador { get; set; }

        [JsonProperty(PropertyName = "cd_tipo_carga", Required = Required.Default)]
        public string TipoCarga { get; set; }

        [JsonProperty(PropertyName = "vl_quantidade_entregas", Required = Required.Default)]
        public int QuantidadeEntregas { get; set; }

        [JsonProperty(PropertyName = "rota", Required = Required.Default)]
        public ColetaPedidoRota Rota { get; set; }

    }
}