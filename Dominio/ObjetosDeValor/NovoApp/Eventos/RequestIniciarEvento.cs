namespace Dominio.ObjetosDeValor.NovoApp.Eventos
{
    public class RequestIniciarEvento
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCarga { get; set; }
        public int codigoTipoEvento { get; set; }
        public Dominio.ObjetosDeValor.NovoApp.Comum.Coordenada coordenada { get; set; }
        public string observacao { get; set; }
    }
}
