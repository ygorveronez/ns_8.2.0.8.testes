using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete
{
    public class CTe
    {
        [JsonProperty("receivableIdentifier")]
        public string IdentificadorCTe { get; set; }

        [JsonProperty("shippingCompanyDocument")]
        public string CNPJEmpresa { get; set; }

        [JsonProperty("shippingCompanyName")]
        public string Empresa { get; set; }

        [JsonProperty("expectedPaymentDate")]
        public DateTime DataPagamento { get; set; }

        [JsonProperty("amount")]
        public decimal ValorTotal { get; set; }

        [JsonProperty("percentageFuelAnticipation")]
        public int PercentualDeAntecipacao { get; set; }

        [JsonProperty("shipperDocument")]
        public string TomadorCNPJ { get; set; }

        [JsonProperty("shipperName")]
        public string TomadorNome { get; set; }

        [JsonProperty("documentNumber")]
        public string NumeroCTe { get; set; }

        [JsonProperty("documentType")]
        public string TipoDocumento { get; set; }

        [JsonProperty("documentKey")]
        public string ChaveCTe { get; set; }

        [JsonProperty("documentEmissionDate")]
        public DateTime? DataEmissaoDocumento { get; set; }

        [JsonProperty("urlWebhook")]
        public string URLCliente { get; set; }
    }
}
