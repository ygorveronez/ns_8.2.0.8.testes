namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ModeloCobrancaEstadiaTracking
    {
        PorEtapa = 1,
        PorEtapaAcumulada = 2,
        PorViagem = 3
    }

    public static class ModeloCobrancaEstadiaTrackingHelper
    {
        public static string Descricao(this ModeloCobrancaEstadiaTracking ModeloCobrancaEstadiaTracking)
        {
            switch (ModeloCobrancaEstadiaTracking)
            {
                case ModeloCobrancaEstadiaTracking.PorEtapa:
                    return "Por etapa";
                case ModeloCobrancaEstadiaTracking.PorEtapaAcumulada:
                    return "Por etapa acumulada";
                case ModeloCobrancaEstadiaTracking.PorViagem:
                    return "Por viagem";
                default:
                    return string.Empty;
            }
        }
    }
}

