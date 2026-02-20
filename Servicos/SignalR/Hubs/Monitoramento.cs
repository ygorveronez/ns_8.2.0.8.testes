using Dominio.MSMQ;

namespace Servicos.SignalR.Hubs
{
    public enum MonitoramentoHubs
    {
        ListaMonitoramentosAtualizados = 1,
    }

    public class Monitoramento : SignalRBase<Monitoramento>
    {
        public Monitoramento()
        {
        }

        public static string GetHub(MonitoramentoHubs metodo)
        {
            switch (metodo)
            {
                case MonitoramentoHubs.ListaMonitoramentosAtualizados:
                    return "MonitoramentosAtualizados";
                default:
                    return "";
            }
        }

        public void MonitoramentosAtualizados(Notification notification)
        {
            dynamic Content = notification.Content;
            var retorno = new
            {
                Monitoramentos = Content.objetoMonitoramento,
            };

            SendToAll("informarListaMonitoramentoAtualizada", retorno);
        }

        public override string GetKey()
        {
            return Context.User.Identity.Name;
        }

        public override void ProcessarNotificacao(Notification notification)
        {
            switch (notification.Service)
            {
                case "MonitoramentosAtualizados":
                    MonitoramentosAtualizados(notification);
                    break;
                default:
                    break;
            }
        }
    }
}
