using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LoggiFaturas
{
    public class RetornoLoggiFatura
    {
        [JsonProperty(PropertyName = "ZCDSMM_I_FATURA_RECEIPT", Required = Required.Default)]
        public RetornoLoggiFaturaReceipt RetornoLoggiFaturaReceipt { get; set; }
    }

    public class RetornoLoggiFaturaReceipt
    {
        [JsonProperty(PropertyName = "ZCDSMM_I_FATURA_RECEIPTType", Required = Required.Default)]
        public RetornoLoggiFaturaDados RetornoLoggiFaturaDados { get; set; }
    }

    public class RetornoLoggiFaturaDados
    {
        [JsonProperty(PropertyName = "Etapa", Required = Required.Default)]
        public string Etapa { get; set; }

        [JsonProperty(PropertyName = "Center", Required = Required.Default)]
        public string Erro { get; set; }

        [JsonProperty(PropertyName = "EtapaText", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "Amount", Required = Required.Default)]
        public string Valor { get; set; }

        [JsonProperty(PropertyName = "CreateBy", Required = Required.Default)]
        public string CriadoPor { get; set; }

        [JsonProperty(PropertyName = "Quantity", Required = Required.Default)]
        public string Quantidade { get; set; }

        [JsonProperty(PropertyName = "BpNumber", Required = Required.Default)]
        public string CNPJTransportador { get; set; }

        [JsonProperty(PropertyName = "FiscalDocument", Required = Required.Default)]
        public string TipoDocumento { get; set; }

        [JsonProperty(PropertyName = "Plant", Required = Required.Default)]
        public string CNPJFilial { get; set; }

        [JsonProperty(PropertyName = "Purchasing", Required = Required.Default)]
        public string Compra { get; set; }

        [JsonProperty(PropertyName = "CostCenter", Required = Required.Default)]
        public string CentroCusto { get; set; }

        [JsonProperty(PropertyName = "BapiMsg", Required = Required.Default)]
        public string MensagemFilial { get; set; }

        [JsonProperty(PropertyName = "CreateDt", Required = Required.Default)]
        public string DataCriacao { get; set; }

        [JsonProperty(PropertyName = "PurchaseGroup", Required = Required.Default)]
        public string GrupoCompra { get; set; }

        [JsonProperty(PropertyName = "Supplier", Required = Required.Default)]
        public string Fornecedor { get; set; }

        [JsonProperty(PropertyName = "InvoiceNumber", Required = Required.Default)]
        public string NumeroFatura { get; set; }

        [JsonProperty(PropertyName = "MaterialNumber", Required = Required.Default)]
        public string NumeroMaterial { get; set; }

        [JsonProperty(PropertyName = "MatText", Required = Required.Default)]
        public string TextoMaterial { get; set; }

        [JsonProperty(PropertyName = "DocumentDate", Required = Required.Default)]
        public string DataDocumento { get; set; }

        [JsonProperty(PropertyName = "CompanyCode", Required = Required.Default)]
        public string CompanyCode { get; set; }
    }
}

/* --- RESPONSE 201 - CREATED
{
    "ZCDSMM_I_FATURA_RECEIPT": {
        "ZCDSMM_I_FATURA_RECEIPTType": {
            "Etapa": "0",
            "Center": "",
            "EtapaText": "",
            "Amount": "100.000",
            "CreateBy": "",
            "Quantity": "1.000",
            "BpNumber": "03262073000220",
            "FiscalDocument": "Cte",
            "Plant": "24217653000200",
            "Purchasing": "",
            "CostCenter": "L30500",
            "BapiMsg": "",
            "CreateDt": "",
            "PurchaseGroup": "101",
            "Supplier": "",
            "InvoiceNumber": "900101",
            "MaterialNumber": "600081",
            "MatText": "",
            "DocumentDate": "09.02.2024",
            "CompanyCode": "LL4B"
        }
    }
}*/