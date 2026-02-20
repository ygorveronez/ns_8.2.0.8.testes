using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class ItemProdutoPedido
    {
        [DataMember(Name = "number")]
        public int number { get; set; }

        [DataMember(Name = "deletionIndicator")]
        public string deletionIndicator { get; set; }
        
        [DataMember(Name = "plant")]
        public string plant { get; set; }

        [DataMember(Name = "changeDate")]
        public string changeDate { get; set; }

        [DataMember(Name = "shortText")]
        public string shortText { get; set; }

        [DataMember(Name = "mateialNumberMatnr")]
        public string mateialNumberMatnr { get; set; }

        [DataMember(Name = "materialNumberEmatn")]
        public string MaterialNumberEmatn { get; set; }

        [DataMember(Name = "companyCode")]
        public string companyCode { get; set; }

        [DataMember(Name = "receivingLocation")]
        public string receivingLocation { get; set; }

        [DataMember(Name = "storageLocation")]
        public string storageLocation { get; set; }

        [DataMember(Name = "materialGroup")]
        public string materialGroup { get; set; }

        [DataMember(Name = "incoterms1")]
        public string incoterms1 { get; set; }

        [DataMember(Name = "incoterms2")]
        public string incoterms2 { get; set; }

        [DataMember(Name = "confControl")]
        public string confControl { get; set; }

        [DataMember(Name = "accountAssignmentCategory")]
        public string accountAssignmentCategory { get; set; }

        [DataMember(Name = "purchaseOrderQuantity")]
        public decimal purchaseOrderQuantity { get; set; }

        [DataMember(Name = "orderUnit")]
        public string orderUnit { get; set; }

        [DataMember(Name = "orderPriceUnit")]
        public string orderPriceUnit { get; set; }

        [DataMember(Name = "orderPriceUnitConversionNumerator")]
        public double orderPriceUnitConversionNumerator { get; set; }

        [DataMember(Name = "orderPriceUnitConversionDenominator")]
        public double orderPriceUnitConversionDenominator { get; set; }

        [DataMember(Name = "netPrice")]
        public double netPrice { get; set; }

        [DataMember(Name = "priceUnit")]
        public double priceUnit { get; set; }

        [DataMember(Name = "netOrderValue")]
        public double netOrderValue { get; set; }

        [DataMember(Name = "grossOrderValue")]
        public decimal grossOrderValue { get; set; }

        [DataMember(Name = "overdeliveryTolerance")]
        public decimal overdeliveryTolerance { get; set; }

        [DataMember(Name = "unlimitedOverdeliveryAllowed")]
        public string unlimitedOverdeliveryAllowed { get; set; }

        [DataMember(Name = "underdeliveryTolerance")]
        public string underdeliveryTolerance { get; set; }

        [DataMember(Name = "deliveryCompleted")]
        public string deliveryCompleted { get; set; }

        [DataMember(Name = "taxCode")]
        public string taxCode { get; set; }

        [DataMember(Name = "category")]
        public string category { get; set; }

        [DataMember(Name = "meterialUsage")]
        public string meterialUsage { get; set; }

        [DataMember(Name = "materialOrigin")]
        public string materialOrigin { get; set; }

        [DataMember(Name = "nmcCode")]
        public string nmcCode { get; set; }

        [DataMember(Name = "matCategory")]
        public string matCategory { get; set; }

        [DataMember(Name = "producedInHouse")]
        public string producedInHouse { get; set; }

        [DataMember(Name = "partnerFunction")]
        public string partnerFunction { get; set; }

        [DataMember(Name = "materialItems")]
        public object materialItems { get; set; }

        [DataMember(Name = "conditions")]
        public List<Condicao> conditions { get; set; }
    }
}
