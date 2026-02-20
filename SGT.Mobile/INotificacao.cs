using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SGT.Mobile
{
    [ServiceContract]
    public interface INotificacao
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.Notificacao> ObterNotificacao(string token, int usuario, int empresaMultisoftware, int codigoNotificacao);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoResumida>> ObterNotificacoes(string token, int usuario, int empresaMultisoftware, bool somenteNaoLidas);
    }
}
