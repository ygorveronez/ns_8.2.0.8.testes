using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao
{
    public class RequestInvoiceProcessing
    {
        [JsonProperty("accessKey")]
        public string accessKey { get; set; }

        [JsonProperty("annulationFlag")]
        public string annulationFlag { get; set; }

        [JsonProperty("annulationReason")]
        public string annulationReason { get; set; }

        [JsonProperty("baselineDate")]
        public string baselineDate { get; set; }

        [JsonProperty("cancellationFlag")]
        public string cancellationFlag { get; set; }

        [JsonProperty("checkDigit")]
        public string checkDigit { get; set; }

        [JsonProperty("cnpjDest")]
        public string cnpjDest { get; set; }

        [JsonProperty("cnpjUnl")]
        public string cnpjUnl { get; set; }

        [JsonProperty("companyCode")]
        public string companyCode { get; set; }

        [JsonProperty("complementSESFlag")]
        public bool complementSESFlag { get; set; }

        [JsonProperty("freeFreightFlag")]
        public bool freeFreightFlag { get; set; }

        [JsonProperty("complementarOccurrenceGrossValue")]
        public decimal complementarOccurrenceGrossValue { get; set; }

        [JsonProperty("complementarOccurrencePisValue")]
        public decimal complementarOccurrencePisValue { get; set; }
        
        [JsonProperty("complementarOccurrenceConfinsValue")]
        public decimal complementarOccurrenceConfinsValue { get; set; }

        [JsonProperty("complementarOccurrenceIcmsValue")]
        public decimal complementarOccurrenceIcmsValue { get; set; }

        [JsonProperty("headerText")]
        public string headerText { get; set; }

        [JsonProperty("headerTextSummary")]
        public string headerTextSummary { get; set; }

        [JsonProperty("inOutState")]
        public string inOutState { get; set; }

        [JsonProperty("invoiceDate")]
        public string invoiceDate { get; set; }

        [JsonProperty("invoiceNumber")]
        public string invoiceNumber { get; set; }
        public string nfeIssType { get; set; }

        [JsonProperty("nfType")]
        public string nfType { get; set; }

        [JsonProperty("objectName")]
        public string objectName { get; set; }
        public string postingDate { get; set; }

        [JsonProperty("protocolNumber")]
        public string protocolNumber { get; set; }
        public string randomNumber { get; set; }

        [JsonProperty("referenceDocumentCategory")]
        public string referenceDocumentCategory { get; set; }

        [JsonProperty("totalAmount")]
        public decimal totalAmount { get; set; }

        [JsonProperty("transactionCode")]
        public string transactionCode { get; set; }

        [JsonProperty("valueNet")]
        public decimal valueNet { get; set; }

        [JsonProperty("vendorID")]
        public string vendorID { get; set; }
        public List<Item> items { get; set; }
        public List<WithholdItem> withholdItems { get; set; }
    }
}
