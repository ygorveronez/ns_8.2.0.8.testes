using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades
{
    public sealed class FiltroPesquisaMotivoIrregularidade
    {
        public string Descricao { get; set; }
        public SituacaoAtivaPesquisa Situacao { get; set; }
        public int CodigoIrregularidade { get; set; }

    }
}
