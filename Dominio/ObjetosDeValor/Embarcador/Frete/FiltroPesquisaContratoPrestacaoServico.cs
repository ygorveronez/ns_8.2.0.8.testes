namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaContratoPrestacaoServico
    {
        public string Descricao { get; set; }

        public Enumeradores.SituacaoContratoPrestacaoServico? Situacao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }
    }
}
