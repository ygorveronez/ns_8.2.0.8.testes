using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Emissao.Integracao.Rest.Class;

namespace Emissao.Integracao.Rest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICte" in both code and config file together.
    [ServiceContract]
    public interface ICTe
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
           ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Wrapped,
              UriTemplate = "BuscarCTePorPeriodo/?DataInicial={DataInicial}&DataFinal={DataFinal}&Status={Status}&Inicio={Inicio}&Limite={Limite}")]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Rest.CTe>> BuscarCTePorPeriodo(string DataInicial, string DataFinal, string Status, int Inicio, int Limite);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
             RequestFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "ConsultarXMLCTe/?Protocolo={protocolo}&Status={Status}")]
        Retorno<string> ConsultarXMLCTe(string Protocolo, string Status);

        [OperationContract]
        [WebInvoke(Method = "GET",
         ResponseFormat = WebMessageFormat.Json,
          RequestFormat = WebMessageFormat.Json,
              BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "BuscarCTePeriodoAnterior")]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.CTe>> BuscarCTePeriodoAnterior();
    }
}
