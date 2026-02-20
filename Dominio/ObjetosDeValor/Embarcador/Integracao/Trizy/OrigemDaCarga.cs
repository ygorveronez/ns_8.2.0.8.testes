namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class OrigemDaCarga
    {
        public string operation { get; set; }
        public string expectedAt { get; set; }
        public PontoCoordenadas point { get; set; }
        public Cliente client { get; set; }
        public Eventos events { get; set; }
    }
}
