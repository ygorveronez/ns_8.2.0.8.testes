namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog
{
    public class RetornoSefaz
    {
        public int status { get; set; }
        public string description { get; set; }
        public string protocolo { get; set; }
        public RetornoSefazType type { get; set; }
    }
}
