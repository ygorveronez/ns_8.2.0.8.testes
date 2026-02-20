/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />
/// <reference path="Acordo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroValoresOutrosRecursos
var _CRUDValoresOutrosRecursos;
var _gridValoresOutrosRecursos;
var _valoresOutrosRecursos;

/*
 * Declaração das Classes
 */

var CRUDValoresOutrosRecursos = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarValoresOutrosRecursosClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarValoresOutrosRecursosClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirValoresOutrosRecursosClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var CadastroValoresOutrosRecursos = function () {
    this.Codigo = PropertyEntity({});
    this.TipoMaoDeObra = PropertyEntity({ getType: typesKnockout.string, text: "Tipo Outros Recursos:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });
    this.PrecoAtual = PropertyEntity({ getType: typesKnockout.decimal, text: "Preço Atual:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });
}

var ValoresOutrosRecursos = function () {
    this.ListaValoresOutrosRecursos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(false) });

    this.ListaValoresOutrosRecursos.val.subscribe(function () {
        recarregarGridValoresOutrosRecursos();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarValoresOutrosRecursosModalClick, type: types.event, text: "Adicionar Item" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridValoresOutrosRecursos() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarValoresOutrosRecursosClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoMaoDeObra", title: "Tipo Outros Recursos", width: "50%" },
        { data: "PrecoAtual", title: "Preço Atual", width: "20%" }
    ];

    _gridValoresOutrosRecursos = new BasicDataTable(_valoresOutrosRecursos.ListaValoresOutrosRecursos.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridValoresOutrosRecursos.CarregarGrid([]);
}

function LoadAcordoValoresOutrosRecursos() {

    _valoresOutrosRecursos = new ValoresOutrosRecursos();
    KoBindings(_valoresOutrosRecursos, "knockoutValoresOutrosRecursos");

    _cadastroValoresOutrosRecursos = new CadastroValoresOutrosRecursos();
    KoBindings(_cadastroValoresOutrosRecursos, "knockoutCadastroValoresOutrosRecursos");

    _CRUDValoresOutrosRecursos = new CRUDValoresOutrosRecursos();
    KoBindings(_CRUDValoresOutrosRecursos, "knockoutCRUDValoresOutrosRecursos");

    loadGridValoresOutrosRecursos();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarValoresOutrosRecursosClick() {
    if (_CAMPOS_BLOQUEADOS) return;
    if (ValidarCamposObrigatorios(_cadastroValoresOutrosRecursos)) {
        _valoresOutrosRecursos.ListaValoresOutrosRecursos.val().push(obterCadastroValoresOutrosRecursosSalvar());
        recarregarGridValoresOutrosRecursos();
        fecharModalCadastroValoresOutrosRecursos();

    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
}

function adicionarValoresOutrosRecursosModalClick() {

    _cadastroValoresOutrosRecursos.Codigo.val(guid());

    controlarBotoesCadastroValoresOutrosRecursosHabilitados(false);

    exibirModalCadastroValoresOutrosRecursos();
}

function atualizarValoresOutrosRecursosClick() {
    if (_CAMPOS_BLOQUEADOS) return;
    if (ValidarCamposObrigatorios(_cadastroValoresOutrosRecursos)) {
        var listaValoresOutrosRecursos = obterListaValoresOutrosRecursos();

        for (var i = 0; i < listaValoresOutrosRecursos.length; i++) {
            if (_cadastroValoresOutrosRecursos.Codigo.val() == listaValoresOutrosRecursos[i].Codigo) {
                listaValoresOutrosRecursos.splice(i, 1, obterCadastroValoresOutrosRecursosSalvar());
                break;
            }
        }
        _valoresOutrosRecursos.ListaValoresOutrosRecursos.val(listaValoresOutrosRecursos);
        recarregarGridValoresOutrosRecursos();
        fecharModalCadastroValoresOutrosRecursos();
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
}

function editarValoresOutrosRecursosClick(registroSelecionado) {
    if (_CAMPOS_BLOQUEADOS) return;
    PreencherObjetoKnout(_cadastroValoresOutrosRecursos, { Data: registroSelecionado });

    controlarBotoesCadastroValoresOutrosRecursosHabilitados(true);

    exibirModalCadastroValoresOutrosRecursos();
}

function excluirValoresOutrosRecursosClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o registro?", function () {
        removerValoresOutrosRecursos(_cadastroValoresOutrosRecursos.Codigo.val());
        recarregarGridValoresOutrosRecursos();
        fecharModalCadastroValoresOutrosRecursos();
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposValoresOutrosRecurso() {
    preencherValoresOutrosRecursos([]);
}

function preencherValoresOutrosRecursos(dadosValoresOutrosRecursos) {
    _valoresOutrosRecursos.ListaValoresOutrosRecursos.val(dadosValoresOutrosRecursos);
    recarregarGridValoresOutrosRecursos();
}

function preencherValoresOutrosRecursoSalvar(contrato) {
    contrato["ValoresOutrosRecursos"] = obterListaValoresOutrosRecursosSalvar();
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroValoresOutrosRecursosHabilitados(isEdicao) {
    _CRUDValoresOutrosRecursos.Adicionar.visible(!isEdicao);
    _CRUDValoresOutrosRecursos.Atualizar.visible(isEdicao);
    _CRUDValoresOutrosRecursos.Excluir.visible(isEdicao);
}

function exibirModalCadastroValoresOutrosRecursos() {
    Global.abrirModal('divModalCadastroValoresOutrosRecursos');
    $("#divModalCadastroValoresOutrosRecursos").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroValoresOutrosRecursos);
    });
}

function fecharModalCadastroValoresOutrosRecursos() {
    Global.fecharModal('divModalCadastroValoresOutrosRecursos');
}

function obterCadastroValoresOutrosRecursosSalvar() {
    return {
        Codigo: _cadastroValoresOutrosRecursos.Codigo.val(),
        TipoMaoDeObra: _cadastroValoresOutrosRecursos.TipoMaoDeObra.val(),
        PrecoAtual: _cadastroValoresOutrosRecursos.PrecoAtual.val()
    };
}

function obterListaValoresOutrosRecursos() {
    return _valoresOutrosRecursos.ListaValoresOutrosRecursos.val().slice();
}

function obterListaValoresOutrosRecursosSalvar() {
    var listaValoresOutrosRecursos = obterListaValoresOutrosRecursos();

    return JSON.stringify(listaValoresOutrosRecursos);
}

function recarregarGridValoresOutrosRecursos() {
    var listaValoresOutrosRecursos = obterListaValoresOutrosRecursos();

    _gridValoresOutrosRecursos.CarregarGrid(listaValoresOutrosRecursos);
}

function removerValoresOutrosRecursos(codigo) {
    var listaValoresOutrosRecursos = obterListaValoresOutrosRecursos();

    for (var i = 0; i < listaValoresOutrosRecursos.length; i++) {
        if (codigo == listaValoresOutrosRecursos[i].Codigo) {
            listaValoresOutrosRecursos.splice(i, 1);
            break;
        }
    }

    _valoresOutrosRecursos.ListaValoresOutrosRecursos.val(listaValoresOutrosRecursos);
}