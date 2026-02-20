/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentrosDescarregamento.js" />
/// <reference path="../../Consultas/PeriodoDescarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoCapacidadeDescarregamentoPorPeso.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _capacidadeDescarregamentoAdicional;
var _CRUDCapacidadeDescarregamentoAdicional;
var _pesquisaCapacidadeDescarregamentoAdicional;
var _gridCapacidadeDescarregamentoAdicional;

/*
 * Declaração das Classes
 */

var CRUDCapacidadeDescarregamentoAdicional = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CapacidadeDescarregamentoAdicional = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CapacidadeDescarregamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "*Capacidade de Descarregamento (KG):", required: true, enable: ko.observable(true) });
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Descarregamento:", idBtnSearch: guid(), required: true, enable: ko.observable(true), eventChange: centroDescarregamentoBlur });
    this.Data = PropertyEntity({ val: ko.observable(""), def: "", text: "*Data:", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.HoraInicio = PropertyEntity({ text: "Hora Início:", getType: typesKnockout.time, visible: false });
    this.HoraTermino = PropertyEntity({ text: "Hora Término:", getType: typesKnockout.time, visible: false });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000, enable: ko.observable(true) });
    this.PeriodoDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Período de Descarregamento:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), eventChange: periodoDescarregamentoBlur });
    this.PermitirEdicao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true });

    this.Data.val.subscribe(limparCamposPeriodoDescarregamento);
}

var PesquisaCapacidadeDescarregamentoAdicional = function () {
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Descarregamento:", idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Inicial:", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ val: ko.observable(""), def: "", text: "Data Final:", getType: typesKnockout.date });

    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicial;

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridCapacidadeDescarregamentoAdicional, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCapacidadeDescarregamentoAdicional() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "CapacidadeDescarregamentoAdicional/ExportarPesquisa", titulo: "Capacidade de Descarregamento Adicional" };

    _gridCapacidadeDescarregamentoAdicional = new GridViewExportacao(_pesquisaCapacidadeDescarregamentoAdicional.Pesquisar.idGrid, "CapacidadeDescarregamentoAdicional/Pesquisa", _pesquisaCapacidadeDescarregamentoAdicional, menuOpcoes, configuracoesExportacao);
    _gridCapacidadeDescarregamentoAdicional.CarregarGrid();
}

function loadCapacidadeDescarregamentoAdicional() {
    _capacidadeDescarregamentoAdicional = new CapacidadeDescarregamentoAdicional();
    KoBindings(_capacidadeDescarregamentoAdicional, "knockoutCapacidadeDescarregamentoAdicional");

    HeaderAuditoria("CapacidadeDescarregamentoAdicional", _capacidadeDescarregamentoAdicional);

    _CRUDCapacidadeDescarregamentoAdicional = new CRUDCapacidadeDescarregamentoAdicional();
    KoBindings(_CRUDCapacidadeDescarregamentoAdicional, "knockoutCRUDCapacidadeDescarregamentoAdicional");

    _pesquisaCapacidadeDescarregamentoAdicional = new PesquisaCapacidadeDescarregamentoAdicional();
    KoBindings(_pesquisaCapacidadeDescarregamentoAdicional, "knockoutPesquisaCapacidadeDescarregamentoAdicional", false, _pesquisaCapacidadeDescarregamentoAdicional.Pesquisar.id);

    new BuscarCentrosDescarregamento(_pesquisaCapacidadeDescarregamentoAdicional.CentroDescarregamento);
    new BuscarCentrosDescarregamento(_capacidadeDescarregamentoAdicional.CentroDescarregamento, retornoConsultaCentroDescarregamento);
    new BuscarPeriodoDescarregamento(_capacidadeDescarregamentoAdicional.PeriodoDescarregamento, retornoConsultaPeriodoDescarregamento, _capacidadeDescarregamentoAdicional.Data, _capacidadeDescarregamentoAdicional.CentroDescarregamento);

    loadGridCapacidadeDescarregamentoAdicional();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_capacidadeDescarregamentoAdicional, "CapacidadeDescarregamentoAdicional/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridCapacidadeDescarregamentoAdicional();
                limparCamposCapacidadeDescarregamentoAdicional();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_capacidadeDescarregamentoAdicional, "CapacidadeDescarregamentoAdicional/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridCapacidadeDescarregamentoAdicional();
                limparCamposCapacidadeDescarregamentoAdicional();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposCapacidadeDescarregamentoAdicional();
}

function centroDescarregamentoBlur() {
    if (_capacidadeDescarregamentoAdicional.CentroDescarregamento.val() == "") {
        controlarVisibilidadeCampoPeriodoDescarregamento(false);
        limparCamposPeriodoDescarregamento();
    }
}

function editarClick(registroSelecionado) {
    limparCamposCapacidadeDescarregamentoAdicional();

    _capacidadeDescarregamentoAdicional.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_capacidadeDescarregamentoAdicional, "CapacidadeDescarregamentoAdicional/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaCapacidadeDescarregamentoAdicional.ExibirFiltros.visibleFade(false);

                controlarComponentesHabilitados();
                controlarVisibilidadeCampoPeriodoDescarregamento(retorno.Data.CentroDescarregamento.TipoCapacidadeDescarregamentoPorPeso == EnumTipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_capacidadeDescarregamentoAdicional, "CapacidadeDescarregamentoAdicional/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridCapacidadeDescarregamentoAdicional();
                    limparCamposCapacidadeDescarregamentoAdicional();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function periodoDescarregamentoBlur() {
    if (_capacidadeDescarregamentoAdicional.PeriodoDescarregamento.val() == "") {
        _capacidadeDescarregamentoAdicional.HoraInicio.val("");
        _capacidadeDescarregamentoAdicional.HoraTermino.val("");
    }
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _capacidadeDescarregamentoAdicional.Codigo.val() > 0;
    var edicaoHabilitada = (isEdicao && _capacidadeDescarregamentoAdicional.PermitirEdicao.val());

    _CRUDCapacidadeDescarregamentoAdicional.Atualizar.visible(edicaoHabilitada);
    _CRUDCapacidadeDescarregamentoAdicional.Excluir.visible(edicaoHabilitada);
    _CRUDCapacidadeDescarregamentoAdicional.Adicionar.visible(!isEdicao);
}

function controlarVisibilidadeCampoPeriodoDescarregamento(exibir) {
    _capacidadeDescarregamentoAdicional.PeriodoDescarregamento.required(exibir);
    _capacidadeDescarregamentoAdicional.PeriodoDescarregamento.visible(exibir);
}

function controlarCamposHabilitados() {
    var edicaoHabilitada = _capacidadeDescarregamentoAdicional.PermitirEdicao.val();

    _capacidadeDescarregamentoAdicional.CapacidadeDescarregamento.enable(edicaoHabilitada);
    _capacidadeDescarregamentoAdicional.CentroDescarregamento.enable(edicaoHabilitada);
    _capacidadeDescarregamentoAdicional.Data.enable(edicaoHabilitada);
    _capacidadeDescarregamentoAdicional.PeriodoDescarregamento.enable(edicaoHabilitada);
    _capacidadeDescarregamentoAdicional.Observacao.enable(edicaoHabilitada);
}

function controlarComponentesHabilitados() {
    controlarBotoesHabilitados();
    controlarCamposHabilitados();
}

function limparCamposCapacidadeDescarregamentoAdicional() {
    LimparCampos(_capacidadeDescarregamentoAdicional);
    controlarComponentesHabilitados();
    controlarVisibilidadeCampoPeriodoDescarregamento(false);
}

function limparCamposPeriodoDescarregamento() {
    _capacidadeDescarregamentoAdicional.PeriodoDescarregamento.codEntity(0);
    _capacidadeDescarregamentoAdicional.PeriodoDescarregamento.val("");
    _capacidadeDescarregamentoAdicional.PeriodoDescarregamento.entityDescription("");
    _capacidadeDescarregamentoAdicional.HoraInicio.val("");
    _capacidadeDescarregamentoAdicional.HoraTermino.val("");
}

function recarregarGridCapacidadeDescarregamentoAdicional() {
    _gridCapacidadeDescarregamentoAdicional.CarregarGrid();
}

function retornoConsultaCentroDescarregamento(registroSelecionado) {
    _capacidadeDescarregamentoAdicional.CentroDescarregamento.codEntity(registroSelecionado.Codigo);
    _capacidadeDescarregamentoAdicional.CentroDescarregamento.val(registroSelecionado.Descricao);
    _capacidadeDescarregamentoAdicional.CentroDescarregamento.entityDescription(registroSelecionado.Descricao);

    controlarVisibilidadeCampoPeriodoDescarregamento(registroSelecionado.TipoCapacidadeDescarregamentoPorPeso == EnumTipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento);
    limparCamposPeriodoDescarregamento();
}

function retornoConsultaPeriodoDescarregamento(registroSelecionado) {
    _capacidadeDescarregamentoAdicional.PeriodoDescarregamento.codEntity(registroSelecionado.Codigo);
    _capacidadeDescarregamentoAdicional.PeriodoDescarregamento.val(registroSelecionado.Descricao);
    _capacidadeDescarregamentoAdicional.PeriodoDescarregamento.entityDescription(registroSelecionado.Descricao);
    _capacidadeDescarregamentoAdicional.HoraInicio.val(registroSelecionado.HoraInicio);
    _capacidadeDescarregamentoAdicional.HoraTermino.val(registroSelecionado.HoraTermino);
}
