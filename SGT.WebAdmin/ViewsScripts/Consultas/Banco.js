/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarBanco = function (knout, tituloOpcional, tituloGridOpcional, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: tituloOpcional != null ? tituloOpcional : Localization.Resources.Consultas.Banco.ConsultaDeBancos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGridOpcional != null ? tituloGridOpcional : Localization.Resources.Consultas.Banco.ListaDeBancos, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Banco/Pesquisar", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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