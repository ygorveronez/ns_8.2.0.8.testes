using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.TruckPad
{
    public class CIOT
    {
        public string contractor_operation_type { get; set; }
        public string date_departure { get; set; }
        public string date_finish { get; set; }
        public string toll_value { get; set; }
        public string freight_value_gross { get; set; }
        public List<Contratado> hired { get; set; }
        public string fare_amount { get; set; }
        public string fare_quantity { get; set; }
        public List<Veiculo> vehicle { get; set; }
    }
}
