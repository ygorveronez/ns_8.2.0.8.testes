/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarValePedagio = function (knout, callbackRetorno, basicGrid) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.NumeroValePedagio = PropertyEntity({ text: "Vale Pedágio", col: 12 });
        this.SituacaoIntegracao = PropertyEntity({ visible: false, text: "Situação Integração Vale Pedágio", val: ko.observable(EnumSituacaoIntegracao.Integrado)});
        this.Titulo = PropertyEntity({ text: "Vale Pedágio", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Vale Pedágio", type: types.local });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback();
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };

        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ValePedagio/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ValePedagio/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.DescricaoOuCodigoIntegracao.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });

    this.abrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };
}