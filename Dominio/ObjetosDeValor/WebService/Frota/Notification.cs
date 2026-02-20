namespace Dominio.ObjetosDeValor.WebService.Frota
{
    public partial class Notification
    {
        public string ait { get; set; }

        public string at { get; set; }

        public string time { get; set; }

        public string indication_limit_at { get; set; }

        public string code { get; set; }

        public string description { get; set; }

        public string address { get; set; }

        public string city_code { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public decimal? amount { get; set; }

        public decimal? discount_amount { get; set; }
    }
}
