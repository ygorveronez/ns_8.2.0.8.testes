/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var FaixasTemperatura = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno, basicGrid, afterDefaultCallback) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : "Pesquisar faixas de temperatura", type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : "Faixas de temperatura", type: types.local });
        this.Descricao = PropertyEntity({ col: 6, text: "Descrição" });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    } else if (afterDefaultCallback != null) {
        callback = function (e) {
            divBusca.DefCallback(e);
            afterDefaultCallback(e);
        }
    }

    var url = "Monitoramento/ObterFaixasTemperatura";

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
                Global.setarFocoProximoCampo(knout.id);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.Destroy = function () {
        divBusca.Destroy();
    };
}