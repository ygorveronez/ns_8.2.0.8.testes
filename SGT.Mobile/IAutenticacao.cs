using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SGT.Mobile
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAutenticacao" in both code and config file together.
    [ServiceContract]
    public interface IAutenticacao
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "AutenticarUsuario/{token}/{cpf}/{senha}")]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario> AutenticarUsuario(string cpf, string senha, string token);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterURL/{token}/{cpf}/{senha}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.URLAcesso>> ObterURL(string cpf, string senha, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "AutenticarUsuarioPost")]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario> AutenticarUsuarioPost(Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Credencial credencial, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> DeslogarUsuario(string token, int usuario, int empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterSessao")]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Usuario> ObterSessao(Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Credencial credencial, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> ValidarWebService(string token);
    }
}
