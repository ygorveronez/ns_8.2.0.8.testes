/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumstatusColetaContainer.js" />

var BuscarJustificativaContainer = function (knout, callbackRetorno, basicGrid, knoutStatusColetaContainerOpcoes) {
    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = basicGrid != null;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Justificativas de Container", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Taxas de Terceiro", type: types.local });

        this.Descricao = PropertyEntity({ col: 8, text: "Descrição: " });
        this.StatusContainer = PropertyEntity({ col: 4, val: ko.observable(EnumStatusColetaContainer.Todas), options: ko.observable(EnumStatusColetaContainer.obterOpcoesPesquisa()), def: EnumStatusColetaContainer.Todas, text: "Situação: ", visible: true });

        this.DescricaoAtivo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = function () {
        if (knoutStatusColetaContainerOpcoes) {
            knoutOpcoes.StatusContainer.options(knoutStatusColetaContainerOpcoes.options());
            knoutOpcoes.StatusContainer.val(knoutStatusColetaContainerOpcoes.val());
            knoutOpcoes.StatusContainer.visible = false;
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "JustificativaContainer/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "JustificativaContainer/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback));
    }

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
};