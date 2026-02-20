using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao
{
    public class ResponseRouter
    {
        public string Code { get; set; }
        public List<ResponseWayPoint> Waypoints { get; set; }
        public List<ResponseRoute> Routes { get; set; }
        public List<List<double>> Durations { get; set; }
        public List<List<double>> Distances { get; set; }
    }
}
