using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa
{
    public class PagamentoLinhas
    {
        [JsonProperty(PropertyName = "numeroPedido")]
        public int NumeroPedido { get; set; }

        [JsonProperty(PropertyName = "numeroNotaFiscal")]
        public int NumeroNF { get; set; }

        [JsonProperty(PropertyName = "cnpjDestino")]
        public string CNPJDestino { get; set; }

        [JsonProperty(PropertyName = "tipoDocumento")]
        public string TipoDocumento { get; set; }

        [JsonProperty(PropertyName = "numeroDocumento")]
        public int NumeroDocumento { get; set; }

        [JsonProperty(PropertyName = "chaveDocumento")]
        public string ChaveDocumento { get; set; }

        [JsonProperty(PropertyName = "protocoloDocumento")]
        public string ProtocoloDocumento { get; set; }

        [JsonProperty(PropertyName = "valorFrete")]
        public decimal ValorFrete { get; set; }
    }
}
