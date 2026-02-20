/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />


var _CRUDDataFixaVencimentoCiot;
var _cadastroDataFixaVencimentoCiot;
var _dataFixaVencimentoCiot;
var _gridDataFixaVencimentoCiot;

/*
 * Declaração das Classes
 */
var CRUDDataFixaVencimentoCiot = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarDataFixaVencimentoCiotClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarDataFixaVencimentoCiotClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirDataFixaVencimentoCiotClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
};

var CadastroDataFixaVencimentoCiot = function () {
    this.Codigo = PropertyEntity({});
    this.DiaInicialEmissao = PropertyEntity({ text: "Dia Inicial Emissão: ", getType: typesKnockout.int, required: true });
    this.DiaFinalEmissao = PropertyEntity({ text: "Dia Final Emissão: ", getType: typesKnockout.int, required: true });
    this.DiaVencimentoCIOT = PropertyEntity({ text: "Dia Vencimento CIOT: ", getType: typesKnockout.int, required: true });
}


var DataFixaVencimentoCiot = function () {
    this.ListaDataFixaVencimentoCiot = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaDataFixaVencimentoCiot.val.subscribe(function () {
        recarregarGridDataFixaVencimentoCiot();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDataFixaVencimentoCiotModalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadGridDataFixaVencimentoCiot() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarDataFixaVencimentoCiotClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "DiaInicialEmissao", title: "Dia Inicial Emissão", width: "33%" },
        { data: "DiaFinalEmissao", title: "Dia Final Emissão", width: "33%" },
        { data: "DiaVencimentoCIOT", title: "Dia Vencimento CIOT", width: "33%" }
    ];

    _gridDataFixaVencimentoCiot = new BasicDataTable(_dataFixaVencimentoCiot.ListaDataFixaVencimentoCiot.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridDataFixaVencimentoCiot.CarregarGrid([]);
}

function LoadConfiguracaoDataFixaVencimentoCiot() {

    _dataFixaVencimentoCiot = new DataFixaVencimentoCiot();
    KoBindings(_dataFixaVencimentoCiot, "knockoutDataFixaVencimentoCiot");

    _cadastroDataFixaVencimentoCiot = new CadastroDataFixaVencimentoCiot();
    KoBindings(_cadastroDataFixaVencimentoCiot, "knockoutCadastroDataFixaVencimentoCiot");

    _CRUDDataFixaVencimentoCiot = new CRUDDataFixaVencimentoCiot();
    KoBindings(_CRUDDataFixaVencimentoCiot, "knockoutCRUDDataFixaVencimentoCiot");

    $("#liTabDataFixaVencimentoCiot").hide();
    loadGridDataFixaVencimentoCiot();
}

function LimparCamposDataFixaVencimentoCiot() {
    LimparCampos(_configuracaoDataFixaVencimentoCiot);
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function adicionarDataFixaVencimentoCiotClick() {

    if (!validarCadastroDataFixaVencimentoCio())
        return;

    _dataFixaVencimentoCiot.ListaDataFixaVencimentoCiot.val().push(obterDataFixaVencimentoCiotSalvar());
    recarregarGridDataFixaVencimentoCiot();
    fecharModalCadastroDataFixaVencimentoCiot();
}

function adicionarDataFixaVencimentoCiotModalClick() {

    _cadastroDataFixaVencimentoCiot.Codigo.val(guid());

    controlarBotoesCadastroDataFixaVencimentoCiotHabilitados(false);

    exibirModalDataFixaVencimentoCiot();
}

function atualizarDataFixaVencimentoCiotClick() {

    if (!validarCadastroDataFixaVencimentoCio())
        return;

    var listaDataFixaVencimentoCiot = obterListaDataFixaVencimentoCiot();
    for (var i = 0; i < listaDataFixaVencimentoCiot.length; i++) {
        if (_cadastroDataFixaVencimentoCiot.Codigo.val() == listaDataFixaVencimentoCiot[i].Codigo) {
            listaDataFixaVencimentoCiot.splice(i, 1, obterDataFixaVencimentoCiotSalvar());
            break;
        }
    }
    _dataFixaVencimentoCiot.ListaDataFixaVencimentoCiot.val(listaDataFixaVencimentoCiot);
    recarregarGridDataFixaVencimentoCiot();
    fecharModalCadastroDataFixaVencimentoCiot();
}

function editarDataFixaVencimentoCiotClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroDataFixaVencimentoCiot, { Data: registroSelecionado });
    _cadastroDataFixaVencimentoCiot.Codigo.val(registroSelecionado.Codigo);
    _cadastroDataFixaVencimentoCiot.DiaInicialEmissao.val(registroSelecionado.DiaInicialEmissao);
    _cadastroDataFixaVencimentoCiot.DiaFinalEmissao.val(registroSelecionado.DiaFinalEmissao);
    _cadastroDataFixaVencimentoCiot.DiaVencimentoCIOT.val(registroSelecionado.DiaVencimentoCIOT);

    controlarBotoesCadastroDataFixaVencimentoCiotHabilitados(true);

    exibirModalDataFixaVencimentoCiot();
}

function excluirDataFixaVencimentoCiotClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o registro?", function () {
        removerFechamentoFreteAcrescimoDesconto(_cadastroDataFixaVencimentoCiot.Codigo.val());
        recarregarGridDataFixaVencimentoCiot();
        fecharModalCadastroDataFixaVencimentoCiot();
    });
}

/*
 * Declaração das Funções Públicas
 */
function limparCamposValoresOutrosRecursoFechamento() {
    preencherFechamentoFreteAcrescimoDesconto([]);
}

function preencherFechamentoFreteAcrescimoDesconto(dadosFechamentoFreteAcrescimoDesconto) {
    _dataFixaVencimentoCiot.ListaDataFixaVencimentoCiot.val(dadosFechamentoFreteAcrescimoDesconto);
    recarregarGridDataFixaVencimentoCiot();

    _dataFixaVencimentoCiot.Adicionar.visible(_fechamentoFrete.Situacao.val() == EnumSituacaoFechamentoFrete.Aberto);
}

function preencherFechamentoFreteAcrescimoDescontoSalvar(contrato) {
    contrato["FechamentoFreteAcrescimoDesconto"] = obterListaFechamentoFreteAcrescimoDescontoSalvar();
}

/*
 * Declaração das Funções
 */
function controlarBotoesCadastroDataFixaVencimentoCiotHabilitados(isEdicao) {
    _CRUDDataFixaVencimentoCiot.Adicionar.visible(!isEdicao);
    _CRUDDataFixaVencimentoCiot.Atualizar.visible(isEdicao);
    _CRUDDataFixaVencimentoCiot.Excluir.visible(isEdicao);
}

function exibirModalDataFixaVencimentoCiot() {
    Global.abrirModal('divModalIntervaloTempoLiberaDocumentoEmitidoEscrituracao');
    $("#divModalIntervaloTempoLiberaDocumentoEmitidoEscrituracao").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroDataFixaVencimentoCiot);
    });
}

function fecharModalCadastroDataFixaVencimentoCiot() {
    Global.fecharModal('divModalIntervaloTempoLiberaDocumentoEmitidoEscrituracao');
}

function obterDataFixaVencimentoCiotSalvar() {
    return {
        Codigo: _cadastroDataFixaVencimentoCiot.Codigo.val(),
        DiaInicialEmissao: _cadastroDataFixaVencimentoCiot.DiaInicialEmissao.val(),
        DiaFinalEmissao: _cadastroDataFixaVencimentoCiot.DiaFinalEmissao.val(),
        DiaVencimentoCIOT: _cadastroDataFixaVencimentoCiot.DiaVencimentoCIOT.val(),
    };
}

function obterListaDataFixaVencimentoCiot() {
    return _dataFixaVencimentoCiot.ListaDataFixaVencimentoCiot.val().slice();
}

function obterListaFechamentoFreteAcrescimoDescontoSalvar() {
    var listaDataFixaVencimentoCiot = obterListaDataFixaVencimentoCiot();

    return JSON.stringify(listaDataFixaVencimentoCiot);
}

function recarregarGridDataFixaVencimentoCiot() {
    var listaDataFixaVencimentoCiot = obterListaDataFixaVencimentoCiot();

    _gridDataFixaVencimentoCiot.CarregarGrid(listaDataFixaVencimentoCiot);
}

function CarregarDataFixaVencimentoCiot() {
    _configuracaoCIOT.ConfiguracaoDataFixaVencimentoCiot.val(JSON.stringify(obterListaDataFixaVencimentoCiot()));
}

function LimpaGridDataFixaVencimentoCiot() {
    _gridDataFixaVencimentoCiot.CarregarGrid([]);
}

function CarregaGridDataFixaVencimentoCiot() {
    _dataFixaVencimentoCiot.ListaDataFixaVencimentoCiot.val(_configuracaoCIOT.ConfiguracaoDataFixaVencimentoCiot.val());
    _gridDataFixaVencimentoCiot.CarregarGrid(_configuracaoCIOT.ConfiguracaoDataFixaVencimentoCiot.val());
}


function removerFechamentoFreteAcrescimoDesconto(codigo) {
    var listaDataFixaVencimentoCiot = obterListaDataFixaVencimentoCiot();

    for (var i = 0; i < listaDataFixaVencimentoCiot.length; i++) {
        if (codigo == listaDataFixaVencimentoCiot[i].Codigo) {
            listaDataFixaVencimentoCiot.splice(i, 1);
            break;
        }
    }

    _dataFixaVencimentoCiot.ListaDataFixaVencimentoCiot.val(listaDataFixaVencimentoCiot);
}

function validarCadastroDataFixaVencimentoCio() {
    var listaDataFixaVencimentoCiot = obterListaDataFixaVencimentoCiot()


    if (!(0 < parseInt(_cadastroDataFixaVencimentoCiot.DiaInicialEmissao.val()) && parseInt(_cadastroDataFixaVencimentoCiot.DiaInicialEmissao.val()) <= 31) ||
        !(0 < parseInt(_cadastroDataFixaVencimentoCiot.DiaFinalEmissao.val()) && parseInt(_cadastroDataFixaVencimentoCiot.DiaFinalEmissao.val()) <= 31) ||
        !(0 < parseInt(_cadastroDataFixaVencimentoCiot.DiaVencimentoCIOT.val()) && parseInt(_cadastroDataFixaVencimentoCiot.DiaVencimentoCIOT.val()) <= 31)) {
        exibirMensagem(tipoMensagem.atencao, "Dias Fora do período", "Insira um dia valido");
        return false;
    } 
    return true;
}
