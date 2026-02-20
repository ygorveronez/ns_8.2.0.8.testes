using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;

namespace Servicos.Embarcador.Hubs
{
    public class Notificacao : HubBase<Notificacao>
    {
        public void NotificarUsuario(Dominio.Entidades.Embarcador.Notificacoes.Notificacao notificacao)
        {
            dynamic retorno = Servicos.Embarcador.Notificacao.Notificacao.ObterObjetoRetorno(notificacao);

            SendToClient(notificacao.Usuario.Codigo.ToString(), "NotificarUsuario", Newtonsoft.Json.JsonConvert.SerializeObject(retorno));
        }

        public void InformarPercentualProcessado(TipoNotificacao tipoNotificacao, int codigoObjeto, string pagina, decimal percentual, Dominio.Entidades.Usuario usuario)
        {
            var retorno = new
            {
                Codigo = codigoObjeto,
                TipoNotificacao = tipoNotificacao,
                Percentual = percentual,
                Pagina = pagina
            };
                        
            if (usuario != null)
                SendToClient(usuario.Codigo.ToString(), "InformarPercentualProcessado", Newtonsoft.Json.JsonConvert.SerializeObject(retorno));
            else
                SendToAll("InformarPercentualProcessado", Newtonsoft.Json.JsonConvert.SerializeObject(retorno));
        }

        public void NotificarUsuarioMSMQ(Dominio.MSMQ.Notification notification)
        {
            if (notification.UsersID?.Count > 0)
                SendToClient(notification.UsersID[0].ToString(), "NotificarUsuario", Newtonsoft.Json.JsonConvert.SerializeObject(notification.Content));
        }
    }
}
