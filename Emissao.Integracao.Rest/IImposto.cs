using System.ServiceModel;
using System.ServiceModel.Web;
using Emissao.Integracao.Rest.Class;

namespace Emissao.Integracao.Rest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IImposto" in both code and config file together.
    [ServiceContract]
    public interface IImposto
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
                         ResponseFormat = WebMessageFormat.Json,
                         RequestFormat = WebMessageFormat.Json,
                         BodyStyle = WebMessageBodyStyle.Wrapped,
                         UriTemplate = "CalcularImpostoMotorista")]
        Retorno<int> CalcularImpostoMotorista(Dominio.ObjetosDeValor.ImpostoMotorista.ParametrosCalculo parametrosCalculo);

        [OperationContract]
        [WebInvoke(Method = "GET",
           ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Wrapped,
              UriTemplate = "ConsultarImpostosPorProtocolo/{protocolo}")]
        Retorno<Dominio.ObjetosDeValor.ImpostoMotorista.ImpostosCalculados> ConsultarImpostosPorProtocolo(string protocolo);

        [OperationContract]
        [WebInvoke(Method = "POST",
                         ResponseFormat = WebMessageFormat.Json,
                         RequestFormat = WebMessageFormat.Json,
                         BodyStyle = WebMessageBodyStyle.Wrapped,
                         UriTemplate = "CalcularICMS")]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS> CalcularICMS(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoICMS parametrosCalculo);

        [OperationContract]
        [WebInvoke(Method = "POST",
                         ResponseFormat = WebMessageFormat.Json,
                         RequestFormat = WebMessageFormat.Json,
                         BodyStyle = WebMessageBodyStyle.Wrapped,
                         UriTemplate = "CalcularISS")]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS> CalcularISS(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoISS parametrosCalculo);

        [OperationContract]
        [WebInvoke(Method = "POST",
                         ResponseFormat = WebMessageFormat.Json,
                         RequestFormat = WebMessageFormat.Json,
                         BodyStyle = WebMessageBodyStyle.Wrapped,
                         UriTemplate = "CalcularPISCOFINS")]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoPISCOFINS> CalcularPISCOFINS(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoPISCOFINS parametrosCalculo);
    }
}
