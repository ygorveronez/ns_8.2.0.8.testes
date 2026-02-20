namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PeriodoFechamento
    {
        Decendial = 10,
        Quinzenal = 15,
        Mensal = 30
    }

    public static class PeriodoFechamentoHelper
    {
        public static string ObterDescricao(this PeriodoFechamento periodoFechamento)
        {
            switch (periodoFechamento)
            {
                case PeriodoFechamento.Mensal: return "Mensal";
                case PeriodoFechamento.Quinzenal: return "Quinzenal";
                case PeriodoFechamento.Decendial: return "Decendial";
                default: return string.Empty;
            }
        }
    }
}
