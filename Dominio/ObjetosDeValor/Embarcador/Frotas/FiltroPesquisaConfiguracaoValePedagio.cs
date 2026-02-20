using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public sealed class FiltroPesquisaConfiguracaoValePedagio
    {
        public int CodigoTipoOperacao { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoGrupoPessoas { get; set; }

        public SituacaoAtivoPesquisa Situacao { get; set; }

        public TipoIntegracao? TipoIntegracao { get; set; }
    }
}
