using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega;

namespace SGT.Mobile
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMonitoramento" in both code and config file together.
    [ServiceContract]
    public interface IMonitoramento
    {

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterParada/{codigoCargaEntrega}/{clienteMultisoftware}")]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada> ObterParada(string codigoCargaEntrega, string clienteMultisoftware);

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterCargas/{dataUltimaVerificacao}/{clienteMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>> ObterCargas(string dataUltimaVerificacao, string clienteMultisoftware);

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterCarga/{codigoCarga}/{clienteMultisoftware}")]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga> ObterCarga(string codigoCarga, string clienteMultisoftware);

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterEntrega/{codigoEntrega}/{clienteMultisoftware}")]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada> ObterEntrega(string codigoEntrega, string clienteMultisoftware);

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterAtendimentos/{codigoCarga}/{clienteMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>> ObterAtendimentos(string codigoCarga, string clienteMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> AtualizarDadosPosicionamento(int clienteMultisoftware, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada> coordenadas, int codigoCarga = 0);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        Retorno<bool> Confirmar(Dominio.ObjetosDeValor.Embarcador.Mobile.Request.Confirmar parameters);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        Retorno<bool> Rejeitar(Dominio.ObjetosDeValor.Embarcador.Mobile.Request.Rejeitar parameters);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> IniciarViagem(int clienteMultisoftware, int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterMotivosRejeicaoEntrega/{clienteMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>> ObterMotivosRejeicaoEntrega(string clienteMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterMotivosRejeicaoColeta/{clienteMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>> ObterMotivosRejeicaoColeta(string clienteMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterMotivosRetificacaoColeta/{clienteMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta>> ObterMotivosRetificacaoColeta(string clienteMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterMotivosFalhaGTA/{clienteMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoFalhaGTA>> ObterMotivosFalhaGTA(string clienteMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterJustificativasTemperatura/{clienteMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.JustificativaTemperatura>> ObterJustificativasTemperatura(string clienteMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterRegrasReconhecimentoCanhoto/{clienteMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ConfiguracaoReconhecimentoCanhoto>> ObterRegrasReconhecimentoCanhoto(string clienteMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarImagem(int clienteMultisoftware, int codigoCargaEntrega, string imagem, Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterTiposEventos/{clienteMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.TipoEvento>> ObterTiposEventos(string clienteMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> IniciarEvento(int codigoCarga, int codigoTipoEvento, int clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada, string observacao);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<List<RecebedoresAutorizadosPorDestinatario>> ObterDadosReconhecimentoFacialRecebedor(int clienteMultisoftware, int codigoCarga);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> FinalizarEvento(int codigoCarga, int codigoTipoEvento, int clienteMultisoftware, long milisegundosEvento, string observacao);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarChaveNFeDevolucao(int clienteMultisoftware, int codigoCargaEntrega, Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota nfeOrigem, string chaveNFe, string observacaoMotorista, string imagem);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> NotaFiscalColetaEntrega(int clienteMultisoftware, int codigoCargaEntrega, string foto, string dataEnvio);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> AssinaturaProdutor(int clienteMultisoftware, int codigoCargaEntrega, string imagem, string dataEnvio);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> FotoRecebedor(int clienteMultisoftware, int codigoCargaEntrega, string imagem, string dataEnvio);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> GuiaTransporteAnimalColetaEntrega(int clienteMultisoftware, int codigoCargaEntrega, string codigoBarras, string numeroNF, string serie, string uf, int quantidade);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> AtualizarCheckList(int clienteMultisoftware, int codigoCargaEntrega, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RespostaCheckList> respostas);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> JustificarParadaNaoProgramada(int clienteMultisoftware, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.JustificativaParadaNaoProgramada> listaJustificativas);
    }
}
