namespace Dominio.ObjetosDeValor.Embarcador.Integracao.P44
{
    public class ShipmentStop
    {
        public int stopNumber { get; set; }
        public AppointmentWindow appointmentWindow { get; set; }
        public GeoCoordinates geoCoordinates { get; set; }
        public Location location { get; set; }
    }
}
