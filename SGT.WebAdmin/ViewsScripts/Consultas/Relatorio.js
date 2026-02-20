/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarRelatorio = function (knout, codigoControleRelatorios, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Descricao = PropertyEntity({ text: Localization.Resources.Consultas.Relatorios.Descricao, col: 12 });
        this.CodigoControleRelatorios = PropertyEntity({ val: ko.observable(codigoControleRelatorios), def: codigoControleRelatorios, visible: false });
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Relatorios.BuscarModelosRelatorios, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Relatorios.Relatorio, type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.Relatorios.Pesquisar, visible: true
        });
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


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Relatorios/Relatorio/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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