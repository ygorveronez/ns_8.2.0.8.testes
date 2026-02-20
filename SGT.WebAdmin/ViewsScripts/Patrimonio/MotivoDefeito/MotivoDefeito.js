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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMotivoDefeito;
var _MotivoDefeito;
var _pesquisaMotivoDefeito;

var PesquisaMotivoDefeito = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoDefeito.CarregarGrid();
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

var MotivoDefeito = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDMotivoDefeito = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadMotivoDefeito() {
    _MotivoDefeito = new MotivoDefeito();

    KoBindings(_MotivoDefeito, "knockoutCadastroMotivoDefeito");
    HeaderAuditoria("MotivoDefeito", _MotivoDefeito);

    _crudMotivoDefeito = new CRUDMotivoDefeito();
    KoBindings(_crudMotivoDefeito, "knockoutCRUDMotivoDefeito");

    _pesquisaMotivoDefeito = new PesquisaMotivoDefeito();
    KoBindings(_pesquisaMotivoDefeito, "knockoutPesquisaMotivoDefeito", false, _pesquisaMotivoDefeito.Pesquisar.id);

    buscarMotivoDefeito();
}

function adicionarClick(e, sender) {
    Salvar(_MotivoDefeito, "MotivoDefeito/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridMotivoDefeito.CarregarGrid();
                limparCamposMotivoDefeito();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_MotivoDefeito, "MotivoDefeito/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoDefeito.CarregarGrid();
                limparCamposMotivoDefeito();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Marca EPI " + _MotivoDefeito.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_MotivoDefeito, "MotivoDefeito/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoDefeito.CarregarGrid();
                    limparCamposMotivoDefeito();
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
    limparCamposMotivoDefeito();
}

//*******MÉTODOS*******

function buscarMotivoDefeito() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoDefeito, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMotivoDefeito = new GridView(_pesquisaMotivoDefeito.Pesquisar.idGrid, "MotivoDefeito/Pesquisa", _pesquisaMotivoDefeito, menuOpcoes);
    _gridMotivoDefeito.CarregarGrid();
}

function editarMotivoDefeito(MotivoDefeitoGrid) {
    limparCamposMotivoDefeito();
    _MotivoDefeito.Codigo.val(MotivoDefeitoGrid.Codigo);
    BuscarPorCodigo(_MotivoDefeito, "MotivoDefeito/BuscarPorCodigo", function (arg) {
        _pesquisaMotivoDefeito.ExibirFiltros.visibleFade(false);
        _crudMotivoDefeito.Atualizar.visible(true);
        _crudMotivoDefeito.Cancelar.visible(true);
        _crudMotivoDefeito.Excluir.visible(true);
        _crudMotivoDefeito.Adicionar.visible(false);
    }, null);
}

function limparCamposMotivoDefeito() {
    _crudMotivoDefeito.Atualizar.visible(false);
    _crudMotivoDefeito.Cancelar.visible(false);
    _crudMotivoDefeito.Excluir.visible(false);
    _crudMotivoDefeito.Adicionar.visible(true);
    LimparCampos(_MotivoDefeito);
}