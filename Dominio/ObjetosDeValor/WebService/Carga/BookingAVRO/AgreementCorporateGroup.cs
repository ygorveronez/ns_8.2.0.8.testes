using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga.BookingAVRO
{
    public class AgreementCorporateGroup
    {
        public int corporateGroupId { get; set; }
        public string corporateGroupName { get; set; }
        public List<string> customerType { get; set; }
    }
}
