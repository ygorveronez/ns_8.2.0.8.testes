using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SGT.Mobile
{
    [ServiceContract]
    public interface IManobra
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> FinalizarIntervalo(string token, int usuario, int empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao> FinalizarManobra(string token, int usuario, int empresaMultisoftware, int codigoManobra, string QRCodeLocal);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> IniciarIntervalo(string token, int usuario, int empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterManobras/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra>> ObterManobras(string token, string usuario, string empresaMultisoftware);
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterManobrasPorMotorista/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra>> ObterManobrasPorMotorista(string token, string usuario, string empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> RemoverManobraTracaoVinculada(string token, int usuario, int empresaMultisoftware, int codigoManobra);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> RemoverReservaManobra(string token, int usuario, int empresaMultisoftware, int codigoManobra);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> ReservarManobra(string token, int usuario, int empresaMultisoftware, int codigoManobra);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> VincularManobraTracao(string token, int usuario, int empresaMultisoftware, int codigoManobra, string placa);
    }
}
