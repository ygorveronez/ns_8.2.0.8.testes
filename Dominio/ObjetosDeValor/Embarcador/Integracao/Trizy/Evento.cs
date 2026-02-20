namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Evento
    {
        public string type { get; set; }
        public string title { get; set; }
        public string expectedAt { get; set; }
        public bool required { get; set; }
        public string status { get; set; }
    }
}
