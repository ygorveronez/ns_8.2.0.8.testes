using System.Collections.Generic;

namespace SGT.WebServiceMagalu.Models
{
    public class volume
    {
        public label label { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int length { get; set; }
        public decimal weight { get; set; }
        public List<product> products { get; set; }
    }
}