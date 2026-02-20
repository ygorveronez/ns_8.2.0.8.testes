using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPedidos" in both code and config file together.
    [ServiceContract]
    public interface IPedidos
    {
        [OperationContract]
        Retorno<bool> AdicionarPedidoCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Task<Retorno<Dominio.ObjetosDeValor.ProcessadorTarefas.RetornoAdicionarRequestAssincrono>> AdicionarPedidoEmLote(List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracao);

        [OperationContract]
        Retorno<bool> AlterarSaldoProdutoPedido(int protocoloIntegracaoPedido, List<Dominio.ObjetosDeValor.WebService.Pedido.AlteracaoSaldoProdutoPedido> Alteracoes, string motivoAlteracao);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarPedido(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao);
        
        [OperationContract]
        Task<Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> AdicionarPedidoNovoAsync(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentosPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ExcluirNotaFiscal(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, int protocoloNotaFiscal);

        [OperationContract]
        Retorno<bool> InformarSeparacaoPedido(Dominio.ObjetosDeValor.WebService.Pedido.SeparacaoPedido protocoloSeparacaoPedido);

        [OperationContract]
        Retorno<bool> IntegrarDadosNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoCarregamento(int protocolo);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDoPedido(int protocoloIntegracaoPedido, string motivoDoCancelamento);

        [OperationContract]
        Retorno<bool> LiberarEmissaoSemNFe(int protocoloIntegracaoCarga);

        [OperationContract]
        Retorno<string> EnviarArquivoAnexoPDF(Stream arquivo);

        [OperationContract]
        Retorno<bool> IntegrarArquivoAnexoPDF(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.WebService.Carga.TokenArquivoAnexo> tokensArquivoAnexo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Pedido>> BuscarPedidosPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarPedidosPendentesIntegracao(List<int> protocolosPedido);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Pedido.SituacaoPedido> ConsultarSituacaoPedido(int protocoloIntegracaoPedido);

        [OperationContract]
        Retorno<bool> AtualizarPedido(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao dadosIntegracao, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<bool> AtualizarValorFrete(Dominio.ObjetosDeValor.WebService.Carga.Protocolo protocolo, Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor valorFrete, Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor valorFreteFilialEmissora);

        [OperationContract]
        Retorno<bool> AtualizarPedidoProduto(Dominio.ObjetosDeValor.WebService.Pedido.AtualizacaoPedidoProduto atualizacaoPedidoProduto);

        [OperationContract]
        Retorno<bool> RemoverPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<bool> AlterarPedido(Dominio.ObjetosDeValor.WebService.Pedido.AlteracaoPedido alteracaoPedido);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> BuscarTiposOperacoesPendentesIntegracao(int? quantidade);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoTiposOperacoes(List<int> listaProtocolos);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarPedidoPorProtocolo(int protocolo);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote> EnviarPacote(Dominio.ObjetosDeValor.WebService.Rest.Pedidos.Pacote pacote);

        [OperationContract]
        Retorno<bool> AjustarDatasDoPedido(Dominio.ObjetosDeValor.WebService.Pedido.AtualizarDatasPedido atualizarDatasPedido);

        [OperationContract]
        Retorno<bool> AtualizarDataUltimaGeracao(Dominio.ObjetosDeValor.WebService.Pedido.AtualizarDatasPedido atualizarDatasPedido);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Pedido>> ConsultarPedidosPorNotaFiscal(Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>> ObterAgendamentos(Dominio.ObjetosDeValor.WebService.Pedido.ObterAgendamentos obterAgendamentos);

        [OperationContract]
        Retorno<bool> AlterarSituacaoComercialPedido(Dominio.ObjetosDeValor.WebService.Pedido.AlterarSituacaoComercialPedido alterarSituacaoComercialPedido);

        [OperationContract]
        Retorno<bool> AtualizarPedidoObservacaoCte(Dominio.ObjetosDeValor.WebService.Pedido.AtualizarPedidoObservacaoCte atualizarPedidoObservacaoCte);
    }
}
