using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class MontarCargaParametros
    {
        [JsonProperty(PropertyName = "codigoArmazem", Required = Required.Default)]
        public string CodigoIntegracaoFilial { get; set; }

        [JsonProperty(PropertyName = "codigoTransporte", Required = Required.Default)]
        public string CNPJTransportadoraCarga { get; set; }

        [JsonProperty(PropertyName = "documentoMotorista", Required = Required.Default)]
        public string CpfMotorista { get; set; }

        [JsonProperty(PropertyName = "motorista", Required = Required.Default)]
        public string NomeMotorista { get; set; }

        [JsonProperty(PropertyName = "codigoOrdem", Required = Required.Default)]
        public string NumeroPedidoEmbarcador { get; set; }

        [JsonProperty(PropertyName = "placa", Required = Required.Default)]
        public string PlacaVeiculo { get; set; }

        [JsonProperty(PropertyName = "identificadorRota", Required = Required.Default)]
        public string CNPJRemetentePedido { get; set; }

        [JsonProperty(PropertyName = "dataIdealizada", Required = Required.Default)]
        public string DataCarregamentoCarga { get; set; }

        [JsonProperty(PropertyName = "numDocumentoCarga", Required = Required.Default)]
        public string ProtocoloDaCarga { get; set; }

        [JsonProperty(PropertyName = "nomeTransporte", Required = Required.Default)]
        public string RazaoSocialTransportadora { get; set; }

        [JsonProperty(PropertyName = "numeroDoca", Required = Required.Default)]
        public string NumeroDoca { get; set; }

        [JsonProperty(PropertyName = "tipoOperacao", Required = Required.Default)]
        public string TipoOperacao { get; set; }

        [JsonProperty(PropertyName = "tipoAcao", Required = Required.Default)]
        public string TipoAcao { get; set; }
    }
}
