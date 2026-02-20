using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SGT.Mobile
{
    [ServiceContract]
    public interface IControleCarregamento
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> FinalizarCarregamento(string token, int usuario, int empresaMultisoftware, int codigoControleCarregamento);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> IniciarCarregamento(string token, int usuario, int empresaMultisoftware, int codigoControleCarregamento);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterTodosEmCarregamento/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>> ObterTodosEmCarregamento(string token, string usuario, string empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterTodosEmDoca/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>> ObterTodosEmDoca(string token, string usuario, string empresaMultisoftware);
    }
}
