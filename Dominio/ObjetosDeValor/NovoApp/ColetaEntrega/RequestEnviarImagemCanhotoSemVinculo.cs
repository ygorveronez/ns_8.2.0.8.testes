namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestEnviarImagemCanhotoSemVinculo
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCargaEntrega { get; set; }
        public string imagem { get; set; }
        public string IdTrizy { get; set; }
    }
}
