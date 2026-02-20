namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCotacaoSolicitacaoLicitacao
    {
        ContratacaoFrete = 1,
        IdeiaFrete = 2
    }

    public static class TipoCotacaoSolicitacaoLicitacaoHelper
    {
        public static string ObterDescricao(this TipoCotacaoSolicitacaoLicitacao tipoCotacao)
        {
            switch (tipoCotacao)
            {
                case TipoCotacaoSolicitacaoLicitacao.ContratacaoFrete: return "Contratação de Frete";
                case TipoCotacaoSolicitacaoLicitacao.IdeiaFrete: return "Ideia de Frete";
                default: return string.Empty;
            }
        }
    }
}
