var BuscarCargaEntrega = function (knout, callbackRetorno, knoutCarga) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Controle de entrega", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Paradas da carga", type: types.local });
        this.Carga = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), visible: false, text: "Carga:", idBtnSearch: guid() });
        this.NotaFiscal = PropertyEntity({ getType: typesKnockout.int, col: 4, visible: true, text: "Nota Fiscal:", configInt: { thousands: "" } });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = null;
    if (knoutCarga != null) {
        knoutOpcoes.Carga.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.Carga.val(knoutCarga.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaEntrega/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Chave.val(knout.val());
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