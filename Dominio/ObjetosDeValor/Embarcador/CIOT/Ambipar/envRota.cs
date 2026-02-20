namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class envRota
    {
        public int? GoingTripId { get; set; }
        public int? ReturnTripId { get; set; }
        public bool Active { get; set; }
        public string TripNickName { get; set; }
        public bool HaveReturn { get; set; }
        public string Stops { get; set; }
        public string ReturnStops { get; set; }
        public GoingTrip GoingTrip { get; set; }
        public ReturnTrip ReturnTrip { get; set; }
    }

    public class GoingTrip
    {
        public string OriginDescription { get; set; }
        public string ArrivalDescription { get; set; }
        public string OriginCep { get; set; }
        public string ArrivalCep { get; set; }
    }

    public class ReturnTrip
    {
        public string OriginDescription { get; set; }
        public string ArrivalDescription { get; set; }
        public string OriginCep { get; set; }
        public string ArrivalCep { get; set; }
    }
}