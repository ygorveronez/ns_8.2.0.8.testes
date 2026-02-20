/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroMonitoramentoEventoCausa;
var _CRUDcadastroMonitoramentoEventoCausa;
var _gridMonitoramentoEventoCausa;
var _MonitoramentoEventoCausa;

/*
 * Declaração das Classes
 */

var CRUDCadastroMonitoramentoEventoCausa = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoCausaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarMonitoramentoEventoCausaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirMonitoramentoEventoCausaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CadastroMonitoramentoEventoCausa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true });
    this.TipoAlerta = PropertyEntity({ text: "*Tipo Alerta: ", val: ko.observable(_monitoramentoEvento.TipoMonitoramentoEvento.val()), options: EnumTipoAlerta.obterOpcoes(), def: _monitoramentoEvento.TipoMonitoramentoEvento.val(), visible: true, enable: false });
}

var MonitoramentoEventoCausa = function () {
    this.ListaCausa = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaCausa.val.subscribe(function () {
        recarregarGridMonitoramentoEventoCausa();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoCausaModalClick, type: types.event, text: "Adicionar Causa" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMonitoramentoEventoCausa() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 3, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarMonitoramentoEventoCausaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Ativo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%", className: "text-align-left", orderable: false },
        { data: "AtivoDescricao", title: "Status", width: "30%", className: "text-align-center", orderable: false }
    ];

    _gridMonitoramentoEventoCausa = new BasicDataTable(_MonitoramentoEventoCausa.ListaCausa.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridMonitoramentoEventoCausa.CarregarGrid([]);
}

function loadMonitoramentoEventoCausa() {
    _MonitoramentoEventoCausa = new MonitoramentoEventoCausa();
    KoBindings(_MonitoramentoEventoCausa, "knockoutMonitoramentoEventoCausa");

    _cadastroMonitoramentoEventoCausa = new CadastroMonitoramentoEventoCausa();
    KoBindings(_cadastroMonitoramentoEventoCausa, "knockoutCadastroMonitoramentoEventoCausa");

    _CRUDcadastroMonitoramentoEventoCausa = new CRUDCadastroMonitoramentoEventoCausa();
    KoBindings(_CRUDcadastroMonitoramentoEventoCausa, "knockoutCRUDCadastroMonitoramentoEventoCausa");

    loadGridMonitoramentoEventoCausa();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMonitoramentoEventoCausaClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoCausa)) {
        _MonitoramentoEventoCausa.ListaCausa.val().push(obterCadastroMonitoramentoEventoCausaSalvar());

        recarregarGridMonitoramentoEventoCausa();
        fecharModalCadastroMonitoramentoEventoCausa();
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function adicionarMonitoramentoEventoCausaModalClick() {
    _cadastroMonitoramentoEventoCausa.Codigo.val(guid());
    _cadastroMonitoramentoEventoCausa.TipoAlerta.val(_monitoramentoEvento.TipoMonitoramentoEvento.val());

    controlarBotoesCadastroMonitoramentoEventoCausaHabilitados(false);

    exibirModalCadastroMonitoramentoEventoCausa();
}

function atualizarMonitoramentoEventoCausaClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoCausa)) {
        var ListaCausa = obterListaCausa();

        ListaCausa.forEach(function (tratativa, i) {
            if (_cadastroMonitoramentoEventoCausa.Codigo.val() == tratativa.Codigo) {
                ListaCausa.splice(i, 1, obterCadastroMonitoramentoEventoCausaSalvar());
            }
        });

        _MonitoramentoEventoCausa.ListaCausa.val(ListaCausa);

        recarregarGridMonitoramentoEventoCausa();
        fecharModalCadastroMonitoramentoEventoCausa();
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function editarMonitoramentoEventoCausaClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroMonitoramentoEventoCausa, { Data: registroSelecionado });
    controlarBotoesCadastroMonitoramentoEventoCausaHabilitados(true);
    exibirModalCadastroMonitoramentoEventoCausa();
}

function excluirMonitoramentoEventoCausaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a causa?", function () {
        removerMonitoramentoEventoCausa(_cadastroMonitoramentoEventoCausa.Codigo.val());
        fecharModalCadastroMonitoramentoEventoCausa();
    });
}

/*
 * Declaração das Funções Públicas
 */

function obterMonitoramentoEventoCausasSalvar() {
    return JSON.stringify(obterListaCausa());
}

function preencherMonitoramentoEventoCausa(dadosCausa) {
    _MonitoramentoEventoCausa.ListaCausa.val(dadosCausa);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroMonitoramentoEventoCausaHabilitados(isEdicao) {
    _CRUDcadastroMonitoramentoEventoCausa.Adicionar.visible(!isEdicao);
    _CRUDcadastroMonitoramentoEventoCausa.Atualizar.visible(isEdicao);
    _CRUDcadastroMonitoramentoEventoCausa.Excluir.visible(isEdicao);
}

function exibirModalCadastroMonitoramentoEventoCausa() {
    Global.abrirModal('divModalCadastroMonitoramentoEventoCausa');
    $("#divModalCadastroMonitoramentoEventoCausa").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroMonitoramentoEventoCausa);
    });
}

function fecharModalCadastroMonitoramentoEventoCausa() {
    Global.fecharModal('divModalCadastroMonitoramentoEventoCausa');
}

function limparCamposMonitoramentoEventoCausa() {
    preencherMonitoramentoEventoCausa([]);
}

function obterCadastroMonitoramentoEventoCausaSalvar() {
    return {
        Codigo: _cadastroMonitoramentoEventoCausa.Codigo.val(),
        Descricao: _cadastroMonitoramentoEventoCausa.Descricao.val(),
        Ativo: _cadastroMonitoramentoEventoCausa.Ativo.val(),
        AtivoDescricao: _cadastroMonitoramentoEventoCausa.Ativo.val() ? "Ativo" : "Inativo"
    };
}

function obterListaCausa() {
    return _MonitoramentoEventoCausa.ListaCausa.val().slice();
}

function recarregarGridMonitoramentoEventoCausa() {
    _gridMonitoramentoEventoCausa.CarregarGrid(obterListaCausa());
}

function removerMonitoramentoEventoCausa(codigo) {
    var ListaCausa = obterListaCausa();

    ListaCausa.forEach(function (tratativa, i) {
        if (codigo == tratativa.Codigo) {
            ListaCausa.splice(i, 1);
        }
    });

    _MonitoramentoEventoCausa.ListaCausa.val(ListaCausa);
}