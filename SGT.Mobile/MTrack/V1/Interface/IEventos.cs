using Dominio.ObjetosDeValor.NovoApp.Comum;
using Dominio.ObjetosDeValor.NovoApp.Eventos;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEventos" in both code and config file together.
    [ServiceContract]
    public interface IEventos
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseBool IniciarEvento(RequestIniciarEvento request);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseBool FinalizarEvento(RequestFinalizarEvento request);
    }
}
