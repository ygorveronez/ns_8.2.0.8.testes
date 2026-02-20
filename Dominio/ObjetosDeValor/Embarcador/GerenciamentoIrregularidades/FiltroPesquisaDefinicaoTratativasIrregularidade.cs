using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades
{
    public sealed class FiltroPesquisaDefinicaoTratativasIrregularidade
    {
        public int CodigoPortfolio { get; set; }
        public int CodigoIrregularidade { get; set; }
        public SituacaoAtivaPesquisa Situacao { get; set; }

    }
}
