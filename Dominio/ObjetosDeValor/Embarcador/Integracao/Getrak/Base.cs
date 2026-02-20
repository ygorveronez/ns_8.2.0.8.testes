namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak
{
    public class Base
    {
        public long timestamp { get; set; }
        public int status { get; set; }
        public string error { get; set; }
        public string exception { get; set; }
        public string message { get; set; }
        public string path { get; set; }
    }
}
