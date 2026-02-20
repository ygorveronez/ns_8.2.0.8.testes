using System.ServiceModel;
using Dominio.ObjetosDeValor.NovoApp.Autenticacao;
using System.ServiceModel.Web;
using Dominio.ObjetosDeValor.NovoApp.Comum;
using System.Collections.Generic;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAutenticacao" in both code and config file together.
    [ServiceContract]
    public interface IAutenticacao
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseLogin Login(RequestLogin requestLogin);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseBool Logout();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseBool TestarSessao(int clienteMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<string> ObterMenus(int clienteMultisoftware);
    }
}
