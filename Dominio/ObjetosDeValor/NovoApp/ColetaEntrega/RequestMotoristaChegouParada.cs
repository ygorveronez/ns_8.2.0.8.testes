namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestMotoristaChegouParada
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCargaEntrega { get; set; }
        public long data { get; set; }

        public Dominio.ObjetosDeValor.NovoApp.Comum.Coordenada coordenada { get; set; }
    }
}
