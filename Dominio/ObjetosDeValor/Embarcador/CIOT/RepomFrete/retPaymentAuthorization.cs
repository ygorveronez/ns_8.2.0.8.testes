using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retPaymentAuthorization
    {
        public Int64 ShippingId { get; set; }
        public string Identifier { get; set; }
        public string PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public Int64? BatchId { get; set; }
        public string Message { get; set; }
    }
}