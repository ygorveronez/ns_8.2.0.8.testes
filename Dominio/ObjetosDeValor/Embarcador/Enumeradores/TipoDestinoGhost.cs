namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDestinoGhost
    {
        FilaH = 0,
        MuleSoft = 1
    }

    public static class TipoDestinoGhostHelper
    {
        public static string ObterDescricao(this TipoDestinoGhost situacao)
        {
            switch (situacao)
            {
                case TipoDestinoGhost.FilaH: return "Mule -> FilaH";
                case TipoDestinoGhost.MuleSoft: return "FilaH -> Mule";
                default: return string.Empty;
            }
        }
    }
}
