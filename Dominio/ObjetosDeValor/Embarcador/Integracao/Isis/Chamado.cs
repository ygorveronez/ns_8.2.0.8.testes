using CsvHelper.Configuration.Attributes;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Isis
{
    public class Chamado
    {
        [Name("Year")]
        public string Year { get; set; }
        [Name("Company")]
        public string Company { get; set; }
        [Name("ORIGINAL INVOICE#")]
        public string OriginalInvoice { get; set; }
        [Name("DOCUMENT NUMBER")]
        public string DocumentoNumber { get; set; }
        [Name("Customer Number")]
        public string CustomerNumber { get; set; }
        [Name("SKU")]
        public string Sku { get; set; }
        [Name("AMMOUNT")]
        public string Ammount { get; set; }
        [Name("Return Type")]
        public string ReturnType { get; set; }
        [Name("CARRIER INVOICE DT")]
        public string CarrierInvoiceDT { get; set; }
        [Name("Freight Carrier Code")]
        public string FreightCarrierCode { get; set; }
        [Name("Partial/Full")]
        public string PartialFull { get; set; }
        [Name("CNPJ")]
        public string CNPJ { get; set; }
    }
}
