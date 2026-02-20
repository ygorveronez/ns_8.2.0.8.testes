/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Consultas/DimensaoPneu.js" />
/// <reference path="../Consultas/MarcaPneu.js" />

var BuscarModeloPneu = function (knout, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Modelo de Rodagem de Pneu", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Modelos de Rodagem de Pneu", type: types.local });
        this.Descricao = PropertyEntity({ text: "Descrição:", col: 12 });
        this.Marca = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid() });
        this.Dimensao = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Dimensão:", idBtnSearch: guid() });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), text: "Ativo:", visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, false, function () {
        new BuscarMarcaPneu(knoutOpcoes.Marca);
        new BuscarDimensaoPneu(knoutOpcoes.Dimensao);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ModeloPneu/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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
    });
}