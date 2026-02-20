namespace Dominio.ObjetosDeValor.NovoApp.Eventos
{
    public class RequestFinalizarEvento
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCarga { get; set; }
        public int codigoTipoEvento { get; set; }
        public long milisegundosEvento { get; set; }
        public string observacao { get; set; }
    }
}
