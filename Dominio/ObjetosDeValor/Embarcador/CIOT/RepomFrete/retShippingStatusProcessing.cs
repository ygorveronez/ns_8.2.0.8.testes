using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retShippingStatusProcessing : retPadrao
    {
        public retShippingStatusProcessingResult Result { get; set; }
    }

    public class retShippingStatusProcessingResult
    {
        public string OperationKey { get; set; }
        public List<retShippingStatusProcessingResults> Results { get; set; }
    }

    public class retShippingStatusProcessingResults
    {
        public Int64 ShippingId { get; set; }
        public string Identifier { get; set; }
        public string Status { get; set; }
        public List<Errors> Errors { get; set;}
    }
}
