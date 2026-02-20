/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.signalR-2.4.2.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var SignalrNotificacao;
var SignalrControleSaldo;
var SignalRCargaAlterada;
var SignalROcorrenciaAlterada;
var SignalRFechamentoAlterada;
var SignalRFluxoColetaEntregaAlterada;
var SignalRAvariaAlterada;
var SignalRAjusteTabelaAtualizado;
var SignalRIntegracaoAvon;
var SignalRJanelaCarregamento;
var SignalRCargaMDFeManual;
var SignalRCargaMDFeAquaviario;
var SignalRNFSManual;
var SignalRBaixaTituloReceber;
var SignalRFatura;
var SignalRProvisao;
var SignalRCarregamentoAutomatico;
var SignalRCancelamentoProvisao;
var SignalRPagamento;
var SignalRFilaCarregamento;
var SignalRFilaCarregamentoReversa;
var SignalRManobra;
var SignalRChat;
var SignalRFluxoPatio;
var SignalRGestaoPatio;
var SignalRChamado;
var SignalRColetaEntrega;
var SignalRComprovanteEntrega;
var SignalRMontagemFeeder;
var SignalRAcompanhamentoCarga;
var SignalRMonitoramento;
var SignalRGestaoDevolucao;
var SignalRIntegracaoMercadoLivre;
var SignalRPedidos;

//*******EVENTOS*******


//criar o mapeamento de todos os metodos js que devem entrar na camada de persistencia antes de conenectar com o HUB, mudar o callback na classe desejada
var SignalRAtualizarSaldoEvent = function (retorno) { };
var SignalRNotificarUsuarioEvent = function (retorno) { };
var SignalRInformarPercentualProcessadoEvent = function (retorno) { };
var SignalRCargaAlteradaEvent = function (retorno) { };
var SignalRTransbordoCargaAtualizadaEvent = function (retorno) { };
var SignalROcorrenciaAlteradaEvent = function (retorno) { };
var SignalROcorrenciaLoteAlteradaEvent = function (retorno) { };
var SignalRCancelamentoOcorrenciaAlteradaEvent = function (retorno) { };
var SignalRCancelamentoOcorrenciaDocumentoAlteradoEvent = function (retorno) { };
var SignalRFechamentoAlteradaEvent = function (retorno) { };
var SignalRFluxoColetaEntregaAlteradaEvent = function (retorno) { };
var SignalRAvariaAlteradaEvent = function (retorno) { };
var SignalRAjusteTabelaAtualizadoEvent = function (retorno) { };
var SignalRAjusteTabelaAplicadoEvent = function (retorno) { };
var SignalRMinutaAvonAtualizadaEvent = function (retorno) { };
var SignalRMercadoLivreHUAtualizadoEvent = function (retorno) { };
var SignalRCargaQuantidadeDocumentosGerados = function (retorno) { };
var SignalRCargaQuantidadeDocumentosEmitidos = function (retorno) { };
var SignalRCargaCancelamentoAlteradoEvent = function (retorno) { };
var SignalRCargaInformarRetornoCalculoFreteEvent = function (retorno) { };
var SignalRCargaInformarMensagemAlertaEvent = function (retorno) { };
var SignalRCargaRetornoProcessamentoDocumentosFiscaisEvent = function (retorno) { };
var SignalRCargaMDFeManualAlteradoEvent = function (retorno) { };
var SignalRCargaMDFeAquaviarioAlteradoEvent = function (retorno) { };
var SignalRCargaMDFeAquaviarioAlteradoCancelamentoEvent = function (retorno) { }
var SignalRCargaMDFeManualAlteradoCancelamentoEvent = function (retorno) { };
var SignalRJanelaCarregamentoAlteradaEvent = function (retorno) { };
var SignalRNFSManualAlteradoCancelamentoEvent = function (retorno) { };
var SignalRInformarLancamentoNFSManualAtualizadaEvent = function (retorno) { };
var SignalRBaixaTituloReceberGeracaoEvent = function (retorno) { };
var SignalRBaixaTituloReceberFinalizacaoEvent = function (retorno) { };
var SignalRBaixaTituloReceberAtualizacaoEvent = function (retorno) { };
var SignalRFaturaFechamentoEvent = function (retorno) { };
var SignalRFaturaCancelamentoEvent = function (retorno) { };
var SignalRFaturaAtualizacaoEvent = function (retorno) { };
var SignalRProvisaoAtualizacaoEvent = function (retorno) { };
var SignalRProvisaoFechamentoEvent = function (retorno) { };
var SignalRCarregamentoAutomaticoAtualizacaoEvent = function (retorno) { };
var SignalRCarregamentoAutomaticoFechamentoEvent = function (retorno) { };
var SignalRCargaEmLoteAtualizacaoEvent = function (retorno) { };
var SignalRCargaEmLoteFechamentoEvent = function (retorno) { };
var SignalRCargaBackgroundFinalizadoEvent = function (retorno) { };
var SignalRFiltroPesquisaGestaoPedidoSessaoRoteirizadorEvent = function (retorno) { };
var SignalRPagamentoAtualizacaoEvent = function (retorno) { };
var SignalRPagamentoFechamentoEvent = function (retorno) { };
var SignalRCancelamentoPagamentoAtualizacaoEvent = function (retorno) { };
var SignalRCancelamentoPagamentoFechamentoEvent = function (retorno) { };
var SignalRCancelamentoProvisaoAtualizacaoEvent = function (retorno) { };
var SignalRCancelamentoProvisaoFechamentoEvent = function (retorno) { };
var SignalRFilaCarregamentoAlteradaEvent = function (retorno) { };
var SignalRFilaCarregamentoSituacaoAlteradaEvent = function (retorno) { };
var SignalRFilaCarregamentoReversaAlteradaEvent = function (retorno) { };
var SignalRChatAtualizacaoEvent = function (retorno) { };
var SignalRManobraAlteradaEvent = function (retorno) { };
var SignalRManobraTracaoAlteradaEvent = function (retorno) { };
var SignalRManobraTracaoRemovidaEvent = function (retorno) { };
var SignalRChamadoAdicionadoOuAtualizadoEvent = function (retorno) { };
var SignalRChamadoEscalarTempoExcedidoEvent = function (retorno) { };
var SignalRChamadoMensagemRecebidaChatEvent = function (retorno) { };
var SignalRChamadoMensagemEnviadaChatEvent = function (retorno) { };
var SignalRCargaDadosTransporteAtualizadoEvent = function (retorno) { };
var SignalRColetaEntregaMensagemChatEnviadaEvent = function (retorno) { };
var SignalRColetaEntregaMensagemRecebidaEvent = function (retorno) { };
var SignalRColetaEntregaAtendimentoAtualizadoEvent = function (retorno) { };
var SignalRColetaEntregaNovoAtendimentoEvent = function (retorno) { };
var SignalRFluxoPatioPesagemInicialAtualizadaEvent = function (retorno) { };
var SignalRFluxoPatioPesagemFinalAtualizadaEvent = function (retorno) { };
var SignalRGestaoPatioFluxoCargaAtualizadoEvent = function (retorno) { };
var SignalRComprovantesEntregaStatus = function (retorno) { };
var SignalRMontagemFeederAlteradoEvent = function (retorno) { };
var SignalRAcompanhamentoCargaInformarCardAtualizado = function (retorno) { };
var SignalRAcompanhamentoCargaInformarListaCardAtualizado = function (retorno) { };
var SignalRAcompanhamentoCargaAtualizarCardMensagens = function (retorno) { };
var SignalRChatNotificarMensagemUsuarioEvent = function (retorno) { };
var SignalRChatAtualizarStatusUsuarioEvent = function (retorno) { };
var SignalRCargaEncerramentoAtualizadoEvent = function (retorno) { };
var SignalRTransbordoAtualizadoEvent = function (retorno) { };
var SignalRMonitoramentoInformarListaMonitoramentoAtualizada = function (retorno) { };
var SignalRRolagemContainerAlteradoEvent = function (retorno) { };
var SignalRPedidosMensagemChatEnviadaEvent = function (retorno) { };
var SignalRPedidosMensagemRecebidaEvent = function (retorno) { }
var SignalRGestaoDevolucaoInformarGestaoDevolucaoAtualizada = function (retorno) { };
var SignalRChamadoCanceladoEvent = function (retorno) { };

function createSignalRHubConnection(url, callbacks) {
    let hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(url, { 
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
        })
        .withServerTimeout(120000)
        .withKeepAliveInterval(60000)
        .configureLogging(signalR.LogLevel.Warning)
        .build();

    for (const element of callbacks) {

        let callback = element;

        hubConnection.on(callback.method, (parameter) => {
            callback.fnCallback(parameter);
        });
    }

    function start() {
        try {
            hubConnection.start();
        } catch (err) {
            setTimeout(start, 5000);
        }
    };

    hubConnection.onclose(() => {
        start();
    });

    start();

    return hubConnection;
}

function loadSignalRhubConnections(callback) {

    Object.defineProperty(WebSocket, 'OPEN', { value: 1, });

    ////*******NOTIFICAÇÕES*******
    SignalrNotificacao = createSignalRHubConnection("/hubs/notificacao", [
        { method: "notificarUsuario", fnCallback: (r) => { SignalRNotificarUsuarioEvent(r); } },
        { method: "informarPercentualProcessado", fnCallback: (r) => { SignalRInformarPercentualProcessadoEvent(r); } }
    ]);

    ////*******CONTROLE DE SALDO*******
    SignalrControleSaldo = createSignalRHubConnection("/hubs/controleSaldo", [
        { method: "atualizarSaldo", fnCallback: (r) => { SignalRAtualizarSaldoEvent(r); } }
    ]);

    ////*******CARGA*******
    SignalRCargaAlterada = createSignalRHubConnection("/hubs/carga", [
        { method: "informarCargaAlterada", fnCallback: (r) => { SignalRCargaAlteradaEvent(r); } },
        { method: "informarQuantidadeDocumentosGerados", fnCallback: (r) => { SignalRCargaQuantidadeDocumentosGerados(r); } },
        { method: "informarQuantidadeDocumentosEmitidos", fnCallback: (r) => { SignalRCargaQuantidadeDocumentosEmitidos(r); } },
        { method: "informarTransbordoCargaAtualizada", fnCallback: (r) => { SignalRTransbordoCargaAtualizadaEvent(r); } },
        { method: "InformarCancelamentoAtualizado", fnCallback: (r) => { SignalRCargaCancelamentoAlteradoEvent(r); } },
        { method: "InformarRetornoCalculoFrete", fnCallback: (r) => { SignalRCargaInformarRetornoCalculoFreteEvent(r); } },
        { method: "InformarMensagemAlerta", fnCallback: (r) => { SignalRCargaInformarMensagemAlertaEvent(r); } },
        { method: "InformarCargaDadosTransporteIntegracaoAtualizado", fnCallback: (r) => { SignalRCargaDadosTransporteAtualizadoEvent(r); } },
        { method: "InformarEncerramentoAtualizado", fnCallback: (r) => { SignalRCargaEncerramentoAtualizadoEvent(r); } },
        { method: "informarTransbordoAtualizado", fnCallback: (r) => { SignalRTransbordoAtualizadoEvent(r); } }
    ]);


    ////*******FLUXO COLETA ENTREGA*******
    SignalRFluxoColetaEntregaAlterada = createSignalRHubConnection("/hubs/fluxoColetaEntrega", [
        { method: "informarFluxoColetaEntregaAtualizada", fnCallback: (r) => { SignalRFluxoColetaEntregaAlteradaEvent(r); } }
    ]);

    ////*******FECHAMENTO*******
    SignalRFechamentoAlterada = createSignalRHubConnection("/hubs/fechamento", [
        { method: "informarFechamentoAtualizada", fnCallback: (r) => { SignalRFechamentoAlteradaEvent(r); } }
    ]);

    ////*******OCORRENCIA*******
    SignalROcorrenciaAlterada = createSignalRHubConnection("/hubs/ocorrencia", [
        { method: "informarOcorrenciaAtualizada", fnCallback: (r) => { SignalROcorrenciaAlteradaEvent(r); } },
        { method: "informarOcorrenciaLoteAtualizada", fnCallback: (r) => { SignalROcorrenciaLoteAlteradaEvent(r); } },
        { method: "informarCancelamentoOcorrenciaAtualizada", fnCallback: (r) => { SignalRCancelamentoOcorrenciaAlteradaEvent(r); } },
        { method: "informarCancelamentoOcorrenciaDocumentoAlterado", fnCallback: (r) => { SignalRCancelamentoOcorrenciaDocumentoAlteradoEvent(r); } }
    ]);

    ////*******AVARIA*******
    SignalRAvariaAlterada = createSignalRHubConnection("/hubs/avaria", [
        { method: "informarAvariaAtualizada", fnCallback: (r) => { SignalRAvariaAlteradaEvent(r); } }
    ]);

    ////*******AJUSTE TABELA*******
    SignalRAjusteTabelaAtualizado = createSignalRHubConnection("/hubs/ajusteTabela", [
        { method: "informarAjusteTabelaAtualizado", fnCallback: (r) => { SignalRAjusteTabelaAtualizadoEvent(r); } },
        { method: "informarAjusteTabelaAplicado", fnCallback: (r) => { SignalRAjusteTabelaAplicadoEvent(r); } }
    ]);

    ////*******AVON*******
    SignalRIntegracaoAvon = createSignalRHubConnection("/hubs/integracaoAvon", [
        { method: "informarMinutaAtualizada", fnCallback: (r) => { SignalRMinutaAvonAtualizadaEvent(r); } }
    ]);

    ////*******Mercado Livre*******
    SignalRIntegracaoMercadoLivre = createSignalRHubConnection("/hubs/integracaoMercadoLivre", [
        { method: "informarHUAtualizado", fnCallback: (r) => { SignalRMercadoLivreHUAtualizadoEvent(r); } }
    ]);

    ////*******FILA DE CARREGAMENTO*******
    SignalRFilaCarregamento = createSignalRHubConnection("/hubs/filaCarregamento", [
        { method: "informarFilaAlterada", fnCallback: (r) => { SignalRFilaCarregamentoAlteradaEvent(r); } },
        { method: "informarSituacaoFilaAlterada", fnCallback: (r) => { SignalRFilaCarregamentoSituacaoAlteradaEvent(r); } }
    ]);

    ////*******FILA DE CARREGAMENTO REVERSA*******
    SignalRFilaCarregamentoReversa = createSignalRHubConnection("/hubs/filaCarregamentoReversa", [
        { method: "informarFilaCarregamentoReversaAlterada", fnCallback: (r) => { SignalRFilaCarregamentoReversaAlteradaEvent(r); } }
    ]);

    ////*******JANELA DE CARREGAMENTO*******
    SignalRJanelaCarregamento = createSignalRHubConnection("/hubs/janelaCarregamento", [
        { method: "informarJanelaCarregamentoAlterada", fnCallback: (r) => { SignalRJanelaCarregamentoAlteradaEvent(r); } }
    ]);

    ////*******MANOBRA*******
    SignalRManobra = createSignalRHubConnection("/hubs/manobra", [
        { method: "informarManobraAlterada", fnCallback: (r) => { SignalRManobraAlteradaEvent(r); } },
        { method: "informarManobraTracaoAlterada", fnCallback: (r) => { SignalRManobraTracaoAlteradaEvent(r); } },
        { method: "informarManobraTracaoRemovida", fnCallback: (r) => { SignalRManobraTracaoRemovidaEvent(r); } }
    ]);

    ////*******MDF-E MANUAL*******
    SignalRCargaMDFeManual = createSignalRHubConnection("/hubs/mdfeManual", [
        { method: "informarCargaMDFeManualAtualizado", fnCallback: (r) => { SignalRCargaMDFeManualAlteradoEvent(r); } },
        { method: "informarCargaMDFeManualAtualizadoCancelamento", fnCallback: (r) => { SignalRCargaMDFeManualAlteradoCancelamentoEvent(r); } }
    ]);

    ////*******MDF-E Aquaviario*******
    SignalRCargaMDFeAquaviario = createSignalRHubConnection("/hubs/mdfeAquaviario", [
        { method: "informarCargaMDFeAquaviarioAtualizado", fnCallback: (r) => { SignalRCargaMDFeAquaviarioAlteradoEvent(r); } },
        { method: "informarCargaMDFeAquaviarioAtualizadoCancelamento", fnCallback: (r) => { SignalRCargaMDFeAquaviarioAlteradoCancelamentoEvent(r); } }
    ]);

    ////*******NFS MANUAL*******
    SignalRNFSManual = createSignalRHubConnection("/hubs/nfsManual", [
        { method: "informarCargaMDFeManualAtualizado", fnCallback: (r) => { SignalRCargaMDFeManualAlteradoEvent(r); } },
        { method: "informarNFSManualAtualizadoCancelamento", fnCallback: (r) => { SignalRNFSManualAlteradoCancelamentoEvent(r); } },
        { method: "informarLancamentoNFSManualAtualizada", fnCallback: (r) => { SignalRInformarLancamentoNFSManualAtualizadaEvent(r); } }
    ]);

    ////*******Baixa de Títulos a Receber********  
    SignalRBaixaTituloReceber = createSignalRHubConnection("/hubs/baixaTituloReceber", [
        { method: "informarQuantidadeTitulosGerados", fnCallback: (r) => { SignalRBaixaTituloReceberGeracaoEvent(r); } },
        { method: "informarQuantidadeTitulosFinalizados", fnCallback: (r) => { SignalRBaixaTituloReceberFinalizacaoEvent(r); } },
        { method: "informarBaixaAtualizada", fnCallback: (r) => { SignalRBaixaTituloReceberAtualizacaoEvent(r); } }
    ]);

    ////*******FATURA********   
    SignalRFatura = createSignalRHubConnection("/hubs/fatura", [
        { method: "informarQuantidadeDocumentosProcessadosFechamento", fnCallback: (r) => { SignalRFaturaFechamentoEvent(r); } },
        { method: "informarQuantidadeDocumentosProcessadosCancelamento", fnCallback: (r) => { SignalRFaturaCancelamentoEvent(r); } },
        { method: "informarFaturaAtualizada", fnCallback: (r) => { SignalRFaturaAtualizacaoEvent(r); } }
    ]);

    ////*******Provisão********    
    SignalRProvisao = createSignalRHubConnection("/hubs/provisao", [
        { method: "informarProvisaoAtualizada", fnCallback: (r) => { SignalRProvisaoAtualizacaoEvent(r); } },
        { method: "informarQuantidadeDocumentosProcessadosFechamentoProvisao", fnCallback: (r) => { SignalRProvisaoFechamentoEvent(r); } }
    ]);

    ////*******CancelamentoProvisão********  
    SignalRCancelamentoProvisao = createSignalRHubConnection("/hubs/cancelamentoProvisao", [
        { method: "informarCancelamentoProvisaoAtualizada", fnCallback: (r) => { SignalRCancelamentoProvisaoAtualizacaoEvent(r); } },
        { method: "informarQuantidadeDocumentosProcessadosFechamentoCancelamentoProvisao", fnCallback: (r) => { SignalRCancelamentoProvisaoFechamentoEvent(r); } }
    ]);

    ////*******Montagem Carga********   
    SignalRCarregamentoAutomatico = createSignalRHubConnection("/hubs/montagemCarga", [
        { method: "informarQuantidadeProcessadosCarregamentoAutomatico", fnCallback: (r) => { SignalRCarregamentoAutomaticoAtualizacaoEvent(r); } },
        { method: "informarCarregamentoAutomaticoFinalizado", fnCallback: (r) => { SignalRCarregamentoAutomaticoFechamentoEvent(r); } },
        { method: "informarQuantidadeProcessadosCargaEmLote", fnCallback: (r) => { SignalRCargaEmLoteAtualizacaoEvent(r); } },
        { method: "informarCargaEmLoteFinalizado", fnCallback: (r) => { SignalRCargaEmLoteFechamentoEvent(r); } },
        { method: "informarCargaBackgroundFinalizado", fnCallback: (r) => { SignalRCargaBackgroundFinalizadoEvent(r); } },
        { method: "informarFiltroPesquisaGestaoPedidoSessaoRoteirizador", fnCallback: (r) => { SignalRFiltroPesquisaGestaoPedidoSessaoRoteirizadorEvent(r); } }
    ]);

    ////*******Pagamento********    
    SignalRPagamento = createSignalRHubConnection("/hubs/pagamento", [
        { method: "informarPagamentoAtualizada", fnCallback: (r) => { SignalRPagamentoAtualizacaoEvent(r); } },
        { method: "informarQuantidadeDocumentosProcessadosFechamentoPagamento", fnCallback: (r) => { SignalRPagamentoFechamentoEvent(r); } },
        { method: "informarCancelamentoPagamentoAtualizada", fnCallback: (r) => { SignalRCancelamentoPagamentoAtualizacaoEvent(r); } },
        { method: "informarQuantidadeDocumentosProcessadosFechamentoCancelamentoPagamento", fnCallback: (r) => { SignalRCancelamentoPagamentoFechamentoEvent(r); } }
    ]);

    ////*******Chat******** 
    SignalRChat = createSignalRHubConnection("/hubs/chat", [
        { method: "notificarMensagemUsuario", fnCallback: (r) => { SignalRChatNotificarMensagemUsuarioEvent(r); } },
        { method: "atualizarStatusUsuario", fnCallback: (r) => { SignalRChatAtualizarStatusUsuarioEvent(r); } }
    ]);

    ////*******Coleta Entrega**********
    SignalRColetaEntrega = createSignalRHubConnection("/hubs/controleColetaEntrega", [
        { method: "mensagemChatEnviada", fnCallback: (r) => { SignalRColetaEntregaMensagemChatEnviadaEvent(r); } },
        { method: "mensagemRecebida", fnCallback: (r) => { SignalRColetaEntregaMensagemRecebidaEvent(r); } },
        { method: "atendimentoAtualizado", fnCallback: (r) => { SignalRColetaEntregaAtendimentoAtualizadoEvent(r); } },
        { method: "novoAtendimento", fnCallback: (r) => { SignalRColetaEntregaNovoAtendimentoEvent(r); } }
    ]);

    ////*******Comprovante Entrega**********
    SignalRComprovanteEntrega = createSignalRHubConnection("/hubs/comprovanteEntrega", [
        { method: "informarComprovantesEntregaStatus", fnCallback: (r) => { SignalRComprovantesEntregaStatus(r); } }
    ]);

    ////*******Fluxo Pátio**********
    SignalRFluxoPatio = createSignalRHubConnection("/hubs/fluxoPatio", [
        { method: "informarPesagemInicialAtualizada", fnCallback: (r) => { SignalRFluxoPatioPesagemInicialAtualizadaEvent(r); } },
        { method: "informarPesagemFinalAtualizada", fnCallback: (r) => { SignalRFluxoPatioPesagemFinalAtualizadaEvent(r); } }
    ]);

    ////*******Gestão Pátio**********
    SignalRGestaoPatio = createSignalRHubConnection("/hubs/gestaoPatio", [
        { method: "fluxoCargaAtualizado", fnCallback: (r) => { SignalRGestaoPatioFluxoCargaAtualizadoEvent(r); } }
    ]);
    
    ////*******CHAMADO*******
    SignalRChamado = createSignalRHubConnection("/hubs/chamado", [
        { method: "informarChamadoAdicionadoOuAlterado", fnCallback: (r) => { SignalRChamadoAdicionadoOuAtualizadoEvent(r); } },
        { method: "escalarTempoExcedidoChamado", fnCallback: (r) => { SignalRChamadoEscalarTempoExcedidoEvent(r); } },
        { method: "chamadoMensagemRecebida", fnCallback: (r) => { SignalRChamadoMensagemRecebidaChatEvent(r); } },
        { method: "chamadoMensagemEnviada", fnCallback: (r) => { SignalRChamadoMensagemEnviadaChatEvent(r); } },
        { method: "informarChamadoCancelado", fnCallback: (r) => { SignalRChamadoCanceladoEvent(r); } }
    ]);


    ////*******MONTEGEM CARGA FEEDER*******
    SignalRMontagemFeeder = createSignalRHubConnection("/hubs/montagemFeeder", [
        { method: "informarMontagemFeederAtualizado", fnCallback: (r) => { SignalRMontagemFeederAlteradoEvent(r); } }
    ]);

    ////*******ACOMPANHAMENTO CARGA*******
    SignalRAcompanhamentoCarga = createSignalRHubConnection("/hubs/acompanhamentoCarga", [
        { method: "informarCardAtualizado", fnCallback: (r) => { SignalRAcompanhamentoCargaInformarCardAtualizado(r); } },
        { method: "informarListaCardAtualizado", fnCallback: (r) => { SignalRAcompanhamentoCargaInformarListaCardAtualizado(r); } },
        { method: "atualizarCardMensagens", fnCallback: (r) => { SignalRAcompanhamentoCargaAtualizarCardMensagens(r); } },
        { method: "InformarRetornoProcessamentoDocumentosFiscais", fnCallback: (r) => { SignalRCargaRetornoProcessamentoDocumentosFiscaisEvent(r); } }
    ]);

    ////********* PEDIDOS ***********
    SignalRPedidos = createSignalRHubConnection("/hubs/pedidos", [
        { method: "mensagemChatEnviada", fnCallback: (r) => { SignalRPedidosMensagemChatEnviadaEvent(r); } },
        { method: "mensagemRecebida", fnCallback: (r) => { SignalRPedidosMensagemRecebidaEvent(r); } }
    ]);

    ////******* MONITORAMENTO *******
    SignalRMonitoramento = createSignalRHubConnection("/hubs/monitoramento", [
        { method: "informarListaMonitoramentoAtualizada", fnCallback: (r) => { SignalRMonitoramentoInformarListaMonitoramentoAtualizada(r); } }
    ]);

    ////******* GESTÃO DEVOLUÇÃO *******
    SignalRGestaoDevolucao = createSignalRHubConnection("/hubs/gestaoDevolucao", [
        { method: "informarGestaoDevolucaoAtualizada", fnCallback: (r) => { SignalRGestaoDevolucaoInformarGestaoDevolucaoAtualizada(r); } }
    ]);

    callback();
}