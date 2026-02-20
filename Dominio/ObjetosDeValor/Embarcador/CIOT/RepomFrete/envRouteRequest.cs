using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class envRouteRequest
    {
        public string BranchIdentifier { get; set; }
        public string OriginIBGECode { get; set; }
        public string DestinyIBGECode { get; set; }
        public bool RoundTrip { get; set; }
        public string Note { get; set; }
        public string TraceIdentifier { get; set; }
        public string ShippingPaymentPlaceType { get; set; }
        public List<PreferredWays> PreferredWays { get; set; }
        public List<string> RouteStopIBGE { get; set; }
    }

    public class PreferredWays
    {
        public string HighwayNames { get; set; }
    }
}