namespace SGT.WebServiceMagalu.Models
{
    public class product
    {
        public string id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int length { get; set; }
        public decimal weight { get; set; }
        public category category { get; set; }
        public string description { get; set; }
    }
}