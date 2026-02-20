using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml;
using Emissao.Integracao.Rest.Class;

namespace Emissao.Integracao.Rest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICargas" in both code and config file together.
    [ServiceContract]
    public interface ICargas
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
                         ResponseFormat = WebMessageFormat.Xml,
                         RequestFormat = WebMessageFormat.Xml,
                         BodyStyle = WebMessageBodyStyle.Bare,
                         UriTemplate = "IntegrarXMLCarga")]
        Retorno<int> IntegrarXMLCarga(XmlElement xmlCarga);

        [OperationContract]
        [WebInvoke(Method = "POST",
                 ResponseFormat = WebMessageFormat.Json,
                 RequestFormat = WebMessageFormat.Json,
                 BodyStyle = WebMessageBodyStyle.Wrapped,
                 UriTemplate = "IntegrarDadosTransporteCarga")]
        Retorno<bool> IntegrarDadosTransporteCarga(Dominio.ObjetosDeValor.WebService.Rest.DadosTransporteCarga dadosTransporte);
    }
}
