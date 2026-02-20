using System.Collections.Generic;

namespace SGT.WebServiceMagalu.Models
{
    public class origin
    {
        public List<phone> phones { get; set; }
        public seller seller { get; set; }
        public address address { get; set; }
        public pickupPlace pickupPlace { get; set; }
    }
}