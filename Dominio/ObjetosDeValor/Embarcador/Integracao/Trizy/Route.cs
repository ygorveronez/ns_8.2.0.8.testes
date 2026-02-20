using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Route
    {
        public string provider { get; set; }
        public List<RouteSegment>? segments { get; set; } 
    }

    public class RouteSegment
    {
        public RouteSegmentPolyline polyline { get; set; }
    }
    public class RouteSegmentPolyline
    {
        public string encoded { get; set; }
    }
}