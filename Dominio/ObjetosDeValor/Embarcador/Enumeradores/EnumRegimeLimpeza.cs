namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumRegimeLimpeza
    {
        LimpezaASeco = 1,
        LimpezaComAgua = 2,
        LimpezaComAguaEAgenteDeLimpeza = 3,
        LimpezaEDesinfeccao = 4
    }

    public static class EnumRegimeLimpezaHelper
    {
        public static string ObterDescricao(this EnumRegimeLimpeza regime)
        {
            switch (regime)
            {
                case EnumRegimeLimpeza.LimpezaASeco: return "Regime A – Limpeza a seco";
                case EnumRegimeLimpeza.LimpezaComAgua: return "Regime B – Limpeza com Água";
                case EnumRegimeLimpeza.LimpezaComAguaEAgenteDeLimpeza: return "Regime C – Limpeza com água e um agente de limpeza";
                case EnumRegimeLimpeza.LimpezaEDesinfeccao: return "Regime D – Limpeza e Desinfecção";
                default: return string.Empty;
            }
        }
    }
}