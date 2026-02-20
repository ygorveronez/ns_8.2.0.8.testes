namespace Dominio.ObjetosDeValor.NovoApp.Comum
{
    public class Coordenada
    {
        public long id { get; set; }

        public long dataCoordenada { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        public decimal? velocidade { get; set; }

        public long diferencaEmMilissegundos { get; set; }

        public decimal nivelBateria { get; set; }

        public decimal nivelSinalGPS { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint ConverterParaWayPoint()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
            {
                Latitude = this.latitude,
                Longitude = this.longitude
            };
        }
    }
}
