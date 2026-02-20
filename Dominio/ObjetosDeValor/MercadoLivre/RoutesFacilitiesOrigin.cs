using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MercadoLivre
{
    public class RoutesFacilitiesOrigin
    {
        public string facility_id { get; set; }
        public string cnpj { get; set; }
        public string name { get; set; }
        public string ie { get; set; }
        public RoutesFacilitiesAddress address { get; set; }
    }

    public class RoutesFacilitiesAddress
    {
        public string street_name { get; set; }
        public string number { get; set; }
        public string neighborhood { get; set; }
        public string city { get; set; }
        public string city_code { get; set; }
        public string zip_code { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
    }
}