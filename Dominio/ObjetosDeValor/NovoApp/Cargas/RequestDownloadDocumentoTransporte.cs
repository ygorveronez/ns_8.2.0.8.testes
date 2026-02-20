namespace Dominio.ObjetosDeValor.NovoApp.Cargas
{
    public class RequestDownloadDocumentoTransporte
    {
        public int clienteMultisoftware { get; set; }
        public int codigoDocumento { get; set; }
        public Dominio.ObjetosDeValor.NovoApp.Enumeradores.TipoDocumentoTransporte tipoDocumentoTransporte { get; set; }
    }
}
