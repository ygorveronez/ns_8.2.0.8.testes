/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Localidade.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridServicoNFSe;
var _servicoNFSe;
var _pesquisaServicoNFSe;


var _statusText = [{ text: "Ativo", value: "A" }, { text: "Inativo", value: "I" }];

var PesquisaServicoNFSe = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade:", idBtnSearch: guid() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridServicoNFSe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ServicoNFSe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 100 });
    this.Numero = PropertyEntity({ text: "* Número: ", maxlength: 20, getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: true } });
    this.CNAE = PropertyEntity({ text: "CNAE: ", maxlength: 100 });
    this.NBS = PropertyEntity({ text: "NBS: ", maxlength: 9 });
    this.CodigoTributacao = PropertyEntity({ text: "Cod. Tributação: ", maxlength: 100 });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusText, def: "A", text: "*Situação: " });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Localidade:", idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadServicoNFSe() {

    _servicoNFSe = new ServicoNFSe();
    KoBindings(_servicoNFSe, "knockoutCadastroServicoNFSe");

    HeaderAuditoria("ServicoNFSe", _servicoNFSe);

    _pesquisaServicoNFSe = new PesquisaServicoNFSe();
    KoBindings(_pesquisaServicoNFSe, "knockoutPesquisaServicoNFSe", false, _pesquisaServicoNFSe.Pesquisar.id);

    new BuscarLocalidades(_pesquisaServicoNFSe.Localidade);
    new BuscarLocalidades(_servicoNFSe.Localidade);

    buscarServicoNFSes();

}

function adicionarClick(e, sender) {
    Salvar(e, "ServicoNFSe/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridServicoNFSe.CarregarGrid();
                limparCamposServicoNFSe();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ServicoNFSe/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridServicoNFSe.CarregarGrid();
                limparCamposServicoNFSe();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a servicoNFSe " + _servicoNFSe.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_servicoNFSe, "ServicoNFSe/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridServicoNFSe.CarregarGrid();
                limparCamposServicoNFSe();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposServicoNFSe();
}

//*******MÉTODOS*******


function buscarServicoNFSes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarServicoNFSe, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridServicoNFSe = new GridView(_pesquisaServicoNFSe.Pesquisar.idGrid, "ServicoNFSe/Pesquisa", _pesquisaServicoNFSe, menuOpcoes, null);
    _gridServicoNFSe.CarregarGrid();
}

function editarServicoNFSe(servicoNFSeGrid) {
    limparCamposServicoNFSe();
    _servicoNFSe.Codigo.val(servicoNFSeGrid.Codigo);
    BuscarPorCodigo(_servicoNFSe, "ServicoNFSe/BuscarPorCodigo", function (arg) {
        _pesquisaServicoNFSe.ExibirFiltros.visibleFade(false);
        _servicoNFSe.Atualizar.visible(true);
        _servicoNFSe.Cancelar.visible(true);
        _servicoNFSe.Excluir.visible(true);
        _servicoNFSe.Adicionar.visible(false);
    }, null);
}

function limparCamposServicoNFSe() {
    _servicoNFSe.Atualizar.visible(false);
    _servicoNFSe.Cancelar.visible(false);
    _servicoNFSe.Excluir.visible(false);
    _servicoNFSe.Adicionar.visible(true);
    LimparCampos(_servicoNFSe);
}
