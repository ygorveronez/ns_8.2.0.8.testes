namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestEnviarImagemOcorrencia
    {
        public int clienteMultisoftware { get; set; }
        public int codigoOcorrencia { get; set; }
        public string imagem { get; set; }
    }
}
