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
/// <reference path="../../Enumeradores/EnumSituacaoAbastecimento.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Equipamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridReprocessarAbastecimento;
var _reprocessarAbastecimento;

var PesquisaReprocessarAbastecimento = function () {
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo: ", idBtnSearch: guid(), visible: true });
    this.Equipamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Equipamento: ", idBtnSearch: guid(), visible: true });
    this.SituacaoAbastecimento = PropertyEntity({ val: ko.observable(), options: EnumSituacaoAbastecimento.obterOpcoesReprocessarAbastecimento(), text: "Situação do Abastecimento: " });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, required: false, enable: ko.observable(true), val: ko.observable(""), def: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, required: false, enable: ko.observable(true), val: ko.observable(""), def: ko.observable("") });
    
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(true), enable: ko.observable(true) });
    this.Abastecimentos = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });
    this.ListaAbastecimentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridReprocessarAbastecimento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Reprocessar = PropertyEntity({ eventClick: ReprocessarClick, type: types.event, text: "Reprocessar", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******


function loadReprocessarAbastecimento() {
    _reprocessarAbastecimento = new PesquisaReprocessarAbastecimento();
    KoBindings(_reprocessarAbastecimento, "knockoutPesquisaReprocessarAbastecimento", false, _reprocessarAbastecimento.Pesquisar.id);

    new BuscarVeiculos(_reprocessarAbastecimento.Veiculo);
    new BuscarEquipamentos(_reprocessarAbastecimento.Equipamento);

    CriarGridReprocessarAbastecimento();
}

function CriarGridReprocessarAbastecimento() {
    var somenteLeitura = false;

    //_reprocessarAbastecimento.SelecionarTodos.visible(false);
    //_reprocessarAbastecimento.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _reprocessarAbastecimento.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    _gridReprocessarAbastecimento = new GridView(_reprocessarAbastecimento.Abastecimentos.idGrid, "ReprocessarAbastecimento/Pesquisa", _reprocessarAbastecimento, null, null, 15, null, null, null, multiplaescolha, 500);
    _gridReprocessarAbastecimento.CarregarGrid();
}

function buscarAbastecimentos() {
    _reprocessarAbastecimento.SelecionarTodos.visible(true);
    _reprocessarAbastecimento.SelecionarTodos.val(false);

    _gridReprocessarAbastecimento.CarregarGrid();
    _gridReprocessarAbastecimento.AtualizarRegistrosSelecionados([]);
}

//*******MÉTODOS*******

function ReprocessarClick(e) {
    var msgAviso = "Realmente deseja reprocessar o(s) abastecimento(s) selecionado(s)?";

    exibirConfirmacao("Confirmação", msgAviso, function () {
        var abastecimentosSelecionados = null;
        abastecimentosSelecionados = _gridReprocessarAbastecimento.ObterMultiplosSelecionados();

        var codigosAbastecimentos = new Array();
        for (var i = 0; i < abastecimentosSelecionados.length; i++)
            codigosAbastecimentos.push(abastecimentosSelecionados[i].DT_RowId);

        _reprocessarAbastecimento.ListaAbastecimentos.val(JSON.stringify(codigosAbastecimentos));

        executarReST("ReprocessarAbastecimento/ReprocessarAbastecimento", RetornarObjetoPesquisa(_reprocessarAbastecimento), function (r) {
            if (r.Success) {
                if (r.Data) {
                    buscarAbastecimentos();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Abastecimento(s) reprocessado(s)!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}