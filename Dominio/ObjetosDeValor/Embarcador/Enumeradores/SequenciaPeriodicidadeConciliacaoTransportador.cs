namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SequenciaPeriodicidadeConciliacaoTransportador
    {
        // Bimestral
        BimestralJaneiro = 1,
        BimestralFevereiro = 2,
        // Trimestral
        TrimestralJaneiro = 3,
        TrimestralFevereiro = 4,
        TrimestralMarco = 5,
        // Semestral
        SemestralJaneiro = 6,
        SemestralFevereiro = 7,
        SemestralMarco = 8,
        SemestralAbril = 9,
        SemestralMaio = 10,
        SemestralJunho = 11,

    }

    public static class SequenciaPeriodicidadeConciliacaoTransportadorHelper
    {
        public static string ObterDescricao(this SequenciaPeriodicidadeConciliacaoTransportador sequencia)
        {
            switch (sequencia)
            {
                // Bimestral
                case SequenciaPeriodicidadeConciliacaoTransportador.BimestralJaneiro: return "Jan/Mar/Mai/Jul/Set/Nov";
                case SequenciaPeriodicidadeConciliacaoTransportador.BimestralFevereiro: return "Fev/Abr/Jun/Ago/Out/Dez";

                // Trimestral
                case SequenciaPeriodicidadeConciliacaoTransportador.TrimestralJaneiro: return "Jan/Abr/Jul/Out";
                case SequenciaPeriodicidadeConciliacaoTransportador.TrimestralFevereiro: return "Fev/Mai/Ago/Nov";
                case SequenciaPeriodicidadeConciliacaoTransportador.TrimestralMarco: return "Mar/Jun/Set/Dez";

                // Semestral
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralJaneiro: return "Jan/Jul";
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralFevereiro: return "Fev/Ago";
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralMarco: return "Mar/Set";
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralAbril: return "Abr/Out";
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralMaio: return "Mai/Nov";
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralJunho: return "Jun/Dez";

                default: return string.Empty;
            }
        }
    }
}
