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
/// <reference path="../../Enumeradores/EnumEstado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContingenciaMDFeEstado;
var _contingenciaMDFeEstado;
var _pesquisaContingenciaMDFeEstado;

var _tipoEmissaoContingencia = [
    { text: "1 - Normal", value: 1 },
    { text: "5 - Contingência MDFE", value: 5 },
];

var PesquisaContingenciaMDFeEstado = function () {
    this.Nome = PropertyEntity({ text: "Nome: " });
    this.Sigla = PropertyEntity({ text: "Sigla: ", val: ko.observable(EnumEstado.Todos), options: EnumEstado.obterOpcoesPesquisa(), def: EnumEstado.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContingenciaMDFeEstado.CarregarGrid();
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

var ContingenciaMDFeEstado = function () {
    this.Codigo = PropertyEntity({ text: "Sigla: ", val: ko.observable(""), def: "", enable: ko.observable(false)});
    this.Nome = PropertyEntity({ text: "Nome: ", enable: ko.observable(false) });
    this.TipoEmissaoMDFe = PropertyEntity({ text: "*Tipo Emissão: ", options: _tipoEmissaoContingencia, val: ko.observable(1), def: 1, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDContingenciaMDFeEstado = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadContingenciaMDFeEstado() {
    _contingenciaMDFeEstado = new ContingenciaMDFeEstado();
    KoBindings(_contingenciaMDFeEstado, "knockoutCadastroContingenciaMDFeEstado");

    HeaderAuditoria("Estado", _contingenciaMDFeEstado);

    _crudContingenciaMDFeEstado = new CRUDContingenciaMDFeEstado();
    KoBindings(_crudContingenciaMDFeEstado, "knockoutCRUDContingenciaMDFeEstado");

    _pesquisaContingenciaMDFeEstado = new PesquisaContingenciaMDFeEstado();
    KoBindings(_pesquisaContingenciaMDFeEstado, "knockoutPesquisaContingenciaMDFeEstado", false, _pesquisaContingenciaMDFeEstado.Pesquisar.id);

    buscarContingenciaMDFeEstado();
}

function atualizarClick(e, sender) {
    Salvar(_contingenciaMDFeEstado, "ContingenciaMDFeEstado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridContingenciaMDFeEstado.CarregarGrid();
                limparCamposContingenciaMDFeEstado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposContingenciaMDFeEstado();
}

//*******MÉTODOS*******


function buscarContingenciaMDFeEstado() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContingenciaMDFeEstado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridContingenciaMDFeEstado = new GridView(_pesquisaContingenciaMDFeEstado.Pesquisar.idGrid, "ContingenciaMDFeEstado/Pesquisa", _pesquisaContingenciaMDFeEstado, menuOpcoes, { column: 0, dir: orderDir.asc });
    _gridContingenciaMDFeEstado.CarregarGrid();
}

function editarContingenciaMDFeEstado(contingenciaMDFeEstadoGrid) {
    limparCamposContingenciaMDFeEstado();
    _contingenciaMDFeEstado.Codigo.val(contingenciaMDFeEstadoGrid.Codigo);
    BuscarPorCodigo(_contingenciaMDFeEstado, "ContingenciaMDFeEstado/BuscarPorCodigo", function (arg) {
        _pesquisaContingenciaMDFeEstado.ExibirFiltros.visibleFade(false);
        _crudContingenciaMDFeEstado.Atualizar.visible(true);
        _crudContingenciaMDFeEstado.Cancelar.visible(true);
    }, null);
}

function limparCamposContingenciaMDFeEstado() {
    _crudContingenciaMDFeEstado.Atualizar.visible(false);
    _crudContingenciaMDFeEstado.Cancelar.visible(false);
    LimparCampos(_contingenciaMDFeEstado);
}