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
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumCSTICMS.js" />
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Consultas/NaturezaOperacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridObservacaoFiscal;
var _observacaoFiscal;
var _pesquisaObservacaoFiscal;


var _cstICMS = [
    { text: "Selecione", value: 0 },
    { text: "CST 00", value: EnumCSTICMS.CST00 },
    { text: "CST 10", value: EnumCSTICMS.CST10 },
    { text: "CST 20", value: EnumCSTICMS.CST20 },
    { text: "CST 30", value: EnumCSTICMS.CST30 },
    { text: "CST 40", value: EnumCSTICMS.CST40 },
    { text: "CST 41", value: EnumCSTICMS.CST41 },
    { text: "CST 50", value: EnumCSTICMS.CST50 },
    { text: "CST 51", value: EnumCSTICMS.CST51 },
    { text: "CST 60", value: EnumCSTICMS.CST60 },
    { text: "CST 61", value: EnumCSTICMS.CST61 },
    { text: "CST 70", value: EnumCSTICMS.CST70 },
    { text: "CST 90", value: EnumCSTICMS.CST90 },
    { text: "CSOSN 101", value: EnumCSTICMS.CSOSN101 },
    { text: "CSOSN 102", value: EnumCSTICMS.CSOSN102 },
    { text: "CSOSN 103", value: EnumCSTICMS.CSOSN103 },
    { text: "CSOSN 201", value: EnumCSTICMS.CSOSN201 },
    { text: "CSOSN 202", value: EnumCSTICMS.CSOSN202 },
    { text: "CSOSN 203", value: EnumCSTICMS.CSOSN203 },
    { text: "CSOSN 300", value: EnumCSTICMS.CSOSN300 },
    { text: "CSOSN 400", value: EnumCSTICMS.CSOSN400 },
    { text: "CSOSN 500", value: EnumCSTICMS.CSOSN500 },
    { text: "CSOSN 900", value: EnumCSTICMS.CSOSN900 }
]

var PesquisaObservacaoFiscal = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridObservacaoFiscal.CarregarGrid();
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

var ObservacaoFiscal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ text: "*Observação: ", required: true, maxlength: 1000 });
    this.NCMProduto = PropertyEntity({ text: "NCM: ", required: false, maxlength: 8 });
    this.CSTICMS = PropertyEntity({ val: ko.observable(true), options: _cstICMS, val: ko.observable(0), def: ko.observable(0), text: "CST/CSOSN ICMS: " });

    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid(), required: false, visible: true });
    this.Atividade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Atividade:", idBtnSearch: guid(), required: false, visible: true });
    this.NaturezaDaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza de Operação:", idBtnSearch: guid(), required: false, visible: true });
    this.CFOPNotaFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP:", idBtnSearch: guid(), required: false, visible: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), required: false, visible: false });    

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadObservacaoFiscal() {

    _pesquisaObservacaoFiscal = new PesquisaObservacaoFiscal();
    KoBindings(_pesquisaObservacaoFiscal, "knockoutPesquisaObservacaoFiscal", false, _pesquisaObservacaoFiscal.Pesquisar.id);

    _observacaoFiscal = new ObservacaoFiscal();
    KoBindings(_observacaoFiscal, "knockoutCadastroObservacaoFiscal");

    HeaderAuditoria("ObservacaoFiscal", _observacaoFiscal);

    new BuscarCFOPNotaFiscal(_observacaoFiscal.CFOPNotaFiscal);
    new BuscarNaturezasOperacoesNotaFiscal(_observacaoFiscal.NaturezaDaOperacao);
    new BuscarAtividades(_observacaoFiscal.Atividade);
    new BuscarEstados(_observacaoFiscal.Estado);

    buscarObservacaoFiscals();
}

function adicionarClick(e, sender) {
    Salvar(e, "ObservacaoFiscal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridObservacaoFiscal.CarregarGrid();
                limparCamposObservacaoFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ObservacaoFiscal/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridObservacaoFiscal.CarregarGrid();
                limparCamposObservacaoFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a observação " + _observacaoFiscal.Observacao.val() + "?", function () {
        ExcluirPorCodigo(_observacaoFiscal, "ObservacaoFiscal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridObservacaoFiscal.CarregarGrid();
                limparCamposObservacaoFiscal();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposObservacaoFiscal();
}

//*******MÉTODOS*******


function buscarObservacaoFiscals() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarObservacaoFiscal, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridObservacaoFiscal = new GridView(_pesquisaObservacaoFiscal.Pesquisar.idGrid, "ObservacaoFiscal/Pesquisa", _pesquisaObservacaoFiscal, menuOpcoes, null);
    _gridObservacaoFiscal.CarregarGrid();
}

function editarObservacaoFiscal(observacaoFiscalGrid) {
    limparCamposObservacaoFiscal();
    _observacaoFiscal.Codigo.val(observacaoFiscalGrid.Codigo);
    BuscarPorCodigo(_observacaoFiscal, "ObservacaoFiscal/BuscarPorCodigo", function (arg) {
        _pesquisaObservacaoFiscal.ExibirFiltros.visibleFade(false);
        _observacaoFiscal.Atualizar.visible(true);
        _observacaoFiscal.Cancelar.visible(true);
        _observacaoFiscal.Excluir.visible(true);
        _observacaoFiscal.Adicionar.visible(false);
    }, null);
}

function limparCamposObservacaoFiscal() {
    _observacaoFiscal.Atualizar.visible(false);
    _observacaoFiscal.Cancelar.visible(false);
    _observacaoFiscal.Excluir.visible(false);
    _observacaoFiscal.Adicionar.visible(true);
    LimparCampos(_observacaoFiscal);
    _observacaoFiscal.CSTICMS.val(0);
}
