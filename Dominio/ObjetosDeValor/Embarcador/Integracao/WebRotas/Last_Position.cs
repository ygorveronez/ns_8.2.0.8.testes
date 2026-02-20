using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas
{
    public class Last_Position
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? Position_ignition_state { get; set; }
        public int? Speed { get; set; }
        public int? Voltage { get; set; }
        public DateTime Position_date_time { get; set; }
        public int? Temperature { get; set; }
    }
}
