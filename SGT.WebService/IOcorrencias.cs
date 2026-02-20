using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOcorrencias" in both code and config file together.
    [ServiceContract]
    public interface IOcorrencias
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>> BuscarTiposOcorrencia(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>> BuscarMovivosAprovacaoOcorrencia(int? inicio, int? limite);

        [OperationContract]
        Retorno<int> AdicionarOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia ocorrencia);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntrega>> BuscarEntregasPedidoPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoPedidosPendentes(List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntregaProtocolo> protocolos);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia>> BuscarOcorrencia(string cnpjcpfCliente, string cnpjcpfDestinatario, string cnpjcpfRecebedor, string cnpjcpfRemetente, int? numeroCTe, string numeroNota, int? numeroPedido, string numeroPedidoCliente, string numeroSolicitacao, int? serieCTe, int? serieNota, int? inicio, int? limite, string numeroPedidoNoCliente);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> BuscarDocumentosOcorrencia(int? protocoloOcorrencia, int? numeroOcorrencia);

        [OperationContract]
        Retorno<bool> SolicitarCancelamentoOcorrencia(int protocoloOcorrencia, string motivoCancelamento);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia> BuscarSituacaoOcorrencia(int protocoloOcorrencia);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti>> BuscarOcorrenciasPorTransportador();

        [OperationContract]
        Retorno<string> ConfirmarIntegracaoOcorrenciaTransportador(int protocolo);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento>> BuscarOcorrenciasCanceladasPorTransportador();

        [OperationContract]
        Retorno<string> ConfirmarIntegracaoOcorrenciaCanceladaTransportador(int protocolo);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti> BuscarOcorrenciaPorProtocoloETransportador(int protocoloOcorrencia);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoDetalhesOcorrencia> BuscarOcorrenciaPorNumero(int numeroOcorrencia);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>> BuscarOcorrenciasEntrega(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>> BuscarOcorrenciasEntregaPorNumeroPedido(string numeroPedidoEmbarcador, string dataInicial, string dataFinal);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>> BuscarOcorrenciasEntregaPorNumeroPedidoPaginado(string numeroPedidoEmbarcador, string dataInicial, string dataFinal, int? inicioRegistros, int? limiteRegistros);


        [OperationContract]
        Retorno<int> BuscarQuantidadeOcorrenciasColetaEntregaNaoIntegradas(string dataInicial, string dataFinal);

        [OperationContract]
        Retorno<bool> SolicitarReenvioIntegracaoOcorrenciaColetaEntrega(string dataInicial, string dataFinal, int? quantidade);

        [OperationContract]
        Retorno<int> EnviarOcorrencia(Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti ocorrenciaIntegracao);

        [OperationContract]
        Retorno<bool> EnviarCancelamentoOcorrencia(Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento ocorrenciaCancelamento);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoOcorrencia(List<int> protocolos);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>> BuscarOcorrenciasPendentesIntegracao(int? quantidade);
    }
}
