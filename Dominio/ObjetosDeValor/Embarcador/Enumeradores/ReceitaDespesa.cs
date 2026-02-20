namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ReceitaDespesa
    {
        Receita = 1,
        Despesa = 2,
        Outros = 3
    }

    public static class ReceitaDespesaHelper
    {
        public static string ObterDescricao(this ReceitaDespesa receitaDespesa)
        {
            switch (receitaDespesa)
            {
                case ReceitaDespesa.Receita: return Localization.Resources.Enumeradores.ReceitaDespesa.Receita;
                case ReceitaDespesa.Despesa: return Localization.Resources.Enumeradores.ReceitaDespesa.Despesa;
                case ReceitaDespesa.Outros: return Localization.Resources.Enumeradores.ReceitaDespesa.Outros;
                default: return string.Empty;
            }
        }
    }
}
