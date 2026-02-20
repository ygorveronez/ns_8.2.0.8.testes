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
/// <reference path="GrupoProdutoTMSDepreciacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGrupoProdutoTMS;
var _grupoProdutoTMS;
var _crudGrupoProdutoTMS;
var _pesquisaGrupoProdutoTMS;

var PesquisaGrupoProdutoTMS = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoProdutoTMS.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var GrupoProdutoTMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 550, required: true });
    this.GrupoProdutoTMSPai = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Pai:", idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 557 });

    this.Depreciacao = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
}

var CRUDGrupoProdutoTMS = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadGrupoProdutoTMS() {
    _grupoProdutoTMS = new GrupoProdutoTMS();
    KoBindings(_grupoProdutoTMS, "knockoutCadastroGrupoProdutoTMS");

    HeaderAuditoria("GrupoProdutoTMS", _grupoProdutoTMS);

    _crudGrupoProdutoTMS = new CRUDGrupoProdutoTMS();
    KoBindings(_crudGrupoProdutoTMS, "knockoutCRUDGrupoProdutoTMS");

    _pesquisaGrupoProdutoTMS = new PesquisaGrupoProdutoTMS();
    KoBindings(_pesquisaGrupoProdutoTMS, "knockoutPesquisaGrupoProdutoTMS", false, _pesquisaGrupoProdutoTMS.Pesquisar.id);

    new BuscarGruposProdutosTMS(_grupoProdutoTMS.GrupoProdutoTMSPai, null);

    buscarGruposProduto();

    loadGrupoProdutoTMSDepreciacao();
}

function adicionarClick(e, sender) {
    var valido = true;

    if (!validaCamposObrigatoriosGrupoProdutoTMSDepreciacao()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos da Depreciação");
        return;
    }

    if (valido) {
        _grupoProdutoTMS.Depreciacao.val(JSON.stringify(RetornarObjetoPesquisa(_grupoProdutoTMSDepreciacao)));

        Salvar(_grupoProdutoTMS, "GrupoProdutoTMS/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    _gridGrupoProdutoTMS.CarregarGrid();
                    limparCamposGrupoProdutoTMS();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    }
}

function atualizarClick(e, sender) {
    var valido = true;

    if (!validaCamposObrigatoriosGrupoProdutoTMSDepreciacao()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos da Depreciação");
        return;
    }

    if (valido) {
        _grupoProdutoTMS.Depreciacao.val(JSON.stringify(RetornarObjetoPesquisa(_grupoProdutoTMSDepreciacao)));

        Salvar(_grupoProdutoTMS, "GrupoProdutoTMS/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridGrupoProdutoTMS.CarregarGrid();
                    limparCamposGrupoProdutoTMS();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    }
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o grupo de produto " + _grupoProdutoTMS.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_grupoProdutoTMS, "GrupoProdutoTMS/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridGrupoProdutoTMS.CarregarGrid();
                    limparCamposGrupoProdutoTMS();
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
    resetarTabs();
    limparCamposGrupoProdutoTMS();
}

//*******MÉTODOS*******

function editarGrupoProdutoTMS(grupoProdutoTMSGrid) {
    limparCamposGrupoProdutoTMS();
    _grupoProdutoTMS.Codigo.val(grupoProdutoTMSGrid.Codigo);
    BuscarPorCodigo(_grupoProdutoTMS, "GrupoProdutoTMS/BuscarPorCodigo", function (arg) {
        _pesquisaGrupoProdutoTMS.ExibirFiltros.visibleFade(false);
        _crudGrupoProdutoTMS.Atualizar.visible(true);
        _crudGrupoProdutoTMS.Cancelar.visible(true);
        _crudGrupoProdutoTMS.Excluir.visible(true);
        _crudGrupoProdutoTMS.Adicionar.visible(false);

        if (arg.Data.Depreciacao !== null && arg.Data.Depreciacao !== undefined) {
            PreencherObjetoKnout(_grupoProdutoTMSDepreciacao, { Data: arg.Data.Depreciacao });
        }
    }, null);
}


function buscarGruposProduto() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarGrupoProdutoTMS, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridGrupoProdutoTMS = new GridView(_pesquisaGrupoProdutoTMS.Pesquisar.idGrid, "GrupoProdutoTMS/Pesquisa", _pesquisaGrupoProdutoTMS, menuOpcoes, null);
    _gridGrupoProdutoTMS.CarregarGrid();
}


function limparCamposGrupoProdutoTMS() {
    resetarTabs();
    _crudGrupoProdutoTMS.Atualizar.visible(false);
    _crudGrupoProdutoTMS.Cancelar.visible(false);
    _crudGrupoProdutoTMS.Excluir.visible(false);
    _crudGrupoProdutoTMS.Adicionar.visible(true);
    LimparCampos(_grupoProdutoTMS);
    limparCamposGrupoProdutoTMSDepreciacao();
}


function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function resetarTabs() {
    $("#liTabGrupoProduto a:first").tab("show");
}