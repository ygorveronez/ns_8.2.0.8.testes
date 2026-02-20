namespace Dominio.ObjetosDeValor.Embarcador.Fechamento
{
    public sealed class FiltroPesquisaFechamentoJustificativaAcrescimoDesconto
    {
        public string Descricao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Situacao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativaPesquisa TipoJustificativa { get; set; }

    }
}
