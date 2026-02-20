namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class Coordenada
    {
        public long id { get; set; }

        public string dataCoordenada { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        public double? Velocidade { get; set; }

        public long diferencaEmMilissegundos { get; set; }

        public decimal nivelBateria { get; set; }

        public decimal nivelSinalGPS { get; set; }
    }
}
