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
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Enumeradores/EnumTipoCFOP.js" />
/// <reference path="../../Enumeradores/EnumListaRetornoSefaz.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRetornoSefaz;
var _retornoSefaz;
var _pesquisaRetornoSefaz;
var _crudRetornoSefaz;

var PesquisaRetornoSefaz = function () {
    this.MensagemRetornoSefaz = PropertyEntity({ text: "Retorno Serfaz: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRetornoSefaz.CarregarGrid();
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

var RetornoSefaz = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.MensagemRetornoSefaz = PropertyEntity({ text: "*Retorno Serfaz: ", required: true, maxlength: 1000 });
    this.AbreviacaoRetornoSefaz = PropertyEntity({ text: "*Abrevisção: ", required: true, maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDRetornoSefaz = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadRetornoSefaz() {

    _pesquisaRetornoSefaz = new PesquisaRetornoSefaz();
    KoBindings(_pesquisaRetornoSefaz, "knockoutPesquisaRetornoSefaz", false, _pesquisaRetornoSefaz.Pesquisar.id);

    _retornoSefaz = new RetornoSefaz();
    KoBindings(_retornoSefaz, "knockoutCadastroRetornoSefaz");

    HeaderAuditoria("RetornoSefaz", _retornoSefaz);

    _crudRetornoSefaz = new CRUDRetornoSefaz();
    KoBindings(_crudRetornoSefaz, "knockoutCRUDRetornoSefaz");
    
    buscarRetornoSefazs();
}

function adicionarClick(e, sender) {
    Salvar(_retornoSefaz, "RetornoSefaz/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridRetornoSefaz.CarregarGrid();
                limparCamposRetornoSefaz();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_retornoSefaz, "RetornoSefaz/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridRetornoSefaz.CarregarGrid();
                limparCamposRetornoSefaz();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o cadastro de " + _retornoSefaz.MensagemRetornoSefaz.val() + "?", function () {
        ExcluirPorCodigo(_retornoSefaz, "RetornoSefaz/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRetornoSefaz.CarregarGrid();
                    limparCamposRetornoSefaz();
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
    limparCamposRetornoSefaz();
}

//*******MÉTODOS*******

function buscarRetornoSefazs() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRetornoSefaz, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRetornoSefaz = new GridView(_pesquisaRetornoSefaz.Pesquisar.idGrid, "RetornoSefaz/Pesquisa", _pesquisaRetornoSefaz, menuOpcoes, null);
    _gridRetornoSefaz.CarregarGrid();
}

function editarRetornoSefaz(retornoSefazGrid) {
    limparCamposRetornoSefaz();
    _retornoSefaz.Codigo.val(retornoSefazGrid.Codigo);
    BuscarPorCodigo(_retornoSefaz, "RetornoSefaz/BuscarPorCodigo", function (arg) {
        _pesquisaRetornoSefaz.ExibirFiltros.visibleFade(false);
        _crudRetornoSefaz.Atualizar.visible(true);
        _crudRetornoSefaz.Cancelar.visible(true);
        _crudRetornoSefaz.Excluir.visible(true);
        _crudRetornoSefaz.Adicionar.visible(false);
    }, null);
}

function limparCamposRetornoSefaz() {
    _crudRetornoSefaz.Atualizar.visible(false);
    _crudRetornoSefaz.Cancelar.visible(false);
    _crudRetornoSefaz.Excluir.visible(false);
    _crudRetornoSefaz.Adicionar.visible(true);
    LimparCampos(_retornoSefaz);
}