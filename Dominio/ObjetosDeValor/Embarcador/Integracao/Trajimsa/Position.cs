namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TrustTrack
{
    public class Position
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Altitude { get; set; }
        public int Direction { get; set; }
        public int Speed { get; set; }
        public int Satellites_count { get; set; }

    }
}
