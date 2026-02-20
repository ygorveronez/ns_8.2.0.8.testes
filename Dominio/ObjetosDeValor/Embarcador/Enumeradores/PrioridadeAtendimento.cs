namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PrioridadeAtendimento
    {
        Baixa = 1,
        Normal = 2,
        Alta = 3
    }

    public static class PrioridadeAtendimentoHelper
    {
        public static string ObterDescricao(this PrioridadeAtendimento prioridade)
        {
            switch (prioridade)
            {
                case PrioridadeAtendimento.Baixa: return "Baixa";
                case PrioridadeAtendimento.Normal: return "Normal";
                case PrioridadeAtendimento.Alta: return "Alta";
                default: return string.Empty;
            }
        }
    }
}
