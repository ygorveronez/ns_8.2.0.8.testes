namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum HubNonHub
    {
        NonHub = 0,
        Hub = 1,
    }

    public static class HubNonHubHelper
    {
        public static string ObterDescricao(this HubNonHub hubNonHub)
        {
            switch (hubNonHub)
            {
                case HubNonHub.NonHub: return "Non-Hub";
                case HubNonHub.Hub: return "Hub";
                default: return string.Empty;
            }
        }
    }
}