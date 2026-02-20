namespace Dominio.ObjetosDeValor.NovoApp.Cargas
{
    public class RequestIniciarViagem
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCarga { get; set; }
        public Dominio.ObjetosDeValor.NovoApp.Comum.Coordenada coordenada { get; set; }
    }
}
