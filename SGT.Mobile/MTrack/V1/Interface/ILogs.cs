using Dominio.ObjetosDeValor.NovoApp.Comum;
using Dominio.ObjetosDeValor.NovoApp.Logs;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILogs" in both code and config file together.
    [ServiceContract]
    public interface ILogs
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseBool ArmazenarLogApp(RequestArmazenarLogApp request);
    }
}
