namespace Servicos.Embarcador.Hubs
{
    public enum IntegracaoMercadoLivreHubs
    {
        InformarHUAtualizado = 1,
    }

    public class IntegracaoMercadoLivre : HubBase<IntegracaoMercadoLivre>
    {
        public void InformarHUAtualizado(int codigoCarga)
        {
            var retorno = new
            {
                CodigoCarga = codigoCarga
            };

			SendToAll("informarHUAtualizado", retorno);
        }

        public static string GetHub(IntegracaoMercadoLivreHubs metodo)
        {
            switch (metodo)
            {
                case IntegracaoMercadoLivreHubs.InformarHUAtualizado:
                    return "InformarHUAtualizado";

                default:
                    return "";
            }
        }

        public void ProcessarNotificacao(Dominio.MSMQ.Notification notification)
        {
            switch (notification.Service)
            {
                case "InformarHUAtualizado":
                    InformarHUAtualizado((int)notification.Content);
                    break;

                default:
                    break;
            }
        }
    }
}
