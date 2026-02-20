namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestEnvioFotoNotaFiscal
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCargaEntrega { get; set; }
        public string imagem { get; set; }
        public long data { get; set; }
    }
}
