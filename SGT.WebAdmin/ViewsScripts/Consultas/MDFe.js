var BuscarMDFesSemCarga = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Numero = PropertyEntity({ text: "Número: ", col: 4 });
        this.Veiculo = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: "Situação: " });
        this.Titulo = PropertyEntity({ text: "Buscar MDFe-s sem Carga", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "MDF-es sem Carga", type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarVeiculos(knoutOpcoes.Veiculo);
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaPedidoDocumentoMDFe/ConsultarMDFesSemCarga", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Numero.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    })
}

var BuscarMDFes = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Numero = PropertyEntity({ text: "Número: ", col: 4 });
        this.Veiculo = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
        this.Carga = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: true });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: "Situação: " });
        this.Titulo = PropertyEntity({ text: "Buscar MDFe-s", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "MDF-es", type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarVeiculos(knoutOpcoes.Veiculo);
        new BuscarCargas(knoutOpcoes.Carga);
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaPedidoDocumentoMDFe/ConsultarMDFes", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Numero.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    })
}