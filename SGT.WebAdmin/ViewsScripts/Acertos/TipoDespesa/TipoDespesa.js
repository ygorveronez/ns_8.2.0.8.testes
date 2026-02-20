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
/// <reference path="../../Enumeradores/EnumTipoDespesa.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoDespesa;
var _tipoDespesa;
var _pesquisaTipoDespesa;

var PesquisaTipoDespesa = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", maxlength: 50 });
    this.TipoDespesa = PropertyEntity({ val: ko.observable(0), options: _TiposDespesasPesquisa, def: 0 , text: "Tipo de Despesa: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoDespesa.CarregarGrid();
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

var TipoDespesa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.TipoDespesa = PropertyEntity({ val: ko.observable(true), options: _TiposDespesas, def: true, text: "*Tipo de Despesa: " });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", maxlength: 50 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadTipoDespesa() {

    _tipoDespesa = new TipoDespesa();
    KoBindings(_tipoDespesa, "knockoutCadastroTipoDespesa");

    _pesquisaTipoDespesa = new PesquisaTipoDespesa();
    KoBindings(_pesquisaTipoDespesa, "knockoutPesquisaTipoDespesa", false, _pesquisaTipoDespesa.Pesquisar.id);

    HeaderAuditoria("TipoDespesa", _tipoDespesa);

    buscarTiposDespesas();
}

function adicionarClick(e, sender) {
    Salvar(e, "TipoDespesa/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoDespesa.CarregarGrid();
                limparCamposTipoDespesa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "TipoDespesa/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoDespesa.CarregarGrid();
                limparCamposTipoDespesa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo de Despesa " + _tipoDespesa.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoDespesa, "TipoDespesa/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTipoDespesa.CarregarGrid();
                limparCamposTipoDespesa();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoDespesa();
}

//*******MÉTODOS*******


function buscarTiposDespesas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoDespesa, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoDespesa = new GridView(_pesquisaTipoDespesa.Pesquisar.idGrid, "TipoDespesa/Pesquisa", _pesquisaTipoDespesa, menuOpcoes, null);
    _gridTipoDespesa.CarregarGrid();
}

function editarTipoDespesa(tipoDespesaGrid) {
    limparCamposTipoDespesa();
    _tipoDespesa.Codigo.val(tipoDespesaGrid.Codigo);
    BuscarPorCodigo(_tipoDespesa, "TipoDespesa/BuscarPorCodigo", function (arg) {
        _pesquisaTipoDespesa.ExibirFiltros.visibleFade(false);
        _tipoDespesa.Atualizar.visible(true);
        _tipoDespesa.Cancelar.visible(true);
        _tipoDespesa.Excluir.visible(true);
        _tipoDespesa.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoDespesa() {
    _tipoDespesa.Atualizar.visible(false);
    _tipoDespesa.Cancelar.visible(false);
    _tipoDespesa.Excluir.visible(false);
    _tipoDespesa.Adicionar.visible(true);
    LimparCampos(_tipoDespesa);
}
