namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class retornoWebService
    {
        public bool erro { get; set; }
        public string mensagem { get; set; }
        public string jsonEnvio { get; set; }
        public string jsonRetorno { get; set; }
        public int StatusCode { get; set; }
    }
}
