namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestEnviarAssinatura
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCargaEntrega { get; set; }
        public string imagem { get; set; }
        public long dataEnvio { get; set; }
    }
}
