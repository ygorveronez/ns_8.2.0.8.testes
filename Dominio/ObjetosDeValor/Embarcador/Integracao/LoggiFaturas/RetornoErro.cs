using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LoggiFaturas
{
    public class RetornoErro
    {
        [JsonProperty(PropertyName = "error", Required = Required.Default)]
        public RetornoErroLoggiFaturas RetornoErroLoggiFaturas { get; set; }
    }

    public class RetornoErroLoggiFaturas
    {
        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "code", Required = Required.Default)]
        public string Codigo { get; set; }
    }
}

/* --- RESPONSE 400
{
    "error": {
        "code": "Z_TMS/012",
        "message": "ptNão é permitido chave duplicada em tabela ZTBMM_FAT_REC (Fatura)",
        "innererror": {
            "application": {
                "component_id": "",
                "service_namespace": "/SAP/",
                "service_id": "ZGW_MM_FATURA_RECEIPT_SRV",
                "service_version": "0001"
            },
            "transactionid": "bdf9687dbaa5492f95af9cdfa4d9648a",
            "timestamp": "",
            "Error_Resolution": {
                "SAP_Transaction": "",
                "SAP_Note": "See SAP Note 1797736 for error analysis (https://service.sap.com/sap/support/notes/1797736)"
            },
            "errordetails": {
                "errordetail": {
                    "ContentID": "",
                    "code": "/IWBEP/CX_MGW_BUSI_EXCEPTION",
                    "message": "Não é permitido chave duplicada em tabela ZTBMM_FAT_REC (Fatura)",
                    "propertyref": "",
                    "severity": "error",
                    "target": "",
                    "transition": "false"
                }
            }
        }
    }
}*/