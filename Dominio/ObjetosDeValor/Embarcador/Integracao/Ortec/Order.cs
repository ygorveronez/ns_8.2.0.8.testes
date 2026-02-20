using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec
{
    public class order
    {
        public int id { get; set; }
        public string order_number { get; set; }
        public string colog_CNPJ { get; set; }
        public decimal weight { get; set; }
        public string invoice_number { get; set; }
        public string invoice_serie { get; set; }
        public string invoice_date { get; set; }
        public string invoice_seller { get; set; }
        public string invoice_customer { get; set; }
        public reentrega reentrega { get; set; }
        public transport_details transport_details { get; set; }
        public List<SKU> SKUs { get; set; }

    }
}
