var BuscarCargaPedidoParaEncaixeDeSubcontratacao = function (knout, callbackRetorno, gridCallback) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.CodigoPedidoEmbarcador = PropertyEntity({ text: "Número do Pedido: ", col: 4 });
        this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 4 });
        this.NumeroNF = PropertyEntity({ text: "Número da NF: ", col: 4 });
        this.EstadoDestino = PropertyEntity({ type: types.entity, col: 12, codEntity: ko.observable(0), required: false, text: "Estado de Destino:", idBtnSearch: guid() });
        this.Titulo = PropertyEntity({ text: "Buscar Pedidos para Encaixe", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pedidos", type: types.local });
        

        this.Situacao = PropertyEntity({ text: "Situação: ", col: 12, visible: false, val: ko.observable(11) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarEstados(knoutOpcoes.EstadoDestino);
    }, null, null, null, gridCallback);

    knoutOpcoes.Situacao.val(11);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }
    //var callback = function (e) {
    //    knout.codEntity(e.Codigo);
    //    knout.val(e.OrigemDestino);
    //    knoutOpcoes.CodigoCargaEmbarcador.val(knoutOpcoes.CodigoCargaEmbarcador.def);
    //    $("#" + idDiv).modal('hide');
    //    Global.setarFocoProximoCampo(knout.id);
    //};

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }
  

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Carga/PesquisaCargasPedidoParaEncaixeDeSubcontratacao", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CodigoCargaEmbarcador.val(knout.val());
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
