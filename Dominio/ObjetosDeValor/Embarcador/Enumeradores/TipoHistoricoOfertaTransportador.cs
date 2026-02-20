namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoHistoricoOfertaTransportador
    {
        Registrada = 1,
        Escolhida = 2,
        Rejeitada = 3
    }

    public static class TipoHistoricoOfertaTransportadorHelper
    {
        public static string ObterCorFonte(this TipoHistoricoOfertaTransportador tipo)
        {
            switch (tipo)
            {
                case TipoHistoricoOfertaTransportador.Escolhida: return "#666666";
                case TipoHistoricoOfertaTransportador.Rejeitada: return "#ffffff";
                default: return string.Empty;
            }
        }

        public static string ObterCorLinha(this TipoHistoricoOfertaTransportador tipo)
        {
            switch (tipo)
            {
                case TipoHistoricoOfertaTransportador.Escolhida: return "#85de7b";
                case TipoHistoricoOfertaTransportador.Rejeitada: return "#d13636";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this TipoHistoricoOfertaTransportador tipo)
        {
            switch (tipo)
            {
                case TipoHistoricoOfertaTransportador.Escolhida: return "Escolhida";
                case TipoHistoricoOfertaTransportador.Registrada: return "Registrada";
                case TipoHistoricoOfertaTransportador.Rejeitada: return "Rejeitada";
                default: return string.Empty;
            }
        }
    }
}
