using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class OrdemProduto
    {
        [DataMember(Name = "number")]
        public string number { get; set; }

        [DataMember(Name = "companyCode")]
        public string companyCode { get; set; }

        [DataMember(Name = "documentCategory")]
        public string documentCategory { get; set; }

        [DataMember(Name = "documentType")]
        public string documentType { get; set; }

        [DataMember(Name = "deletionIndicator")]
        public string deletionIndicator { get; set; }

        [DataMember(Name = "status")]
        public string status { get; set; }

        [DataMember(Name = "creationDate")]
        public string creationDate { get; set; }

        [DataMember(Name = "accountNumber")]
        public string accountNumber { get; set; }

        [DataMember(Name = "paymentKeyTerms")]
        public string paymentKeyTerms { get; set; }

        [DataMember(Name = "cashDiscountDays")]
        public decimal cashDiscountDays { get; set; }

        [DataMember(Name = "cashDiscountPercentage")]
        public decimal cashDiscountPercentage { get; set; }

        [DataMember(Name = "purchasingOrganization")]
        public string purchasingOrganization { get; set; }

        [DataMember(Name = "purchasingGroup")]
        public string purchasingGroup { get; set; }

        [DataMember(Name = "currencyKey")]
        public string currencyKey { get; set; }

        [DataMember(Name = "exchangeRate")]
        public decimal exchangeRate { get; set; }

        [DataMember(Name = "indicatorExchangeRate")]
        public string IndicadorTaxaCambio { get; set; }

        [DataMember(Name = "purchaseDate")]
        public string purchaseDate { get; set; }

        [DataMember(Name = "startValidityPeriod")]
        public string startValidityPeriod { get; set; }

        [DataMember(Name = "endValidityPeriod")]
        public string endValidityPeriod { get; set; }

        [DataMember(Name = "userCreator")]
        public string UsuarioCriacao { get; set; }

        [DataMember(Name = "condition")]
        public string condition { get; set; }

        [DataMember(Name = "partners")]
        public List<OrdemProdutoParceiro> partners { get; set; }

        [DataMember(Name = "productOrderItems")]
        public List<ItemProdutoPedido> productOrderItems { get; set; }
    }
}
