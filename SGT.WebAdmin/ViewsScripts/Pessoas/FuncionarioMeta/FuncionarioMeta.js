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
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="FuncionarioMetaTabela.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridFuncionarioMeta;
var _funcionarioMeta;
var _pesquisaFuncionarioMeta;

var PesquisaFuncionarioMeta = function () {
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid() });
    this.DataVigencia = PropertyEntity({ text: "Data Vigência: ", getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFuncionarioMeta.CarregarGrid();
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

var FuncionarioMeta = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });

    this.DataVigencia = PropertyEntity({ text: "*Data Vigência: ", getType: typesKnockout.date, required: ko.observable(true) });

    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });

    this.Metas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

var CRUDFuncionarioMeta = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadFuncionarioMeta() {
    _funcionarioMeta = new FuncionarioMeta();
    KoBindings(_funcionarioMeta, "knockoutCadastroFuncionarioMeta");

    HeaderAuditoria("FuncionarioMeta", _funcionarioMeta);

    _crudFuncionarioMeta = new CRUDFuncionarioMeta();
    KoBindings(_crudFuncionarioMeta, "knockoutCRUDFuncionarioMeta");

    _pesquisaFuncionarioMeta = new PesquisaFuncionarioMeta();
    KoBindings(_pesquisaFuncionarioMeta, "knockoutPesquisaFuncionarioMeta", false, _pesquisaFuncionarioMeta.Pesquisar.id);

    new BuscarFuncionario(_pesquisaFuncionarioMeta.Funcionario);
    new BuscarFuncionario(_funcionarioMeta.Funcionario);

    buscarFuncionarioMeta();

    loadFuncionarioMetaTabela();
}

function adicionarClick(e, sender) {
    Salvar(_funcionarioMeta, "FuncionarioMeta/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridFuncionarioMeta.CarregarGrid();
                limparCamposFuncionarioMeta();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_funcionarioMeta, "FuncionarioMeta/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridFuncionarioMeta.CarregarGrid();
                limparCamposFuncionarioMeta();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a tabela de metas?", function () {
        ExcluirPorCodigo(_funcionarioMeta, "FuncionarioMeta/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridFuncionarioMeta.CarregarGrid();
                limparCamposFuncionarioMeta();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposFuncionarioMeta();
}

//*******MÉTODOS*******


function buscarFuncionarioMeta() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFuncionarioMeta, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFuncionarioMeta = new GridView(_pesquisaFuncionarioMeta.Pesquisar.idGrid, "FuncionarioMeta/Pesquisa", _pesquisaFuncionarioMeta, menuOpcoes, null);
    _gridFuncionarioMeta.CarregarGrid();
}

function editarFuncionarioMeta(funcionarioMetaGrid) {
    limparCamposFuncionarioMeta();
    _funcionarioMeta.Codigo.val(funcionarioMetaGrid.Codigo);
    BuscarPorCodigo(_funcionarioMeta, "FuncionarioMeta/BuscarPorCodigo", function (arg) {
        _pesquisaFuncionarioMeta.ExibirFiltros.visibleFade(false);
        _crudFuncionarioMeta.Atualizar.visible(true);
        _crudFuncionarioMeta.Cancelar.visible(true);
        _crudFuncionarioMeta.Excluir.visible(true);
        _crudFuncionarioMeta.Adicionar.visible(false);

        RecarregarGridTabelaFuncionarioMeta();
    }, null);
}

function limparCamposFuncionarioMeta() {
    _crudFuncionarioMeta.Atualizar.visible(false);
    _crudFuncionarioMeta.Cancelar.visible(false);
    _crudFuncionarioMeta.Excluir.visible(false);
    _crudFuncionarioMeta.Adicionar.visible(true);
    LimparCampos(_funcionarioMeta);

    LimparCamposFuncionarioMetaTabela();
}