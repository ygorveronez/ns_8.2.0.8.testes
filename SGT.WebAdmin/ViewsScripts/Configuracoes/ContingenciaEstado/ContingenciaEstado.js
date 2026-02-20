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

var _gridContingenciaEstado;
var _contingenciaEstado;
var _pesquisaContingenciaEstado;

var _tipoEmissaoContingencia = [
    { text: "1 - Normal", value: "1" },
    //{ text: "4 - EPEC pela SVC", value: "4" },
    { text: "5 - Contingência FSDA", value: "5" },
    { text: "7 - SVC-RS", value: "7" },
    { text: "8 - SVC-SP", value: "8" }
];

var _tipoEmissaoContingenciaSemFSDA = [
    { text: "1 - Normal", value: "1" },
    //{ text: "4 - EPEC pela SVC", value: "4" },
    { text: "7 - SVC-RS", value: "7" },
    { text: "8 - SVC-SP", value: "8" }
];

var PesquisaContingenciaEstado = function () {
    this.Nome = PropertyEntity({ text: "Nome: " });
    this.Sigla = PropertyEntity({ text: "Sigla: ", val: ko.observable(EnumEstado.Todos), options: EnumEstado.obterOpcoesPesquisa(), def: EnumEstado.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContingenciaEstado.CarregarGrid();
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

var ContingenciaEstado = function () {
    this.Codigo = PropertyEntity({ text: "Sigla: ", val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.Nome = PropertyEntity({ text: "Nome: ", enable: ko.observable(false) });
    this.TipoEmissao = PropertyEntity({ text: "*Tipo Emissão: ", val: ko.observable("1"), options: _CONFIGURACAO_TMS.HabilitarFSDA ? _tipoEmissaoContingencia : _tipoEmissaoContingenciaSemFSDA, def: "1", required: ko.observable(true), enable: ko.observable(true) });
    this.HabilitarContingenciaEPECAutomaticamente = PropertyEntity({ text: "Habilitar contingência EPEC automaticamente", getType: typesKnockout.bool, val: ko.observable(false), def: false });
};

var CRUDContingenciaEstado = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadContingenciaEstado() {

    if (_CONFIGURACAO_TMS.PermiteHabilitarContingenciaEPECAutomaticamente) {
        _tipoEmissaoContingenciaSemFSDA.push({ text: "4 - EPEC pela SVC", value: "4" });
        _tipoEmissaoContingencia.push({ text: "4 - EPEC pela SVC", value: "4" });
    }

    _contingenciaEstado = new ContingenciaEstado();
    KoBindings(_contingenciaEstado, "knockoutCadastroContingenciaEstado");

    HeaderAuditoria("Estado", _contingenciaEstado);

    _crudContingenciaEstado = new CRUDContingenciaEstado();
    KoBindings(_crudContingenciaEstado, "knockoutCRUDContingenciaEstado");

    _pesquisaContingenciaEstado = new PesquisaContingenciaEstado();
    KoBindings(_pesquisaContingenciaEstado, "knockoutPesquisaContingenciaEstado", false, _pesquisaContingenciaEstado.Pesquisar.id);

    buscarContingenciaEstado();   

}

function atualizarClick(e, sender) {
    Salvar(_contingenciaEstado, "ContingenciaEstado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridContingenciaEstado.CarregarGrid();
                limparCamposContingenciaEstado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposContingenciaEstado();
}

//*******MÉTODOS*******


function buscarContingenciaEstado() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContingenciaEstado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridContingenciaEstado = new GridView(_pesquisaContingenciaEstado.Pesquisar.idGrid, "ContingenciaEstado/Pesquisa", _pesquisaContingenciaEstado, menuOpcoes, { column: 0, dir: orderDir.asc });
    _gridContingenciaEstado.CarregarGrid();
}

function editarContingenciaEstado(contingenciaEstadoGrid) {
    limparCamposContingenciaEstado();
    _contingenciaEstado.Codigo.val(contingenciaEstadoGrid.Codigo);
    BuscarPorCodigo(_contingenciaEstado, "ContingenciaEstado/BuscarPorCodigo", function (arg) {
        _pesquisaContingenciaEstado.ExibirFiltros.visibleFade(false);
        _crudContingenciaEstado.Atualizar.visible(true);
        _crudContingenciaEstado.Cancelar.visible(true);
    }, null);
}

function limparCamposContingenciaEstado() {
    _crudContingenciaEstado.Atualizar.visible(false);
    _crudContingenciaEstado.Cancelar.visible(false);
    LimparCampos(_contingenciaEstado);
}