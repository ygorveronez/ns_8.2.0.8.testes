namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFinanceira
    {
        Liberada = 0,
        Bloqueada = 1
    }

    public static class SituacaoFinanceiraHelper
    {
        public static string ObterDescricao(this SituacaoFinanceira situacaoFinanceira)
        {
            switch (situacaoFinanceira)
            {
                case SituacaoFinanceira.Liberada: return "Liberada";
                case SituacaoFinanceira.Bloqueada: return "Bloqueada";
                default: return string.Empty;
            }
        }
    }
}
