using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Google
{
    public class routes
    {
        public List<legs> legs { get; set; }

        public List<int> waypoint_order { get; set; }
    }

    public class Route
    {
        public List<routes> routes { get; set; }

        public string status { get; set; }
    }
}
