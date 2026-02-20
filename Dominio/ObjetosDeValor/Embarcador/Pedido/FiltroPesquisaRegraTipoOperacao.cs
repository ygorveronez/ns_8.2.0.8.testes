using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaRegraTipoOperacao
    {
        public int CodigoTipoDocumentoTransporte { get; set; }
        public int CodigoTipoEntrega { get; set; }
        public Enumeradores.SimNao? QuantidadeEtapas { get; set; }
        public int CodigoCategoriaPessoa { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
    }
}
