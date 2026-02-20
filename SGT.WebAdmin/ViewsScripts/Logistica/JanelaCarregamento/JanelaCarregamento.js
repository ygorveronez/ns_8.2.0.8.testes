/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaCarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="CapacidadeCarregamentoDados.js" />
/// <reference path="CargaPendente.js" />
/// <reference path="../JanelaCarregamentoTransportador/CargaLacre.js" />
/// <reference path="CalendarioCarregamento.js" />
/// <reference path="DetalheCarga.js" />
/// <reference path="DisponibilidadeCarregamento.js" />
/// <reference path="ListaCarregamento.js" />
/// <reference path="TabelaCarregamento.js" />
/// <reference path="DisponibilidadeVeiculo.js" />
/// <reference path="IntegracaoAVIPED.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pesquisaJanelaCarregamento;
var _centroCarregamentoAtual;
var _legendaJanelaCarregamento;
var _listaCarregamento;
var _disponibilidadeCarregamento;
var _dadosPesquisaCarregamento;
var _fluxoPorCarga;
var _geradorCor;
var _isFullScreen = false;
var _cargasCarregamento = [];

/*
 * Declaração das Classes
 */

var PesquisaJanelaCarregamento = function () {
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDeCarregamento.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, def: Global.DataAtual() });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CentroDeCarregamento.getRequiredFieldDescription(), issue: 320, idBtnSearch: guid(), required: true });
    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoCargaJanelaCarregamento.obterOpcoesPesquisaJanelaCarregamento(), text: Localization.Resources.Cargas.Carga.SituacaoDaCarga.getFieldDescription() });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDaCarga.getFieldDescription(), def: "", val: ko.observable(""), maxlength: 50 });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoPedido.getFieldDescription(), def: "", val: ko.observable(""), maxlength: 50 });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Destino.getFieldDescription(), idBtnSearch: guid() });
    this.ExibirCargaQueNaoEstaoEmInicioViagem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Cargas.Carga.ExibirApenasCargasQueNaoDeramInicioDeViagem });
    this.ExibirSomenteGradesLivres = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Cargas.Carga.ExibirSomenteGradesLivres, visible: ko.observable(false) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.ModeloVeicularDeCarga.getFieldDescription(), idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Motorista.getFieldDescription(), issue: 145, idBtnSearch: guid() });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoBooking.getFieldDescription(), maxlength: 50 });
    this.NumeroExp = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroExp.getFieldDescription(), maxlength: 150 });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Operador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_operadorLogistica.OperadorSupervisor) });
    this.Ordem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Ordem.getFieldDescription(), maxlength: 50 });
    this.PaisDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.PaisDeDestino.getFieldDescription(), idBtnSearch: guid() });
    this.PortoSaida = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeSaida.getFieldDescription(), maxlength: 150 });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Rota.getFieldDescription(), issue: 830, idBtnSearch: guid() });
    this.TipoEmbarque = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDeEmbarque.getFieldDescription(), maxlength: 150 });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Transportador.getFieldDescription(), issue: 69, idBtnSearch: guid() });
    this.UFDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.UFDestino.getFieldDescription(), idBtnSearch: guid() });
    this.UFOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.EstadoUFOrigem.getFieldDescription(), idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Veiculo.getFieldDescription(), issue: 143, idBtnSearch: guid() });
    this.SituacaoCargaJanelaCarregamento = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoCargaJanelaCarregamento.obterOpcoesPesquisa(), text: Localization.Resources.Cargas.Carga.StatusCarregamento.getFieldDescription() });
    this.SituacaoCotacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoCargaJanelaCarregamentoCotacao.obterOpcoesAprovacao(), text: Localization.Resources.Cargas.Carga.SituacaoCotacao.getFieldDescription() });
    this.SituacaoLeilao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoCotacaoPesquisa.obterOpcoesPesquisa(), text: Localization.Resources.Cargas.Carga.Situacao.getFieldDescription() });
    this.StatusRecomendacaoGR = PropertyEntity({ val: ko.observable(""), def: "", options: EnumStatusRecomendacaoGR.obterOpcoesPesquisa(), text: Localization.Resources.Cargas.Carga.StatusRecomendacaoGR.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoKlios) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            CarregarDadosPesquisa();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Cargas.Carga.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Cargas.Carga.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.LiberarCargasParaTransportadores = PropertyEntity({
        eventClick: exibirTipoTransportadorCargaPorDataCarregamento, type: types.event, text: Localization.Resources.Cargas.Carga.LiberarCargasParaTransportadores, idGrid: guid(), visible: ko.observable(false)
    });

    this.ExportarProdutosAgendados = PropertyEntity({
        eventClick: exportarProdutosAgendadosClick, type: types.event, text: Localization.Resources.Cargas.Carga.ExportarProdutosAgendados, idGrid: guid(), visible: ko.observable(true)
    });

    this.ImportarAlteracaoHorario = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Cargas.Carga.ImportarAlteracaoDeHorario,
        visible: _CONFIGURACAO_TMS.PermitirImportarAlteracaoDataCarregamentoJanelaCarregamento,
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: "JanelaCarregamento/ImportarAlteracaoHorario",
        UrlConfiguracao: "JanelaCarregamento/ConfiguracaoImportacaoAlteracaoHorario",
        CodigoControleImportacao: EnumCodigoControleImportacao.O034_AlteracaoHorarioCarregamento,
    });
};

var LegendaJanelaCarregamento = function () {
    this.AguardandoAceiteTransportador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AguardandoAceiteDoTransportador, visible: ko.observable(false), totalItens: ko.observable("") });
    this.AguardandoConfirmacaoTransportador = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.AguardandoConfimacaoDoTransportador), visible: ko.observable(true), totalItens: ko.observable("") });
    this.AguardandoEncosta = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AguardandoEncosta, visible: ko.observable(_CONFIGURACAO_TMS.UtilizarFilaCarregamento), totalItens: ko.observable("") });
    this.AguardandoLiberacaoTransportadores = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AguardandoLiberacaoParaTransportadores, visible: ko.observable(true), totalItens: ko.observable("") });
    this.Descarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Descarregamento, visible: ko.observable(false), totalItens: ko.observable("") });
    this.Faturada = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Faturada, visible: ko.observable(true), totalItens: ko.observable("") });
    this.FOB = PropertyEntity({ text: Localization.Resources.Cargas.Carga.FOB, visible: ko.observable(false), totalItens: ko.observable("") });
    this.ProntaCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ProntaParaCarregamento, visible: ko.observable(true), totalItens: ko.observable("") });
    this.SemTransportador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SemTransportador, visible: ko.observable(true), totalItens: ko.observable("") });
    this.SemValorFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SemValorDeFrete, visible: ko.observable(true), totalItens: ko.observable("") });
    this.ListaLegendaDinamica = ko.observableArray([]);
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadJanelaCarregamento() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            carregarConfiguracaoJanelaCarregamento(function (configuracaoJanelaCarregamento) {
                _pesquisaJanelaCarregamento = new PesquisaJanelaCarregamento();
                KoBindings(_pesquisaJanelaCarregamento, "knockoutPesquisaJanelaCarregamento", false, _pesquisaJanelaCarregamento.Pesquisar.id);

                LimparCampos(_pesquisaJanelaCarregamento);

                HeaderAuditoria("CargaJanelaCarregamento", _pesquisaJanelaCarregamento);

                _legendaJanelaCarregamento = new LegendaJanelaCarregamento();
                KoBindings(_legendaJanelaCarregamento, "knockoutLegendaJanelaCarregamento");
                PreencherObjetoKnoutLegenda(_legendaJanelaCarregamento, configuracaoJanelaCarregamento.Legendas);
                carregarLegendasDinamicas(configuracaoJanelaCarregamento.LegendasDinamicas);

                new BuscarCentrosCarregamento(_pesquisaJanelaCarregamento.CentroCarregamento, retornoConsultaCentroCarregamento, null, null, true);
                new BuscarClientes(_pesquisaJanelaCarregamento.Destinatario);
                new BuscarLocalidades(_pesquisaJanelaCarregamento.Destino, Localization.Resources.Cargas.Carga.BuscarCidadeDeDestino, Localization.Resources.Cargas.Carga.CidadesDeDestino);
                new BuscarMotoristas(_pesquisaJanelaCarregamento.Motorista);
                new BuscarTiposOperacao(_pesquisaJanelaCarregamento.TipoOperacao);
                new BuscarTransportadores(_pesquisaJanelaCarregamento.Transportador, null, null, true);
                new BuscarEstados(_pesquisaJanelaCarregamento.UFDestino);
                new BuscarEstados(_pesquisaJanelaCarregamento.UFOrigem);
                new BuscarVeiculos(_pesquisaJanelaCarregamento.Veiculo);
                new BuscarRotasFrete(_pesquisaJanelaCarregamento.Rota);
                new BuscarPaises(_pesquisaJanelaCarregamento.PaisDestino);
                new BuscarModelosVeicularesCarga(_pesquisaJanelaCarregamento.ModeloVeicularCarga);
                new BuscarOperador(_pesquisaJanelaCarregamento.Operador);

                $("#" + _pesquisaJanelaCarregamento.Situacao.id).selectpicker('val', configuracaoJanelaCarregamento.SituacoesPadraoPesquisa);

                _listaCarregamento = new ListaCarregamento();

                loadCapacidadeCarregamentoDados();
                loadControleSaldo();
                loadInteressadosCarga();
                loadHistoricoTransportadoresInteressadosCarga();
                loadTipoTransportadorCarga();
                LoadCargaPendente();
                LoadCargaVeiculosDisponiveis();
                loadSolicitarReagendamento();
                loadDetalhePedido();
                loadTransportadoresOfertados();
                loadObservacoes();
                loadHorarioCarregamento();
                loadHorarioCarregamentoPeriodo();
                loadLocalCarregamento();
                loadAreaVeiculo();
                loadTabClick();
                loadScrollfix();
                loadFullScreen();
                loadObservacaoGuarita();
                loadDisponibilidadeVeiculo();
                loadCotacaoHistorico();
                BuscarCentroCarregamentoPadrao();
                loadMotivoAtrasoCarregamentoAlterar();
                loadLiberacaoSemIntegracaoGR();
                loadRetornoMultiplaSelecao();
                loadRejeitarCargaTransportador();
                loadVisualizacoesCarga();
            });
        });
    });
}

function loadTabClick() {
    $("body").on("click", "#btnDisponibilidadeCarregamento", function () {
        setTimeout(function () {
            if (_disponibilidadeCarregamento != null)
                _disponibilidadeCarregamento.Render();
        }, 50);
    });

    $("body").on("click", "#btnJanelaCarregamento", function () {
        setTimeout(function () {
            _listaCarregamento.Render();
        }, 50);
    });
}

function loadScrollfix() {
    $('.scrollable, .fc-scroller').bind('mousewheel DOMMouseScroll', function (e) {
        if ($(this)[0].scrollHeight !== $(this).outerHeight()) {
            var e0 = e.originalEvent,
                delta = e0.wheelDelta || -e0.detail;

            this.scrollTop += (delta < 0 ? 1 : -1) * 30;
            e.preventDefault();
        }
    });
}

function loadFullScreen() {
    if (window.JANELA_CARREGAMENTO_FULL_SCREEN_FIX === true) return;
    window.JANELA_CARREGAMENTO_FULL_SCREEN_FIX = true;

    $('body').on('click', '#divGeralCarregamento .jarviswidget-fullscreen-btn', function (e) {
        _isFullScreen = !_isFullScreen;
        toggleFullScreen();
    });

    setFullScreenOff();
}

/*
 * Declaração das Funções Públicas
 */

function bloquearCargaCotacao(codigoJanelaCarregamento) {
    executarReST("JanelaCarregamento/BloquearCargaCotacao", { Codigo: codigoJanelaCarregamento }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaBloqueadaParaCotacaoComSucesso);
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function bloquearCargaFilaCarregamento(codigoJanelaCarregamento) {
    executarReST("JanelaCarregamento/BloquearCargaFilaCarregamento", { Codigo: codigoJanelaCarregamento }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaBloqueadaParaFilaDeCarregamentoComSucesso);
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function descartarCargaCotacao(codigoJanelaCarregamento) {
    executarReST("JanelaCarregamento/DescartarCargaCotacao", { Codigo: codigoJanelaCarregamento }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CotacaoCargaDescartadaComSucesso);
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function FluxoPorCarga() {
    // Existe dois tipos de filtros
    // Filtros padrões:
    // É a maioria dos campos, filtra as cargas na lista (direita)
    // Filtra as cargas que são exibidas no calendário
    //
    // Filtro por carga:
    // Nesse filtro, é buscado no banco A carga específica, e navegado 
    // até a mesma, mudando a data do filtro e rolando até a carga na grid
    //

    // FluxoPorCarga retorna quando o fluxo do filtro é por carga
    return _dadosPesquisaCarregamento.CodigoCargaEmbarcador != "" || _dadosPesquisaCarregamento.CodigoPedidoEmbarcador != "";
}

function liberarCargaCotacao(codigoJanelaCarregamento) {
    executarReST("JanelaCarregamento/LiberarCargaCotacao", { Codigo: codigoJanelaCarregamento }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaLiberadaParaCotacaoComSucesso);
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function liberarCargaFilaCarregamento(codigoJanelaCarregamento) {
    executarReST("JanelaCarregamento/LiberarCargaFilaCarregamento", { Codigo: codigoJanelaCarregamento }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaLiberadaParaFilaDeCarregamentoComSucesso);
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function desagendarCarga(codigoCarga) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaDesagendarCarga, function () {
        executarReST("JanelaCarregamento/DesagendarCargaFilaCarregamento", { Codigo: codigoCarga }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Data);
                    CarregarDadosPesquisa();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function exportarProdutosAgendadosClick() {
    //CodigoCarga
    if (!_listaCarregamento.IsCalendario())
        _cargasCarregamento = _listaCarregamento._getInstance()._gridTabelaCarregamento.GridViewTableData().map(function (r) {
            return r.CodigoCarga
        });

    executarDownload("JanelaCarregamento/ExportarProdutosAgendados", { Cargas: JSON.stringify(_cargasCarregamento) });
}

/*
 * Declaração das Funções Privadas
 */

function RejeitarCargaJanelaCarregamentoTransportador(carga) {
    exibirMotivoRejeicaoTransportador(carga);
}

function retornarParaNovaLiberacao(janelaCarregamentoSelecionada) {
    executarReST("JanelaCarregamento/RetornarParaNovaLiberacao", { Codigo: janelaCarregamentoSelecionada.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Carga retornada para nova liberação");
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function carregarConfiguracaoJanelaCarregamento(callback) {
    executarReST("JanelaCarregamento/ObterConfiguracao", undefined, function (retorno) {
        if (retorno.Success)
            callback(retorno.Data);
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function carregarLegendasDinamicas(legendasDinamicas) {
    for (var i = 0; i < legendasDinamicas.length; i++) {
        var legendaDinamica = legendasDinamicas[i];

        _legendaJanelaCarregamento.ListaLegendaDinamica.push({
            text: legendaDinamica.Descricao,
            cssClass: Global.ObterClasseDinamica(legendaDinamica.Cores)
        });
    }
}

function toggleFullScreen() {
    if (!_listaCarregamento.IsCalendario()) return;

    if (_isFullScreen)
        setFullScreenOn();
    else
        setFullScreenOff();
}

function setFullScreenOff() {
    var alturaPadrao = 607;
    _listaCarregamento.SetAltura(alturaPadrao);
}

function setFullScreenOn() {
    var $legenda = $("#knockoutLegendaJanelaCarregamento");
    var $conteudo = $("#divGeralCarregamento div[role=content]");

    var $capacidadePorDocas = $("#knockoutCapacidadeDocasJanelaCarregamento");
    var alturaCapacidadePorDocas = $capacidadePorDocas.is(":visible") ? $capacidadePorDocas.outerHeight() : 0;

    var $capacidadeCarregamento = $("#knockoutCapacidadeJanelaCarregamento");
    var alturaCapacidadeCarregamento = $capacidadeCarregamento.is(":visible") ? $capacidadeCarregamento.outerHeight() : 0;

    var alturaConteudo = $conteudo.outerHeight();
    var paddingConteudo = ObterPaddingsElementos($conteudo);

    var marginLegenda = ObterMargensElementos($legenda);
    var alturaLegenda = $legenda.height();

    var paddingOutrosElementos = 25;
    var alturaCalendario = ((alturaConteudo - paddingConteudo) - (alturaLegenda + marginLegenda) - alturaCapacidadePorDocas - alturaCapacidadeCarregamento - paddingOutrosElementos);

    _listaCarregamento.SetAltura(alturaCalendario);
}

function ObterPaddingsElementos($element) {
    var paddingTop = parseInt($element.css('padding-top').replace('px', '')) || 0;
    var paddingBottom = parseInt($element.css('padding-bottom').replace('px', '')) || 0;

    return paddingTop + paddingBottom;
}

function ObterMargensElementos($element) {
    var marginTop = parseInt($element.css('margin-top').replace('px', '')) || 0;
    var marginBottom = parseInt($element.css('margin-bottom').replace('px', '')) || 0;

    return marginTop + marginBottom;
}

function CarregarDadosPesquisa() {
    if (!ValidarCamposObrigatorios(_pesquisaJanelaCarregamento))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);

    _geradorCor = new GeradorCorGradeTipoOperacao();
    _pesquisaJanelaCarregamento.LiberarCargasParaTransportadores.visible(_CONFIGURACAO_TMS.PermitirInformarTipoTransportadorPorDataCarregamentoJanelaCarregamento);

    // Guarda os valores da pesquisa
    _dadosPesquisaCarregamento = RetornarObjetoPesquisa(_pesquisaJanelaCarregamento);
    _dadosPesquisaCarregamento.CodigoCargaEmbarcador = $.trim(_dadosPesquisaCarregamento.CodigoCargaEmbarcador);
    _dadosPesquisaCarregamento.CodigoPedidoEmbarcador = $.trim(_dadosPesquisaCarregamento.CodigoPedidoEmbarcador);

    if (FluxoPorCarga()) {
        executarReST("JanelaCarregamento/BuscarDataCarga", _dadosPesquisaCarregamento, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data != null) {
                    _fluxoPorCarga = retorno.Data;

                    if (_fluxoPorCarga.DataCarregamento) {
                        _dadosPesquisaCarregamento.DataCarregamento = _fluxoPorCarga.DataCarregamento;
                        //_pesquisaJanelaCarregamento.DataCarregamento.val(_fluxoPorCarga.DataCarregamento);
                    }

                    BuscaDeCarregamentos();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        BuscaDeCarregamentos();

    recarregarTotalizadoresJanelaCarregamento();
}

function BuscaDeCarregamentos() {
    executarReST("JanelaCarregamento/ObterInformacoesCentroCarregamento", _dadosPesquisaCarregamento, function (r) {
        if (r.Success) {

            _centroCarregamentoAtual = r.Data;
            _pesquisaJanelaCarregamento.ExibirFiltros.visibleFade(false);
            _legendaJanelaCarregamento.AguardandoAceiteTransportador.visible(r.Data.ExibirSituacaoAguardandoAceiteTransportador);
            _legendaJanelaCarregamento.Descarregamento.visible(r.Data.GerarJanelaCarregamentoDestino);

            RenderizarDadosCarregamento();

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function BuscarCentroCarregamentoPadrao() {
    executarReST("DadosPadrao/ObterCentroCarregamento", {}, function (r) {
        if (r.Success && r.Data) {
            _pesquisaJanelaCarregamento.CentroCarregamento.val(r.Data.Descricao);
            _pesquisaJanelaCarregamento.CentroCarregamento.codEntity(r.Data.Codigo);

            CarregarDadosPesquisa();
        }
    });
}

function RenderizarDadosCarregamento() {
    // Exibe conteúdo
    $("#divGeralCarregamento").removeClass("d-none");

    var todasAbasOcultas = !_centroCarregamentoAtual.Configuracao.JanelaCarregamentoAbaPendentes && !_centroCarregamentoAtual.Configuracao.JanelaCarregamentoAbaExcedentes && !_centroCarregamentoAtual.Configuracao.JanelaCarregamentoAbaReservas;

    if (_centroCarregamentoAtual.ExibirSomenteJanelaCarregamento || todasAbasOcultas) {
        $("#knockoutCargaPendente").css('display', 'none');
        $(".container-janela-carregamento").removeClass('col-lg-9');
    } else {
        $("#knockoutCargaPendente").css('display', 'block');
        $(".container-janela-carregamento").addClass('col-lg-9');
    }

    _listaCarregamento.Load();


    // Destroi instancia para gerar o conteúdo novo
    if (_disponibilidadeCarregamento != null)
        _disponibilidadeCarregamento.Destroy();

    // Instancia conteúdo e carrega
    _disponibilidadeCarregamento = new DisponibilidadeCarregamento();
    _disponibilidadeCarregamento.Load();

    // Carrega conteúdos
    if (!_centroCarregamentoAtual.ExibirSomenteJanelaCarregamento || todasAbasOcultas) {
        if (_centroCarregamentoAtual.Configuracao.JanelaCarregamentoAbaPendentes === true)
            RecarregarCargasPendentes();
        else
            OcultarAbaPendete();

        if (_centroCarregamentoAtual.Configuracao.JanelaCarregamentoAbaExcedentes === true)
            RecarregarCargasExcedentes();
        else
            OcultarAbaExcedentes();

        if (_centroCarregamentoAtual.Configuracao.JanelaCarregamentoAbaReservas === true)
            RecarregarCargasEmReserva();
        else
            OcultarAbaReserva();
    }

    AtivarPrimeiraAba();
    buscarCapacidadeCarregamentoDados();
}

function AtivarPrimeiraAba() {
    $(".container-carga-pendente > ul > li:visible a").first().click();
}

function OcultarAbaPendete() {
    $("a[href='#tabCargasPendentes']").parent().hide();
}

function ExibirAbaPendete() {
    $("a[href='#tabCargasPendentes']").parent().show();
}

function OcultarAbaExcedentes() {
    $("a[href='#tabCargasExcedentes']").parent().hide();
}

function ExibirAbaExcedentes() {
    $("a[href='#tabCargasExcedentes']").parent().show();
}

function OcultarAbaReserva() {
    $("a[href='#tabCargasEmReserva']").parent().hide();
}

function ExibirAbaReserva() {
    $("a[href='#tabCargasEmReserva']").parent().show();
}

function retornoConsultaCentroCarregamento(registroSelecionado) {
    _pesquisaJanelaCarregamento.CentroCarregamento.codEntity(registroSelecionado.Codigo);
    _pesquisaJanelaCarregamento.CentroCarregamento.entityDescription(registroSelecionado.Descricao);
    _pesquisaJanelaCarregamento.CentroCarregamento.val(registroSelecionado.Descricao);
    _pesquisaJanelaCarregamento.ExibirSomenteGradesLivres.visible(registroSelecionado.ExibirVisualizacaoDosTiposDeOperacao);
}

function recarregarTotalizadoresJanelaCarregamento() {
    executarReST("JanelaCarregamento/ObterTotalizadoresLegenda", RetornarObjetoPesquisa(_pesquisaJanelaCarregamento), function (retorno) {
        if (retorno.Success)
            PreencherObjetoKnoutLegendaTotalizadores(_legendaJanelaCarregamento, retorno.Data);
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            PreencherObjetoKnoutLegendaTotalizadores(_legendaJanelaCarregamento);
        }
    });
}

function ReverterSituacaoNoShowCarga(carga) {
    executarReST("JanelaCarregamento/ReverterSituacaoNoShow", { CodigoCarga: carga.Carga.Codigo }, function (retorno) {
        if (retorno.Success)
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
        else
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);

    });
}

function ConfirmarRetirada(carga) {
    executarReST("JanelaCarregamento/ConfirmarRetirada", { CodigoCarga: carga.Carga.Codigo }, function (r) {
        if (r.Success) {
            carga.Editavel = false;
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, r.Msg);
        } else
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);

    });
}