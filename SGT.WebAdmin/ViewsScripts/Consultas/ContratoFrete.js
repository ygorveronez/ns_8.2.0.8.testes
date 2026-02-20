/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Cliente.js" />

var BuscarContratoFrete = function (knout, callbackRetorno, basicGrid, situacoes, numeroCIOT) {

    var idDiv = guid();
    var GridConsulta;

    var situacao = situacoes != null ? JSON.stringify([].concat(situacoes)) : "";

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ContratoFrete.BuscarContratosFrete, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ContratoFrete.ContratosFrete, type: types.local });

        this.NumeroContrato = PropertyEntity({ col: 3, text: Localization.Resources.Consultas.ContratoFrete.NumeroContrato.getFieldDescription() });
        this.Carga = PropertyEntity({ col: 3, text: Localization.Resources.Consultas.ContratoFrete.Carga.getFieldDescription() });
        this.TransportadorTerceiro = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text:"Teceiro", idBtnSearch: guid() });
        this.SituacaoContratoFrete = PropertyEntity({ val: ko.observable(situacao), def: situacao, visible: false });
        this.NumeroCIOT = PropertyEntity({ text: "Número CIOT: ", visible: false });
        this.DataInicialContratoFrete = PropertyEntity({ text: "Data Inicial: "  });
        this.DataFinalContratoFrete = PropertyEntity({ text: "Data Final: ", visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.ContratoFrete.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (numeroCIOT != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.NumeroCIOT.val(numeroCIOT.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarClientes(knoutOpcoes.TransportadorTerceiro);
    }, null, true);

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFrete/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFrete/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroContrato.val(knout.val());
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