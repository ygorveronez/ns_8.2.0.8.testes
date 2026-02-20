using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retRouteRequestAutomatic
    {
        public retRouteRequestAutomaticData Data { get; set; }
        public List<Errors> Errors { get; set; }
    }

    public class retRouteRequestAutomaticData
    {
        public int? Status { get; set; }
        public string RouteCode { get; set; }
        public string TraceCodeRepom { get; set; }
        public string ExternalTraceCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}