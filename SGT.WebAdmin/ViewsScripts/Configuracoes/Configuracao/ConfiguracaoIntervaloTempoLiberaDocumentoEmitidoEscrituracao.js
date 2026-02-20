/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />


var _CRUDIntervaloTempoLiberaDocumentoEmitidoEscrituracao;
var _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao;
var _intervaloTempoLiberaDocumentoEmitidoEscrituracao;
var _gridIntervaloTempoLiberaDocumentoEmitidoEscrituracao;

/*
 * Declaração das Classes
 */
var CRUDIntervaloTempoLiberaDocumentoEmitidoEscrituracao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarIntervaloTempoLiberaDocumentoEmitidoEscrituracaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarIntervaloTempoLiberaDocumentoEmitidoEscrituracaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirIntervaloTempoLiberaDocumentoEmitidoEscrituracaoClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
};

var CadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable('0') });
    this.DiaInicial = PropertyEntity({ text: "Dia Inicial: ", getType: typesKnockout.int, required: true });
    this.DiaFinal = PropertyEntity({ text: "Dia Final: ", getType: typesKnockout.int, required: true });
    this.IntervaloHora = PropertyEntity({ text: "Intervalo em horas: ", getType: typesKnockout.int, required: true });
}


var IntervaloTempoLiberaDocumentoEmitidoEscrituracao = function () {
    this.ListaIntervaloLiberacao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaIntervaloLiberacao.val.subscribe(function () {
        recarregarGridIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarIntervaloTempoLiberaDocumentoEmitidoEscrituracaoModalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadGridIntervaloTempoLiberaDocumentoEmitidoEscrituracao() {
    let linhasPorPaginas = 5;
    let ordenacao = { column: 0, dir: orderDir.asc };
    let opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarIntervaloTempoLiberaDocumentoEmitidoEscrituracaoClick, icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    let header = [
        { data: "Codigo", visible: false },
        { data: "DiaInicial", title: "Dia Inicial", width: "33%" },
        { data: "DiaFinal", title: "Dia Final", width: "33%" },
        { data: "IntervaloHora", title: "Intervalo em horas", width: "33%" }
    ];

    _gridIntervaloTempoLiberaDocumentoEmitidoEscrituracao = new BasicDataTable(_intervaloTempoLiberaDocumentoEmitidoEscrituracao.ListaIntervaloLiberacao.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridIntervaloTempoLiberaDocumentoEmitidoEscrituracao.CarregarGrid([]);
}

function loadConfiguracaoIntervaloTempoLiberaDocumentoEmitidoEscrituracao() {

    $("#container-configuracao-intervalo-tempo-libera-documento-emitido-escrituracao").appendTo("#configuracao-intervalo-tempo-libera-documento-emitido-escrituracao");

    _intervaloTempoLiberaDocumentoEmitidoEscrituracao = new IntervaloTempoLiberaDocumentoEmitidoEscrituracao();
    KoBindings(_intervaloTempoLiberaDocumentoEmitidoEscrituracao, "knockoutIntervaloTempoLiberaDocumentoEmitidoEscrituracao");

    _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao = new CadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
    KoBindings(_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao, "knockoutCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao");

    _CRUDIntervaloTempoLiberaDocumentoEmitidoEscrituracao = new CRUDIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
    KoBindings(_CRUDIntervaloTempoLiberaDocumentoEmitidoEscrituracao, "knockoutCRUDIntervaloTempoLiberaDocumentoEmitidoEscrituracao");

    $("#container-configuracao-intervalo-tempo-libera-documento-emitido-escrituracao").hide();
    loadGridIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
}

function LimparCamposIntervaloTempoLiberaDocumentoEmitidoEscrituracao() {
    LimparCampos(_intervaloTempoLiberaDocumentoEmitidoEscrituracao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function adicionarIntervaloTempoLiberaDocumentoEmitidoEscrituracaoClick() {

    if (!validarCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao())
        return;

    _intervaloTempoLiberaDocumentoEmitidoEscrituracao.ListaIntervaloLiberacao.val().push(obterIntervaloTempoLiberaDocumentoEmitidoEscrituracaoSalvar());
    recarregarGridIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
    fecharModalCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
}

function adicionarIntervaloTempoLiberaDocumentoEmitidoEscrituracaoModalClick() {

   // _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.Codigo.val(guid());

    controlarBotoesCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracaoHabilitados(false);

    exibirModalIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
}

function atualizarIntervaloTempoLiberaDocumentoEmitidoEscrituracaoClick() {

    if (!validarCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao())
        return;

    let listaIntervaloLiberacao = obterListaIntervaloLiberacao();
    for (let i = 0; i < listaIntervaloLiberacao.length; i++) {
        if (_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.Codigo.val() == listaIntervaloLiberacao[i].Codigo) {
            listaIntervaloLiberacao.splice(i, 1, obterIntervaloTempoLiberaDocumentoEmitidoEscrituracaoSalvar());
            break;
        }
    }
    _intervaloTempoLiberaDocumentoEmitidoEscrituracao.ListaIntervaloLiberacao.val(listaIntervaloLiberacao);
    recarregarGridIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
    fecharModalCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
}

function editarIntervaloTempoLiberaDocumentoEmitidoEscrituracaoClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao, { Data: registroSelecionado });
    _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.Codigo.val(registroSelecionado.Codigo);
    _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.DiaInicial.val(registroSelecionado.DiaInicial);
    _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.DiaFinal.val(registroSelecionado.DiaFinal);
    _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.IntervaloHora.val(registroSelecionado.IntervaloHora);

    controlarBotoesCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracaoHabilitados(true);

    exibirModalIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
}

function excluirIntervaloTempoLiberaDocumentoEmitidoEscrituracaoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o registro?", function () {
        removerIntervaloTempoLiberaDocumentoEmitidoEscrituracao(_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.Codigo.val());
        recarregarGridIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
        fecharModalCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao();
    });
}


/*
 * Declaração das Funções
 */

function controlarBotoesCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracaoHabilitados(isEdicao) {
    _CRUDIntervaloTempoLiberaDocumentoEmitidoEscrituracao.Adicionar.visible(!isEdicao);
    _CRUDIntervaloTempoLiberaDocumentoEmitidoEscrituracao.Atualizar.visible(isEdicao);
    _CRUDIntervaloTempoLiberaDocumentoEmitidoEscrituracao.Excluir.visible(isEdicao);
}

function exibirModalIntervaloTempoLiberaDocumentoEmitidoEscrituracao() {
    Global.abrirModal('divModalIntervaloTempoLiberaDocumentoEmitidoEscrituracao');
    $("#divModalIntervaloTempoLiberaDocumentoEmitidoEscrituracao").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao);
    });
}

function fecharModalCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao() {
    Global.fecharModal('divModalIntervaloTempoLiberaDocumentoEmitidoEscrituracao');
}

function obterIntervaloTempoLiberaDocumentoEmitidoEscrituracaoSalvar() {

    return {
        Codigo: _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.Codigo.val(),
        DiaInicial: _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.DiaInicial.val(),
        DiaFinal: _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.DiaFinal.val(),
        IntervaloHora: _cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.IntervaloHora.val(),
    };
}

function obterListaIntervaloLiberacao() {
    return _intervaloTempoLiberaDocumentoEmitidoEscrituracao.ListaIntervaloLiberacao.val().slice();
}

function recarregarGridIntervaloTempoLiberaDocumentoEmitidoEscrituracao() {
    let listaIntervaloLiberacao = obterListaIntervaloLiberacao();

    _gridIntervaloTempoLiberaDocumentoEmitidoEscrituracao.CarregarGrid(listaIntervaloLiberacao);
}

function obtemIntervaloTempoLiberaDocumentoEmitidoEscrituracao() {
    _configuracaoEmbarcador.ConfiguracaoIntervaloTempoLiberaDocumentoEmitidoEscrituracao.val(JSON.stringify(obterListaIntervaloLiberacao()));
}

function LimpaIntervaloTempoLiberaDocumentoEmitidoEscrituracao() {
    _gridIntervaloTempoLiberaDocumentoEmitidoEscrituracao.CarregarGrid([]);
}

function preencheGridIntervaloTempoLiberaDocumentoEmitidoEscrituracao(listaIntervaloLiberacao) {
    _intervaloTempoLiberaDocumentoEmitidoEscrituracao.ListaIntervaloLiberacao.val(listaIntervaloLiberacao);
    _gridIntervaloTempoLiberaDocumentoEmitidoEscrituracao.CarregarGrid(listaIntervaloLiberacao);
}

function removerIntervaloTempoLiberaDocumentoEmitidoEscrituracao(codigo) {
    let listaIntervaloLiberacao = obterIntervaloTempoLiberaDocumentoEmitidoEscrituracao();

    for (let i = 0; i < listaIntervaloLiberacao.length; i++) {
        if (codigo == listaIntervaloLiberacao[i].Codigo) {
            listaIntervaloLiberacao.splice(i, 1);
            break;
        }
    }

    _intervaloTempoLiberaDocumentoEmitidoEscrituracao.ListaIntervaloLiberacao.val(listaIntervaloLiberacao);
}

function obterListaIntervaloTempoLiberaDocumentoEmitidoEscrituracaoSalvar(configuracao) {
    let listaIntervaloLiberacao = obterListaIntervaloLiberacao();
    configuracao["ConfiguracaoIntervaloTempoLiberaDocumentoEmitidoEscrituracao"] = JSON.stringify(listaIntervaloLiberacao);
}

function removerIntervaloTempoLiberaDocumentoEmitidoEscrituracao(codigo) {
    let listaIntervaloLiberacao = obterListaIntervaloLiberacao();

    for (let i = 0; i < listaIntervaloLiberacao.length; i++) {
        if (codigo == listaIntervaloLiberacao[i].Codigo) {
            listaIntervaloLiberacao.splice(i, 1);
            break;
        }
    }

    _intervaloTempoLiberaDocumentoEmitidoEscrituracao.ListaIntervaloLiberacao.val(listaIntervaloLiberacao);
}

function validarCadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao() {

    if (!(0 < parseInt(_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.DiaInicial.val()) && parseInt(_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.DiaInicial.val()) <= 31) ||
        !(0 < parseInt(_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.DiaFinal.val()) && parseInt(_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.DiaFinal.val()) <= 31) ||
        !(0 < parseInt(_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.IntervaloHora.val()) && parseInt(_cadastroIntervaloTempoLiberaDocumentoEmitidoEscrituracao.IntervaloHora.val()) <= 31)) {
        exibirMensagem(tipoMensagem.atencao, "Dias Fora do período", "Insira um dia valido");
        return false;
    }
    return true;
}
