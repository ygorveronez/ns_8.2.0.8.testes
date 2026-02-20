using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac
{
    public class Vehicle
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string UCCType { get; set; }
        public string PositionTime { get; set; }
        public DateTime PositionTimeDT { get { return DateTime.Parse(PositionTime); } }
        public string ObcProfileCode { get; set; }
        public string ObcProfileName { get; set; }
        public double? FCALVel { get; set; }
        public double? FCALRPM { get; set; }
    }

}
