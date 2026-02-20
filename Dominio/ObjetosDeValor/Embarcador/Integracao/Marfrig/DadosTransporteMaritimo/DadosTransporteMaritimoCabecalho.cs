using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo
{
    public sealed class DadosTransporteMaritimoCabecalho
    {
        [JsonProperty(PropertyName = "filial", Order = 1, Required = Required.Default)]
        public string Filial { get; set; }

        [JsonProperty(PropertyName = "numeroEXP", Order = 2, Required = Required.Default)]
        public string NumeroEXP { get; set; }

        [JsonProperty(PropertyName = "numeroPedido", Order = 3, Required = Required.Default)]
        public string NumeroPedido { get; set; }

        [JsonProperty(PropertyName = "protocoloReferencia", Order = 4, Required = Required.Default)]
        public string ProtocoloReferencia { get; set; }

        [JsonProperty(PropertyName = "tipoProduto", Order = 5, Required = Required.Default)]
        public string TipoProduto { get; set; }

        [JsonProperty(PropertyName = "importador", Order = 6, Required = Required.Default)]
        public string Importador { get; set; }

        [JsonProperty(PropertyName = "especiePedido", Order = 7, Required = Required.Default)]
        public string EspeciePedido { get; set; }

        [JsonProperty(PropertyName = "codigNCM", Order = 8, Required = Required.Default)]
        public string CodigoNcm { get; set; }

        [JsonProperty(PropertyName = "incoterm", Order = 9, Required = Required.Default)]
        public string Incoterm { get; set; }

        [JsonProperty(PropertyName = "descricaoCarga", Order = 10, Required = Required.Default)]
        public string DescricaoCarga { get; set; }

        [JsonProperty(PropertyName = "descricaoIdentificacaoCarga", Order = 11, Required = Required.Default)]
        public string DescricaoIdentificacaoCarga { get; set; }

        [JsonProperty(PropertyName = "contratoFob", Order = 12, Required = Required.Default)]
        public string ContratoFob { get; set; }
    }
}
