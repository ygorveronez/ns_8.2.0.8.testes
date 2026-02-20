/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Filial.js" />  
/// <reference path="../../Consultas/TipoDocumentoTransporte.js" />  
/// <reference path="../../Consultas/CanalEntrega.js" />  
/// <reference path="../../Consultas/CanalVenda.js" />  
/// <reference path="../../Consultas/CategoriaPessoa.js" />  
/// <reference path="../../Consultas/TipoOperacao.js" />  

//*******MAPEAMENTO KNOUCKOUT*******

var _gridBloqueioEmissaoPorNaoConformidade;
var _bloqueioEmissaoPorNaoConformidade;
var _pesquisaBloqueioEmissaoPorNaoConformidade;

var _situacaoOptions = [
    { text: "Inativo", value: 0 },
    { text: "Ativo", value: 1 },
];

var _situacaoPesquisaOptions = [
    { text: "Todos", value: null },
    { text: "Inativo", value: 0 },
    { text: "Ativo", value: 1 },
];

var PesquisaBloqueioEmissaoPorNaoConformidade = function () {
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação: ", type: types.multiplesEntities, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoNaoConformidade = PropertyEntity({ text: "Tipo não Conformidade: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _situacaoPesquisaOptions, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBloqueioEmissaoPorNaoConformidade.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var BloqueioEmissaoPorNaoConformidade = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoOptions, def: 0, text: "Situação: " });
    this.TipoNaoConformidade = PropertyEntity({ text: "*Tipo não Conformidade: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDBloqueioEmissaoPorNaoConformidade = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadBloqueioEmissaoPorNaoConformidade() {
    _bloqueioEmissaoPorNaoConformidade = new BloqueioEmissaoPorNaoConformidade();
    KoBindings(_bloqueioEmissaoPorNaoConformidade, "knockoutBloqueioEmissaoPorNaoConformidade");

    HeaderAuditoria("BloqueioEmissaoPorNaoConformidade", _bloqueioEmissaoPorNaoConformidade);

    BloqueioEmissaoPorNaoConformidade = new CRUDBloqueioEmissaoPorNaoConformidade();
    KoBindings(BloqueioEmissaoPorNaoConformidade, "knockoutCRUDBloqueioEmissaoPorNaoConformidade");

    _pesquisaBloqueioEmissaoPorNaoConformidade = new PesquisaBloqueioEmissaoPorNaoConformidade();
    KoBindings(_pesquisaBloqueioEmissaoPorNaoConformidade, "knockoutPesquisaBloqueioEmissaoPorNaoConformidade", false, _pesquisaBloqueioEmissaoPorNaoConformidade.Pesquisar.id);

    new BuscarTiposOperacao(_pesquisaBloqueioEmissaoPorNaoConformidade.TipoOperacao);
    new BuscarNaoConformidade(_bloqueioEmissaoPorNaoConformidade.TipoNaoConformidade);
    new BuscarNaoConformidade(_pesquisaBloqueioEmissaoPorNaoConformidade.TipoNaoConformidade);

    buscarBloqueioEmissaoPorNaoConformidade();
    LoadTipoOperacao();
}

function adicionarClick(e, sender) {
    preencherListasTipoOperacao();
    Salvar(_bloqueioEmissaoPorNaoConformidade, "BloqueioEmissaoPorNaoConformidade/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridBloqueioEmissaoPorNaoConformidade.CarregarGrid();
                limparCamposRegraTipoOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasTipoOperacao();
    Salvar(_bloqueioEmissaoPorNaoConformidade, "BloqueioEmissaoPorNaoConformidade/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridBloqueioEmissaoPorNaoConformidade.CarregarGrid();
                limparCamposRegraTipoOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o bloqueio?", function () {
        ExcluirPorCodigo(_bloqueioEmissaoPorNaoConformidade, "BloqueioEmissaoPorNaoConformidade/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridBloqueioEmissaoPorNaoConformidade.CarregarGrid();
                    limparCamposRegraTipoOperacao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposRegraTipoOperacao();
}

function preencherListasTipoOperacao() {
    _bloqueioEmissaoPorNaoConformidade.TiposOperacao.val(JSON.stringify(_gridTipoOperacao.BuscarRegistros()));
}

//*******MÉTODOS*******

function buscarBloqueioEmissaoPorNaoConformidade() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraTipoOperacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridBloqueioEmissaoPorNaoConformidade = new GridView(_pesquisaBloqueioEmissaoPorNaoConformidade.Pesquisar.idGrid, "BloqueioEmissaoPorNaoConformidade/Pesquisa", _pesquisaBloqueioEmissaoPorNaoConformidade, menuOpcoes);
    _gridBloqueioEmissaoPorNaoConformidade.CarregarGrid();
}

function editarRegraTipoOperacao(regraTipoOperacao) {
    limparCamposRegraTipoOperacao();
    _bloqueioEmissaoPorNaoConformidade.Codigo.val(regraTipoOperacao.Codigo);
    BuscarPorCodigo(_bloqueioEmissaoPorNaoConformidade, "BloqueioEmissaoPorNaoConformidade/BuscarPorCodigo", function (arg) {
        _pesquisaBloqueioEmissaoPorNaoConformidade.ExibirFiltros.visibleFade(false);
        BloqueioEmissaoPorNaoConformidade.Atualizar.visible(true);
        BloqueioEmissaoPorNaoConformidade.Cancelar.visible(true);
        BloqueioEmissaoPorNaoConformidade.Excluir.visible(true);
        BloqueioEmissaoPorNaoConformidade.Adicionar.visible(false);
        RecarregarGridTipoOperacao();
    }, null);
}

function limparCamposRegraTipoOperacao() {
    BloqueioEmissaoPorNaoConformidade.Atualizar.visible(false);
    BloqueioEmissaoPorNaoConformidade.Cancelar.visible(false);
    BloqueioEmissaoPorNaoConformidade.Excluir.visible(false);
    BloqueioEmissaoPorNaoConformidade.Adicionar.visible(true);
    LimparCampos(_bloqueioEmissaoPorNaoConformidade);
    LimparCamposTipoOperacao();
}

