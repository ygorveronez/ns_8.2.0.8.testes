namespace Dominio.ObjetosDeValor.WebService.CTe
{
    public class RequestCtePorOcorrencia : Dominio.ObjetosDeValor.WebService.RequestPaginacao
    {
        public int ProtocoloIntegracaoOcorrencia { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno TipoDocumentoRetorno { get; set; }
    }
}
