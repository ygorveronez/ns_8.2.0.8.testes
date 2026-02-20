/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="Localidade.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="RegiaoPrazoEntrega.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegiao;
var _regiao;
var _pesquisaRegiao;
var _regiaoPadraoTempo;
var _CRUDRegiao;

var PesquisaRegiao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.LocalidadePolo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade Polo: ", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.LocalidadeRegiao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade da Região: ", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação: ", idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga: ", idBtnSearch: guid() });
    this.CodigoIntegracao = PropertyEntity({ text: "Código para Integração: ", required: false, maxlength: 50, issue: 15 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegiao.CarregarGrid();
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

var Regiao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 150 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código para Integração: ", required: false, maxlength: 50, issue: 15 });
    this.LocalidadePolo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Localidade Polo:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.DiasPrazoEntrega = PropertyEntity({ text: "Prazo de Entrega (Dias): ", getType: typesKnockout.int, maxlength: 3 });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Localidades = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.RegioesPrazoEntrega = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDRegiao = function () {

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "Regiao/Importar",
        UrlConfiguracao: "Regiao/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O045_Regiao,
        CallbackImportacao: function () {
            _gridRegiao.CarregarGrid();
        }
    });

    this.ImportarPrazos = PropertyEntity({
        type: types.local,
        text: "Importar Prazos",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "Regiao/ImportarPrazos",
        UrlConfiguracao: "Regiao/ConfiguracaoImportacaoPrazos",
        CodigoControleImportacao: EnumCodigoControleImportacao.O060_RegiaoPrazoEntrega,
        CallbackImportacao: function () {
            _gridRegiao.CarregarGrid();
        }
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadRegiao() {
    _regiao = new Regiao();
    KoBindings(_regiao, "knockoutCadastroRegiao");

    _CRUDRegiao = new CRUDRegiao();
    KoBindings(_CRUDRegiao, "knockoutCRUDCadastroRegiao");

    HeaderAuditoria("Regiao", _regiao);

    _pesquisaRegiao = new PesquisaRegiao();
    KoBindings(_pesquisaRegiao, "knockoutPesquisaRegiao", false, _pesquisaRegiao.Pesquisar.id);

    loadLocalidade();
    LoadRegiaoPrazoEntrega();
    new BuscarLocalidades(_pesquisaRegiao.LocalidadePolo);
    new BuscarLocalidades(_pesquisaRegiao.LocalidadeRegiao);
    new BuscarFilial(_pesquisaRegiao.Filial);
    new BuscarTiposdeCarga(_pesquisaRegiao.TipoCarga);
    new BuscarTiposOperacao(_pesquisaRegiao.TipoOperacao);
    new BuscarLocalidades(_regiao.LocalidadePolo);
    new BuscarClientes(_regiao.Recebedor);

    buscarRegiaos();
}

function adicionarClick(e, sender) {
    resetarTabs();
    preencherLocalidades();

    if (!ValidarCamposObrigatorios(_regiao)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Preencha os campos obrigatórios.");
        return;
    }

    Salvar(_regiao, "Regiao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridRegiao.CarregarGrid();
                limparCamposRegiao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    resetarTabs();
    preencherLocalidades();

    Salvar(_regiao, "Regiao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridRegiao.CarregarGrid();
                limparCamposRegiao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    resetarTabs();
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a região " + _regiao.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_regiao, "Regiao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegiao.CarregarGrid();
                    limparCamposRegiao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposRegiao();
}

//*******MÉTODOS*******


function buscarRegiaos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegiao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRegiao = new GridView(_pesquisaRegiao.Pesquisar.idGrid, "Regiao/Pesquisa", _pesquisaRegiao, menuOpcoes, null);
    _gridRegiao.CarregarGrid();
}

function editarRegiao(regiaoGrid) {
    limparCamposRegiao();
    _regiao.Codigo.val(regiaoGrid.Codigo);
    BuscarPorCodigo(_regiao, "Regiao/BuscarPorCodigo", function (arg) {
        _pesquisaRegiao.ExibirFiltros.visibleFade(false);
        _CRUDRegiao.Atualizar.visible(true);
        _CRUDRegiao.Cancelar.visible(true);
        _CRUDRegiao.Excluir.visible(true);
        _CRUDRegiao.Adicionar.visible(false);
        recarregarGridLocalidades();
        RecarregarGridRegiaoPrazoEntrega();
    }, null);
}

function limparCamposRegiao() {
    resetarTabs();
    _CRUDRegiao.Atualizar.visible(false);
    _CRUDRegiao.Cancelar.visible(false);
    _CRUDRegiao.Excluir.visible(false);
    _CRUDRegiao.Adicionar.visible(true);

    LimparCampos(_regiao);
    limparCamposLocalidades();
    LimparCamposRegiaoPrazoEntrega();
    recarregarGridLocalidades();
    RecarregarGridRegiaoPrazoEntrega();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function preencherLocalidades() {
    _regiao.Localidades.val(JSON.stringify(_localidade.Localidade.basicTable.BuscarRegistros()));
}

function preencherListasTipoOperacao() {
    _regiao.TiposOperacao.val(JSON.stringify(_gridPrazoEntregaTipoOperacao.BuscarRegistros()));
}
