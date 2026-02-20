using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class envShippingPaymen
    {
        public string BranchCode { get; set; }
        public Int64 ShippingID { get; set; }
        public decimal TotalUnloadWeight { get; set; }
        public List<envShippingPaymenDocuments> Documents { get; set; }
    }

    public class envShippingPaymenDocuments
    {
        public string Number { get; set; }
        public string Series { get; set; }
        public string BranchCode { get; set; }
        public string DocumentType { get; set; }
        public string DocumentStatus { get; set; }
    }
}