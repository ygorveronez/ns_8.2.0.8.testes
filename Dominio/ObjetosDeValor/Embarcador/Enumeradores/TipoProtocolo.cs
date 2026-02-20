namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoProtocolo
    {
        Padrao = 0,
        HTTP = 1,
        HTTPS = 2
    }

    public static class TipoProtocoloHelper
    {
        public static string ObterDescricao(this TipoProtocolo TipoProtocolo)
        {
            switch (TipoProtocolo) 
            {
                case TipoProtocolo.Padrao: return "Padrão";
                case TipoProtocolo.HTTP: return "HTTP";
                case TipoProtocolo.HTTPS: return "HTTPS";
                default: return "";
            }
        }

        public static string ObterProtocolo(this TipoProtocolo TipoProtocolo)
        {
            switch (TipoProtocolo)
            {
                case TipoProtocolo.Padrao: return "";
                case TipoProtocolo.HTTP: return "http";
                case TipoProtocolo.HTTPS: return "https";
                default: return "";
            }
        }
    }
}
