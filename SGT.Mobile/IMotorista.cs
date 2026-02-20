using System.ServiceModel;
using System.ServiceModel.Web;

namespace SGT.Mobile
{
    [ServiceContract]
    public interface IMotorista
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa> AlterarReboque(string token, int usuario, int empresaMultisoftware, string placa);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa> AlterarTracao(string token, int usuario, int empresaMultisoftware, string placa);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> FinalizarJornada(string token, int usuario, int empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> IniciarJornada(string token, int usuario, int empresaMultisoftware);
    }
}
