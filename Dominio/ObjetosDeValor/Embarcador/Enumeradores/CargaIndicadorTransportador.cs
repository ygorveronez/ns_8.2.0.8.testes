namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CargaIndicadorTransportador
    {
        NaoInformado = 0,
        InformadoViaShareRota = 1,
        InformadoManualmente = 2,
    }

    public static class CargaIndicadorTransportadorHelper
    {
        public static string ObterDescricao(this CargaIndicadorTransportador indicador)
        {
            switch (indicador)
            {
                case CargaIndicadorTransportador.InformadoViaShareRota: return "Definido pela oferta de share";
                case CargaIndicadorTransportador.InformadoManualmente: return "Definido manualmente";
                default: return string.Empty;
            }
        }
    }
}
