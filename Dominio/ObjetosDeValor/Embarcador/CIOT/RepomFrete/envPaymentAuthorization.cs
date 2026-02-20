using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class envPaymentAuthorization
    {
        public Int64 ShippingId { get; set; }
        public string Identifier { get; set; }
        public Int64? Branch { get; set; }
        public string BranchCode { get; set; }
        public string PaymentDate { get; set; }
    }
}