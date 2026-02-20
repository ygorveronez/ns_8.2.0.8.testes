/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarMovimentacaoContaPagar = function (knout, callbackRetorno,baseGrid,knouttermoQuitacao) {
    var idDiv = guid();
    var GridConsulta;

 

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Pesquisa  Movimentações Conta Pagar", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Movimentações", type: types.local });

        this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", col: 4, visible: ko.observable(true) });
        this.Transportador = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, visible: false });
        this.DataInicio = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.date, visible: false });
        this.DataFinal = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.date, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var obterValoresKnout = null;
    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: knoutOpcoes.SelecionarTodos,
        callbackNaoSelecionado: function () { },
        callbackSelecionado: callbackRetorno != null ? callbackRetorno : function () {
            alert("Esta funciona")
        },
        callbackSelecionarTodos: function () { },
        somenteLeitura: false
    };

    if (knouttermoQuitacao != null) {
        obterValoresKnout = function () {
            knoutOpcoes.Transportador.val(knouttermoQuitacao.Transportador.codEntity());
            knoutOpcoes.DataInicio.val(knouttermoQuitacao.DataInicio.val());
            knoutOpcoes.DataFinal.val(knouttermoQuitacao.DataFinal.val());
        }
    }
    
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, obterValoresKnout, null, true);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            $("#" + idDiv).modal('hide');
            callbackRetorno(e);
        }
    }

    var url = "MovimentacaoContaPagar/PesquisarMovimentacao";
  
    if (multiplaescolha != null) {
        var objetoBasicGrid = { basicGrid: baseGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, 15, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc }, 15);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })

    this.Destroy = function () {
        divBusca.Destroy();
    };
}