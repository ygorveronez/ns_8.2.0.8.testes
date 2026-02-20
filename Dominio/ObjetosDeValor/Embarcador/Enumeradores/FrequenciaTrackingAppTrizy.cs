namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FrequenciaTrackingAppTrizy
    {
        VeryLow = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        VeryHigh= 4
    }

    public static class FrequenciaTrackingAppTrizyHelper
    {
        public static string ObterDescricao(this FrequenciaTrackingAppTrizy status)
        {
            switch (status)
            {
                case FrequenciaTrackingAppTrizy.VeryLow: return "Muito baixo";
                case FrequenciaTrackingAppTrizy.Low: return "Baixo";
                case FrequenciaTrackingAppTrizy.Medium: return "Médio";
                case FrequenciaTrackingAppTrizy.VeryHigh: return "Muito alto";
                case FrequenciaTrackingAppTrizy.High:
                default: return "Alto";
            }
        }
        public static string ObterFrequencia(this FrequenciaTrackingAppTrizy status)
        {
            switch (status)
            {
                case FrequenciaTrackingAppTrizy.VeryLow: return "VERY_LOW";
                case FrequenciaTrackingAppTrizy.Low: return "LOW";
                case FrequenciaTrackingAppTrizy.Medium: return "MEDIUM";
                case FrequenciaTrackingAppTrizy.VeryHigh: return "VERY_HIGH";
                case FrequenciaTrackingAppTrizy.High:
                default: return "HIGH";
            }
        }
    }
}
