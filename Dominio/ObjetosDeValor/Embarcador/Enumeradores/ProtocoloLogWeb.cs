namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ProtocoloLogWeb
    {
        UDP = 0,
        TCP = 1
    }

    public static class ProtocoloLogWebHelper
    {
        public static string ObterDescricao(this ProtocoloLogWeb protocoloLogWeb)
        {
            switch (protocoloLogWeb)
            {
                case ProtocoloLogWeb.UDP: return "UDP";
                case ProtocoloLogWeb.TCP: return "TCP";
                default: return "UDP";
            }
        }
    }
}

