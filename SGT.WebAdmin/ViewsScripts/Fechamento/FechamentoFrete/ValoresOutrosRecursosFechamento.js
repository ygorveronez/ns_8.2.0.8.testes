/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="Ocorrencias.js" />
/// <reference path="FechamentoFrete.js" />
/// <reference path="../../Consultas/ContratoFreteTransportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoFrete.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _cadastroValoresOutrosRecursosFechamento;
var _CRUDValoresOutrosRecursosFechamento;
var _gridValoresOutrosRecursosFechamento;
var _valoresOutrosRecursosFechamento;

/*
 * Declaração das Classes
 */
var CRUDValoresOutrosRecursosFechamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarValoresOutrosRecursosFechamentoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarValoresOutrosRecursosFechamentoClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirValoresOutrosRecursosFechamentoClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
};

var CadastroValoresOutrosRecursosFechamento = function () {
    this.Codigo = PropertyEntity({});
    this.ValoresOutrosRecursosFechamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Valor Outro Recurso:", required: true, idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tomador:", required: true, idBtnSearch: guid() });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.int, text: "*Quantidade:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true), maxlength: 3 });
}

var ValoresOutrosRecursosFechamento = function () {
    this.ListaValoresOutrosRecursosFechamento = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaValoresOutrosRecursosFechamento.val.subscribe(function () {
        recarregarGridValoresOutrosRecursosFechamento();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarValoresOutrosRecursosFechamentoModalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadGridValoresOutrosRecursosFechamento() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarValoresOutrosRecursosFechamentoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "ValoresOutrosRecursos", title: "Tipo Outros Recursos", width: "35%" },
        { data: "Tomador", title: "Tomador", width: "35%" },
        { data: "Quantidade", title: "Quantidade", width: "20%", className: "text-align-center" }
    ];

    _gridValoresOutrosRecursosFechamento = new BasicDataTable(_valoresOutrosRecursosFechamento.ListaValoresOutrosRecursosFechamento.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridValoresOutrosRecursosFechamento.CarregarGrid([]);
}

function loadValoresOutrosRecursosFechamento() {

    _valoresOutrosRecursosFechamento = new ValoresOutrosRecursosFechamento();
    KoBindings(_valoresOutrosRecursosFechamento, "knockoutValoresOutrosRecursosFechamento");

    _cadastroValoresOutrosRecursosFechamento = new CadastroValoresOutrosRecursosFechamento();
    KoBindings(_cadastroValoresOutrosRecursosFechamento, "knockoutCadastroValoresOutrosRecursosFechamento");

    _CRUDValoresOutrosRecursosFechamento = new CRUDValoresOutrosRecursosFechamento();
    KoBindings(_CRUDValoresOutrosRecursosFechamento, "knockoutCRUDValoresOutrosRecursosFechamento");

    loadGridValoresOutrosRecursosFechamento();

    new BuscarOutrosValoresRecursos(_cadastroValoresOutrosRecursosFechamento.ValoresOutrosRecursosFechamento, null, null, _dadosFechamento.Contrato);
    new BuscarContratoFreteTransportadorCliente(_cadastroValoresOutrosRecursosFechamento.Tomador, null, null, _dadosFechamento.Contrato);
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function adicionarValoresOutrosRecursosFechamentoClick() {
    if (!ValidarCamposObrigatorios(_cadastroValoresOutrosRecursosFechamento)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
        return;
    }

    if (!validarCadastroValoresOutrosRecursosFechamentoDuplicado())
        return;

    _valoresOutrosRecursosFechamento.ListaValoresOutrosRecursosFechamento.val().push(obterCadastroValoresOutrosRecursosFechamentoSalvar());
    recarregarGridValoresOutrosRecursosFechamento();
    fecharModalCadastroValoresOutrosRecursosFechamento();
}

function adicionarValoresOutrosRecursosFechamentoModalClick() {

    _cadastroValoresOutrosRecursosFechamento.Codigo.val(guid());

    controlarBotoesCadastroValoresOutrosRecursosFechamentoHabilitados(false);

    exibirModalCadastroValoresOutrosRecursosFechamento();
}

function atualizarValoresOutrosRecursosFechamentoClick() {
    if (!ValidarCamposObrigatorios(_cadastroValoresOutrosRecursosFechamento)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
        return;
    }

    if (!validarCadastroValoresOutrosRecursosFechamentoDuplicado())
        return;

    var listaValoresOutrosRecursosFechamento = obterListaValoresOutrosRecursosFechamento();
    for (var i = 0; i < listaValoresOutrosRecursosFechamento.length; i++) {
        if (_cadastroValoresOutrosRecursosFechamento.Codigo.val() == listaValoresOutrosRecursosFechamento[i].Codigo) {
            listaValoresOutrosRecursosFechamento.splice(i, 1, obterCadastroValoresOutrosRecursosFechamentoSalvar());
            break;
        }
    }
    _valoresOutrosRecursosFechamento.ListaValoresOutrosRecursosFechamento.val(listaValoresOutrosRecursosFechamento);
    recarregarGridValoresOutrosRecursosFechamento();
    fecharModalCadastroValoresOutrosRecursosFechamento();

}

function editarValoresOutrosRecursosFechamentoClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroValoresOutrosRecursosFechamento, { Data: registroSelecionado });
    _cadastroValoresOutrosRecursosFechamento.ValoresOutrosRecursosFechamento.codEntity(registroSelecionado.CodigoValoresOutrosRecursos);
    _cadastroValoresOutrosRecursosFechamento.ValoresOutrosRecursosFechamento.val(registroSelecionado.ValoresOutrosRecursos);
    _cadastroValoresOutrosRecursosFechamento.Tomador.codEntity(registroSelecionado.CodigoTomador);
    _cadastroValoresOutrosRecursosFechamento.Tomador.val(registroSelecionado.Tomador);

    controlarBotoesCadastroValoresOutrosRecursosFechamentoHabilitados(true);

    exibirModalCadastroValoresOutrosRecursosFechamento();
}

function excluirValoresOutrosRecursosFechamentoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o registro?", function () {
        removerValoresOutrosRecursosFechamento(_cadastroValoresOutrosRecursosFechamento.Codigo.val());
        recarregarGridValoresOutrosRecursosFechamento();
        fecharModalCadastroValoresOutrosRecursosFechamento();
    });
}

/*
 * Declaração das Funções Públicas
 */
function limparCamposValoresOutrosRecursoFechamento() {
    preencherValoresOutrosRecursosFechamento([]);
}

function preencherValoresOutrosRecursosFechamento(dadosValoresOutrosRecursosFechamento) {
    _valoresOutrosRecursosFechamento.ListaValoresOutrosRecursosFechamento.val(dadosValoresOutrosRecursosFechamento);
    recarregarGridValoresOutrosRecursosFechamento();

    _valoresOutrosRecursosFechamento.Adicionar.visible(_fechamentoFrete.Situacao.val() == EnumSituacaoFechamentoFrete.Aberto);
}

function preencherValoresOutrosRecursoFechamentoSalvar(contrato) {
    contrato["ValoresOutrosRecursosFechamento"] = obterListaValoresOutrosRecursosFechamentoSalvar();
}

/*
 * Declaração das Funções
 */
function controlarBotoesCadastroValoresOutrosRecursosFechamentoHabilitados(isEdicao) {
    _CRUDValoresOutrosRecursosFechamento.Adicionar.visible(!isEdicao);
    _CRUDValoresOutrosRecursosFechamento.Atualizar.visible(isEdicao);
    _CRUDValoresOutrosRecursosFechamento.Excluir.visible(isEdicao);
}

function exibirModalCadastroValoresOutrosRecursosFechamento() {
    Global.abrirModal('divModalCadastroValoresOutrosRecursosFechamento');
    $("#divModalCadastroValoresOutrosRecursosFechamento").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroValoresOutrosRecursosFechamento);
    });
}

function fecharModalCadastroValoresOutrosRecursosFechamento() {
    Global.fecharModal('divModalCadastroValoresOutrosRecursosFechamento');
}

function obterCadastroValoresOutrosRecursosFechamentoSalvar() {
    return {
        Codigo: _cadastroValoresOutrosRecursosFechamento.Codigo.val(),
        CodigoValoresOutrosRecursos: _cadastroValoresOutrosRecursosFechamento.ValoresOutrosRecursosFechamento.codEntity(),
        CodigoTomador: _cadastroValoresOutrosRecursosFechamento.Tomador.codEntity(),
        ValoresOutrosRecursos: _cadastroValoresOutrosRecursosFechamento.ValoresOutrosRecursosFechamento.val(),
        Tomador: _cadastroValoresOutrosRecursosFechamento.Tomador.val(),
        Quantidade: _cadastroValoresOutrosRecursosFechamento.Quantidade.val()
    };
}

function obterListaValoresOutrosRecursosFechamento() {
    return _valoresOutrosRecursosFechamento.ListaValoresOutrosRecursosFechamento.val().slice();
}

function obterListaValoresOutrosRecursosFechamentoSalvar() {
    var listaValoresOutrosRecursosFechamento = obterListaValoresOutrosRecursosFechamento();

    return JSON.stringify(listaValoresOutrosRecursosFechamento);
}

function recarregarGridValoresOutrosRecursosFechamento() {
    var listaValoresOutrosRecursosFechamento = obterListaValoresOutrosRecursosFechamento();

    _gridValoresOutrosRecursosFechamento.CarregarGrid(listaValoresOutrosRecursosFechamento, (_fechamentoFrete.Situacao.val() == EnumSituacaoFechamentoFrete.Aberto));
}

function removerValoresOutrosRecursosFechamento(codigo) {
    var listaValoresOutrosRecursosFechamento = obterListaValoresOutrosRecursosFechamento();

    for (var i = 0; i < listaValoresOutrosRecursosFechamento.length; i++) {
        if (codigo == listaValoresOutrosRecursosFechamento[i].Codigo) {
            listaValoresOutrosRecursosFechamento.splice(i, 1);
            break;
        }
    }

    _valoresOutrosRecursosFechamento.ListaValoresOutrosRecursosFechamento.val(listaValoresOutrosRecursosFechamento);
}

function validarCadastroValoresOutrosRecursosFechamentoDuplicado() {
    var listaValoresOutrosRecursosFechamento = obterListaValoresOutrosRecursosFechamento();

    for (var i = 0; i < listaValoresOutrosRecursosFechamento.length; i++) {
        var valoresOutrosRecursos = listaValoresOutrosRecursosFechamento[i];

        if (
            (_cadastroValoresOutrosRecursosFechamento.Codigo.val() != valoresOutrosRecursos.Codigo) &&
            (_cadastroValoresOutrosRecursosFechamento.ValoresOutrosRecursosFechamento.codEntity() == valoresOutrosRecursos.CodigoValoresOutrosRecursos) &&
            (_cadastroValoresOutrosRecursosFechamento.Tomador.codEntity() == valoresOutrosRecursos.CodigoTomador)
        ) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "O registro já foi adicionado, favor verificar!");
            return false;
        }
    }

    return true;
}
