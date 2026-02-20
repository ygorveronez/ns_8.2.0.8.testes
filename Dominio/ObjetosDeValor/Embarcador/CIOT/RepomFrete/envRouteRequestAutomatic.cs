using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class envRouteRequestAutomatic
    {
        public string BranchCode { get; set; }
        public string OriginIBGECode { get; set; }
        public string DestinyIBGECode { get; set; }
        public bool RoundTrip { get; set; }
        public string TraceIdentifier { get; set; }
        public List<string> RouteStopIBGE { get; set; }
    }
}