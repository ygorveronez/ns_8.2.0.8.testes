namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PeriodicidadeConciliacaoTransportador
    {
        Mensal = 1,
        Bimestral = 2,
        Trimestral = 3,
        Semestral = 4,
        Anual = 5,
    }

    public static class PeriodicidadeConciliacaoTransportadorHelper
    {
        public static string ObterDescricao(this PeriodicidadeConciliacaoTransportador periodicidade)
        {
            switch (periodicidade)
            {
                case PeriodicidadeConciliacaoTransportador.Mensal: return "Mensal";
                case PeriodicidadeConciliacaoTransportador.Bimestral: return "Bimestral";
                case PeriodicidadeConciliacaoTransportador.Trimestral: return "Trimestral";
                case PeriodicidadeConciliacaoTransportador.Semestral: return "Semestral";
                case PeriodicidadeConciliacaoTransportador.Anual: return "Anual";
                default: return string.Empty;
            }
        }
    }
}
