using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Dominio.ObjetosDeValor.NovoApp.Cargas;
using Dominio.ObjetosDeValor.NovoApp.Comum;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICargas" in both code and config file together.
    [ServiceContract]
    public interface ICargas
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseBool IniciarViagem(RequestIniciarViagem requestLogin);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        string BaixarDocumentosDeTransporteCarga(RequestDownloadDocumentoTransporte request);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseBool RecusarCarga(RequestRecusarCarga request);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseBool AceitarCargaMotorista(RequestAceitarCargaMotorista request);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<ResponseObterCargas> ObterCargas(int clienteMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<ResponseObterCargas> ObterCargaEspecifica(int clienteMultisoftware, int codigoCarga, string codigoCargaEmbarcador);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Parada ObterParada(int clienteMultisoftware, int codigoCargaEntrega);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseDocumentosTransporteCarga ObterDocumentosDeTransporteCarga(int clienteMultisoftware, int codigoCarga);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseBool AdicionarAnexosCarga(RequestSalvarAnexo request);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResponseObterExtratoViagem ObterExtratoViagem(int clienteMultisoftware);
    }
}
