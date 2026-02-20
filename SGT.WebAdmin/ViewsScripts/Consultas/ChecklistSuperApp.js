/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Globais.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Rest.js" />

var BuscarChecklistsSuperApp = function (knout, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Checklists", type: types.local });
        this.TituloGrid = PropertyEntity({ text:"Checklists", type: types.local });
        this.Titulo = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Titulo.getFieldDescription() });
        this.Codigo = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription(), val: ko.observable(0), def: 0 });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Buscar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, false);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ChecklistSuperApp/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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
