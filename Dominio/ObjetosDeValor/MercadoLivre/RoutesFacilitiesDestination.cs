using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MercadoLivre
{
    public class RoutesFacilitiesDestination
    {
        public string facility_id { get; set; }
        public string cnpj { get; set; }
        public string name { get; set; }
        public string ie { get; set; }
        public RoutesFacilitiesAddress address { get; set; }
    }
}