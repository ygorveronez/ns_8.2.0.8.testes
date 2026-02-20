using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retRouteRequest : retPadrao
    {
        public List<RouteRequest> Result { get; set; }
    }

    public class RouteRequest
    {
        public int? TraceCode { get; set; }
        public int? RouteCode { get; set; }
        public decimal? Distance { get; set; }
        public bool RoundTrip { get; set; }
        public string Name { get; set; }
        public decimal? TotalVPRValue { get; set; }
        public string TraceIdentifier { get; set; }
    }
}