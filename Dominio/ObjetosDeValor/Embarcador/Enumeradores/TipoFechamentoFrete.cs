namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFechamentoFrete
    {
        FechamentoPorKm = 1,
        FechamentoPorFaixaKm = 2
    }

    public static class TipoFechamentoFreteHelper
    {
        public static string ObterDescricao(this TipoFechamentoFrete tipo)
        {
            switch (tipo)
            {
                case TipoFechamentoFrete.FechamentoPorFaixaKm: return "Fechamento por faixa de km";
                case TipoFechamentoFrete.FechamentoPorKm: return "Fechamento por km";
                default: return string.Empty;
            }
        }
    }
}
