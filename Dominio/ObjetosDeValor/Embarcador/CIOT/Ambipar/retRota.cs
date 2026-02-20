namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class retRota
    {
        public int? status { get; set; }
        public retRotaData data { get; set; }
        public string message { get; set; }
    }

    public class retRotaData
    {
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public int? shipperId { get; set; }
        public int? GoingTripId { get; set; }
        public int? ReturnTripId { get; set; }
        public bool Active { get; set; }
        public string TripNickName { get; set; }
        public bool HaveReturn { get; set; }
        public string Stops { get; set; }
        public string ReturnStops { get; set; }
        public int? GoingRouterTripId { get; set; }
        public int? TotalDistance { get; set; }
        public int? TotalTolls { get; set; }
        public int? ID { get; set; }
        public int? ReturnRouterTripId { get; set; }
    }
}