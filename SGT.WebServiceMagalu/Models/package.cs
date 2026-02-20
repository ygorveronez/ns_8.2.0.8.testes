using System.Collections.Generic;

namespace SGT.WebServiceMagalu.Models
{
    public class package
    {
        public string id { get; set; }
        public origin origin { get; set; }
        public invoice invoice { get; set; }
        public List<volume> volumes { get; set; }
        public deadline deadline { get; set; }
        public destination destination { get; set; }
        public packingList packingList { get; set; }
        public deliveryService deliveryService { get; set; }
    }
}