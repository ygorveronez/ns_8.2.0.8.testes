namespace SGT.WebServiceMagalu.Models
{
    public class invoice
    {
        public string key { get; set; }
        public string cfop { get; set; }
        public string serie { get; set; }
        public amount amount {get; set;}
        public int number { get; set; }
        public string issueDate { get; set; }
    }
}