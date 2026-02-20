using System.Collections.Generic;

namespace SGT.WebServiceMagalu.Models
{
    public class destination
    {
        public List<phone> phones { get; set; }
        public seller seller { get; set; }
        public address address { get; set; }
        public deliveryPlace deliveryPlace { get; set; }
    }
}