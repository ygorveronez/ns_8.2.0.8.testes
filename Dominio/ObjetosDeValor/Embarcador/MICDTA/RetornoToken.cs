namespace Dominio.ObjetosDeValor.Embarcador.MICDTA
{
    public class RetornoToken
    {
        public string message { get; set; }
        public string token { get; set; }

        public string setToken { get; set; }
        public string xCSRFToken { get; set; }
    }
}
