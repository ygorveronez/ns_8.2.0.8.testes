namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PeriodoPagamento
    {
        Quinzenal = 15,
        Mensal = 30
    }

    public static class PeriodoPagamentoHelper
    {
        public static string ObterDescricao(this PeriodoPagamento periodoPagamento)
        {
            switch (periodoPagamento)
            {
                case PeriodoPagamento.Mensal: return "Mensal";
                case PeriodoPagamento.Quinzenal: return "Quinzenal";
                default: return string.Empty;
            }
        }

        public static string ObterValorOuPadrao(this PeriodoPagamento? periodoPagamento, int valorPadrao = 30)
        {
            return ((int?)periodoPagamento ?? valorPadrao).ToString();
        }
    }
}
