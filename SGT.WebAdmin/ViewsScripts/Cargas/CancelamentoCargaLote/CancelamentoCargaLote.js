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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/JustificativaCancelamentoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _selecaoCarga;
var _cancelamentoCarga;
var _CRUDCancelamentoLote;
var _gridSelecaoCarga;

var SelecaoCarga = function () {
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCargaLote.NumeroBooking, getType: typesKnockout.string, enable: ko.observable(true), required: false, visible: ko.observable(false) });
    this.DataInicioEmissao = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCargaLote.DataEmissaoInicial.getFieldDescription(), getType: typesKnockout.date, enable: ko.observable(true), required: false, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinalEmissao = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCargaLote.DataEmissaoFinal.getFieldDescription(), getType: typesKnockout.date, enable: ko.observable(true), required: false });

    this.DataInicioEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicioEmissao;

    var opcoesSituacaoCarga = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ? EnumSituacoesCarga.obterOpcoesTMS() : EnumSituacoesCarga.obterOpcoesEmbarcador();

    this.SituacaoCarga = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCargaLote.SituacaoCarga.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), options: opcoesSituacaoCarga, def: new Array(), visible: ko.observable(false) });
    this.TipoPropostaMultimodal = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCargaLote.TipoProposta.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), options: EnumTipoPropostaMultimodal.obterOpcoesPerfil(), def: new Array(), visible: ko.observable(false) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Cargas.CancelamentoCargaLote.EmpresaFilial.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });//Verificar depois
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCargaLote.Tomador.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PedidoViagemDirecao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Cargas.CancelamentoCargaLote.NavioViagemDirecao.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Cargas.CancelamentoCargaLote.TerminalOrigem.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCargaLote.TerminalDestino.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCargaLote.Origem.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCargaLote.Destino.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCargaLote.TipoOperacao.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.CancelamentoCargaLote.Veiculo.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.CancelamentoCargaLote.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCargaLote.Destinatario.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCargaLote.Remetente.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.CancelamentoCargaLote.NumeroCarga.getFieldDescription(), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.DataCriacaoCarga = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCargaLote.DataCriacaoCarga.getFieldDescription(), getType: typesKnockout.date, enable: ko.observable(true), required: false });
    
    this.Cargas = PropertyEntity({ idGrid: "grid-pesquisa-cargas-para-cancelamento", enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.Cargas.CancelamentoCargaLote.MarcarDesmarcarTodos, visible: ko.observable(false), enable: ko.observable(true) });

    this.JustificativaCancelamentoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: Localization.Resources.Cargas.CancelamentoCargaLote.JustificativaCancelamentoCarga.getRequiredFieldDescription(), issue: 0, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.CancelamentoCargaLote.Justificativa.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.NaoDuplicarCarga = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCargaLote.NaoDuplicarCarga.getFieldDescription(), issue: 1525, val: ko.observable(false), def: false, enable: ko.observable(false), visible: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCargaLote.MotivoCancelamento.getFieldDescription(), val: ko.observable(""), issue: 632, maxlength: 255, required: false, enable: ko.observable(true), getType: typesKnockout.string, visible: ko.observable(false) });
    this.ListaCargas = PropertyEntity({ text: "", val: ko.observable(""), required: false, enable: ko.observable(true), getType: typesKnockout.string, visible: ko.observable(false) });

    this.Pesquisa = PropertyEntity({ eventClick: PesquisaCargasClick, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: ko.observable(true), enable: ko.observable(true) });
};

var CancelamentoCarga = function () {
    this.JustificativaCancelamentoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: Localization.Resources.Cargas.CancelamentoCargaLote.JustificativaCancelamentoCarga.getFieldDescription(), issue: 0, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Cargas.CancelamentoCargaLote.Justificativa.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.NaoDuplicarCarga = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCargaLote.NaoDuplicarCarga, issue: 1525, val: ko.observable(false), def: false, enable: ko.observable(false), visible: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Cargas.CancelamentoCargaLote.MotivoCancelamento, val: ko.observable(""), issue: 632, maxlength: 255, required: true, enable: ko.observable(true) });
};

var CRUDCancelamentoLote = function () {
    this.GerarCancelamento = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.CancelamentoCargaLote.GerarCancelamentos), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadCancelamentoCargaLote() {
    _selecaoCarga = new SelecaoCarga();
    KoBindings(_selecaoCarga, "knockoutSelecaoCarga");

    _cancelamentoCarga = new CancelamentoCarga();
    KoBindings(_cancelamentoCarga, "knockoutCancelamentoCarga");

    _CRUDCancelamentoLote = new CRUDCancelamentoLote();
    KoBindings(_CRUDCancelamentoLote, "knockoutCRUDCancelamentoLote");

    new BuscarClientes(_selecaoCarga.Tomador);
    new BuscarClientes(_selecaoCarga.Destinatario);
    new BuscarClientes(_selecaoCarga.Remetente);
    new BuscarTransportadores(_selecaoCarga.Empresa, null, null, true);
    new BuscarPedidoViagemNavio(_selecaoCarga.PedidoViagemDirecao);
    new BuscarTipoTerminalImportacao(_selecaoCarga.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_selecaoCarga.TerminalDestino);
    new BuscarLocalidades(_selecaoCarga.Origem);
    new BuscarLocalidades(_selecaoCarga.Destino);
    new BuscarTiposOperacao(_selecaoCarga.TipoOperacao);
    new BuscarVeiculos(_selecaoCarga.Veiculo);
    new BuscarMotoristas(_selecaoCarga.Motorista);
    new BuscarCargas(_selecaoCarga.NumeroCarga)

    new BuscarJustificativas(_cancelamentoCarga.Justificativa);
    new BuscarJustificativaCancelamentoCarga(_cancelamentoCarga.JustificativaCancelamentoCarga);

    configurarLayoutPorTipoSistemaCancelamentoCargaLote();
    ControleExibicaoNaoDuplicarCarga();

    buscarCargasParaCancelamento();
}

function configurarLayoutPorTipoSistemaCancelamentoCargaLote() {
    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _selecaoCarga.NumeroBooking.visible(true);
        _selecaoCarga.TipoPropostaMultimodal.visible(true);
        _selecaoCarga.PedidoViagemDirecao.visible(true);
        _selecaoCarga.TerminalOrigem.visible(true);
        _selecaoCarga.TerminalDestino.visible(true);
        _selecaoCarga.PedidoViagemDirecao.required(true);
        _selecaoCarga.TerminalOrigem.required(true);

        _cancelamentoCarga.Justificativa.visible(true);
        _cancelamentoCarga.Justificativa.required(true);
    } else {
        _selecaoCarga.SituacaoCarga.visible(true);
        _selecaoCarga.Veiculo.visible(true);
        _selecaoCarga.Motorista.visible(true);
    }

    if (!_CONFIGURACAO_TMS.ExibirJustificativaCancelamentoCarga) {
        _cancelamentoCarga.JustificativaCancelamentoCarga.required(false);
        _cancelamentoCarga.JustificativaCancelamentoCarga.visible(false);
    } else {
        _cancelamentoCarga.JustificativaCancelamentoCarga.required(true);
        _cancelamentoCarga.JustificativaCancelamentoCarga.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _selecaoCarga.Empresa.text(Localization.Resources.Cargas.CancelamentoCargaLote.Transportador.getFieldDescription());
    }
}

function PesquisaCargasClick(e, sender) {
    if (ValidarCamposObrigatorios(_selecaoCarga)) {
        buscarCargasParaCancelamento();
    }
}

function AdicionarClick(e, sender) {
    if (ValidarCamposObrigatorios(_cancelamentoCarga)) {
        if (!_selecaoCarga.SelecionarTodos.val()) {
            GerarCancelamentoLote(false);
        } else {
            GerarCancelamentoLote(true);
        }
    }
}

//*******MÉTODOS*******

function GerarCancelamentoLote(todosSelecionado) {
    var data = null;

    if (todosSelecionado) {

        _selecaoCarga.Motivo.val(_cancelamentoCarga.Motivo.val());
        _selecaoCarga.Justificativa.codEntity(_cancelamentoCarga.Justificativa.codEntity());
        _selecaoCarga.Justificativa.val(_cancelamentoCarga.Justificativa.val());
        _selecaoCarga.JustificativaCancelamentoCarga.codEntity(_cancelamentoCarga.JustificativaCancelamentoCarga.codEntity());
        _selecaoCarga.JustificativaCancelamentoCarga.val(_cancelamentoCarga.JustificativaCancelamentoCarga.val());
        _selecaoCarga.NaoDuplicarCarga.val(_cancelamentoCarga.NaoDuplicarCarga.val());

        data = RetornarObjetoPesquisa(_selecaoCarga);
    }
    else
        data = {
            ListaCargas: PreencherListaCodigos(),
            Motivo: _cancelamentoCarga.Motivo.val(),
            Justificativa: _cancelamentoCarga.Justificativa.codEntity(),
            JustificativaCancelamentoCarga: _cancelamentoCarga.JustificativaCancelamentoCarga.codEntity(),
            NaoDuplicarCarga: _cancelamentoCarga.NaoDuplicarCarga.val(),
            ConsultarTodos: false
        };

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.CancelamentoCargaLote.RealmenteSesejaCancelarCargasCelecionadas, function () {
        executarReST("CancelamentoCargaLote/GerarCancelamentoLote", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (!string.IsNullOrWhiteSpace(arg.Msg))
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                    else
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.CancelamentoCargaLote.ProcessoSolicitacaoCancelamentoRealizadoSucessoFavorAcompanheTelaCancelamentoCarga);
                    limparCamposSelecaoCarga();
                    limparCamposCancelamento();
                    buscarCargasParaCancelamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function PreencherListaCodigos() {
    var codigos = new Array();
    var titulosSelecionados = _gridSelecaoCarga.ObterMultiplosSelecionados();
    $.each(titulosSelecionados, function (i, carga) {
        codigos.push({ Codigo: carga.Codigo });
    });
    return JSON.stringify(codigos);
}

function buscarCargasParaCancelamento() {
    var somenteLeitura = false;

    _selecaoCarga.SelecionarTodos.visible(true);
    _selecaoCarga.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoCarga.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    _gridSelecaoCarga = new GridView(_selecaoCarga.Cargas.idGrid, "CancelamentoCargaLote/PesquisaCargasParaCancelamento", _selecaoCarga, null, null, null, null, null, null, multiplaescolha);
    _gridSelecaoCarga.SetPermitirEdicaoColunas(true);
    _gridSelecaoCarga.SetSalvarPreferenciasGrid(true);
    _gridSelecaoCarga.CarregarGrid();
}

function limparCamposSelecaoCarga() {
    LimparCampos(_selecaoCarga);
    _selecaoCarga.DataInicioEmissao.val(Global.DataAtual());
}

function limparCamposCancelamento() {
    LimparCampos(_cancelamentoCarga);
    _cancelamentoCarga.Motivo.val("");
}

function ControleExibicaoNaoDuplicarCarga() {
    if (!_CONFIGURACAO_TMS.TrocarPreCargaPorCarga) {
        _cancelamentoCarga.NaoDuplicarCarga.visible(true);
        _cancelamentoCarga.NaoDuplicarCarga.enable(true);
        _cancelamentoCarga.Justificativa.enable(true);
        _cancelamentoCarga.JustificativaCancelamentoCarga.enable(true);
        _cancelamentoCarga.NaoDuplicarCarga.val(false);
    }
    else if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.SempreDuplicarCargaCancelada) && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_NaoDuplicarCarga, _PermissoesPersonalizadasCancelamentoCarga)) {
        _cancelamentoCarga.NaoDuplicarCarga.visible(true);
        _cancelamentoCarga.NaoDuplicarCarga.enable(true);
        _cancelamentoCarga.Justificativa.enable(true);
        _cancelamentoCarga.NaoDuplicarCarga.val(_CONFIGURACAO_TMS.DefaultTrueDuplicarCarga);
    } else {
        _cancelamentoCarga.NaoDuplicarCarga.visible(false);
        _cancelamentoCarga.NaoDuplicarCarga.enable(false);
        _cancelamentoCarga.NaoDuplicarCarga.val(false);
    }
}