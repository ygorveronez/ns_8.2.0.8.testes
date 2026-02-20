using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.Faturamento
{
    public class RequestDocumentoPagamento
    {
        public TipoDocumentoRetorno TipoDocumentoRetorno { get; set; }
        public int Inicio { get; set; }
        public int Limite { get; set; }
    }
}
