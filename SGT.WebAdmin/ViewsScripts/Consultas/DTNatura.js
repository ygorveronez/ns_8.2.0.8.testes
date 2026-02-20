var BuscarDTsNatura = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar DT da Natura", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "DT's da Natura", type: types.local });
        this.NumeroDocumentoTransporte = PropertyEntity({ col: 6, text: "Número do DT:", getType: typesKnockout.int });
        this.NumeroNotaFiscal = PropertyEntity({ col: 6, text: "Número da NF:", getType: typesKnockout.int });
        this.DataInicial = PropertyEntity({ col: 6, text: "Data Inicial:", getType: typesKnockout.date });
        this.DataFinal = PropertyEntity({ col: 6, text: "Data Final:", getType: typesKnockout.date });
        this.SemCarga = PropertyEntity({ visible: false, getType: typesKnockout.bool, val: ko.observable(true), def: true });

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
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "DocumentoTransporteNatura/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroDocumentoTransporte.val(knout.val());
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