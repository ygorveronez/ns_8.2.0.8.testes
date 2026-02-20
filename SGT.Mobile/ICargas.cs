using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;


namespace SGT.Mobile
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICargas" in both code and config file together.
    [ServiceContract]
    public interface ICargas
    {

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "BuscarCargaAgNotaPorUsuario/{token}/{dataUltimaVerificacao}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> BuscarCargaAgNotaPorUsuario(string usuario, string dataUltimaVerificacao, string token);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "BuscarCargaPorUsuario/{token}/{dataUltimaVerificacao}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> BuscarCargaPorUsuario(string usuario, string dataUltimaVerificacao, string token);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "BuscarCargasCanceladas/{token}/{dataUltimaVerificacao}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> BuscarCargasCanceladas(string usuario, string dataUltimaVerificacao, string token);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "BuscarCargasFinalizadas/{token}/{dataUltimaVerificacao}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> BuscarCargasFinalizadas(string usuario, string dataUltimaVerificacao, string token);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "BuscarMotivosOcorrencia/{token}/{dataUltimaVerificacao}/{usuario}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia>> BuscarMotivosOcorrencia(string usuario, string dataUltimaVerificacao, string token);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "BuscarClientesDigitalizacaoCanhoto/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> BuscarClientesDigitalizacaoCanhoto(string token, string usuario, string empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "BuscarGrupoClientesDigitalizacaoCanhoto/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>> BuscarGrupoClientesDigitalizacaoCanhoto(string token, string usuario, string empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<int> EnviarOcorrencia(int usuario, int ocorrencia, int empresaMultisoftware, string dataOcorrencia, int motivo, string observacao, string latitude, string longitude, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarDocumentoOcorrencia(int usuario, int ocorrencia, int documento, int empresaMultisoftware, string token, string numeroDocumentoRecebedor, string nomeRecebedor);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarImagemOcorrencia(int usuario, int ocorrencia, int empresaMultisoftware, string tokenImagem, string token, int codigoCargaEntrega);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<string> EnviarByteImagemOcorrencia(Stream imagem);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarOcorrenciaIntegracao(int usuario, int ocorrencia, int empresaMultisoftware, string token);


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> InformarEtapaFluxoPatio(int usuario, int empresaMultisoftware, int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxo, string dataEtapa, string token, string latitude, string longitude);


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> AtualizarDadosPosicionamento(string token, int usuario, int empresaMultisoftware, string data, string latitude, string longitude, int codigoCarga);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatio> ObterEtapaFluxoPatio(int usuario, int empresaMultisoftware, int carga, string token, int filial, string placa);


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<string> EnviarByteImagemNFe(Stream imagem);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarChaveNotaParcial(string chaveNFe, int usuario, int codigoCarga, int empresaMultisoftware, string token);
    }
}
