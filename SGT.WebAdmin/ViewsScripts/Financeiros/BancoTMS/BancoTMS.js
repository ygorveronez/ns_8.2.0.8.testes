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
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../Enumeradores/EnumTipoCFOP.js" />
/// <reference path="../../Enumeradores/EnumListaBancoTMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridBancoTMS;
var _bancoTMS;
var _pesquisaBancoTMS;


var PesquisaBancoTMS = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, maxlength: 4, configInt: { precision: 0, allowZero: false } });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBancoTMS.CarregarGrid();
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

var BancoTMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 200 });
    this.Numero = PropertyEntity({ text: "*Número: ", required: true, maxlength: 4, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false } });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integracao: ", getType: typesKnockout.string, maxlength: 200 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadBancoTMS() {
    _pesquisaBancoTMS = new PesquisaBancoTMS();
    KoBindings(_pesquisaBancoTMS, "knockoutPesquisaBancoTMS", false, _pesquisaBancoTMS.Pesquisar.id);

    _bancoTMS = new BancoTMS();
    KoBindings(_bancoTMS, "knockoutCadastroBancoTMS");

    HeaderAuditoria("Banco", _bancoTMS);

    buscarBancoTMS();
}

function adicionarClick(e, sender) {
    Salvar(e, "BancoTMS/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridBancoTMS.CarregarGrid();
                limparCamposBancoTMS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "BancoTMS/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridBancoTMS.CarregarGrid();
                limparCamposBancoTMS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o banco " + _bancoTMS.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_bancoTMS, "BancoTMS/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridBancoTMS.CarregarGrid();
                limparCamposBancoTMS();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposBancoTMS();
}

//*******MÉTODOS*******


function buscarBancoTMS() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBancoTMS, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridBancoTMS = new GridView(_pesquisaBancoTMS.Pesquisar.idGrid, "BancoTMS/Pesquisa", _pesquisaBancoTMS, menuOpcoes, null);
    _gridBancoTMS.CarregarGrid();
}

function editarBancoTMS(bancoTMSGrid) {
    limparCamposBancoTMS();
    _bancoTMS.Codigo.val(bancoTMSGrid.Codigo);
    BuscarPorCodigo(_bancoTMS, "BancoTMS/BuscarPorCodigo", function (arg) {
        _pesquisaBancoTMS.ExibirFiltros.visibleFade(false);
        _bancoTMS.Atualizar.visible(true);
        _bancoTMS.Cancelar.visible(true);
        _bancoTMS.Excluir.visible(true);
        _bancoTMS.Adicionar.visible(false);
    }, null);
}

function limparCamposBancoTMS() {
    _bancoTMS.Atualizar.visible(false);
    _bancoTMS.Cancelar.visible(false);
    _bancoTMS.Excluir.visible(false);
    _bancoTMS.Adicionar.visible(true);
    LimparCampos(_bancoTMS);
}