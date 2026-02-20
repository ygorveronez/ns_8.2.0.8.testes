namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoInland
    {
        NaoDefinido = 0,
        Rodoviario = 1,
        Ferroviario = 2,
        Fluvial = 3
    }
    public static class TipoInlandHelper
    {
        public static string ObterDescricao(this TipoInland tipo)
        {
            switch (tipo)
            {
                case TipoInland.Rodoviario: return "Rodoviário";
                case TipoInland.Ferroviario: return "Ferroviário";
                case TipoInland.Fluvial: return "Fluvial";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoIntegracao(this TipoInland tipo)
        {
            switch (tipo)
            {
                case TipoInland.Rodoviario: return "RODOVIARIO";
                case TipoInland.Ferroviario: return "FERROVIARIO";
                case TipoInland.Fluvial: return "FLUVIAL";
                default: return string.Empty;
            }
        }
    }
}