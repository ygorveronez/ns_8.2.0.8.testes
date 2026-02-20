namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestEnviarImagem
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCargaEntrega { get; set; }
        public string imagem { get; set; }
        public Dominio.ObjetosDeValor.NovoApp.Comum.Coordenada coordenada { get; set; }
    }
}
