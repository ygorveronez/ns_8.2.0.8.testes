var BuscarTipoComunicacaoRastreador = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ComunicacaoRastreador.BuscarTipoDeComunicacaoDoRastreador, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ComunicacaoRastreador.TiposDeComunicacaoDoRastreador, type: types.local });

        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.Ativo = PropertyEntity({ visible: false, def: 1, val: ko.observable(1) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoComunicacaoRastreador/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
};