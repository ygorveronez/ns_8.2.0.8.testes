using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SGT.Mobile
{
    [ServiceContract]
    public interface IOcorrenciaPatio
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> Adicionar(string token, int usuario, int empresaMultisoftware, int codigoCentroCarregamento, int codigoVeiculo, int codigoTipo, string descricao);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterTipos/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.OcorrenciaPatioTipo>> ObterTipos(string token, string usuario, string empresaMultisoftware);
    }
}
