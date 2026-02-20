namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class retRotaPolyline
    {
        public int? status { get; set; }
        public retRotaPolylineData data { get; set; }
        public string message { get; set; }
    }

    public class retRotaPolylineData
    {
        public string shipperId { get; set; }
        public string routeType { get; set; }
        public string tripNickName { get; set; }
        public string tripId { get; set; }
        public int? TotalTolls { get; set; }
        public string routerTripId { get; set; }
        public string routeId { get; set; }
    }
}