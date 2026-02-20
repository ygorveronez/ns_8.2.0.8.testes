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


var _gridNaturezaNFSe;
var _naturezaNFSe;
var _pesquisaNaturezaNFSe;


var _statusText = [{ text: "Ativo", value: "A" }, { text: "Inativo", value: "I"}];

var PesquisaNaturezaNFSe = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0),  text: "Localidade:", idBtnSearch: guid() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNaturezaNFSe.CarregarGrid();
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

var NaturezaNFSe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 100 });
    this.Numero = PropertyEntity({ text: "* Número: ", maxlength: 20, getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: true }});
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusText, def: "A", text: "*Situação: " });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Localidade:", idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.IntegracaoMigrateNFSeNatureza = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Natureza NFSe Migrate:"), idBtnSearch: guid(), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoMigrate), visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoMigrate) });

    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadNaturezaNFSe() {
    _naturezaNFSe = new NaturezaNFSe();
    KoBindings(_naturezaNFSe, "knockoutCadastroNaturezaNFSe");

    HeaderAuditoria("NaturezaNFSe", _naturezaNFSe);

    _pesquisaNaturezaNFSe = new PesquisaNaturezaNFSe();
    KoBindings(_pesquisaNaturezaNFSe, "knockoutPesquisaNaturezaNFSe", false, _pesquisaNaturezaNFSe.Pesquisar.id);

    new BuscarLocalidades(_pesquisaNaturezaNFSe.Localidade);
    new BuscarLocalidades(_naturezaNFSe.Localidade);
    new BuscarIntegracaoMigrateNFSeNatureza(_naturezaNFSe.IntegracaoMigrateNFSeNatureza);

    buscarNaturezaNFSes();

}

function adicionarClick(e, sender) {
    Salvar(e, "NaturezaNFSe/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridNaturezaNFSe.CarregarGrid();
                limparCamposNaturezaNFSe();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "NaturezaNFSe/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridNaturezaNFSe.CarregarGrid();
                limparCamposNaturezaNFSe();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a naturezaNFSe " + _naturezaNFSe.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_naturezaNFSe, "NaturezaNFSe/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridNaturezaNFSe.CarregarGrid();
                limparCamposNaturezaNFSe();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposNaturezaNFSe();
}

//*******MÉTODOS*******


function buscarNaturezaNFSes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarNaturezaNFSe, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridNaturezaNFSe = new GridView(_pesquisaNaturezaNFSe.Pesquisar.idGrid, "NaturezaNFSe/Pesquisa", _pesquisaNaturezaNFSe, menuOpcoes, null);
    _gridNaturezaNFSe.CarregarGrid();
}

function editarNaturezaNFSe(naturezaNFSeGrid) {
    limparCamposNaturezaNFSe();
    _naturezaNFSe.Codigo.val(naturezaNFSeGrid.Codigo);
    BuscarPorCodigo(_naturezaNFSe, "NaturezaNFSe/BuscarPorCodigo", function (arg) {
        _pesquisaNaturezaNFSe.ExibirFiltros.visibleFade(false);
        _naturezaNFSe.Atualizar.visible(true);
        _naturezaNFSe.Cancelar.visible(true);
        _naturezaNFSe.Excluir.visible(true);
        _naturezaNFSe.Adicionar.visible(false);
    }, null);
}

function limparCamposNaturezaNFSe() {
    _naturezaNFSe.Atualizar.visible(false);
    _naturezaNFSe.Cancelar.visible(false);
    _naturezaNFSe.Excluir.visible(false);
    _naturezaNFSe.Adicionar.visible(true);
    LimparCampos(_naturezaNFSe);
}
