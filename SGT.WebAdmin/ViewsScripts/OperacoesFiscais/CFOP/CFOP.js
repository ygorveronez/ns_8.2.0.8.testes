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
/// <reference path="../../Consultas/NaturezaOperacao.js" />
/// <reference path="../../Enumeradores/EnumTipoCFOP.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoCFOPConsulta = [{ text: "Todos", value: "" },
                         { text: "Entrada", value: EnumTipoCFOP.Entrada },
                         { text: "Saída", value: EnumTipoCFOP.Saida }];

var _tipoCFOP = [{ text: "Entrada", value: EnumTipoCFOP.Entrada },
                 { text: "Saída", value: EnumTipoCFOP.Saida }];

var _gridCFOP;
var _cfop;
var _pesquisaCFOP;

var PesquisaCFOP = function () {
    this.CFOP = PropertyEntity({ text: "CFOP: ", getType: typesKnockout.int, maxlength: 4, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Tipo = PropertyEntity({ val: ko.observable(0), options: _tipoCFOPConsulta, text: "Tipo: ", def: 0 });
    this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza de Operação: ", idBtnSearch: guid() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCFOP.CarregarGrid();
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

var CFOP = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CFOP = PropertyEntity({ text: "*CFOP: ",issue: 741, getType: typesKnockout.int, maxlength: 4, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Natureza da Operação:", issue: 740, required: true, idBtnSearch: guid() });
    this.Tipo = PropertyEntity({ val: ko.observable(0), options: _tipoCFOP, text: "*Tipo: ", def: 0 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadCFOP() {

    _cfop = new CFOP();
    KoBindings(_cfop, "knockoutCadastroCFOP");

    _pesquisaCFOP = new PesquisaCFOP();
    KoBindings(_pesquisaCFOP, "knockoutPesquisaCFOP", false, _pesquisaCFOP.Pesquisar.id);

    HeaderAuditoria("CFOP", _cfop);

    new BuscarNaturezasOperacoes(_cfop.NaturezaOperacao);
    new BuscarNaturezasOperacoes(_pesquisaCFOP.NaturezaOperacao);

    buscarCFOPs();

}

function adicionarClick(e, sender) {
    Salvar(e, "CFOP/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados salvos com sucesso!");
                _gridCFOP.CarregarGrid();
                limparCamposCFOP();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "CFOP/Salvar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados salvos com sucesso!");
            _gridCFOP.CarregarGrid();
            limparCamposCFOP();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposCFOP();
}

//*******MÉTODOS*******

function buscarCFOPs() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCFOP, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridCFOP = new GridView(_pesquisaCFOP.Pesquisar.idGrid, "CFOP/Consultar", _pesquisaCFOP, menuOpcoes, null);
    _gridCFOP.CarregarGrid();
}

function editarCFOP(cfopGrid) {
    limparCamposCFOP();
    _cfop.Codigo.val(cfopGrid.Codigo);
    BuscarPorCodigo(_cfop, "CFOP/BuscarPorCodigo", function (arg) {
        _pesquisaCFOP.ExibirFiltros.visibleFade(false);
        _cfop.Atualizar.visible(true);
        _cfop.Cancelar.visible(true);
        _cfop.Adicionar.visible(false);
    }, null);
}

function limparCamposCFOP() {
    _cfop.Atualizar.visible(false);
    _cfop.Cancelar.visible(false);
    _cfop.Adicionar.visible(true);
    LimparCampos(_cfop);
}

