namespace Dominio.ObjetosDeValor.WebService.MDFe
{
    public class RequestMDFe
    {
        public int ProtocoloIntegracaoCarga { get; set; }
        public int Inicio { get; set; }
        public int Limite { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno TipoDocumentoRetorno { get; set; }
    }
}
