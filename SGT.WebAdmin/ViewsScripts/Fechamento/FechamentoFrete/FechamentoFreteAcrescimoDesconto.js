/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />


var _cadastroFechamentoJustificativaAcrescimoDesconto;
var _CRUDFechamentoFreteAcrescimoDesconto;
var _gridFechamentoFreteAcrescimoDesconto;
var _fechamentoFreteAcrescimoDesconto;

/*
 * Declaração das Classes
 */
var CRUDFechamentoFreteAcrescimoDesconto = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarFechamentoFreteAcrescimoDescontoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarFechamentoFreteAcrescimoDescontoClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirFechamentoFreteAcrescimoDescontoClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
};

var CadastroFechamentoFreteAcrescimoDesconto = function () {
    this.Codigo = PropertyEntity({});
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa: ", required: true, idBtnSearch: guid() });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true), maxlength: 15 });
    this.TipoJustificativa = PropertyEntity({ val: ko.observable(EnumTipoJustificativa.Acrescimo), options: EnumTipoJustificativa.obterOpcoes(), def: EnumTipoJustificativa.Acrescimo, text: "Tipo da Justificativa:", visible: false });
}

var FechamentoFreteAcrescimoDesconto = function () {
    this.ListaFechamentoFreteAcrescimoDesconto = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaFechamentoFreteAcrescimoDesconto.val.subscribe(function () {
        recarregarGridFechamentoFreteAcrescimoDesconto();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarFechamentoFreteAcrescimoDescontoModalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadGridFechamentoFreteAcrescimoDesconto() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarFechamentoFreteAcrescimoDescontoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoJustificativa", visible: false },
        { data: "Justificativa", title: "Justificativa", width: "35%" },
        { data: "Valor", title: "Valor", width: "35%" }
    ];

    _gridFechamentoFreteAcrescimoDesconto = new BasicDataTable(_fechamentoFreteAcrescimoDesconto.ListaFechamentoFreteAcrescimoDesconto.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridFechamentoFreteAcrescimoDesconto.CarregarGrid([]);
}

function loadFechamentoFreteAcrescimoDesconto() {

    _fechamentoFreteAcrescimoDesconto = new FechamentoFreteAcrescimoDesconto();
    KoBindings(_fechamentoFreteAcrescimoDesconto, "knockoutFechamentoFreteAcrescimoDesconto");

    _cadastroFechamentoJustificativaAcrescimoDesconto = new CadastroFechamentoFreteAcrescimoDesconto();
    KoBindings(_cadastroFechamentoJustificativaAcrescimoDesconto, "knockoutCadastroFechamentoFreteAcrescimoDesconto");

    _CRUDFechamentoFreteAcrescimoDesconto = new CRUDFechamentoFreteAcrescimoDesconto();
    KoBindings(_CRUDFechamentoFreteAcrescimoDesconto, "knockoutCRUDFechamentoFreteAcrescimoDesconto");

    loadGridFechamentoFreteAcrescimoDesconto();

    BuscarFechamentoJustificativaAcrescimoDesconto(_cadastroFechamentoJustificativaAcrescimoDesconto.Justificativa, preencherJustificativaAcrescimoDesconto);
}

function preencherJustificativaAcrescimoDesconto(data) {
    _cadastroFechamentoJustificativaAcrescimoDesconto.Justificativa.codEntity(data.Codigo);
    _cadastroFechamentoJustificativaAcrescimoDesconto.Justificativa.val(data.Descricao);
    _cadastroFechamentoJustificativaAcrescimoDesconto.TipoJustificativa.val(data.TipoJustificativa);
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function adicionarFechamentoFreteAcrescimoDescontoClick() {
    if (!ValidarCamposObrigatorios(_cadastroFechamentoJustificativaAcrescimoDesconto)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
        return;
    }

    if (!validarCadastroFechamentoFreteAcrescimoDescontoDuplicado())
        return;

    _fechamentoFreteAcrescimoDesconto.ListaFechamentoFreteAcrescimoDesconto.val().push(obterCadastroFechamentoFreteAcrescimoDescontoSalvar());
    recarregarGridFechamentoFreteAcrescimoDesconto();
    fecharModalCadastroFechamentoFreteAcrescimoDesconto();
    AtualizarAcrescimosEDescontosAplicar();
}

function adicionarFechamentoFreteAcrescimoDescontoModalClick() {

    _cadastroFechamentoJustificativaAcrescimoDesconto.Codigo.val(guid());

    controlarBotoesCadastroFechamentoFreteAcrescimoDescontoHabilitados(false);

    exibirModalCadastroFechamentoFreteAcrescimoDesconto();
}

function atualizarFechamentoFreteAcrescimoDescontoClick() {
    if (!ValidarCamposObrigatorios(_cadastroFechamentoJustificativaAcrescimoDesconto)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
        return;
    }

    if (!validarCadastroFechamentoFreteAcrescimoDescontoDuplicado())
        return;

    var listaFechamentoFreteAcrescimoDesconto = obterListaFechamentoFreteAcrescimoDesconto();
    for (var i = 0; i < listaFechamentoFreteAcrescimoDesconto.length; i++) {
        if (_cadastroFechamentoJustificativaAcrescimoDesconto.Codigo.val() == listaFechamentoFreteAcrescimoDesconto[i].Codigo) {
            listaFechamentoFreteAcrescimoDesconto.splice(i, 1, obterCadastroFechamentoFreteAcrescimoDescontoSalvar());
            break;
        }
    }
    _fechamentoFreteAcrescimoDesconto.ListaFechamentoFreteAcrescimoDesconto.val(listaFechamentoFreteAcrescimoDesconto);
    recarregarGridFechamentoFreteAcrescimoDesconto();
    fecharModalCadastroFechamentoFreteAcrescimoDesconto();
    AtualizarAcrescimosEDescontosAplicar();
}

function editarFechamentoFreteAcrescimoDescontoClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroFechamentoJustificativaAcrescimoDesconto, { Data: registroSelecionado });
    _cadastroFechamentoJustificativaAcrescimoDesconto.Justificativa.codEntity(registroSelecionado.CodigoJustificativa);
    _cadastroFechamentoJustificativaAcrescimoDesconto.Justificativa.val(registroSelecionado.Justificativa);
    _cadastroFechamentoJustificativaAcrescimoDesconto.Valor.val(registroSelecionado.Valor);

    controlarBotoesCadastroFechamentoFreteAcrescimoDescontoHabilitados(true);

    exibirModalCadastroFechamentoFreteAcrescimoDesconto();
}

function excluirFechamentoFreteAcrescimoDescontoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o registro?", function () {
        removerFechamentoFreteAcrescimoDesconto(_cadastroFechamentoJustificativaAcrescimoDesconto.Codigo.val());
        recarregarGridFechamentoFreteAcrescimoDesconto();
        fecharModalCadastroFechamentoFreteAcrescimoDesconto();
        AtualizarAcrescimosEDescontosAplicar();
    });
}

/*
 * Declaração das Funções Públicas
 */
function limparCamposValoresOutrosRecursoFechamento() {
    preencherFechamentoFreteAcrescimoDesconto([]);
}

function preencherFechamentoFreteAcrescimoDesconto(dadosFechamentoFreteAcrescimoDesconto) {
    _fechamentoFreteAcrescimoDesconto.ListaFechamentoFreteAcrescimoDesconto.val(dadosFechamentoFreteAcrescimoDesconto);
    recarregarGridFechamentoFreteAcrescimoDesconto();

    _fechamentoFreteAcrescimoDesconto.Adicionar.visible(_fechamentoFrete.Situacao.val() == EnumSituacaoFechamentoFrete.Aberto);
}

function preencherFechamentoFreteAcrescimoDescontoSalvar(contrato) {
    contrato["FechamentoFreteAcrescimoDesconto"] = obterListaFechamentoFreteAcrescimoDescontoSalvar();
}

/*
 * Declaração das Funções
 */
function controlarBotoesCadastroFechamentoFreteAcrescimoDescontoHabilitados(isEdicao) {
    _CRUDFechamentoFreteAcrescimoDesconto.Adicionar.visible(!isEdicao);
    _CRUDFechamentoFreteAcrescimoDesconto.Atualizar.visible(isEdicao);
    _CRUDFechamentoFreteAcrescimoDesconto.Excluir.visible(isEdicao);
}

function exibirModalCadastroFechamentoFreteAcrescimoDesconto() {
    Global.abrirModal('divModalCadastroFechamentoFreteAcrescimoDesconto');
    $("#divModalCadastroFechamentoFreteAcrescimoDesconto").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroFechamentoJustificativaAcrescimoDesconto);
    });
}

function fecharModalCadastroFechamentoFreteAcrescimoDesconto() {
    Global.fecharModal('divModalCadastroFechamentoFreteAcrescimoDesconto');
}

function obterCadastroFechamentoFreteAcrescimoDescontoSalvar() {
    return {
        Codigo: _cadastroFechamentoJustificativaAcrescimoDesconto.Codigo.val(),
        CodigoJustificativa: _cadastroFechamentoJustificativaAcrescimoDesconto.Justificativa.codEntity(),
        Justificativa: _cadastroFechamentoJustificativaAcrescimoDesconto.Justificativa.val(),
        Valor: _cadastroFechamentoJustificativaAcrescimoDesconto.Valor.val(),
        TipoJustificativa: _cadastroFechamentoJustificativaAcrescimoDesconto.TipoJustificativa.val()
    };
}

function obterListaFechamentoFreteAcrescimoDesconto() {
    return _fechamentoFreteAcrescimoDesconto.ListaFechamentoFreteAcrescimoDesconto.val().slice();
}

function obterListaFechamentoFreteAcrescimoDescontoSalvar() {
    var listaFechamentoFreteAcrescimoDesconto = obterListaFechamentoFreteAcrescimoDesconto();

    return JSON.stringify(listaFechamentoFreteAcrescimoDesconto);
}

function recarregarGridFechamentoFreteAcrescimoDesconto() {
    var listaFechamentoFreteAcrescimoDesconto = obterListaFechamentoFreteAcrescimoDesconto();

    _gridFechamentoFreteAcrescimoDesconto.CarregarGrid(listaFechamentoFreteAcrescimoDesconto, (_fechamentoFrete.Situacao.val() == EnumSituacaoFechamentoFrete.Aberto));
}

function removerFechamentoFreteAcrescimoDesconto(codigo) {
    var listaFechamentoFreteAcrescimoDesconto = obterListaFechamentoFreteAcrescimoDesconto();

    for (var i = 0; i < listaFechamentoFreteAcrescimoDesconto.length; i++) {
        if (codigo == listaFechamentoFreteAcrescimoDesconto[i].Codigo) {
            listaFechamentoFreteAcrescimoDesconto.splice(i, 1);
            break;
        }
    }

    _fechamentoFreteAcrescimoDesconto.ListaFechamentoFreteAcrescimoDesconto.val(listaFechamentoFreteAcrescimoDesconto);
}

function validarCadastroFechamentoFreteAcrescimoDescontoDuplicado() {
    var listaFechamentoFreteAcrescimoDesconto = obterListaFechamentoFreteAcrescimoDesconto();

    for (var i = 0; i < listaFechamentoFreteAcrescimoDesconto.length; i++) {
        var fechamentosJustificativaAcrescimoDesconto = listaFechamentoFreteAcrescimoDesconto[i];

        if (
            (_cadastroFechamentoJustificativaAcrescimoDesconto.Codigo.val() != fechamentosJustificativaAcrescimoDesconto.Codigo) &&
            (_cadastroFechamentoJustificativaAcrescimoDesconto.Justificativa.codEntity() == fechamentosJustificativaAcrescimoDesconto.CodigoJustificativa)
        ) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "O registro já foi adicionado, favor verificar!");
            return false;
        }
    }

    return true;
}

function AtualizarAcrescimosEDescontosAplicar() {
    const listaFechamentoFreteAcrescimoDesconto = obterListaFechamentoFreteAcrescimoDesconto();

    let desconto = 0;
    let acrescimo = 0;

    for (let fechamentosJustificativaAcrescimoDesconto of listaFechamentoFreteAcrescimoDesconto) {
        if (fechamentosJustificativaAcrescimoDesconto.TipoJustificativa === EnumTipoJustificativa.Acrescimo)
            acrescimo += Globalize.parseFloat(fechamentosJustificativaAcrescimoDesconto.Valor);
        else
            desconto += Globalize.parseFloat(fechamentosJustificativaAcrescimoDesconto.Valor);
    }

    _resumo.TotalAcrescimosAplicar.val(Globalize.format(acrescimo, "n2"));
    _resumo.TotalDescontosAplicar.val(Globalize.format(desconto, "n2"));
    AtualizarValoresFechamento();
}

function esconderCamposAcrescimosDescontos() {
    $("#knockoutFechamentoFreteAcrescimoDesconto").hide();
    _resumo.TotalAcrescimosAplicar.visible(false);
    _resumo.TotalDescontosAplicar.visible(false);
}