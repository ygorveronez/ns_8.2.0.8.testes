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


//*******MAPEAMENTO KNOUCKOUT*******

var _gridModeloDocumentoFiscal;
var _modeloDocumentoFiscal;
var _pesquisaModeloDocumentoFiscal;


var _statusModeloDocumentoFiscal = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var PesquisaModeloDocumentoFiscal = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Numero = PropertyEntity({ text: "Número: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridModeloDocumentoFiscal.CarregarGrid();
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
};

var ModeloDocumentoFiscal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 100 });
    this.Numero = PropertyEntity({ text: "*Número: ", required: true, maxlength: 10 });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusModeloDocumentoFiscal, def: "A", text: "*Status: ", required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadModeloDocumentoFiscal() {
    _pesquisaModeloDocumentoFiscal = new PesquisaModeloDocumentoFiscal();
    KoBindings(_pesquisaModeloDocumentoFiscal, "knockoutPesquisaModeloDocumentoFiscal", false, _pesquisaModeloDocumentoFiscal.Pesquisar.id);

    _modeloDocumentoFiscal = new ModeloDocumentoFiscal();
    KoBindings(_modeloDocumentoFiscal, "knockoutCadastroModeloDocumentoFiscal");

    HeaderAuditoria("ModeloDocumentoFiscal", _modeloDocumentoFiscal);

    buscarModeloDocumentoFiscals();
}

function adicionarClick(e, sender) {
    Salvar(e, "ModeloNotaFiscal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridModeloDocumentoFiscal.CarregarGrid();
                limparCamposModeloDocumentoFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ModeloNotaFiscal/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridModeloDocumentoFiscal.CarregarGrid();
                limparCamposModeloDocumentoFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o modelo " + _modeloDocumentoFiscal.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_modeloDocumentoFiscal, "ModeloNotaFiscal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridModeloDocumentoFiscal.CarregarGrid();
                limparCamposModeloDocumentoFiscal();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposModeloDocumentoFiscal();
}

//*******MÉTODOS*******


function buscarModeloDocumentoFiscals() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarModeloDocumentoFiscal, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridModeloDocumentoFiscal = new GridView(_pesquisaModeloDocumentoFiscal.Pesquisar.idGrid, "ModeloNotaFiscal/Pesquisa", _pesquisaModeloDocumentoFiscal, menuOpcoes, null);
    _gridModeloDocumentoFiscal.CarregarGrid();
}

function editarModeloDocumentoFiscal(modeloDocumentoFiscalGrid) {
    limparCamposModeloDocumentoFiscal();
    _modeloDocumentoFiscal.Codigo.val(modeloDocumentoFiscalGrid.Codigo);
    BuscarPorCodigo(_modeloDocumentoFiscal, "ModeloNotaFiscal/BuscarPorCodigo", function (arg) {
        _pesquisaModeloDocumentoFiscal.ExibirFiltros.visibleFade(false);
        _modeloDocumentoFiscal.Atualizar.visible(true);
        _modeloDocumentoFiscal.Cancelar.visible(true);
        _modeloDocumentoFiscal.Excluir.visible(true);
        _modeloDocumentoFiscal.Adicionar.visible(false);
    }, null);
}

function limparCamposModeloDocumentoFiscal() {
    _modeloDocumentoFiscal.Atualizar.visible(false);
    _modeloDocumentoFiscal.Cancelar.visible(false);
    _modeloDocumentoFiscal.Excluir.visible(false);
    _modeloDocumentoFiscal.Adicionar.visible(true);
    LimparCampos(_modeloDocumentoFiscal);
}
