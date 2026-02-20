using Dominio.MSMQ;

namespace Servicos.SignalR.Hubs
{
    public enum GestaoDevolucaoHubs
    {
        InformarGestaoDevolucaoAtualizada = 1,
    }

    public class GestaoDevolucao : SignalRBase<GestaoDevolucao>
    {
        public GestaoDevolucao()
        {
        }

        public static string GetHub(GestaoDevolucaoHubs metodo)
        {
            switch (metodo)
            {
                case GestaoDevolucaoHubs.InformarGestaoDevolucaoAtualizada:
                    return "InformarGestaoDevolucaoAtualizada";
                default:
                    return "";
            }
        }

        public void InformarGestaoDevolucaoAtualizada(Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao itemGrid)
        {
            SendToAll("informarGestaoDevolucaoAtualizada", itemGrid);
        }

        public override string GetKey()
        {
            return Context.User.Identity.Name;
        }

        public override void ProcessarNotificacao(Notification notification)
        {
            switch (notification.Service)
            {
                case "InformarGestaoDevolucaoAtualizada":
                    string teste = Newtonsoft.Json.JsonConvert.SerializeObject(notification.Content);
                    InformarGestaoDevolucaoAtualizada(Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao>(teste));
                    break;
                default:
                    break;
            }
        }
    }
}
