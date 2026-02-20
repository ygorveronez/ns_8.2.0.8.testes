using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MercadoLivre
{
    public class RoutesShipments
    {
        public string request_id { get; set; }
        public string entity_id { get; set; }
        public string entity_type { get; set; }
        public Int64 route_id { get; set; }
        public string route_type { get; set; }
        public RoutesFacilitiesOrigin origin { get; set; }
        public RoutesFacilitiesDestination destination { get; set; }
        public List<RoutesFacilitiesFiscalData> fiscal_data {get; set;}
    }
}