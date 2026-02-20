/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarPagamentosFinalizados = function (knout, callbackRetorno, pagamentoLiberado) {

    var idDiv = guid();
    var GridConsulta;

    if (pagamentoLiberado == null)
        pagamentoLiberado = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pagamentos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motivos do Adicional de Frete", type: types.local });
        this.Numero = PropertyEntity({ getType: typesKnockout.int, text: "Número do Pagamento:", col: 3 });
        this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(EnumSituacaoPagamento.Finalizado), visible: false });
        this.PagamentoLiberado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(pagamentoLiberado), visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pagamento/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}