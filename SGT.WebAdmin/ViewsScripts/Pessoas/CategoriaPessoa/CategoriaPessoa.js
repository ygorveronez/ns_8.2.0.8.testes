/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDCategoriaPessoa;
var _categoriaPessoa;
var _pesquisaCategoriaPessoa;
var _gridCategoriaPessoa;

var _corPadrao = '#FFFFFF';

var _ListaCores = [
    { value: '#FFFFFF' },
    { value: '#ED6464' },
    { value: '#ED8664' },
    { value: '#EDA864' },
    { value: '#EDCB64' },
    { value: '#EDED64' },
    { value: '#CBED64' },
    { value: '#A8ED64' },
    { value: '#86ED64' },
    { value: '#64ED64' },
    { value: '#64ED86' },
    { value: '#64EDA8' },
    { value: '#64EDCB' },
    { value: '#64EDED' },
    { value: '#64CBED' },
    { value: '#64A8ED' },
    { value: '#6495ED' },
    { value: '#6486ED' },
    { value: '#6464ED' },
    { value: '#8664ED' },
    { value: '#A864ED' },
    { value: '#CB64ED' },
    { value: '#ED64ED' },
    { value: '#ED64CB' },
    { value: '#ED64A8' },
    { value: '#ED6486' },
    { value: '#8B4513' },
    { value: '#E06F1F' },
    { value: '#EDA978' },
    { value: '#F9E2D2' },
    { value: '#000000' },
    { value: '#708090' },
    { value: '#9AA6B1' },
    { value: '#C5CCD3' },
    { value: '#F1F2F4' }
];

/*
 * Declaração das Classes
 */

var CRUDCategoriaPessoa = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Nova" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CategoriaPessoa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Codigo de Integração:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50  });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.Cor = PropertyEntity({ text: "Cor: ", val: ko.observable(_corPadrao), options: _ListaCores });
}

var PesquisaCategoriaPessoa = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridCategoriaPessoa, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCategoriaPessoa() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "CategoriaPessoa/ExportarPesquisa", titulo: "Categorias de Pessoas" };

    _gridCategoriaPessoa = new GridViewExportacao(_pesquisaCategoriaPessoa.Pesquisar.idGrid, "CategoriaPessoa/Pesquisa", _pesquisaCategoriaPessoa, menuOpcoes, configuracoesExportacao);
    _gridCategoriaPessoa = new GridView(_pesquisaCategoriaPessoa.Pesquisar.idGrid, "CategoriaPessoa/Pesquisa", _pesquisaCategoriaPessoa, menuOpcoes, undefined, null, null, null, null, null, null, null, configuracoesExportacao, undefined, undefined, callbackRowCategoriaPessoa);
    _gridCategoriaPessoa.CarregarGrid();
}

function callbackRowCategoriaPessoa(nRow, aData) {
    var span = $(nRow).find('td').eq(1).find('span')[0];
    if (span) {
        if ($(span).text() != '')
            $(span).html('<span class="grid-color" style="background-color:' + $(span).text() + ';"></span>');
    }
}

function loadCategoriaPessoa() {
    _categoriaPessoa = new CategoriaPessoa();
    KoBindings(_categoriaPessoa, "knockoutCategoriaPessoa");

    HeaderAuditoria("CategoriaPessoa", _categoriaPessoa);

    _CRUDCategoriaPessoa = new CRUDCategoriaPessoa();
    KoBindings(_CRUDCategoriaPessoa, "knockoutCRUDCategoriaPessoa");

    _pesquisaCategoriaPessoa = new PesquisaCategoriaPessoa();
    KoBindings(_pesquisaCategoriaPessoa, "knockoutPesquisaCategoriaPessoa", false, _pesquisaCategoriaPessoa.Pesquisar.id);

    loadGridCategoriaPessoa();

    loadCores();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_categoriaPessoa, "CategoriaPessoa/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridCategoriaPessoa();
                limparCamposCategoriaPessoa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_categoriaPessoa, "CategoriaPessoa/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridCategoriaPessoa();
                limparCamposCategoriaPessoa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposCategoriaPessoa();
}

function editarClick(registroSelecionado) {
    limparCamposCategoriaPessoa();

    _categoriaPessoa.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_categoriaPessoa, "CategoriaPessoa/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaCategoriaPessoa.ExibirFiltros.visibleFade(false);
                controlarBotoesHabilitados();
                setarCor(retorno.Data.Cor);
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
        ExcluirPorCodigo(_categoriaPessoa, "CategoriaPessoa/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridCategoriaPessoa();
                    limparCamposCategoriaPessoa();
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

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _categoriaPessoa.Codigo.val() > 0;

    _CRUDCategoriaPessoa.Atualizar.visible(isEdicao);
    _CRUDCategoriaPessoa.Excluir.visible(isEdicao);
    _CRUDCategoriaPessoa.Adicionar.visible(!isEdicao);
}

function limparCamposCategoriaPessoa() {
    controlarBotoesHabilitados();
    LimparCampos(_categoriaPessoa);
}

function recarregarGridCategoriaPessoa() {
    _gridCategoriaPessoa.CarregarGrid();
}

function loadCores() {
    $("#" + _categoriaPessoa.Cor.id).colorselector({
        callback: function (value) {
            _categoriaPessoa.Cor.val(value);
        }
    });
    setarCor(_corPadrao);
}
function setarCor(cor) {
    if (cor == null)
        cor = _corPadrao;
    console.log(cor);
    $("#" + _categoriaPessoa.Cor.id).colorselector("setValue", cor);
}