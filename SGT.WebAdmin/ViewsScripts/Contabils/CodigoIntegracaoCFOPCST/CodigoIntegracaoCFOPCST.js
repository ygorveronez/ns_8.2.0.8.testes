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
/// <reference path="../../../ViewsScripts/Consultas/Localidade.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCodigoIntegracaoCFOPCST;
var _codigoIntegracaoCFOPCST;
var _pesquisaCodigoIntegracaoCFOPCST;

var PesquisaCodigoIntegracaoCFOPCST = function () {
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: " });
    this.CST = PropertyEntity({ text: "CST: " });
    this.CFOP = PropertyEntity({ text: "CFOP: " });    

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCodigoIntegracaoCFOPCST.CarregarGrid();
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

var CodigoIntegracaoCFOPCST = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CodigoIntegracao = PropertyEntity({ text: "*Código Integração: ", required: ko.observable(true), maxlength: 500 });
    this.CST = PropertyEntity({ text: "*CST: ", required: ko.observable(true), maxlength: 3 });
    this.CFOP = PropertyEntity({ text: "*CFOP: ", required: ko.observable(true), maxlength: 4 });
    
};

var CRUDCodigoIntegracaoCFOPCST = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadCodigoIntegracaoCFOPCST() {
    _codigoIntegracaoCFOPCST = new CodigoIntegracaoCFOPCST();
    KoBindings(_codigoIntegracaoCFOPCST, "knockoutCadastroCodigoIntegracaoCFOPCST");

    HeaderAuditoria("CodigoIntegracaoCFOPCST", _codigoIntegracaoCFOPCST);

    _crudCodigoIntegracaoCFOPCST = new CRUDCodigoIntegracaoCFOPCST();
    KoBindings(_crudCodigoIntegracaoCFOPCST, "knockoutCRUDCodigoIntegracaoCFOPCST");

    _pesquisaCodigoIntegracaoCFOPCST = new PesquisaCodigoIntegracaoCFOPCST();
    KoBindings(_pesquisaCodigoIntegracaoCFOPCST, "knockoutPesquisaCodigoIntegracaoCFOPCST", false, _pesquisaCodigoIntegracaoCFOPCST.Pesquisar.id);

    buscarCodigoIntegracaoCFOPCST();
}

function adicionarClick(e, sender) {
    Salvar(_codigoIntegracaoCFOPCST, "CodigoIntegracaoCFOPCST/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridCodigoIntegracaoCFOPCST.CarregarGrid();
                limparCamposCodigoIntegracaoCFOPCST();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_codigoIntegracaoCFOPCST, "CodigoIntegracaoCFOPCST/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCodigoIntegracaoCFOPCST.CarregarGrid();
                limparCamposCodigoIntegracaoCFOPCST();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o cadastro", function () {
        ExcluirPorCodigo(_codigoIntegracaoCFOPCST, "CodigoIntegracaoCFOPCST/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCodigoIntegracaoCFOPCST.CarregarGrid();
                    limparCamposCodigoIntegracaoCFOPCST();
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
    limparCamposCodigoIntegracaoCFOPCST();
}

//*******MÉTODOS*******

function buscarCodigoIntegracaoCFOPCST() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCodigoIntegracaoCFOPCST, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCodigoIntegracaoCFOPCST = new GridView(_pesquisaCodigoIntegracaoCFOPCST.Pesquisar.idGrid, "CodigoIntegracaoCFOPCST/Pesquisa", _pesquisaCodigoIntegracaoCFOPCST, menuOpcoes);
    _gridCodigoIntegracaoCFOPCST.CarregarGrid();
}

function editarCodigoIntegracaoCFOPCST(codigoIntegracaoCFOPCSTGrid) {
    limparCamposCodigoIntegracaoCFOPCST();
    _codigoIntegracaoCFOPCST.Codigo.val(codigoIntegracaoCFOPCSTGrid.Codigo);
    BuscarPorCodigo(_codigoIntegracaoCFOPCST, "CodigoIntegracaoCFOPCST/BuscarPorCodigo", function (arg) {
        _pesquisaCodigoIntegracaoCFOPCST.ExibirFiltros.visibleFade(false);
        _crudCodigoIntegracaoCFOPCST.Atualizar.visible(true);
        _crudCodigoIntegracaoCFOPCST.Cancelar.visible(true);
        _crudCodigoIntegracaoCFOPCST.Excluir.visible(true);
        _crudCodigoIntegracaoCFOPCST.Adicionar.visible(false);
    }, null);
}

function limparCamposCodigoIntegracaoCFOPCST() {
    _crudCodigoIntegracaoCFOPCST.Atualizar.visible(false);
    _crudCodigoIntegracaoCFOPCST.Cancelar.visible(false);
    _crudCodigoIntegracaoCFOPCST.Excluir.visible(false);
    _crudCodigoIntegracaoCFOPCST.Adicionar.visible(true);
    LimparCampos(_codigoIntegracaoCFOPCST);
}