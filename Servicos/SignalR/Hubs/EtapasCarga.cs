using Dominio.MSMQ;

namespace Servicos.SignalR.Hubs
{

	public enum EtapasCargaHubs
    {
        InformarRetornoCalculoFrete = 1,
        InformarCargaAtualizada = 2
    }

    public class EtapasCarga : SignalRBase<Servicos.Embarcador.Hubs.Carga>
    {
        #region Atributos
        private readonly static Servicos.Embarcador.Hubs.ConnectionMapping<string> _conexoes = new Servicos.Embarcador.Hubs.ConnectionMapping<string>();
        #endregion Atributos

        public EtapasCarga()
        {
        }
        public static string GetHub(EtapasCargaHubs metodo)
        {
            switch (metodo)
            {
                case EtapasCargaHubs.InformarRetornoCalculoFrete:
                    return "InformarRetornoCalculoFrete";
                case EtapasCargaHubs.InformarCargaAtualizada:
                    return "InformarCargaAtualizada";
                default:
                    return "";
            }
        }

        public override string GetKey()
        {
            return Context.User.Identity.Name;
        }

        public override void ProcessarNotificacao(Notification notification)
        {
            switch (notification.Service)
            {
                case "InformarRetornoCalculoFrete":
                    InformarRetornoCalculoFrete(notification);
                    break;
                case "InformarCargaAtualizada":
                    InformarCargaAtualizada(notification);
                    break;
                default:
                    break;
            }
        }

        public void InformarRetornoCalculoFrete(Notification notification)
        {
            var ret = new
            {
                CodigoCarga = notification.Content.CodigoCarga,
                Retorno = notification.Content.Retorno
            };
 
            SendToAll("InformarRetornoCalculoFrete", ret);
        }

        public void InformarCargaAtualizada(Notification notification)
        {
            var retorno = new
            {
                CodigoCarga = notification.Content.CodigoCarga,
                TipoAcao = notification.Content.TipoAcao,
            };

            if (notification.Content.usuarioEnviouCarga != null)
                SendToAllExcept("informarCargaAlterada", retorno, notification.Content.usuarioEnviouCarga.Codigo.ToString());
            else
                SendToAll("informarCargaAlterada", retorno);
        }


    }
}
