namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum Protocolo
    {
        HTTP = 1,
        HTTPS = 2,
        TCP = 3
    }

    public class ProtocoloHelper
    {
        public static string ObterValor(Protocolo protocolo)
        {
            switch (protocolo)
            {
                case Protocolo.HTTP:
                    return "http";
                case Protocolo.HTTPS:
                    return "https";
                case Protocolo.TCP:
                    return "tcp";
                default:
                    return "http";
            }
        }
    }
}
