using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac
{
    public class Position
    {
        public long ID { get; set; }
        public int AccountNumber { get; set; }
        public string VehicleAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PositionTime { get; set; }
        public DateTime PositionTimeDT { get { return DateTime.Parse(PositionTime); } }
        public int Ignition { get; set; }
        public string Landmark { get; set; }
        public int TransmissionChannel { get; set; }
        public string UF { get; set; }
        public string Reference { get; set; }
        public double Speed { get; set; }
        public double Hodometer { get; set; }
    }

}
