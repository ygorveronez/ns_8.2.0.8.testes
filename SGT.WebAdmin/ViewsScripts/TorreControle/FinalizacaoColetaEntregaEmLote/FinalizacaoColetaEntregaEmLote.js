/// <reference path="../../Logistica/Tracking/Tracking.lib.js" />

const MODO_PROCESSAMENTO = 1;
const MODO_FINALIZACOES = 2;

var _pesquisaFinalizacaoColetaEntregaEmLote;
var _finalizacaoColetaEntregaEmLote;
var _processamentoFinalizacaoColetaEntregaEmLote;
var _gridFinalizacaoColetaEntregaEmLote;
var _gridFinalizacaoColetaEntregaEmLoteProcessamento;
var multiplaescolha = {};
var _cabecalhoFinalizacaoColetaEmLote;
var _numeroRegistrosSelecionados;

var PesquisaFinalizacaoColetaEntregaEmLote = function () {
    this.ModoDeUso = PropertyEntity({ val: ko.observable(MODO_PROCESSAMENTO) });
    this.DataInicioEmissao = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.DataInicioEmissao.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), required: false, val: ko.observable(Global.DataHoraAtual()) });
    this.DataFinalEmissao = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.DataFinalEmissao.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), required: false });
    this.DataInicioCriacaoCarga = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.DataInicioCriacaoCarga.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), required: false, val: ko.observable("") });
    this.DataFinalCriacaoCarga = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.DataFinalCriacaoCarga.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), required: false });
    this.DataPrevisaoEntregaPedidoInicial = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.DataPrevisaoEntregaPedidoInicial.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), required: false });
    this.DataPrevisaoEntregaPedidoFinal = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.DataPrevisaoEntregaPedidoFinal.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), required: false });
    this.DataInicioViagemInicial = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.DataInicioViagemInicial.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), required: false });
    this.DataInicioViagemFinal = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.DataInicioViagemFinal.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), required: false });
    this.DataInicioProcessamento = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.DataInicioProcessamento.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), required: false });
    this.DataFinalProcessamento = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.DataFinalProcessamento.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), required: false });


    this.DataInicioEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicioEmissao;

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.TorreControle.FinalizacaoEmLote.Empresa.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.TorreControle.FinalizacaoEmLote.Filial.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.TorreControle.FinalizacaoEmLote.Tomador.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Origens = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: false, text: Localization.Resources.TorreControle.FinalizacaoEmLote.Origens.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.TorreControle.FinalizacaoEmLote.Destino.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TiposOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: false, text: Localization.Resources.TorreControle.FinalizacaoEmLote.TiposOperacao.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.TorreControle.FinalizacaoEmLote.Veiculo.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Placa = PropertyEntity({ type: types.string, codEntity: ko.observable(0), text: Localization.Resources.TorreControle.FinalizacaoEmLote.Placa.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.TorreControle.FinalizacaoEmLote.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.TorreControle.FinalizacaoEmLote.Destinatario.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.TorreControle.FinalizacaoEmLote.Remetente.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.TorreControle.FinalizacaoEmLote.NumeroCarga.getFieldDescription(), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.SituacaoCarga = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.SituacaoCarga.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(EnumSituacoesCarga.Todos), options: EnumSituacoesCarga.obterOpcoesEmbarcador(), def: EnumSituacoesCarga.Todos });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.TorreControle.FinalizacaoEmLote.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoProcessamento = PropertyEntity({ type: types.map, required: false, val: ko.observable(EnumSituacaoProcessamentoFinalizacaoColetaEntregaEmLote.Todos), options: EnumSituacaoProcessamentoFinalizacaoColetaEntregaEmLote.obterOpcoes(), def: EnumSituacaoProcessamentoFinalizacaoColetaEntregaEmLote.Todos, text: Localization.Resources.TorreControle.FinalizacaoEmLote.SituacaoProcessamento.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(true) });
    this.MonitoramentoStatus = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), def: new Array(), options: EnumMonitoramentoStatus.obterOpcoes(), text: Localization.Resources.TorreControle.FinalizacaoEmLote.SituacaoMonitoramento.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(true) });
    this.StatusViagem = PropertyEntity({ text: Localization.Resources.TorreControle.FinalizacaoEmLote.SituacaoControleDeEntrega.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), def: new Array(), options: EnumStatusViagemControleEntrega.obterOpcoes() });

    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            controlarVisibilidadeGridsFinalizacaoEmLote();
            $('.aviso-filtragem-dados').hide();
            $('#carousel-filters').show();
            loadGridProcessamento();
            loadGridFinalizacaoColetaEntregaEmLote();
            loadCarrosselProcessamento();
            Global.fecharModal('divModalFiltroPesquisaFinalizacaoLote');
            $("#quantidade-registros-selecionados").hide();
        }, text: Localization.Resources.TorreControle.FinalizacaoEmLote.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            LimparCampos(_pesquisaFinalizacaoColetaEntregaEmLote);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.LimparFiltros, idGrid: guid(), visible: ko.observable(true)
    });
};

var CabecalhoFinalizacaoColetaEmLote = function () {
    this.Finalizado = PropertyEntity({ val: ko.observable(0), eventClick: function (e) { onclickFiltroCarrossel("Finalizado") } });
    this.Pendente = PropertyEntity({ val: ko.observable(0), eventClick: function (e) { onclickFiltroCarrossel("Pendente") } });
    this.Falha = PropertyEntity({ val: ko.observable(0), eventClick: function (e) { onclickFiltroCarrossel("Falha") } });
}

var FinalizacaoColetaEntregaEmLote = function () {
    this.Grid = PropertyEntity({ idGrid: "grid-finalizacao-coleta-entrega-lote", enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(true), def: true, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.TorreControle.FinalizacaoEmLote.MarcarDesmarcarPrimeiros100.getFieldDescription(), eventClick: callbackSelecionarTodos, visible: ko.observable(true) });
};

var NumeroRegistrosSelecionados = function () {
    this.NumeroRegistrosSelecionados = PropertyEntity({ val: ko.observable(0) });
    this.ConfirmarMultiplasColetasEntregas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarMultiplasColetasEntregasClick, text: Localization.Resources.TorreControle.FinalizacaoEmLote.ConfirmarMultiplasColetas.getFieldDescription(), visible: ko.observable(false) });

}

var FinalizacaoColetaEntregaEmLoteProcessamento = function () {
    this.Grid = PropertyEntity({ idGrid: "grid-finalizacao-coleta-entrega-lote-processamento", enable: ko.observable(true) });
};
function loadFinalizacaoColetaEntregaEmLote() {

    _pesquisaFinalizacaoColetaEntregaEmLote = new PesquisaFinalizacaoColetaEntregaEmLote();
    KoBindings(_pesquisaFinalizacaoColetaEntregaEmLote, "knockoutPesquisaConsultaPorNotaFiscal");

    _finalizacaoColetaEntregaEmLote = new FinalizacaoColetaEntregaEmLote();
    KoBindings(_finalizacaoColetaEntregaEmLote, "knockoutfinalizacaoColetaEntregaEmLote");

    _processamentoFinalizacaoColetaEntregaEmLote = new FinalizacaoColetaEntregaEmLoteProcessamento();
    KoBindings(_processamentoFinalizacaoColetaEntregaEmLote, "knockoutfinalizacaoColetaEntregaEmLoteProcessamento");

    _cabecalhoFinalizacaoColetaEmLote = new CabecalhoFinalizacaoColetaEmLote();
    KoBindings(_cabecalhoFinalizacaoColetaEmLote, "knockoutCabecalhoFinalizacaoColetaEmLote");

    _numeroRegistrosSelecionados = new NumeroRegistrosSelecionados();
    KoBindings(_numeroRegistrosSelecionados, "knockoutNumeroRegistrosSelecionados");

    $('.aviso-filtragem-dados').show();

    BuscarEmpresa(_pesquisaFinalizacaoColetaEntregaEmLote.Empresa);
    BuscarTomadoresCarga(_pesquisaFinalizacaoColetaEntregaEmLote.Tomador);
    BuscarLocalidades(_pesquisaFinalizacaoColetaEntregaEmLote.Origens);
    BuscarLocalidades(_pesquisaFinalizacaoColetaEntregaEmLote.Destino);
    BuscarTiposOperacao(_pesquisaFinalizacaoColetaEntregaEmLote.TiposOperacao);
    BuscarVeiculos(_pesquisaFinalizacaoColetaEntregaEmLote.Veiculo);
    BuscarMotoristas(_pesquisaFinalizacaoColetaEntregaEmLote.Motorista);
    BuscarClientes(_pesquisaFinalizacaoColetaEntregaEmLote.Destinatario);
    BuscarClientes(_pesquisaFinalizacaoColetaEntregaEmLote.Remetente);
    BuscarCargas(_pesquisaFinalizacaoColetaEntregaEmLote.NumeroCarga);
    BuscarFilial(_pesquisaFinalizacaoColetaEntregaEmLote.Filial);
    BuscarTransportadores(_pesquisaFinalizacaoColetaEntregaEmLote.Transportador)
}

function loadGridFinalizacaoColetaEntregaEmLote() {

    var somenteLeitura = false;
    _finalizacaoColetaEntregaEmLote.SelecionarTodos.visible(true);
    _finalizacaoColetaEntregaEmLote.SelecionarTodos.val(false);

    multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: somenteLeitura,
        callbackNaoSelecionado: callbackNaoSelecionado,
        callbackSelecionado: callbackSelecionado,
        classeSelecao: "registro-selecionado"
    };

    _gridFinalizacaoColetaEntregaEmLote = new GridView(_finalizacaoColetaEntregaEmLote.Grid.idGrid, "FinalizacaoColetaEntregaEmLote/Pesquisa", _pesquisaFinalizacaoColetaEntregaEmLote, null, null, 100, undefined, undefined, undefined, multiplaescolha, 100, undefined, undefined, undefined, undefined, undefined, adicionarIconeRastreador);
    _gridFinalizacaoColetaEntregaEmLote.SetCallbackDrawGridView(RecarregarComRegistrosSelecionados);
    _gridFinalizacaoColetaEntregaEmLote.SetPermitirEdicaoColunas(true);
    _gridFinalizacaoColetaEntregaEmLote.SetSalvarPreferenciasGrid(true);
    _gridFinalizacaoColetaEntregaEmLote.CarregarGrid();
}

function loadGridProcessamento() {
    var somenteLeitura = true;

    _gridFinalizacaoColetaEntregaEmLoteProcessamento = new GridView(_processamentoFinalizacaoColetaEntregaEmLote.Grid.idGrid, "FinalizacaoColetaEntregaEmLote/PesquisaProcessamento", _pesquisaFinalizacaoColetaEntregaEmLote, null, null, 10, null, null, null, null);
    _gridFinalizacaoColetaEntregaEmLoteProcessamento.SetPermitirEdicaoColunas(false);
    _gridFinalizacaoColetaEntregaEmLoteProcessamento.SetSalvarPreferenciasGrid(false);
    _gridFinalizacaoColetaEntregaEmLoteProcessamento.CarregarGrid();
}
function RecarregarComRegistrosSelecionados() {
    if (!multiplaescolha.selecionados) return;

    const dadosGrid = _gridFinalizacaoColetaEntregaEmLote.GridViewTableData();
    const selecionados = multiplaescolha.selecionados;

    dadosGrid.forEach(registro => {
        const estaSelecionado = selecionados.some(sel => sel.DT_RowId === registro.DT_RowId);
        const row = $('#' + registro.DT_RowId);

        if (estaSelecionado) {
            row.addClass('registro-selecionado');
        } else {
            row.removeClass('registro-selecionado');
        }
    });

    atualizarExibicaoSelecionados();
}

function ExibirFiltros() {
    Global.abrirModal('divModalFiltroPesquisaFinalizacaoLote');
}
function loadCarrosselProcessamento() {
    const data = RetornarObjetoPesquisa(_cabecalhoFinalizacaoColetaEmLote);

    executarReST("FinalizacaoColetaEntregaEmLote/ObterDadosFiltroCarrosselFinalizacaoColetaEmLote", data, function (retorno) {
        if (retorno.Success && retorno.Data !== false) {
            const situacoes = {};
            retorno.Data.forEach(item => {
                situacoes[item.Situacao] = (situacoes[item.Situacao] || 0) + 1;
            });

            _cabecalhoFinalizacaoColetaEmLote.Finalizado.val(situacoes[2] || 0);
            _cabecalhoFinalizacaoColetaEmLote.Pendente.val(situacoes[0] || 0);
            _cabecalhoFinalizacaoColetaEmLote.Falha.val(situacoes[3] || 0);
        }
    });
}

function mostrarGridProcessamento() {
    $('#grid-finalizacao').removeClass('active');
    $('#grid-processamento').addClass('active');
    $('#texto-botao-visualizao-grids').text('Processamento');
    if (_gridFinalizacaoColetaEntregaEmLoteProcessamento)
        controlarVisibilidadeGridsFinalizacaoEmLote();

    _pesquisaFinalizacaoColetaEntregaEmLote.ModoDeUso.val(MODO_PROCESSAMENTO);
}

function mostrarGridFinalizacoes() {
    if (_pesquisaFinalizacaoColetaEntregaEmLote.ModoDeUso.val() !== MODO_FINALIZACOES) {
        $('#grid-processamento').removeClass('active');
        $('#grid-finalizacao').addClass('active');
        $('#texto-botao-visualizao-grids').text('Finalizações');
        if (_gridFinalizacaoColetaEntregaEmLote)
            controlarVisibilidadeGridsFinalizacaoEmLote();

        _pesquisaFinalizacaoColetaEntregaEmLote.ModoDeUso.val(MODO_FINALIZACOES);
    } else return;
}
function controlarVisibilidadeGridsFinalizacaoEmLote() {
    if ($('#grid-processamento').hasClass('active')) {
        $('#grid-processamento').show();
        $('#grid-finalizacao').hide();
        $("#aviso-selecionar-registro").hide();
        $("#quantidade-registros-selecionados").hide();
        $('#knockoutCabecalhoFinalizacaoColetaEmLote').show();
    } else if ($('#grid-finalizacao').hasClass('active')) {
        $('#grid-finalizacao').show();
        $('#grid-processamento').hide();
        $('#knockoutCabecalhoFinalizacaoColetaEmLote').hide();
        $('#aviso-selecionar-registro').show();
    }
    $('#container-principal').hide();
}

function callbackSelecionado(argumentoNulo, registroSelecionado) {
    const selecionarTodos = _finalizacaoColetaEntregaEmLote.SelecionarTodos.val();
    const dadosGrid = _gridFinalizacaoColetaEntregaEmLote.GridViewTableData();
    const limite = 100;

    if (registroSelecionado) {
        const jaSelecionado = multiplaescolha.selecionados.some(item => item.DT_RowId === registroSelecionado.DT_RowId);
        if (!jaSelecionado) {
            multiplaescolha.selecionados.push(registroSelecionado);
        }
    } else if (selecionarTodos) {
        multiplaescolha.selecionados = dadosGrid.slice(0, limite);
        multiplaescolha.selecionados.forEach(registro => {
            $('#' + registro.DT_RowId).addClass('registro-selecionado');
        });
    } else {
        multiplaescolha.callbackNaoSelecionado();
    }

    atualizarExibicaoSelecionados();
}

function callbackNaoSelecionado(argumentoNulo, registroClicado) {
    if (registroClicado) {
        multiplaescolha.selecionados = multiplaescolha.selecionados.filter(item => item.DT_RowId !== registroClicado.DT_RowId);
    } else {
        multiplaescolha.selecionados = [];
    }

    atualizarExibicaoSelecionados();
}

function callbackSelecionarTodos() {
    const dadosGrid = _gridFinalizacaoColetaEntregaEmLote.GridViewTableData();
    const limiteRegistro = 100;

    const selecionarTodos = !_finalizacaoColetaEntregaEmLote.SelecionarTodos.val();
    _finalizacaoColetaEntregaEmLote.SelecionarTodos.val(selecionarTodos);

    if (selecionarTodos) {
        multiplaescolha.selecionados = dadosGrid.slice(0, limiteRegistro);
        dadosGrid.forEach(registro => {
            $('#' + registro.DT_RowId).removeClass('registro-selecionado');
        });
        multiplaescolha.selecionados.forEach(registro => {
            $('#' + registro.DT_RowId).addClass('registro-selecionado');
        });
    } else {
        multiplaescolha.selecionados = [];
        dadosGrid.forEach(registro => {
            $('#' + registro.DT_RowId).removeClass('registro-selecionado');
        });
    }

    atualizarExibicaoSelecionados();
}
 
function atualizarExibicaoSelecionados() {
    const temSelecionados = multiplaescolha.selecionados.length > 0 && multiplaescolha.selecionados.length !== undefined;
    _numeroRegistrosSelecionados.ConfirmarMultiplasColetasEntregas.visible(temSelecionados);

    if (temSelecionados) {
        $("#aviso-selecionar-registro").hide();
        $("#quantidade-registros-selecionados").show();
    } else if (_pesquisaFinalizacaoColetaEntregaEmLote.ModoDeUso.val() === MODO_FINALIZACOES) {
        $("#aviso-selecionar-registro").show();
        $("#quantidade-registros-selecionados").hide();
    }

    _numeroRegistrosSelecionados.NumeroRegistrosSelecionados.val(multiplaescolha.selecionados.length);
}

function confirmarMultiplasColetasEntregasClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.TorreControle.FinalizacaoEmLote.ConfirmarFinalizarColetasEntregasSelecionadas, function () {
        var dados = RetornarObjetoPesquisa(_finalizacaoColetaEntregaEmLote);
        dados.SelecionarTodos = _finalizacaoColetaEntregaEmLote.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(multiplaescolha.selecionados);
        dados.ItensNaoSelecionados = JSON.stringify(_gridFinalizacaoColetaEntregaEmLote.ObterMultiplosNaoSelecionados());

        var numeroItensSelecionados = multiplaescolha.selecionados.length;

        if (numeroItensSelecionados > 100) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.TorreControle.FinalizacaoEmLote.SelecionarCemRegistrosVez);
            return;
        }

        if (numeroItensSelecionados == 0) {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.TorreControle.FinalizacaoEmLote.SelecionarUmRegistro);
            return;
        }

        executarReST("FinalizacaoColetaEntregaEmLote/FinalizarColetaEntregaEmMassa", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.TorreControle.FinalizacaoEmLote.ColetasEntregasFinalizadas);
                    _gridFinalizacaoColetaEntregaEmLote.CarregarGrid();
                    loadGridFinalizacaoColetaEntregaEmLote();
                    loadGridProcessamento();
                    $("#aviso-selecionar-registro").show();
                    $("#quantidade-registros-selecionados").hide();
                }
                else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        })
    });
}
function onclickFiltroCarrossel(tipoFiltro) {
    if (tipoFiltro == "Finalizado")
        _pesquisaFinalizacaoColetaEntregaEmLote.SituacaoProcessamento.val(EnumSituacaoProcessamentoFinalizacaoColetaEntregaEmLote.Finalizado)

    if (tipoFiltro == "Pendente")
        _pesquisaFinalizacaoColetaEntregaEmLote.SituacaoProcessamento.val(EnumSituacaoProcessamentoFinalizacaoColetaEntregaEmLote.PendenteFinalizacao)

    else if (tipoFiltro == "Falha")
        _pesquisaFinalizacaoColetaEntregaEmLote.SituacaoProcessamento.val(EnumSituacaoProcessamentoFinalizacaoColetaEntregaEmLote.FalhaNaFinalizacao)

    loadGridProcessamento();
}
function adicionarIconeRastreador(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name == "Rastreador") {
        return obterIconeHtmlColunaRastreadorFinalizacao(dadosLinha);
    }
}
function obterIconeHtmlColunaRastreadorFinalizacao(dadosLinha) {
    var icone = ObterIconeStatusTracking(parseInt(dadosLinha.Rastreador), 20);

    return '<div class="tracking-indicador">' + icone + '</div>';
}