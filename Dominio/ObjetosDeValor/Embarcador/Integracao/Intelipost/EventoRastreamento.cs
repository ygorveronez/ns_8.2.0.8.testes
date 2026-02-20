using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost
{
    public class EventoRastreamento
    {
        public string logistic_provider { get; set; }
        public string logistic_provider_id { get; set; }
        public string logistic_provider_federal_tax_id { get; set; }

        public string shipper { get; set; }
        public string shipper_federal_tax_id { get; set; }

        public string invoice_key { get; set; }
        public string invoice_series { get; set; }
        public string invoice_number { get; set; }

        public string tracking_code { get; set; }
        public string order_number { get; set; }
        //public string volume_number { get; set; }

        public List<Evento> events { get; set; }
    }
}
