/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Enumeradores/EnumTipoCapacidadeDescarregamentoPorPeso.js" />
/// <reference path="Email.js" />
/// <reference path="LimiteAgendamento.js" />
/// <reference path="PeriodoDescarregamento.js" />
/// <reference path="QuantidadePorTipoDeCarga.js" />
/// <reference path="TempoDescarregamento.js" />
/// <reference path="TipoCarga.js" />
/// <reference path="Transportador.js" />

// #region Objetos Globais do Arquivo

var _gridCentroDescarregamento;
var _centroDescarregamento;
var _pesquisaCentroDescarregamento;
var _crudCentroDescarregamento;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaCentroDescarregamento = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoCarga.getFieldDescription(), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCentroDescarregamento.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CentroDescarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Dia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mes = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiasNoMes = PropertyEntity({ val: ko.observableArray([1, 2, 3, 4, 5]) });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), required: true, maxlength: 150 });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), maxlength: 400 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid(), issue: 70, visible: ko.observable(true), required: false });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), idBtnSearch: guid(), issue: 70, visible: ko.observable(true), required: false });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Canal Entrega", idBtnSearch: guid(), visible: ko.observable(true), required: false });
    this.NumeroDocas = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.LimitePadrao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoTransportadorCentroDescarregamento = PropertyEntity({ val: ko.observable(EnumTipoTransportadorCentroDescarregamento.Todos), def: EnumTipoTransportadorCentroDescarregamento.Todos });
    this.LiberarCargaAutomaticamenteParaTransportadoras = PropertyEntity({ val: ko.observable(true), def: true });
    this.BloquearJanelaDescarregamentoExcedente = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroDescarregamento.BloqueaJanelaDescarregamentoExcedente });
    this.PermitirBuscarAteFimDaJanela = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroDescarregamento.PermitirBuscarHorarioFimJanela, visible: ko.observable(true) });
    this.UtilizarCapacidadeDescarregamentoPorPeso = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.CapacidadeDescarregamentoPesoLiquido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroDescarregamento.UtilizarPesoLiquido, visible: ko.observable(false) });
    this.TipoCapacidadeDescarregamentoPorPeso = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.CapacidadeDescarregamentoPeso, val: ko.observable(EnumTipoCapacidadeDescarregamentoPorPeso.Todos), def: EnumTipoCapacidadeDescarregamentoPorPeso.Todos, options: EnumTipoCapacidadeDescarregamentoPorPeso.obterOpcoes(), enable: ko.observable(true) });
    this.PercentualToleranciaPesoDescarregamento = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, text: Localization.Resources.Logistica.CentroDescarregamento.PercentualToleranciaPesoDescarregamento.getFieldDescription(), visible: ko.observable(true), maxlength: 5, configDecimal: { allowZero: true } });
    this.ExibirJanelaDescargaPorPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroDescarregamento.ExibirJanelaDescargaPedido });
    this.PermitirGeracaoJanelaParaCargaRedespacho = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroDescarregamento.PermitirGeracaoJanelaParaCargaRedespacho, def: false, visible: _configuracaoCentroDescarregamento.BloquearGeracaoJanelaParaCargaRedespacho });
    this.CapacidadeDescaregamentoPorDia = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroDescarregamento.CapacidadeDescarregamentoDia, def: false, visible: ko.observable(true) });
    this.AprovarAutomaticamenteDescargaComHorarioDisponivel = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroDescarregamento.AprovarAutomaticamenteDescargaComHorarioDisponivel });
    this.TempoPadraoDeEntrega = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.TempoPadraoDeEntrega, type: types.local, getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoPadraoSugestaoHorario = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.TempoPadraoSugestaoHorario, getType: typesKnockout.int, visible: _configuracaoCentroDescarregamento.SugerirDataEntregaAgendamentoColeta, maxlength: 5 });
    this.BuscarSenhaViaIntegracao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroDescarregamento.BuscarSenhaIntegracao, def: false, visible: ko.observable(_configuracaoCentroDescarregamento.PossuiIntegracaoSAD) });
    this.DefinirNaoComparecimentoAutomaticoAposPrazoDataAgendada = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.DefinirSituacaoComoNaoComparecimentoAutomaticamenteAposTerminoDataAgendamento, getType: typesKnockout.bool, val: ko.observable(false) });
    this.UsarLayoutAgendamentoPorCaixaItem = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.UsarLayoutAgendamentoPorCaixaItem, getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirGerarDescargaArmazemExterno = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.PermitirGerarDescargaArmazemExterno, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigeAprovacaoCargaParaDescarregamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroDescarregamento.ExigeAprovacaoCargaParaDescarregamento });
    this.GerarFluxoPatioAposConfirmacaoAgendamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroDescarregamento.GerarFluxoPatioAposConfirmacaoAgendamento });

    this.AprovarAutomaticamenteDescargaComHorarioDisponivel.val.subscribe(function (valor) {
        if (valor)
            $("#liTabAprovacaoAutomatica").show();
        else
            $("#liTabAprovacaoAutomatica").hide();
    });

    this.TiposCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: new Array(), idGrid: guid() });
    this.VeiculosPermitidos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Transportadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Emails = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.TemposDescarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.QuantidadePorTipoDeCarga = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.LimiteAgendamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.PeriodosDescarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.PrevisoesDescarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.LimitesDescarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.LimitesDescarregamento });
    this.ExcecoesCapacidadeDescarregamento = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });
    this.HorariosAprovacaoAutomatica = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0) });

    this.CapacidadeDescarregamentoSegunda = PropertyEntity({ visible: ko.observable(false) });
    this.CapacidadeDescarregamentoTerca = PropertyEntity({ visible: ko.observable(false) });
    this.CapacidadeDescarregamentoQuarta = PropertyEntity({ visible: ko.observable(false) });
    this.CapacidadeDescarregamentoQuinta = PropertyEntity({ visible: ko.observable(false) });
    this.CapacidadeDescarregamentoSexta = PropertyEntity({ visible: ko.observable(false) });
    this.CapacidadeDescarregamentoSabado = PropertyEntity({ visible: ko.observable(false) });
    this.CapacidadeDescarregamentoDomingo = PropertyEntity({ visible: ko.observable(false) });
    this.CapacidadeDescarregamento = PropertyEntity({ visible: ko.observable(false) });

    this.TipoCapacidadeDescarregamentoPorPeso.val.subscribe(controlarVisibilidadeCamposCapacidadeDescarregamentoPorPeso);

    this.NumeroDocas.val.subscribe(function (novoValor) {
        _capacidadeDescarregamento.NumeroDocas.val(novoValor);
    });

    this.TipoTransportadorCentroDescarregamento.val.subscribe(function (novoValor) {
        _transportador.TipoTransportadorCentroDescarregamento.val(novoValor);
    });

    this.LiberarCargaAutomaticamenteParaTransportadoras.val.subscribe(function (novoValor) {
        _transportador.LiberarCargaAutomaticamenteParaTransportadoras.val(novoValor);
    });

    this.CapacidadeDescaregamentoPorDia.val.subscribe(function (novoValor) {
        if (novoValor) {
            $("#liTabCapacidadeDescarregamento").hide();
            $("#liTabCapacidadeDescarregamentoPorDiaMes").show();
            _centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso.val(EnumTipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento);
            _centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso.enable(false);
        }
        else {
            $("#liTabCapacidadeDescarregamento").show();
            $("#liTabCapacidadeDescarregamentoPorDiaMes").hide();
            _centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso.enable(true);

        }
    });
}

var CRUDCentroDescarregamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: "CentroDescarregamento/Importar",
        UrlConfiguracao: "CentroDescarregamento/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O024_CentroDescarregamento,
        CallbackImportacao: function () {

        }
    });
}
var CentroDescarregamentoDiasNoMes = function () {
    this.Dia = PropertyEntity({ val: ko.observable(initDia()), getType: typesKnockout.int });
    this.Mes = PropertyEntity({ val: ko.observable(initMes()), getType: typesKnockout.int });
    this.DiasNoMes = PropertyEntity({ val: ko.observableArray(initDiasNoMes()) });
}


// #endregion Classes

// #region Funções de Inicialização

function loadCentroDescarregamento() {
    _centroDescarregamento = new CentroDescarregamento();
    KoBindings(_centroDescarregamento, "knockoutDetalhes");

    _centroDescarregamentoDiasNoMes = new CentroDescarregamentoDiasNoMes();
    KoBindings(_centroDescarregamentoDiasNoMes, "knoutCapacidadeDescarregamentoMesDia");

    HeaderAuditoria("CentroDescarregamento", _centroDescarregamento);

    _pesquisaCentroDescarregamento = new PesquisaCentroDescarregamento();
    KoBindings(_pesquisaCentroDescarregamento, "knockoutPesquisaCentroDescarregamento", false, _pesquisaCentroDescarregamento.Pesquisar.id);

    _crudCentroDescarregamento = new CRUDCentroDescarregamento();
    KoBindings(_crudCentroDescarregamento, "knockoutCRUDCentroDescarregamento");

    new BuscarCanaisEntrega(_centroDescarregamento.CanalEntrega);
    new BuscarClientes(_centroDescarregamento.Destinatario);
    new BuscarFilial(_centroDescarregamento.Filial);
    new BuscarClientes(_pesquisaCentroDescarregamento.Destinatario);
    new BuscarTiposdeCarga(_pesquisaCentroDescarregamento.TipoCarga);

    $("#" + _centroDescarregamento.UtilizarCapacidadeDescarregamentoPorPeso.id).click(controlarCampoTipoCapacidadeDescarregamentoPorPesoHabilitado);

    loadGridCentroDescarregamentos();
    LoadTipoCarga();
    LoadVeiculoPermitido();
    loadQuantidadePorTipoDeCarga();
    loadLimiteAgendamento();
    LoadTempoDescarregamento();
    LoadCapacidadeDescarregamento();
    LoadImportacaoPeriodo();
    LoadImportacaoPrevisao();
    LoadTransportador();
    LoadEmail();
    loadAprovacaoAutomaticaCentroDescarregamento();
}

function loadGridCentroDescarregamentos() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: function (centroDescarregamentoGrid) { editarCentroDescarregamento(centroDescarregamentoGrid, false); }, tamanho: "15", icone: "" };
    var duplicar = { descricao: Localization.Resources.Gerais.Geral.Duplicar, id: guid(), metodo: function (centroDescarregamentoGrid) { editarCentroDescarregamento(centroDescarregamentoGrid, true); }, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [editar, duplicar] };

    _gridCentroDescarregamento = new GridView(_pesquisaCentroDescarregamento.Pesquisar.idGrid, "CentroDescarregamento/Pesquisa", _pesquisaCentroDescarregamento, menuOpcoes, null);
    _gridCentroDescarregamento.CarregarGrid();
}

// #endregion Métodos de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    _centroDescarregamento.TiposCarga.val(JSON.stringify(_gridTipoCarga.BuscarRegistros()));
    _centroDescarregamento.Transportadores.val(JSON.stringify(_transportador.Transportador.basicTable.BuscarRegistros()));
    _centroDescarregamento.VeiculosPermitidos.val(JSON.stringify(_veiculoPermitido.ModeloVeiculo.basicTable.BuscarRegistros()));
    _centroDescarregamento.Dia.val(_centroDescarregamentoDiasNoMes.Dia.val());
    _centroDescarregamento.Mes.val(_centroDescarregamentoDiasNoMes.Mes.val());

    if (_centroDescarregamento.CanalEntrega.val() == 0 && _centroDescarregamento.Destinatario.val() == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Logistica.CentroCarregamento.InformeCanalEntregaDestinatario);
        return;
    }

    Salvar(_centroDescarregamento, "CentroDescarregamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridCentroDescarregamento.CarregarGrid();
                LimparCamposCentroDescarregamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender, function () {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        resetarTabs();
    });
}

function atualizarClick(e, sender) {
    _centroDescarregamento.TiposCarga.val(JSON.stringify(_gridTipoCarga.BuscarRegistros()));
    _centroDescarregamento.Transportadores.val(JSON.stringify(_transportador.Transportador.basicTable.BuscarRegistros()));
    _centroDescarregamento.VeiculosPermitidos.val(JSON.stringify(_veiculoPermitido.ModeloVeiculo.basicTable.BuscarRegistros()));
    _centroDescarregamento.Dia.val(_centroDescarregamentoDiasNoMes.Dia.val());
    _centroDescarregamento.Mes.val(_centroDescarregamentoDiasNoMes.Mes.val());

    if (_centroDescarregamento.CanalEntrega.val() == 0 && _centroDescarregamento.Destinatario.val() == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Logistica.CentroCarregamento.InformeCanalEntregaDestinatario);
        return;
    }

    Salvar(_centroDescarregamento, "CentroDescarregamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridCentroDescarregamento.CarregarGrid();
                LimparCamposCentroDescarregamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender, function () {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        resetarTabs();
    });
}

function cancelarClick(e) {
    LimparCamposCentroDescarregamento();
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.AtencaoAcentuado, Localization.Resources.Logistica.CentroCarregamento.DesejaRealmenteExcluirJanelaDescarga + _centroDescarregamento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_centroDescarregamento, "CentroDescarregamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridCentroDescarregamento.CarregarGrid();
                    LimparCamposCentroDescarregamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function controlarCampoTipoCapacidadeDescarregamentoPorPesoHabilitado() {
    if (_centroDescarregamento.UtilizarCapacidadeDescarregamentoPorPeso.val() && !_centroDescarregamento.CapacidadeDescaregamentoPorDia.val()) {
        _centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso.enable(true);

        if (_centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso.val() == EnumTipoCapacidadeDescarregamentoPorPeso.Todos)
            _centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso.val(EnumTipoCapacidadeDescarregamentoPorPeso.DiaSemana);
    }
    else {
        _centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso.enable(false);
        _centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso.val(EnumTipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento);
    }
}

function editarCentroDescarregamento(centroDescarregamentoGrid, duplicar) {
    LimparCamposCentroDescarregamento();

    executarReST("CentroDescarregamento/BuscarPorCodigo", { Codigo: centroDescarregamentoGrid.Codigo, Duplicar: duplicar }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_centroDescarregamento, arg);
                _pesquisaCentroDescarregamento.ExibirFiltros.visibleFade(false);
                _crudCentroDescarregamento.Cancelar.visible(true);
                _crudCentroDescarregamento.Excluir.visible(!duplicar);
                _crudCentroDescarregamento.Atualizar.visible(!duplicar);
                _crudCentroDescarregamento.Adicionar.visible(duplicar);

                controlarCampoTipoCapacidadeDescarregamentoPorPesoHabilitado();
                _tempoDescarregamento.TempoPadraoDeEntrega.val(arg.Data.TempoPadraoDeEntrega);
                PreencherObjetoKnout(_limiteAgendamentoPadrao, { Data: arg.Data.LimiteAgendamentoPadrao });
                ControleCamposDiaMes(arg.Data.DiaMes);
                recarregarGridTipoCarga();
                RecarregarGridTempoDescarregamento();
                recarregarGridsCapacidadeDescarregamento();
                RecarregarGridTransportador();
                RecarregarGridEmail();
                RecarregarGridVeiculoPermitido();
                recarregarGridQuantidadePorTipoDeCarga();
                recarregarGridLimiteAgendamento();
                recarregarGridAprovacaoAutomaticaCentroDescarregamento();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function LimparCamposCentroDescarregamento() {
    _crudCentroDescarregamento.Atualizar.visible(false);
    _crudCentroDescarregamento.Cancelar.visible(false);
    _crudCentroDescarregamento.Excluir.visible(false);
    _crudCentroDescarregamento.Adicionar.visible(true);
    LimparCampos(_centroDescarregamento);
    LimparCamposTempoDescarregamento();
    LimparCamposCapacidadeDescarregamento();
    LimparCamposTransportador();
    LimparCamposEmail();
    limparCamposQuantidadePorTipoDeCarga();
    limparCamposLimiteAgendamento();
    limparCamposLimiteAgendamentoPadrao();
    controlarCampoTipoCapacidadeDescarregamentoPorPesoHabilitado();

    $("#" + _centroDescarregamento.UtilizarCapacidadeDescarregamentoPorPeso.id).prop("checked", false);

    recarregarGridTipoCarga();
    RecarregarGridTempoDescarregamento();
    RecarregarGridTransportador();
    RecarregarGridEmail();
    RecarregarGridVeiculoPermitido();
    recarregarGridQuantidadePorTipoDeCarga();
    recarregarGridLimiteAgendamento();
    recarregarGridAprovacaoAutomaticaCentroDescarregamento();
    resetarTabs();
}

function resetarTabs() {
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}

function duplicarJanelaDescarga() {

}

function MesClick(mes) {
    var data = new Date(new Date().getFullYear(), mes, 0);
    _centroDescarregamentoDiasNoMes.Mes.val(mes);
    _centroDescarregamentoDiasNoMes.DiasNoMes.val(ObterDiasNoMes(data))
    if (_centroDescarregamentoDiasNoMes.Dia.val() > _centroDescarregamentoDiasNoMes.DiasNoMes.val()[_centroDescarregamentoDiasNoMes.DiasNoMes.val().length - 1] && _centroDescarregamentoDiasNoMes.Dia.val() > 28)
        _centroDescarregamentoDiasNoMes.Dia.val(_centroDescarregamentoDiasNoMes.DiasNoMes.val()[_centroDescarregamentoDiasNoMes.DiasNoMes.val().length - 1]);
    if (_centroDescarregamento.Codigo.val() > 0)
        ObterCapacidadePorDiaMes();
}

function DiaClick(dia) {
    _centroDescarregamentoDiasNoMes.Dia.val(dia);
    if (_centroDescarregamento.Codigo.val() > 0)
        ObterCapacidadePorDiaMes();
}

function ObterCapacidadePorDiaMes() {
    data = { Codigo: _centroDescarregamento.Codigo.val(), Dia: _centroDescarregamentoDiasNoMes.Dia.val(), Mes: _centroDescarregamentoDiasNoMes.Mes.val(), CopiarPeriodo: false }
    executarReST("CentroDescarregamento/BuscarCapacidadePorDiaMes", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_centroDescarregamento, arg);
                recarregarGridsCapacidadeDescarregamento();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function ObterDiasNoMes(data) {
    var arr = new Array();
    var dias = data.getDate();
    for (var i = 1; i <= dias; i++)
        arr.push(i);
    return arr;
}

function ObterDiasNoMesObj(data) {
    var arr = new Array();
    var dias = data.getDate();
    for (var i = 1; i <= dias; i++)
        arr.push({ text: (i).toString(), value: i });
    return arr;
}

function initDiasNoMes() {
    var data = new Date(new Date().getFullYear(), new Date().getMonth(), 0);
    return ObterDiasNoMes(data);
}

function initDiasNoMesObj() {
    var data = new Date(new Date().getFullYear(), new Date().getMonth(), 0);
    return ObterDiasNoMesObj(data);
}
function initDia() {
    var data = new Date();
    return data.getDate();
}
function initMes() {
    var mes = new Date().getMonth();
    var mesAtual = mes + 1;
    document.getElementById(`knouMes_${mesAtual}`).classList.add("active");
    return (mesAtual);
}
function ControleCamposDiaMes(data) {
    document.getElementById(`knouMes_${data.Mes}`).classList.add("active");
    document.getElementById(`knouMes_1`).classList.remove("active");
    _centroDescarregamentoDiasNoMes.Dia.val(data.Dia);
    _centroDescarregamentoDiasNoMes.Mes.val(data.Mes);
    _centroDescarregamentoDiasNoMes.DiasNoMes.val([]);
    _centroDescarregamentoDiasNoMes.DiasNoMes.val(initDiasNoMes());
}


// #endregion Funções Privadas
