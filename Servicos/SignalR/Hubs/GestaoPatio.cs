using Dominio.MSMQ;
using Newtonsoft.Json;

namespace Servicos.SignalR.Hubs
{
	public enum GestaoPatioHubs
    {
        FluxoCargaAtualizado = 1,
    }

    public class GestaoPatio : SignalRBase<GestaoPatio>
    {
        public GestaoPatio()
        {
        }

        public static string GetHub(GestaoPatioHubs metodo)
        {
            switch (metodo)
            {
                case GestaoPatioHubs.FluxoCargaAtualizado:
                    return "FluxoCargaAtualizado";
                default:
                    return "";
            }
        }

        public void FluxoCargaAtualizado(Notification notification)
        {
            SendToAll("fluxoCargaAtualizado", JsonConvert.SerializeObject(notification.Content));
        }

        public override string GetKey()
        {
            return Context.User.Identity.Name;
        }

        public override void ProcessarNotificacao(Notification notification)
        {
            switch (notification.Service)
            {
                case "FluxoCargaAtualizado":
                    FluxoCargaAtualizado(notification);
                    break;
                default:
                    break;
            }
        }
    }
}
