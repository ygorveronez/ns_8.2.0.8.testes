using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public class Tracepoint
    {
        public int alternatives_count { get; set; }
        public List<double> location { get; set; }
        public double distance { get; set; }
        public string hint { get; set; }
        public string name { get; set; }
        public int matchings_index { get; set; }
        public int waypoint_index { get; set; }
    }
}
