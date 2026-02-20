namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPreCargaOfertaTransportador
    {
        PorTransportador = 0,
        PorRotaGrupo = 1,
        PorRota = 2,
    }

    public static class TipoPreCargaOfertaTransportadorHelper
    {
        public static string ObterCorLinha(this TipoPreCargaOfertaTransportador tipo)
        {
            switch (tipo)
            {
                case TipoPreCargaOfertaTransportador.PorRota: return "#85de7b";
                case TipoPreCargaOfertaTransportador.PorRotaGrupo: return "#c8e8ff";
                case TipoPreCargaOfertaTransportador.PorTransportador: return "#ffffff";
                default: return string.Empty;
            }
        }
    }
}
