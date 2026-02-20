using Dominio.ObjetosDeValor.NovoApp.Comum;
using Dominio.ObjetosDeValor.NovoApp.Notificacao;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface INotificacao
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<ResponseObterNotificacoes> ObterNotificacoes(int ultimoCodigoRecebido);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseBool ConfirmarLeituraNotificacao(RequestConfirmarLeituraNotificacao request);
    }
}
