using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre
{
    public sealed class FiltroPesquisaMercadoLivreHandlingUnitDetail
    {
        public int CodigoCargaMercadoLivre { get; set; }
        public SituacaoIntegracaoMercadoLivre SituacaoIntegracaoMercadoLivre { get; set; }
        public bool ExibirApenasDocumentosComMensagemErro { get; set; }
    }
}
