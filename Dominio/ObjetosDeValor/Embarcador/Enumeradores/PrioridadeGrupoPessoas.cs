namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PrioridadeGrupoPessoas
    {
        Outros = 0,
        TopPrioritario = 2,
    }

    public static class PrioridadeGrupoPessoasHelper
    {
        public static string ObterDescricao(this PrioridadeGrupoPessoas o)
        {
            switch (o)
            {
                case PrioridadeGrupoPessoas.Outros: return "Outros";
                case PrioridadeGrupoPessoas.TopPrioritario: return "Top Prioritário";
                default: return string.Empty;
            }
        }
    }
}
