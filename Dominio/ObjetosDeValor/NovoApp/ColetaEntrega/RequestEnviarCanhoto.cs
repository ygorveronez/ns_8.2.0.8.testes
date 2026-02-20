namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestEnviarCanhoto
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCanhoto { get; set; }
        public bool requerAprovacao { get; set; }
        public bool devolvido { get; set; }
        public string observacao { get; set; }
        public string imagem { get; set; }
        public string IdTrizy { get; set; }
    }
}
