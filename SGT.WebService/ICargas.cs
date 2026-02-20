using CoreWCF;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.WebService.Carga;
using Dominio.ObjetosDeValor.WebService.Rest;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICargas" in both code and config file together.
    [ServiceContract]
    public interface ICargas
    {
        [OperationContract]
        Retorno<bool> InformarSeparacaoMercadoria(int protocolo, int percentualSeparacao, bool separacaoMercadoriaConfirmada);

        [OperationContract]
        Retorno<bool> SalvarNavioViagemDirecao(Dominio.ObjetosDeValor.Embarcador.Carga.Viagem viagem);

        [OperationContract]
        Retorno<bool> AlterarQuantidadeContainerBooking(int novaQuantidade, string numeroBooking);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>> BuscarCargasPorTransportador();

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>> BuscarCargasPorTransportadorV2(bool? naoRetornarCargasComplementares);

        [OperationContract]
        Retorno<string> ConfirmarIntegracaoCargaTransportador(int protocolo);

        [OperationContract]
        Retorno<bool> EncerrarCarga(int protocoloIntegracaoCarga, string ObservacaoEncerramento);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> BuscaTiposDeOperacao(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>> BuscarModelosVeiculares(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>> BuscarTiposDeCargaEmbarcador(int? inicio, int? limite);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> ConsultarCargaAtivaPorNumero(string numeroCarga);

        [OperationContract]
        Retorno<bool> AtualizarPedido(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao dadosIntegracao, int? protocoloCarga, int? protocoloPedido);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao);

        [OperationContract]
        Task<Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> AdicionarCargaNovoAsync(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao);

        [OperationContract]
        Retorno<int> AdicionarAgrupamentoDeCargas(Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.Agrupador Agrupador);

        [OperationContract]
        Retorno<int> AgruparCargas(List<int> ProtocoloCargas);

        [OperationContract]
        Retorno<bool> AtualizarCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<bool> AtualizarValorFrete(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor valorFrete);

        [OperationContract]
        Retorno<bool> AtualizarStatusPreCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.WebService.GestaoPatio.AtualizacaoStatusEtapa> etapas);

        [OperationContract]
        Retorno<bool> AtualizarRolagemCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaAtualizacaoRolagem cargaAtualizacaoRolagem, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao>> BuscarCargasAgrupadasAguardandoIntegracao();

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaAgrupadaAguardandoIntegracao>> BuscarCargasAgrupadasAguardandoIntegracaoPaginado(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoCargasAgrupadas(List<int> ListaProtocoloCarga);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao>> BuscarSituacaoCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao>> BuscarDocumentacao(Dominio.ObjetosDeValor.Embarcador.Documentos.Protocolos protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Documentos.Protocolos>> BuscarDocumentacoesPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarCargasPendentesIntegracao(int? inicio, int? limite, string codigoIntegracaoTipoOperacao, string codigoIntegracaoFilial);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarCargasRedespachoPendenteIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao);

        [OperationContract]
        Retorno<bool> TransferirPedidoEntreCargas(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocoloOriginal, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocoloFinal);

        [OperationContract]
        Retorno<bool> RemoverPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<bool> FecharCarga(int protocoloIntegracaoCarga);

        //[OperationContract]
        //Retorno<string> EnviarArquivoAnexoPDF(Stream arquivo);

        //[OperationContract]
        //Retorno<bool> IntegrarArquivoAnexoPDF(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.WebService.Carga.TokenArquivoAnexo> tokensArquivoAnexo);

        [OperationContract]
        Retorno<bool> AverbarCarga(int? protocoloIntegracaoCarga, int? codigoBooking, string numeroBooking, int? codigoOrdemServico, string numeroOrgemServico, Dominio.Enumeradores.FormaAverbacaoCTE? forma, decimal? novoValorMercadoria);

        [OperationContract]
        Retorno<bool> BloquearCancelamentoCarga(int? protocoloIntegracaoCarga, int? codigoBooking, string numeroBooking, int? codigoOrdemServico, string numeroOrgemServico);

        [OperationContract]
        Retorno<bool> InformarSinitroAvariaCarga(int? protocoloIntegracaoCarga, int? codigoBooking, string numeroBooking, int? codigoOrdemServico, string numeroOrgemServico, string chaveCTe);

        [OperationContract]
        Retorno<Paginacao<int>> BuscarCargasAguardandoConfirmacaoCancelamento(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarRecebimentoCancelamentoCarga(int protocoloIntegracaoCarga);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDaCarga(int protocoloIntegracaoCarga, string motivoDoCancelamento, string usuarioERPSolicitouCancelamento, string controleIntegracaoEmbarcador);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDosDocumentosDaCarga(int protocoloIntegracaoCarga, string motivoDoCancelamento, string usuarioERPSolicitouCancelamento);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDoPedido(int protocoloIntegracaoPedido, string motivoDoCancelamento);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento> ConsultarDetalhesCancelamentoDaCarga(int protocoloIntegracaoCarga);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento> ConsultarDetalhesCancelamentoDocumentosDaCarga(int protocoloIntegracaoCarga);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pedido.RateioFreteProduto>> ConsultarRateioFreteProdutos(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCargaPorCodigosIntegracao(Dominio.ObjetosDeValor.WebService.Carga.CodigosIntegracao codigosIntegracao);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.Resumo.Carga> BuscarResumoCargaPorCodigosIntegracao(Dominio.ObjetosDeValor.WebService.Carga.CodigosIntegracao codigosIntegracao);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentoEmMontagemPendenteIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentosPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentosPendentesIntegracaoPorFilial(int? inicio, int? limite, string codigoFilial);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentosPendentesIntegracaoV2(Dominio.ObjetosDeValor.WebService.Carga.RequestCarregamentosPendentesIntegracao requestCarregamentosPendentesIntegracao);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoPreCarga> BuscarPreCarga(int protocoloPreCarga);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoCarregamento(int protocolo);
        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoCarregamentoEmMontagem(int protocolo);

        [OperationContract]
        Retorno<bool> VincularCargas(List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> protocolos);

        [OperationContract]
        Retorno<bool> BloquearCargaFilaCarregamento(int protocolo);

        [OperationContract]
        Retorno<bool> LiberarCargaFilaCarregamento(int protocolo);

        [OperationContract]
        Retorno<bool> LiberarEmissaoSemNFe(int protocoloIntegracaoCarga);

        [OperationContract]
        Retorno<bool> SolicitarEnvioEmailDocumentosDaCarga(int protocoloIntegracaoCarga, string emails);

        [OperationContract]
        Retorno<bool> ConfirmarImpressaoCarga(int protocolo);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoCargaComCanhotosDigitalizados(int protocolo);

        [OperationContract]
        Retorno<bool> AlterarNumeroCarga(int? protocoloCarga, string numeroCarga);

        [OperationContract]
        Retorno<bool> RemoverOutrosNumerosCarga(int? protocoloCarga, string numeroCarga);

        [OperationContract]
        Retorno<bool> ConsultarCargaPedido(int protocoloPedido);

        [OperationContract]
        Retorno<bool> InformarInicioViagemCarga(int protocoloCarga, string dataHora);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.CargaCancelamento.CargaCancelamento>> BuscarCargasCanceladasPorTransportador();

        [OperationContract]
        Retorno<string> ConfirmarIntegracaoCancelamentoTransportador(int protocolo);

        [OperationContract]
        Retorno<string> BuscarDiarioBordo(int protocoloCarga);

        [OperationContract]
        Retorno<bool> AlterarPedido(Dominio.ObjetosDeValor.WebService.Pedido.AlteracaoPedido alteracaoPedido);

        [OperationContract]
        Retorno<bool> AtualizarModeloVeicularCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular modeloVeicular, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<string> BuscarReciboValePedagio(int protocoloCarga);

        [OperationContract]
        Retorno<List<string>> BuscarListaReciboValePedagio(int protocoloCarga);

        [OperationContract]
        Retorno<string> BuscarDocumentoCIOT(int protocoloCarga);

        [OperationContract]
        Retorno<RetornoCIOT> BuscarCIOT(int protocoloCarga);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoAlteracaoPedido>> ConsultarAlteracoesPedidosFinalizadas(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<int>> BuscarCargasComCanhotosDigitalizados(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> CriarFaixaTemperatura(string codigoIntegracao, string descricao, decimal? faixaInicial, decimal? faixaFinal);

        [OperationContract]
        Retorno<bool> AtualizarFaixaTemperatura(string codigoIntegracao, string descricao, decimal? faixaInicial, decimal? faixaFinal);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.EDINotfis>> BuscarEDINotfisCargas(string dataHoraInicio);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Carga.AgrupamentoCarga> BuscarAgrupamentoCarga(int protocoloCarga);

        [OperationContract]
        Retorno<bool> ConfirmarConsultaAlteracoesPedidosFinalizadas(List<int> protocolosIntegracaoPedido);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>> BuscarCargaPedidoDestinado();

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.ValePedagio>> BuscarDetalhesValePedagio(int protocoloCarga);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoCargaPedidoDestinado(int protocolo);

        [OperationContract]
        Retorno<bool> DisponibilizarCargaPedidosDestinadoParaSeparacao(List<Dominio.ObjetosDeValor.WebService.Carga.Pedido> pedidos);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.Carga> BuscarCargaPorTransportador(int protocoloCarga);

        [OperationContract]
        Retorno<bool> SalvarProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto);

        [OperationContract]
        Retorno<bool> IniciarViagem(string chaveCTe, string dataInicioViagem, string nomeEmpurrador);

        [OperationContract]
        Retorno<bool> FinalizarViagem(string chaveCTe, string dataFinalViagem);

        [OperationContract]
        Retorno<bool> AtualizarPrevisaoEntrega(string chaveCTe, string dataPrevisaoEntrega);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> GerarEncaixeRetira(Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Carregamento carregamentoIntegracao);

        [OperationContract]
        Retorno<bool> AtualizarPLPeObejtoPostalCorreios(List<Dominio.ObjetosDeValor.WebService.Carga.PostagemCorreios> postagensCorreios);

        [OperationContract]
        Retorno<bool> RecebimentoBooking(Dominio.ObjetosDeValor.WebService.Carga.Booking booking);

        [OperationContract]
        Retorno<bool> SolicitarPreCalculoCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaPreCalculoFrete cargaIntegracaoPreCalculo);

        [OperationContract]
        Retorno<bool> InformarRotaCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaRotaFrete rotaFrete);

        [OperationContract]
        Retorno<bool> IntegrarValePedagioCarga(Dominio.ObjetosDeValor.WebService.Carga.ValePedagio integracaoValePedagio);

        [OperationContract]
        Retorno<bool> InformarDadosTransporteCarga(Dominio.ObjetosDeValor.WebService.Carga.DadosTransporte dadosTransporte);

        [OperationContract]
        Retorno<bool> InformarEmbarqueContainer(Dominio.ObjetosDeValor.WebService.Carga.EmbarqueContainer embarqueContainer);

        [OperationContract]
        Retorno<int> GerarCarregamento(Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento);

        [OperationContract]
        Retorno<bool> AjustarDatasPedido(Dominio.ObjetosDeValor.WebService.Carga.AjusteDatasPedido ajusteDatasPedido);

        [OperationContract]
        Retorno<bool> CallbackIntegracaoBooking(Dominio.ObjetosDeValor.WebService.Carga.RetornoBooking retornoBooking);

        [OperationContract]
        Retorno<int> SalvarDocumentoTransporte(DocumentoTransporte documentoTransporte);

        [OperationContract]
        Retorno<int> AdicionarCargaCompleta(CargaIntegracaoCompleta cargaIntegracaoCompleta);

        [OperationContract]
        Retorno<bool> EnviarCancelamentoCarga(Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoCarga envioCancelamentoCarga);

        [OperationContract]
        Retorno<bool> EnviarCargaDocumentos(Dominio.ObjetosDeValor.WebService.Carga.CargaDocumentos cargaDocumentos);

        [OperationContract]
        Retorno<bool> ConfirmarFrete(Protocolos protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>> BuscarModelosVeicularesPendentesIntegracao(int quantidade);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoModeloVeicular(List<int> protocolos);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> ObterCargasAguardandoEnvioDocumentos(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarRecebimentoCargaAguardandoEnvioDocumentos(List<int> protocolos);

        [OperationContract]
        Retorno<List<PreDocumento>> RetornarPreDocumentosPorCarga(int protocoloCarga);

        [OperationContract]
        Retorno<int> ConsultarCargaPorNumeroCarregamento(string pedidoNumeroCarregamento);

        [OperationContract]
        Retorno<bool> SolicitarEmissaoDocumentos(int protocoloCarga);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.FilaHResponse> TotemFilaH(FilaHRequest request);

        [OperationContract]
        Retorno<bool> AlterarDataAgendamentoEntregaPorProtocoloPedido(Dominio.ObjetosDeValor.WebService.Carga.AjusteDataAgendamentoEntrega ajusteDataAgendamentoEntregaIntegracao);

        [OperationContract]
        Retorno<bool> AlterarDadosAgendamentoPedido(AjusteDadosAgendamentoPedido ajusteDadosAgendamentoPedido);

        [OperationContract]
        Retorno<bool> RetornarEtapaNota(int protocoloCarga);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.DocumentosCarga> BuscarDocumentosCarga(int? protocoloCarga, string numeroCarga);

        [OperationContract]
        Retorno<bool> IntegrarAE(IntegracaoCargaAE cargaAE);

        [OperationContract]
        Retorno<bool> AtualizarDadosPagamentoProvedor(DadosPagamentoProvedor dadosPagamentoProvedor);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCargaPorDatas(DateTime dataDe, DateTime dataAte);

        [OperationContract]
        Retorno<bool> RetornoConfirmacaoColeta(RetornoConfirmacaoColeta retornoConfirmacaoColeta);

        [OperationContract]
        Retorno<bool> InformarDadosTransportador(Dominio.ObjetosDeValor.WebService.Carga.DadosTransportador DadosTransportador);

        [OperationContract]
        Retorno<bool> SetarCargaCritica(int protocoloCarga);

        [OperationContract]
        Retorno<bool> SetarPedidoCritico(PedidoCritico pedidoCritico);

    }
}
