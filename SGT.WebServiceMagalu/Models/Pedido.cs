using System.Collections.Generic;

namespace SGT.WebServiceMagalu.Models
{
    public class Pedido
    {
        public order order { get; set; }
        public carrier carrier { get; set; }
        public List<package> packages { get; set; }
    }
}