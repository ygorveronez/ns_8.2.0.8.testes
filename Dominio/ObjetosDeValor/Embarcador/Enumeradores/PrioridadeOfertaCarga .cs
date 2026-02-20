namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PrioridadeOfertaCarga
    {
        PrioridadeZero = 0,
        PrioridadeUm = 1,
        PrioridadeDois = 2,
    }

    public static class PrioridadeOfertaCargaHelper
    {
        public static string ObterDescricao(this PrioridadeOfertaCarga prioridadeOfertaCarga)
        {
            switch (prioridadeOfertaCarga)
            {
                case PrioridadeOfertaCarga.PrioridadeZero: return "Prioridade 0";
                case PrioridadeOfertaCarga.PrioridadeUm: return "Prioridade 1";
                case PrioridadeOfertaCarga.PrioridadeDois: return "Prioridade 2";
                default: return string.Empty;
            }
        }
    }
}
