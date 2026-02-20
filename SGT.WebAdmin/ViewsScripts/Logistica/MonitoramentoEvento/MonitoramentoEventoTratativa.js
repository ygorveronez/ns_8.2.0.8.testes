/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CategoriaResponsavel.js" />
/// <reference path="../../Enumeradores/EnumTipoMonitoramentoEvento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroMonitoramentoEventoTratativa;
var _CRUDcadastroMonitoramentoEventoTratativa;
var _gridMonitoramentoEventoTratativa;
var _monitoramentoEventoTratativa;

/*
 * Declaração das Classes
 */

var CRUDCadastroMonitoramentoEventoTratativa = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoTratativaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarMonitoramentoEventoTratativaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirMonitoramentoEventoTratativaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CadastroMonitoramentoEventoTratativa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CategoriaResponsavel = PropertyEntity({ text: "*Categoria de Responsável:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
    this.EnviarEmail = PropertyEntity({ text: "Enviar E-mail", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarEmailTransportador = PropertyEntity({ text: "Enviar E-mail para o transportador", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarEmailCliente = PropertyEntity({ text: "Enviar E-mail para o cliente da carga", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.Sequencia = PropertyEntity({ text: "*Sequencia: ", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, required: true, enable: false });
    this.Tempo = PropertyEntity({ text: "*Tempo (minutos): ", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true, val: ko.observable(0), def : 0});
}

var MonitoramentoEventoTratativa = function () {
    this.ListaTratativa = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaTratativa.val.subscribe(function () {
        recarregarGridMonitoramentoEventoTratativa();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoTratativaModalClick, type: types.event, text: "Adicionar Notificação de Tratativa" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMonitoramentoEventoTratativa() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 3, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarMonitoramentoEventoTratativaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCategoriaResponsavel", visible: false },
        { data: "EnviarEmail", visible: false },
        { data: "EnviarEmailTransportador", visible: false },
        { data: "EnviarEmailCliente", visible: false },
        { data: "Sequencia", title: "Sequencia", width: "20%", className: "text-align-center", orderable: false },
        { data: "DescricaoCategoriaResponsavel", title: "Categoria de Responsável", width: "40%", className: "text-align-left", orderable: false },
        { data: "Tempo", title: "Tempo", width: "20%", className: "text-align-center", orderable: false }
    ];

    _gridMonitoramentoEventoTratativa = new BasicDataTable(_monitoramentoEventoTratativa.ListaTratativa.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas, null, null, null, reordenarSequencia);
    _gridMonitoramentoEventoTratativa.CarregarGrid([]);
}

function loadMonitoramentoEventoTratativa() {
    _monitoramentoEventoTratativa = new MonitoramentoEventoTratativa();
    KoBindings(_monitoramentoEventoTratativa, "knockoutMonitoramentoEventoTratativa");

    _cadastroMonitoramentoEventoTratativa = new CadastroMonitoramentoEventoTratativa();
    KoBindings(_cadastroMonitoramentoEventoTratativa, "knockoutCadastroMonitoramentoEventoTratativa");

    _CRUDcadastroMonitoramentoEventoTratativa = new CRUDCadastroMonitoramentoEventoTratativa();
    KoBindings(_CRUDcadastroMonitoramentoEventoTratativa, "knockoutCRUDCadastroMonitoramentoEventoTratativa");

    new BuscarCategoriaResponsavel(_cadastroMonitoramentoEventoTratativa.CategoriaResponsavel);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _cadastroMonitoramentoEventoTratativa.EnviarEmailTransportador.visible(false);
        _cadastroMonitoramentoEventoTratativa.EnviarEmailCliente.visible(false);
    }

    loadGridMonitoramentoEventoTratativa();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMonitoramentoEventoTratativaClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoTratativa)) {
        _monitoramentoEventoTratativa.ListaTratativa.val().push(obterCadastroMonitoramentoEventoTratativaSalvar());

        recarregarGridMonitoramentoEventoTratativa();
        fecharModalCadastroMonitoramentoEventoTratativa();
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function adicionarMonitoramentoEventoTratativaModalClick() {
    _cadastroMonitoramentoEventoTratativa.Codigo.val(guid());
    _cadastroMonitoramentoEventoTratativa.Sequencia.val(obterProximaSequencia());

    controlarBotoesCadastroMonitoramentoEventoTratativaHabilitados(false);

    exibirModalCadastroMonitoramentoEventoTratativa();
}

function atualizarMonitoramentoEventoTratativaClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoTratativa)) {
        var listaTratativa = obterListaTratativa();

        listaTratativa.forEach(function (tratativa, i) {
            if (_cadastroMonitoramentoEventoTratativa.Codigo.val() == tratativa.Codigo) {
                listaTratativa.splice(i, 1, obterCadastroMonitoramentoEventoTratativaSalvar());
            }
        });

        _monitoramentoEventoTratativa.ListaTratativa.val(listaTratativa);

        recarregarGridMonitoramentoEventoTratativa();
        fecharModalCadastroMonitoramentoEventoTratativa();
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function editarMonitoramentoEventoTratativaClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroMonitoramentoEventoTratativa, { Data: registroSelecionado });

    _cadastroMonitoramentoEventoTratativa.CategoriaResponsavel.codEntity(registroSelecionado.CodigoCategoriaResponsavel);
    _cadastroMonitoramentoEventoTratativa.CategoriaResponsavel.val(registroSelecionado.DescricaoCategoriaResponsavel);
    _cadastroMonitoramentoEventoTratativa.CategoriaResponsavel.entityDescription(registroSelecionado.DescricaoCategoriaResponsavel);

    controlarBotoesCadastroMonitoramentoEventoTratativaHabilitados(true);

    exibirModalCadastroMonitoramentoEventoTratativa();
}

function excluirMonitoramentoEventoTratativaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a tratativa?", function () {
        removerMonitoramentoEventoTratativa(_cadastroMonitoramentoEventoTratativa.Codigo.val());
        fecharModalCadastroMonitoramentoEventoTratativa();
    });
}

/*
 * Declaração das Funções Públicas
 */

function obterMonitoramentoEventoTratativasSalvar() {
    var listaTratativa = obterListaTratativa();

    return JSON.stringify(listaTratativa);
}

function preencherMonitoramentoEventoTratativa(dadosTratativa) {
    _monitoramentoEventoTratativa.ListaTratativa.val(dadosTratativa);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroMonitoramentoEventoTratativaHabilitados(isEdicao) {
    _CRUDcadastroMonitoramentoEventoTratativa.Adicionar.visible(!isEdicao);
    _CRUDcadastroMonitoramentoEventoTratativa.Atualizar.visible(isEdicao);
    _CRUDcadastroMonitoramentoEventoTratativa.Excluir.visible(isEdicao);
}

function exibirModalCadastroMonitoramentoEventoTratativa() {
    Global.abrirModal('divModalCadastroMonitoramentoEventoTratativa');
    $("#divModalCadastroMonitoramentoEventoTratativa").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroMonitoramentoEventoTratativa);
    });
}

function fecharModalCadastroMonitoramentoEventoTratativa() {
    Global.fecharModal('divModalCadastroMonitoramentoEventoTratativa');
}

function limparCamposMonitoramentoEventoTratativa() {
    preencherMonitoramentoEventoTratativa([]);
}

function obterCadastroMonitoramentoEventoTratativaSalvar() {
    return {
        Codigo: _cadastroMonitoramentoEventoTratativa.Codigo.val(),
        CodigoCategoriaResponsavel: _cadastroMonitoramentoEventoTratativa.CategoriaResponsavel.codEntity(),
        DescricaoCategoriaResponsavel: _cadastroMonitoramentoEventoTratativa.CategoriaResponsavel.val(),
        EnviarEmail: _cadastroMonitoramentoEventoTratativa.EnviarEmail.val(),
        EnviarEmailTransportador: _cadastroMonitoramentoEventoTratativa.EnviarEmailTransportador.val(),
        EnviarEmailCliente: _cadastroMonitoramentoEventoTratativa.EnviarEmailCliente.val(),
        Sequencia: _cadastroMonitoramentoEventoTratativa.Sequencia.val(),
        Tempo: _cadastroMonitoramentoEventoTratativa.Tempo.val()
    };
}

function obterListaTratativa() {
    return _monitoramentoEventoTratativa.ListaTratativa.val().slice();
}

function obterProximaSequencia() {
    var listaTratativa = obterListaTratativa();
    var maiorSequencia = 0;

    listaTratativa.forEach(function (tratativa) {
        if (maiorSequencia < tratativa.Sequencia)
            maiorSequencia = tratativa.Sequencia;
    });

    return maiorSequencia + 1;
}

function recarregarGridMonitoramentoEventoTratativa() {
    var listaTratativa = obterListaTratativa();

    _gridMonitoramentoEventoTratativa.CarregarGrid(listaTratativa);
}

function removerMonitoramentoEventoTratativa(codigo) {
    var listaTratativa = obterListaTratativa();
    var sequenciaRemovida = 0;

    listaTratativa.forEach(function (tratativa, i) {
        if (codigo == tratativa.Codigo) {
            sequenciaRemovida = tratativa.Sequencia;
            listaTratativa.splice(i, 1);
        }
    });

    listaTratativa.forEach(function (tratativa, i) {
        if (sequenciaRemovida < tratativa.Sequencia)
            tratativa.Sequencia -= 1;
    });

    _monitoramentoEventoTratativa.ListaTratativa.val(listaTratativa);
}

function reordenarSequencia(dadosReordenacao) {
    var listaTratativa = obterListaTratativa();

    listaTratativa.forEach(function (tratativa, i) {
        if (tratativa.Sequencia == dadosReordenacao.itemReordenado.posicaoAtual)
            tratativa.Sequencia = dadosReordenacao.itemReordenado.posicaoAnterior;
        else if (tratativa.Sequencia == dadosReordenacao.itemReordenado.posicaoAnterior)
            tratativa.Sequencia = dadosReordenacao.itemReordenado.posicaoAtual;
    });

    _monitoramentoEventoTratativa.ListaTratativa.val(listaTratativa);
}