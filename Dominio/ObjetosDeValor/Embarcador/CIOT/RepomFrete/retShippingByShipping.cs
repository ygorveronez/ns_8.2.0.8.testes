using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retShippingByShipping : retPadrao
    {
        public retShippingByShippingResult Result { get; set; }
    }

    public class retShippingByShippingResult
    {
        public Int64 ShippingId { get; set; }
        public string CIOT { get; set; }
        public int? CIOTInContingency { get; set; }
        public string Status { get; set; }
        public string Identifier { get; set; }
        public string HiredNationalId { get; set; }
        public string OperationIdentifier { get; set; }
        public Int64? CardNumber { get; set; }
        public int? BrazilianRouteTraceCode { get; set; }
        public int? BrazilianRouteRouteCode { get; set; }
        public Int64? VPRCardNumber { get; set; }
        public decimal? VPRValue { get; set; }
        public List<string> VPRAntt { get; set; }
        public string SupplierDocument { get; set; }
        public string SupplierName { get; set; }
        public string RouteDescription { get; set; }
        public string TraceDescription { get; set; }
        public string TraceIdentifier { get; set; }
        public List<retShippingByShippingResultOccurences> Occurences { get; set; }
        public Int64? ContractId { get; set; }
    }

    public class retShippingByShippingResultOccurences
    {
        public string Type { get; set; }
        public string Detail { get; set; }
        public string IssueDate { get; set; }
    }
}
