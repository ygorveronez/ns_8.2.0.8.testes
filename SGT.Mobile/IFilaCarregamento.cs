using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SGT.Mobile
{
    [ServiceContract]
    public interface IFilaCarregamento
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento> AceitarCarga(string token, int usuario, int empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> AdicionarPorPlaca(string token, int usuario, int empresaMultisoftware, string placa, int codigoTipoRetornoCarga);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> AdicionarPorPlacasAtreladas(string token, int usuario, int empresaMultisoftware, string placaTracao, string placaReboque, int codigoTipoRetornoCarga);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> ConfirmarChegadaVeiculoPorCodigo(string token, int usuario, int empresaMultisoftware, int codigoFilaCarregamento);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<TipoRetornoConfirmarChegadaVeiculo> ConfirmarChegadaVeiculoPorPlaca(string token, int usuario, int empresaMultisoftware, string placa);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao> DesatrelarReboque(string token, int usuario, int empresaMultisoftware, string QRCodeLocal);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento> EntrarFilaCarregamento(string token, int usuario, int empresaMultisoftware, TipoFilaCarregamento tipoFilaCarregamento, string latitude, string longitude, int lojaProximidade);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento> Entrar(string token, int usuario, int empresaMultisoftware, int tipoRetornoCarga, string latitude, string longitude, int lojaProximidade);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> InformarDoca(string token, int usuario, int empresaMultisoftware, string hash);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento> ObterDadosFilaCarregamento(string token, int usuario, int empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<string> ObterDetalhesCarga(string token, int usuario, int empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> ObterDadosCarga(string token, int usuario, int empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterDadosCargasMotorista/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> ObterDadosCargasMotorista(string token, string usuario, string empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterPedidosPorCarga/{token}/{usuario}/{empresaMultisoftware}/{codigoCarga}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido>> ObterPedidosPorCarga(string token, string usuario, string empresaMultisoftware, string codigoCarga);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterFilasCarregamentoAguardandoChegadaVeiculo/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo>> ObterFilasCarregamentoAguardandoChegadaVeiculo(string token, string usuario, string empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.MotivoRetiradaFilaCarregamento>> ObterMotivosRetiradaFilaCarregamento(string token, int usuario, int empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.Notificacao> ObterNotificacao(string token, int usuario, int empresaMultisoftware, int codigoNotificacao);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida>> ObterNotificacoes(string token, int usuario, int empresaMultisoftware, bool somenteNaoLidas);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterTiposRetornoCarga/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoRetornoCarga>> ObterTiposRetornoCarga(string token, string usuario, string empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> RecusarCarga(string token, int usuario, int empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> SairFilaCarregamento(string token, int usuario, int empresaMultisoftware, int motivoRetiradaFilaCarregamento);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento> SairReversa(string token, int usuario, int empresaMultisoftware, string hash);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterMotivosAtendimentos/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.MotivoAtendimento>> ObterMotivosAtendimentos(string token, string usuario, string empresaMultisoftware);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterAtendimentosCarga/{token}/{usuario}/{empresaMultisoftware}")]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento>> ObterAtendimentosCarga(string token, string usuario, string empresaMultisoftware);

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<int> SolicitarAtendimento(string token, int usuario, int empresaMultisoftware, string latitude, string longitude, int codigoMotivo, string cnpjCliente, bool retencaoBau, string placaReboque, string dataReentrega, int codigoCarga);

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> AtualizarAtendimento(string token, int usuario, int empresaMultisoftware, int codigoAtendimento, string latitude, string longitude, string dataReentrega);

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> CancelarAtendimento(string token, int usuario, int empresaMultisoftware, int codigoAtendimento);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<string> EnviarByteImagemAtendimento(Stream imagem);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> EnviarImagemAtendimento(int usuario, int codigoAtendimento, int empresaMultisoftware, string tokenImagem, string token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> InformarDataChegada(string token, int usuario, int empresaMultisoftware, string dataChegada, string cnpjCliente, string latitude, string longitude, int codigoCarga);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Retorno<bool> InformarDataSaida(string token, int usuario, int empresaMultisoftware, string dataSaida, string cnpjCliente, string senhaCliente, string latitude, string longitude, int codigoCarga);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ObterArquivoPoliticaPrivacidade/{token}/{empresaMultisoftware}")]
        Retorno<string> ObterArquivoPoliticaPrivacidade(string token, string empresaMultisoftware);
    }
}
