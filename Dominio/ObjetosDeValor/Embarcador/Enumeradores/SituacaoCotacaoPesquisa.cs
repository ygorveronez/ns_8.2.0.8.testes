namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCotacaoPesquisa
    {
        LeilaoAguardandoAprovacaoDoVencedor = 0,
        LeilaoSemLances = 1,
        LeilaoFinalizado = 2,
        LeilaoAbertoParaLances = 3
    }

    public static class SituacaoCotacaoPesquisaHelper
    {
        public static string ObterDescricao(this SituacaoCotacaoPesquisa situacao)
        {
            switch (situacao)
            {
                case SituacaoCotacaoPesquisa.LeilaoAguardandoAprovacaoDoVencedor: return "Leilão aguardando aprovação do vencedor";
                case SituacaoCotacaoPesquisa.LeilaoSemLances: return "Leilão sem lances";
                case SituacaoCotacaoPesquisa.LeilaoFinalizado: return "Leilão finalizado";
                case SituacaoCotacaoPesquisa.LeilaoAbertoParaLances: return "Leilão aberto para lances";
                default: return string.Empty;
            }
        }
    }
}