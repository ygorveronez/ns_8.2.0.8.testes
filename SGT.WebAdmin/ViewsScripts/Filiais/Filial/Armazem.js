/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Tranportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroArmazem;
var _gridArmazem;
var _armazem;

/*
 * Declaração das Classes
 */

var CadastroArmazem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoIntegracao = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), required: true });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarArmazemClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarArmazemClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirArmazemClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var Armazem = function () {
    this.ListaArmazem = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarArmazemModalClick, type: types.event, text: Localization.Resources.Filiais.Filial.AdicionarArmazem });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridArmazem() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 4, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarArmazemClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Situacao", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "60%", className: "text-align-left", orderable: false },
        { data: "CodigoIntegracao", title: Localization.Resources.Gerais.Geral.CodigoIntegracao, width: "15%", className: "text-align-left", orderable: false },
        { data: "SituacaoDescricao", title: Localization.Resources.Gerais.Geral.Situacao, width: "15%", className: "text-align-center", orderable: false }
    ];

    _gridArmazem = new BasicDataTable(_armazem.ListaArmazem.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridArmazem.CarregarGrid([]);
}

function loadArmazem() {
    _armazem = new Armazem();
    KoBindings(_armazem, "knockoutArmazem");

    _cadastroArmazem = new CadastroArmazem();
    KoBindings(_cadastroArmazem, "knockoutCadastroArmazem");

    loadGridArmazem();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarArmazemClick() {
    if (!ValidarCamposObrigatorios(_cadastroArmazem)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (!validarCadastroArmazemDuplicado())
        return;

    _armazem.ListaArmazem.val().push(obterCadastroArmazemSalvar());

    recarregarGridArmazem();
    fecharModalCadastroArmazem();
}

function adicionarArmazemModalClick() {
    _cadastroArmazem.Codigo.val(guid());

    _cadastroArmazem.CodigoIntegracao.enable(true);

    controlarBotoesCadastroArmazemHabilitados(false);
    exibirModalCadastroArmazem();
}

function atualizarArmazemClick() {
    if (!ValidarCamposObrigatorios(_cadastroArmazem)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (!validarCadastroArmazemDuplicado())
        return;

    var listaArmazem = obterListaArmazem();

    listaArmazem.forEach(function (Armazem, i) {
        if (_cadastroArmazem.Codigo.val() == Armazem.Codigo) {
            listaArmazem.splice(i, 1, obterCadastroArmazemSalvar());
        }
    });

    _armazem.ListaArmazem.val(listaArmazem);

    recarregarGridArmazem();
    fecharModalCadastroArmazem();
}

function editarArmazemClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroArmazem, { Data: registroSelecionado });

    _cadastroArmazem.CodigoIntegracao.enable(false);

    controlarBotoesCadastroArmazemHabilitados(true);
    exibirModalCadastroArmazem();
}

function excluirArmazemClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaExcluirRegistro, function () {
        removerArmazem(_cadastroArmazem.Codigo.val());
        fecharModalCadastroArmazem();
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposArmazem() {
    _armazem.ListaArmazem.val([]);
    recarregarGridArmazem();
}

function preencherArmazem(Armazem) {
    _armazem.ListaArmazem.val(Armazem);
    recarregarGridArmazem();
}

function preencherArmazemSalvar(filial) {
    filial["Armazem"] = obterListaArmazemSalvar();
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroArmazemHabilitados(isEdicao) {
    _cadastroArmazem.Adicionar.visible(!isEdicao);
    _cadastroArmazem.Atualizar.visible(isEdicao);
    _cadastroArmazem.Excluir.visible(isEdicao);
}

function exibirModalCadastroArmazem() {
    Global.abrirModal('divModalCadastroArmazem');
    $("#divModalCadastroArmazem").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroArmazem);
    });
}

function fecharModalCadastroArmazem() {
    Global.fecharModal('divModalCadastroArmazem');
}

function obterCadastroArmazemSalvar() {
    return {
        Codigo: _cadastroArmazem.Codigo.val(),
        CodigoIntegracao: _cadastroArmazem.CodigoIntegracao.val(),
        Descricao: _cadastroArmazem.Descricao.val(),
        Situacao: _cadastroArmazem.Situacao.val(),
        SituacaoDescricao: _cadastroArmazem.Situacao.val() ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo
    };
}

function obterListaArmazem() {
    return _armazem.ListaArmazem.val().slice();
}

function obterListaArmazemSalvar() {
    var listaArmazem = obterListaArmazem();
    var listaArmazemSalvar = new Array();

    for (var i = 0; i < listaArmazem.length; i++) {
        var Armazem = listaArmazem[i];

        listaArmazemSalvar.push({
            Codigo: Armazem.Codigo,
            CodigoIntegracao: Armazem.CodigoIntegracao,
            Descricao: Armazem.Descricao,
            Situacao: Armazem.Situacao
        });
    }

    return JSON.stringify(listaArmazemSalvar);
}

function recarregarGridArmazem() {
    var listaArmazem = obterListaArmazem();

    _gridArmazem.CarregarGrid(listaArmazem);
}

function removerArmazem(codigo) {
    var listaArmazem = obterListaArmazem();

    listaArmazem.forEach(function (Armazem, i) {
        if (codigo == Armazem.Codigo)
            listaArmazem.splice(i, 1);
    });

    _armazem.ListaArmazem.val(listaArmazem);
    recarregarGridArmazem();
}

function validarCadastroArmazemDuplicado() {
    var listaArmazem = obterListaArmazem();

    for (var i = 0; i < listaArmazem.length; i++) {
        var Armazem = listaArmazem[i];

        if ((_cadastroArmazem.Codigo.val() != Armazem.Codigo) && _cadastroArmazem.CodigoIntegracao.val() == Armazem.CodigoIntegracao) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Gerais.Geral.RegistroDuplicadoMensagem);
            return false;
        }
    }

    return true;
}