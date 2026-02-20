using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades
{
    public sealed class FiltroPesquisaMotivoDesacordo
    {
        public string Descricao { get; set; }
        public SituacaoAtivaPesquisa Situacao { get; set; }
        public bool SubstituiCTe { get; set; }
        public int CodigoIrregularidade { get; set; }




    }
}
