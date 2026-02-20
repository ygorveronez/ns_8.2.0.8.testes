namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoConsolidacaoMovimentoFinanceiro
    {
        Todos = 0,
        Consolidado = 1,
        NaoConsolidado = 2
    }

    public static class TipoConsolidacaoMovimentoFinanceiroHelper
    {
        public static string ObterDescricao(this TipoConsolidacaoMovimentoFinanceiro situacao)
        {
            switch (situacao)
            {
                case TipoConsolidacaoMovimentoFinanceiro.Consolidado: return "Consolidado";
                case TipoConsolidacaoMovimentoFinanceiro.NaoConsolidado: return "Não Consolidado";
                case TipoConsolidacaoMovimentoFinanceiro.Todos: return null;
                default: return string.Empty;
            }
        }
    }
}
