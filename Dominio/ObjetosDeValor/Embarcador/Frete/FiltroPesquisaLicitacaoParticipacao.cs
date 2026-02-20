namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaLicitacaoParticipacao
    {
        public int CodigoTabelaFrete { get; set; }

        public int CodigoTransportador { get; set; }

        public string DescricaoLicitacao { get; set; }

        public int NumeroLicitacao { get; set; }

        public int NumeroLicitacaoParticipacao { get; set; }

        public Enumeradores.SituacaoLicitacaoParticipacao? Situacao { get; set; }
    }
}
