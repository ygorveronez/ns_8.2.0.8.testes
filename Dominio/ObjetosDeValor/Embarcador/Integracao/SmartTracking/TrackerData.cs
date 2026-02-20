namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SmartTracking
{
    public class TrackerData
    {
        public string trackerId { get; set; }
        public long timestamp { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public double gpsSpeed { get; set; }
        public double direction { get; set; }
    }
}
