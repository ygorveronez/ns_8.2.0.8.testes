using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retOperation : retPadrao
    {
        public List<OperationResult> Result { get; set; }
    }

    public class OperationResult
    {
        public string OperationIdentifier { get; set; }
        public bool Active { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public string CreationUser { get; set; }
        public string CreationDate { get; set; }
        public OperationBrazilianSettings BrazilianSettings { get; set; }
        public OperationDeliveryPlace DeliveryPlace { get; set; }
        public OperationStandardPaymentTerm StandardPaymentTerm { get; set; }
    }

    public class OperationBrazilianSettings
    {
        public bool CIOTObrigatorio { get; set; }
    }

    public class OperationDeliveryPlace
    {
        public string Type { get; set; }
        public int? Id { get; set; }
    }

    public class OperationStandardPaymentTerm
    {
        public string TypeId { get; set; }
        public int? PeriodTerm { get; set; }
        public int? DayDue { get; set; }
    }
}