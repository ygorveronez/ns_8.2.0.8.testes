using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;


namespace SGT.Mobile
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IControleEntrega" in both code and config file together.
    [ServiceContract]
    public interface IControleEntrega
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarImagemEntrega(int usuario, int codigoCargaEntrega, int empresaMultisoftware, string imagem, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarImagemCanhoto(int usuario, int codigoCargaEntrega, int empresaMultisoftware, string imagem, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<string> EnviarByteImagemEntrega(Stream imagem);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> ConfirmarEntrega(int usuario, int codigoCargaEntrega, int empresaMultisoftware, string token, string latitude, string longitude, string data, string observacao);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> RejeitarEntrega(int usuario, int codigoCargaEntrega, int codigoMotivoDevolucao, int empresaMultisoftware, string token, string latitude, string longitude, string data, string observacao);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> VerificarEntregaPendente(int usuario, int codigoCargaEntrega, int empresaMultisoftware, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> IniciarEvento(int usuario, int codigoCarga, int codigoTipoAlerta, int empresaMultisoftware, string token, string latitude, string longitude, string dataEvento);
        
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> FinalizarEvento(int usuario, int codigoCarga, int codigoTipoAlerta, int empresaMultisoftware, string token, string latitude, string longitude, string dataEvento);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterDadosEntrega/{token}/{dataUltimaVerificacao}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>> ObterDadosEntrega(string usuario, string dataUltimaVerificacao, string token);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterDadosCargas/{token}/{dataUltimaVerificacao}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>> ObterDadosCargas(string usuario, string dataUltimaVerificacao, string token);


        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterMotivosDevolucao/{token}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>> ObterMotivosDevolucao(string usuario, string token);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterMotivosRejeicaoColeta/{token}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>> ObterMotivosRejeicaoColeta(string usuario, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> AtualizarDadosPosicionamento(string token, int usuario, int empresaMultisoftware, string data, string latitude, string longitude, int codigoCarga);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> IniciarViagem(int usuario, int codigoCarga, int empresaMultisoftware, string token, string latitude, string longitude, string dataInicio);

    }
}
