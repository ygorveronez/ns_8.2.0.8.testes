using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao
{
    public class ResponseWayPoint
    {
        public string Hint { get; set; }
        public double Distance { get; set; }
        public List<double> Location { get; set; }
        public string Name { get; set; }
        public int Waypoint_index { get; set; }

    }
}
