using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades
{
    public sealed class FiltroPesquisaIrregularidade
    {
        public string Descricao { get; set; }
        public int Sequencia { get; set; }
        public string CodigoIntegracao { get; set; }
        public int CodigoPortfolioModuloControle { get; set; }
        public bool? SeguirAprovacaoTranspPrimeiro { get; set; }
        public SituacaoAtivaPesquisa Situacao { get; set; }

    }
}
