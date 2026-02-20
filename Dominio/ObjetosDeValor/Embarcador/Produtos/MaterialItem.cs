using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class MaterialItem
    {

        [JsonProperty("accountAssignmentSequentialNumber")]
        public int AccountAssignmentSequentialNumber{ get; set; }

        [JsonProperty("transactionEventType")]
        public string TransactionEventType { get; set; }

        [JsonProperty("category")]
        public string Category{ get; set; }

        [JsonProperty("movementType")]
        public string MovementType{ get; set; }

        [JsonProperty("postingDate")]
        public string PostingDate{ get; set; }

        [JsonProperty("quantityMENGE")]
        public decimal QuantityMENGE{ get; set; }

        [JsonProperty("quantityBAMNG")]
        public decimal QuantityBAMNG { get; set; }

        [JsonProperty("localCurrencyAmount")]
        public decimal LocalCurrencyAmount { get; set; }

        [JsonProperty("documentCurrencyAmount")]
        public decimal DocumentCurrencyAmount { get; set; }

        [JsonProperty("deliveryNoteMeasureUnitQuantity")]
        public decimal DeliveryNoteMeasureUnitQuantity { get; set; }

        [JsonProperty("currencyKey")]
        public string CurrencyKey{ get; set; }

        [JsonProperty("debitCreditIndicator")]
        public string DebitCreditIndicator { get; set; }

        [JsonProperty("deliveryCompletedIndicator")]
        public string DeliveryCompletedIndicator{ get; set; }

        [JsonProperty("referenceDocumentNumber")]
        public string ReferenceDocumentNumber{ get; set; }

        [JsonProperty("referenceDocumentFiscalYear")]
        public int ReferenceDocumentFiscalYear{ get; set; }

        [JsonProperty("referenceDocumentNo")]
        public string ReferenceDocumentNo{ get; set; }

        [JsonProperty("referenceDocumentItem")]
        public int ReferenceDocumentItem{ get; set; }

        [JsonProperty("accountingDocumentEntryDate")]
        public string AccountingDocumentEntryDate{ get; set; }

        [JsonProperty("entryTime")]
        public string EntryTime{ get; set; }

        [JsonProperty("materialNumberMatnr")]
        public string MaterialNumberMatnr{ get; set; }

        [JsonProperty("plant")]
        public string Plant{ get; set; }

        [JsonProperty("salesPurchasesCodeTax")]
        public string SalesPurchasesCodeTax{ get; set; }

        [JsonProperty("deliveryNoteMeasureUnit")]
        public string DeliveryNoteMeasureUnit{ get; set; }

        [JsonProperty("materialNumberEMATN")]
        public string MaterialNumberEMATN{ get; set; }

        [JsonProperty("localCurrencykey")]
        public string LocalCurrencykey{ get; set; }
       
        [JsonProperty("documentDate")]
        public string DocumentDate{ get; set; }

        [JsonProperty("creatorPersonName")]
        public string CreatorPersonName{ get; set; }
    }
}
