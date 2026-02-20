/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarTipoDeCargaDoPedido = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : "Pesquisa de Tipos de Carga", type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : "Tipos de Carga", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição: " });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);
    


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoDeCarga/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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
}
