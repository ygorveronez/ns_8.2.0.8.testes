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
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Usuario.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridResponsavelVeiculo;
var _responsavelVeiculo;
var _pesquisaResponsavelVeiculo;

var PesquisaResponsavelVeiculo = function () {
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Responsável:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridResponsavelVeiculo.CarregarGrid();
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
};

var ResponsavelVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 5000, enable: ko.observable(true) });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário Responsável:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
};

var CRUDResponsavelVeiculo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadResponsavelVeiculo() {
    _responsavelVeiculo = new ResponsavelVeiculo();
    KoBindings(_responsavelVeiculo, "knockoutCadastroResponsavelVeiculo");

    HeaderAuditoria("ResponsavelVeiculo", _responsavelVeiculo);

    _crudResponsavelVeiculo = new CRUDResponsavelVeiculo();
    KoBindings(_crudResponsavelVeiculo, "knockoutCRUDResponsavelVeiculo");

    _pesquisaResponsavelVeiculo = new PesquisaResponsavelVeiculo();
    KoBindings(_pesquisaResponsavelVeiculo, "knockoutPesquisaResponsavelVeiculo", false, _pesquisaResponsavelVeiculo.Pesquisar.id);

    new BuscarVeiculos(_pesquisaResponsavelVeiculo.Veiculo);
    new BuscarFuncionario(_pesquisaResponsavelVeiculo.FuncionarioResponsavel);
    new BuscarVeiculos(_responsavelVeiculo.Veiculo);
    new BuscarFuncionario(_responsavelVeiculo.FuncionarioResponsavel);

    buscarResponsavelVeiculo();
}

function adicionarClick(e, sender) {
    Salvar(_responsavelVeiculo, "ResponsavelVeiculo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridResponsavelVeiculo.CarregarGrid();
                limparCamposResponsavelVeiculo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    executarReST("ResponsavelVeiculo/Excluir", { Codigo: _responsavelVeiculo.Codigo.val() }, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro excluído com sucesso.");
            _gridResponsavelVeiculo.CarregarGrid();
            limparCamposResponsavelVeiculo();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function cancelarClick(e) {
    limparCamposResponsavelVeiculo();
}

//*******MÉTODOS*******


function buscarResponsavelVeiculo() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarResponsavelVeiculo, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridResponsavelVeiculo = new GridView(_pesquisaResponsavelVeiculo.Pesquisar.idGrid, "ResponsavelVeiculo/Pesquisa", _pesquisaResponsavelVeiculo, menuOpcoes, null);
    _gridResponsavelVeiculo.CarregarGrid();
}

function editarResponsavelVeiculo(responsavelVeiculoGrid) {
    limparCamposResponsavelVeiculo();
    _responsavelVeiculo.Codigo.val(responsavelVeiculoGrid.Codigo);
    BuscarPorCodigo(_responsavelVeiculo, "ResponsavelVeiculo/BuscarPorCodigo", function (arg) {
        _pesquisaResponsavelVeiculo.ExibirFiltros.visibleFade(false);
        _crudResponsavelVeiculo.Cancelar.visible(true);
        _crudResponsavelVeiculo.Excluir.visible(true);
        _crudResponsavelVeiculo.Adicionar.visible(false);
        SetarEnableCamposKnockout(_responsavelVeiculo, false);
    }, null);
}

function limparCamposResponsavelVeiculo() {
    _crudResponsavelVeiculo.Cancelar.visible(false);
    _crudResponsavelVeiculo.Excluir.visible(false);
    _crudResponsavelVeiculo.Adicionar.visible(true);
    LimparCampos(_responsavelVeiculo);
    SetarEnableCamposKnockout(_responsavelVeiculo, true);
}