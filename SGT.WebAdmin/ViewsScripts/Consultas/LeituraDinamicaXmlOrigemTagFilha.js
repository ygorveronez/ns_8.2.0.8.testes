/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarLeituraDinamicaXmlOrigemTagFilha = function (knout, callbackRetorno, knouLeituraDinamicaXmlOrigem) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.LeituraDinamicaXmlOrigemTagFilha.Pesquisa, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.LeituraDinamicaXmlOrigemTagFilha.Descricao, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.CodigoLeituraDinamicaXmlOrigem = PropertyEntity({ val: ko.observable(0), def: 0, visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametroDinamico = function () {
        if (knouLeituraDinamicaXmlOrigem != null) {
            knoutOpcoes.CodigoLeituraDinamicaXmlOrigem.val(knouLeituraDinamicaXmlOrigem.codEntity());
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, false);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "LeituraDinamicaXmlOrigemTagFilha/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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