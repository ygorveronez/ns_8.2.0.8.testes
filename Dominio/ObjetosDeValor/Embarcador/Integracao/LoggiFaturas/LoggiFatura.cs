using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LoggiFaturas
{
    public class LoggiFatura
    {
        [JsonProperty(PropertyName = "CompanyCode", Required = Required.Default)]
        public string CompanyCode { get; set; }

        [JsonProperty(PropertyName = "InvoiceNumber", Required = Required.Default)]
        public string NumeroFatura { get; set; }

        [JsonProperty(PropertyName = "BpNumber", Required = Required.Default)]
        public string CNPJTransportador { get; set; }

        [JsonProperty(PropertyName = "Quantity", Required = Required.Default)]
        public string Quantidade { get; set; }

        [JsonProperty(PropertyName = "PurchaseGroup", Required = Required.Default)]
        public string GrupoCompra { get; set; }

        [JsonProperty(PropertyName = "MaterialNumber", Required = Required.Default)]
        public string NumeroMaterial { get; set; }

        [JsonProperty(PropertyName = "FiscalDocument", Required = Required.Default)]
        public string TipoDocumento { get; set; }

        [JsonProperty(PropertyName = "DocumentDate", Required = Required.Default)]
        public string DataDocumento { get; set; }

        [JsonProperty(PropertyName = "Amount", Required = Required.Default)]
        public string Valor { get; set; }

        [JsonProperty(PropertyName = "CostCenter", Required = Required.Default)]
        public string CentroCusto { get; set; }

        [JsonProperty(PropertyName = "Plant", Required = Required.Default)]
        public string CNPJTomador { get; set; }
    }
}
