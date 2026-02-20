using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MercadoLivre
{
    public class RoutesFacilitiesFiscalData
    {
        public RoutesFacilitiesFiscalDataInvoice invoice { get; set; }
        public RoutesFacilitiesFiscalDataTax tax { get; set; }
    }

    public class RoutesFacilitiesFiscalDataInvoice
    {
        public string key { get; set; }
        public string type { get; set; }
        public decimal? amount { get; set; }
        public string href { get; set; }
    }

    public class RoutesFacilitiesFiscalDataTax
    {
        public string key { get; set; }
        public string type { get; set; }
        public string href { get; set; }
    }
}