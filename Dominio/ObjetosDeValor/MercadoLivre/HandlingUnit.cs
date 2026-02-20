using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MercadoLivre
{
    public class HandlingUnit
    {
        public string id { get; set; }
        public string date_shipped { get; set; }
        public string channel { get; set; }
        public string facility_id { get; set; }
        public string offset { get; set; }
        public List<HandlingUnitDetails> details { get; set; }
        public List<HandlingUnitFiscalData> fiscal_data { get; set; }
        public HandlingUnitLinks _links { get; set; }
    }
}
