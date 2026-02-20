namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee
{
    public class RetornoFiscal
    {
        public string message { get; set; }
        public int status { get; set; }
        public int total { get; set; }
        public int success { get; set; }
        public int failed { get; set; }
    }
}
