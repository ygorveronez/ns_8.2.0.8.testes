using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MercadoLivre
{
    public class RoutesFacilitiesPaging
    {
        public int limit { get; set; }
        public string next { get; set; }
        public int total_items { get; set; }
    }
}