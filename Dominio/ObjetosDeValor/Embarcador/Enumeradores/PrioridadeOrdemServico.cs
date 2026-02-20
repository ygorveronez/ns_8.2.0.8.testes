namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PrioridadeOrdemServico
    {
        Urgente = 1,
        Alto = 2,
        Medio = 3,
        Baixo = 4
    }

    public static class PrioridadeOrdemServicoHelper
    {
        public static string ObterDescricao(this PrioridadeOrdemServico prioridadeOrdemServico)
        {
            switch (prioridadeOrdemServico)
            {
                case PrioridadeOrdemServico.Urgente: return "Urgente";
                case PrioridadeOrdemServico.Alto: return "Alto";
                case PrioridadeOrdemServico.Medio: return "Médio";
                case PrioridadeOrdemServico.Baixo: return "Baixo";
                default: return string.Empty;
            }
        }
    }
}
