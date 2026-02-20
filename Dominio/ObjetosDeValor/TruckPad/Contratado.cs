using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.TruckPad
{
    public class Contratado
    {
        public string hired_type { get; set; }
        public List<ContratadoDocumento> hired_doc { get; set; }
        public string hired_name { get; set; }
        public string hired_account_bank { get; set; }
        public string hired_account_agency { get; set; }
        public string hired_account_agency_digit { get; set; }
        public string hired_account_number { get; set; }
        public string hired_account_type { get; set; }
        public string hired_company_doc_number { get; set; }
    }
}
