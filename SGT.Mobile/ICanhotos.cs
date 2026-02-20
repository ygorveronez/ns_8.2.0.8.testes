using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;


namespace SGT.Mobile
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICanhotos" in both code and config file together.
    [ServiceContract]
    public interface ICanhotos
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "BuscarCanhotosAlteradosPorUsuario/{token}/{dataUltimaVerificacao}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto>> BuscarCanhotosAlteradosPorUsuario(string usuario, string dataUltimaVerificacao, string token);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "BuscarDocumentosAlteradosPorUsuario/{token}/{dataUltimaVerificacao}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>> BuscarDocumentosAlteradosPorUsuario(string usuario, string dataUltimaVerificacao, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarJustificativaCanhoto(string latitude, string longitude, int usuario, int canhoto, int empresaMultisoftware, string justificativa, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarImagemCanhoto(string latitude, string longitude, int usuario, int canhoto, int empresaMultisoftware, string tokenImagem, string token, string dataEntrega);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<string> EnviarByteImagemCanhoto(Stream imagem);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarImagemCanhotoLeituraOCR(int usuario, int empresaMultisoftware, string tokenImagem, string token, string cnpjEmitente, string codigoGrupoPessoaEmitente);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<int> EnviarCanhotoDigitalizacao(int clienteMultisoftware, int codigoCargaEntrega, string imagem);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarCanhotoDigitalizado(int clienteMultisoftware, int codigoCanhoto, bool requerAprovacao, bool devolvido, string observacao, string imagem, Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada);
    }
}
