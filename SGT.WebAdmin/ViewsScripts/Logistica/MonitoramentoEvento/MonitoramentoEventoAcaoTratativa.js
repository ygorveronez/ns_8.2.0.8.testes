/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroMonitoramentoEventoAcaoTratativa;
var _CRUDcadastroMonitoramentoEventoAcaoTratativa;
var _gridMonitoramentoEventoAcaoTratativa;
var _MonitoramentoEventoAcaoTratativa;

/*
 * Declaração das Classes
 */

var CRUDCadastroMonitoramentoEventoAcaoTratativa = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoAcaoTratativaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarMonitoramentoEventoAcaoTratativaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    //this.Excluir = PropertyEntity({ eventClick: excluirMonitoramentoEventoAcaoTratativaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CadastroMonitoramentoEventoAcaoTratativa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.AcaoMonitorada = PropertyEntity({ text: "Ação Monitorada", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.TipoAlerta = PropertyEntity({ text: "*Tipo Alerta: ", val: ko.observable(_monitoramentoEvento.TipoMonitoramentoEvento.val()), options: EnumTipoAlerta.obterOpcoes(), def: _monitoramentoEvento.TipoMonitoramentoEvento.val(), visible: true, enable: false });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var MonitoramentoEventoAcaoTratativa = function () {
    this.ListaAcaoTratativa = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaAcaoTratativa.val.subscribe(function () {
        recarregarGridMonitoramentoEventoAcaoTratativa();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoAcaoTratativaModalClick, type: types.event, text: "Adicionar Ação Tratativa" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMonitoramentoEventoAcaoTratativa() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 3, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarMonitoramentoEventoAcaoTratativaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Status", visible: false },
        { data: "AcaoMonitorada", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%", className: "text-align-left", orderable: false },
        { data: "StatusDescricao", title: "Status", width: "15%", className: "text-align-center", orderable: false },
        { data: "AcaoMonitoradaDescricao", title: "Ação Monitorada", width: "15%", className: "text-align-center", orderable: false }
    ];

    _gridMonitoramentoEventoAcaoTratativa = new BasicDataTable(_MonitoramentoEventoAcaoTratativa.ListaAcaoTratativa.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridMonitoramentoEventoAcaoTratativa.CarregarGrid([]);
}

function loadMonitoramentoEventoAcaoTratativa() {
    _MonitoramentoEventoAcaoTratativa = new MonitoramentoEventoAcaoTratativa();
    KoBindings(_MonitoramentoEventoAcaoTratativa, "knockoutMonitoramentoEventoAcaoTratativa");

    _cadastroMonitoramentoEventoAcaoTratativa = new CadastroMonitoramentoEventoAcaoTratativa();
    KoBindings(_cadastroMonitoramentoEventoAcaoTratativa, "knockoutCadastroMonitoramentoEventoAcaoTratativa");

    _CRUDcadastroMonitoramentoEventoAcaoTratativa = new CRUDCadastroMonitoramentoEventoAcaoTratativa();
    KoBindings(_CRUDcadastroMonitoramentoEventoAcaoTratativa, "knockoutCRUDCadastroMonitoramentoEventoAcaoTratativa");

    loadGridMonitoramentoEventoAcaoTratativa();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMonitoramentoEventoAcaoTratativaClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoAcaoTratativa)) {
        _MonitoramentoEventoAcaoTratativa.ListaAcaoTratativa.val().push(obterCadastroMonitoramentoEventoAcaoTratativaSalvar());

        recarregarGridMonitoramentoEventoAcaoTratativa();
        fecharModalCadastroMonitoramentoEventoAcaoTratativa();
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function adicionarMonitoramentoEventoAcaoTratativaModalClick() {
    _cadastroMonitoramentoEventoAcaoTratativa.Codigo.val(guid());
    _cadastroMonitoramentoEventoAcaoTratativa.TipoAlerta.val(_monitoramentoEvento.TipoMonitoramentoEvento.val());

    controlarBotoesCadastroMonitoramentoEventoAcaoTratativaHabilitados(false);

    exibirModalCadastroMonitoramentoEventoAcaoTratativa();
}

function atualizarMonitoramentoEventoAcaoTratativaClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoAcaoTratativa)) {
        var ListaAcaoTratativa = obterListaAcaoTratativa();

        ListaAcaoTratativa.forEach(function (tratativa, i) {
            if (_cadastroMonitoramentoEventoAcaoTratativa.Codigo.val() == tratativa.Codigo) {
                ListaAcaoTratativa.splice(i, 1, obterCadastroMonitoramentoEventoAcaoTratativaSalvar());
            }
        });

        _MonitoramentoEventoAcaoTratativa.ListaAcaoTratativa.val(ListaAcaoTratativa);

        recarregarGridMonitoramentoEventoAcaoTratativa();
        fecharModalCadastroMonitoramentoEventoAcaoTratativa();
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function editarMonitoramentoEventoAcaoTratativaClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroMonitoramentoEventoAcaoTratativa, { Data: registroSelecionado });
    controlarBotoesCadastroMonitoramentoEventoAcaoTratativaHabilitados(true);
    exibirModalCadastroMonitoramentoEventoAcaoTratativa();
}

function excluirMonitoramentoEventoAcaoTratativaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a ação de tratativa?", function () {
        removerMonitoramentoEventoAcaoTratativa(_cadastroMonitoramentoEventoAcaoTratativa.Codigo.val());
        fecharModalCadastroMonitoramentoEventoAcaoTratativa();
    });
}

/*
 * Declaração das Funções Públicas
 */

function obterMonitoramentoEventoAcaoTratativaSalvar() {
    return JSON.stringify(obterListaAcaoTratativa());
}

function preencherMonitoramentoEventoAcaoTratativa(dadosAcaoTratativa) {
    _MonitoramentoEventoAcaoTratativa.ListaAcaoTratativa.val(dadosAcaoTratativa);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroMonitoramentoEventoAcaoTratativaHabilitados(isEdicao) {
    _CRUDcadastroMonitoramentoEventoAcaoTratativa.Adicionar.visible(!isEdicao);
    _CRUDcadastroMonitoramentoEventoAcaoTratativa.Atualizar.visible(isEdicao);
    //_CRUDcadastroMonitoramentoEventoAcaoTratativa.Excluir.visible(isEdicao);
}

function exibirModalCadastroMonitoramentoEventoAcaoTratativa() {
    Global.abrirModal('divModalCadastroMonitoramentoEventoAcaoTratativa');
    $("#divModalCadastroMonitoramentoEventoAcaoTratativa").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroMonitoramentoEventoAcaoTratativa);
    });
}

function fecharModalCadastroMonitoramentoEventoAcaoTratativa() {
    Global.fecharModal('divModalCadastroMonitoramentoEventoAcaoTratativa');
}

function limparCamposMonitoramentoEventoAcaoTratativa() {
    preencherMonitoramentoEventoAcaoTratativa([]);
}

function obterCadastroMonitoramentoEventoAcaoTratativaSalvar() {
    return {
        Codigo: _cadastroMonitoramentoEventoAcaoTratativa.Codigo.val(),
        Descricao: _cadastroMonitoramentoEventoAcaoTratativa.Descricao.val(),
        AcaoMonitorada: _cadastroMonitoramentoEventoAcaoTratativa.AcaoMonitorada.val(),
        AcaoMonitoradaDescricao: _cadastroMonitoramentoEventoAcaoTratativa.AcaoMonitorada.val() ? "Sim" : "Não",
        Status: _cadastroMonitoramentoEventoAcaoTratativa.Status.val(),
        StatusDescricao: _cadastroMonitoramentoEventoAcaoTratativa.Status.val() ? "Ativo" : "Inativo"
    };
}

function obterListaAcaoTratativa() {
    return _MonitoramentoEventoAcaoTratativa.ListaAcaoTratativa.val().slice();
}

function recarregarGridMonitoramentoEventoAcaoTratativa() {
    _gridMonitoramentoEventoAcaoTratativa.CarregarGrid(obterListaAcaoTratativa());
}

function removerMonitoramentoEventoAcaoTratativa(codigo) {
    var ListaAcaoTratativa = obterListaAcaoTratativa();

    ListaAcaoTratativa.forEach(function (tratativa, i) {
        if (codigo == tratativa.Codigo) {
            ListaAcaoTratativa.splice(i, 1);
        }
    });

    _MonitoramentoEventoAcaoTratativa.ListaAcaoTratativa.val(ListaAcaoTratativa);
}