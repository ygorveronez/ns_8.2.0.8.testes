using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.TruckPad
{
    public class EncerramentoCIOT
    {
        public string id_freight { get; set; }
        public string protocol_freight { get; set; }
        public string contractor_doc_number { get; set; }
        public string freight_value_gross { get; set; }
        public string date_finish { get; set; }
        public string fare_amount { get; set; }
        public string fare_quantity { get; set; }
        public string payment_method { get; set; }
        public List<Rota> travel_routes { get; set; }
        public string toll_value { get; set; }
        public string fuel_value { get; set; }
        public string contractor_operation_type { get; set; }
        public List<Contratado> hired { get; set; }
        public List<Imposto> tax { get; set; }
    }
}
