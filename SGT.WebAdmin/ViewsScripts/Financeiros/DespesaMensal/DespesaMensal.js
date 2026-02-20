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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/TipoPagamentoRecebimento.js" />
/// <reference path="../../Consultas/TipoDespesaFinanceira.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridDespesaMensal;
var _despesaMensal;
var _pesquisaDespesaMensal;
var _crudDespesaMensal;

var PesquisaDespesaMensal = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDespesaMensal.CarregarGrid();
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

var DespesaMensal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 200 });
    this.Data = PropertyEntity({ text: "*Data Início Vigência: ", getType: typesKnockout.date, required: ko.observable(true) });
    this.DiaProvisao = PropertyEntity({ text: "*Dia Provisão: ", getType: typesKnockout.int, required: ko.observable(true), maxlength: 2 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.ValorProvisao = PropertyEntity({ text: "*Valor Provisão:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(true) });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.TipoPagamentoRecebimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Pagamento Recebimento:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Movimento para Geração do Título:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.TipoDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Despesa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
}

var CRUDDespesaMensal = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadDespesaMensal() {
    carregarLancamentoDespesaMensal("conteudoDespesaMensal", loadPesquisaDespesaMensal);
}

function loadPesquisaDespesaMensal() {
    HeaderAuditoria("DespesaMensal", _despesaMensal);

    _pesquisaDespesaMensal = new PesquisaDespesaMensal();
    KoBindings(_pesquisaDespesaMensal, "knockoutPesquisaDespesaMensal", false, _pesquisaDespesaMensal.Pesquisar.id);

    buscarDespesaMensal();
}

function carregarLancamentoDespesaMensal(idDivConteudo, callback) {
    $.get("Content/Static/Financeiro/DespesaMensal.html?dyn=" + guid(), function (dataConteudo) {
        $("#" + idDivConteudo).html(dataConteudo);

        _despesaMensal = new DespesaMensal();
        KoBindings(_despesaMensal, "knockoutDespesaMensal");

        _crudDespesaMensal = new CRUDDespesaMensal();
        KoBindings(_crudDespesaMensal, "knockoutCRUDDespesaMensal");

        new BuscarClientes(_despesaMensal.Pessoa);
        new BuscarTipoMovimento(_despesaMensal.TipoMovimento);
        new BuscarTipoDespesaFinanceira(_despesaMensal.TipoDespesa);
        new BuscarTipoPagamentoRecebimento(_despesaMensal.TipoPagamentoRecebimento);

        if (callback !== undefined && callback !== null)
            callback();
    });
}

function adicionarClick(e, sender) {
    Salvar(_despesaMensal, "DespesaMensal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                recarregarGridDespesaMensal();
                limparCamposDespesaMensal();
                Global.fecharModal("divModalDespesaMensal");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_despesaMensal, "DespesaMensal/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                recarregarGridDespesaMensal();
                limparCamposDespesaMensal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Despesa Mensal?", function () {
        ExcluirPorCodigo(_despesaMensal, "DespesaMensal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                recarregarGridDespesaMensal();
                limparCamposDespesaMensal();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposDespesaMensal();
}

//*******MÉTODOS*******


function buscarDespesaMensal() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarDespesaMensal, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridDespesaMensal = new GridView(_pesquisaDespesaMensal.Pesquisar.idGrid, "DespesaMensal/Pesquisa", _pesquisaDespesaMensal, menuOpcoes, null);
    _gridDespesaMensal.CarregarGrid();
}

function recarregarGridDespesaMensal() {
    if (_gridDespesaMensal !== undefined && _gridDespesaMensal !== null)
        _gridDespesaMensal.CarregarGrid();
}

function editarDespesaMensal(despesaMensalGrid) {
    limparCamposDespesaMensal();
    _despesaMensal.Codigo.val(despesaMensalGrid.Codigo);
    BuscarPorCodigo(_despesaMensal, "DespesaMensal/BuscarPorCodigo", function (arg) {
        _pesquisaDespesaMensal.ExibirFiltros.visibleFade(false);
        _crudDespesaMensal.Atualizar.visible(true);
        _crudDespesaMensal.Cancelar.visible(true);
        _crudDespesaMensal.Excluir.visible(true);
        _crudDespesaMensal.Adicionar.visible(false);
    }, null);
}

function limparCamposDespesaMensal() {
    if (_despesaMensal.Codigo.val() > 0) {
        _crudDespesaMensal.Atualizar.visible(false);
        _crudDespesaMensal.Cancelar.visible(false);
        _crudDespesaMensal.Excluir.visible(false);
        _crudDespesaMensal.Adicionar.visible(true);
    }
    LimparCampos(_despesaMensal);
}