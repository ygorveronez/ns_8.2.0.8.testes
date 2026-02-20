var BuscarCTesTerceiro = function (knout, callbackRetorno) {

    let idDiv = guid();
    let gridConsulta;

    let OpcoesKnout = function () {
        let self = this;

        this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", col: 2 });
        this.NumeroFinal = PropertyEntity({ text: "Número Final: ", col: 2 });

        this.NumeroInicial.val.subscribe(function (novoValor) {
            if (string.IsNullOrWhiteSpace(self.NumeroFinal.val()))
                self.NumeroFinal.val(novoValor);
        });

        this.TipoCTe = PropertyEntity({ text: "Tipo do CT-e:", options: EnumTipoCTe.ObterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true), col: 2 });
        this.Emitente = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Emitente:", idBtnSearch: guid(), visible: true });
        this.Remetente = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: true });
        this.Destinatario = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: true });

        this.Titulo = PropertyEntity({ text: "Buscar CTe-s de Terceiros", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "CT-es de Terceiros", type: types.local });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });

        //this.BuscaAvancada = PropertyEntity({
        //    eventClick: function (e) {
        //        if (e.BuscaAvancada.visibleFade() == true) {
        //            e.BuscaAvancada.visibleFade(false);
        //            e.BuscaAvancada.icon("fal fa-plus");
        //        } else {
        //            e.BuscaAvancada.visibleFade(true);
        //            e.BuscaAvancada.icon("fal fa-minus");
        //        }
        //    }, buscaAvancada: true, type: types.event, text: "Avançada", idFade: guid(), cssClass: "btn btn-default", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
        //});
    }

    let knoutOpcoes = new OpcoesKnout();

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarClientes(knoutOpcoes.Remetente);
        new BuscarClientes(knoutOpcoes.Destinatario);
        new BuscarClientes(knoutOpcoes.Emitente);
    }, null, true);

    let callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    gridConsulta = new GridView(idDiv + "_tabelaEntidades", "CTeTerceiro/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroInicial.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}