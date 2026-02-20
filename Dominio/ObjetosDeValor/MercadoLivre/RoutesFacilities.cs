using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MercadoLivre
{
    public class RoutesFacilities
    {
        public Int64 route_id { get; set; }
        public string route_type { get; set; }
        public string facility_id { get; set; }
        public RoutesFacilitiesPaging paging { get; set; }
        public List<RoutesFacilitiesItems> items { get; set; }
    }
}
