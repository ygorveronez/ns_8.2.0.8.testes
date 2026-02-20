/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarContratosTransporteFrete = function (knout, callbackRetorno, basicGrid, knoutFiltrarPorTransportadorContrato, knoutTabelaFrete) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ContratosTransporteFrete.BuscarContratos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ContratosTransporteFrete.Contratos, type: types.local });

        this.NomeContrato = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.ContratosTransporteFrete.Descricao.getFieldDescription() });
        this.NumeroContrato = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.ContratosTransporteFrete.Numero.getFieldDescription() });
        this.Transportador = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ContratosTransporteFrete.Transportador.getFieldDescription(), idBtnSearch: guid() });
      
        this.FiltrarSomentePorAtivos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), visible: false });
        this.FiltrarPorTransportadorContrato = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: false });
        this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });

        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.ContratosTransporteFrete.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutTabelaFrete != null || knoutFiltrarPorTransportadorContrato != null) {
        funcaoParametroDinamico = function () {
            if (knoutTabelaFrete != null) {
                knoutOpcoes.TabelaFrete.codEntity(knoutTabelaFrete.codEntity());
                knoutOpcoes.TabelaFrete.val(knoutTabelaFrete.val());
                knoutOpcoes.FiltrarPorTransportadorContrato.val(true);
            }

            if (knoutFiltrarPorTransportadorContrato != null) {
                knoutOpcoes.FiltrarPorTransportadorContrato.val(knoutFiltrarPorTransportadorContrato.val());
            }
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, function () {
        new BuscarTransportadores(knoutOpcoes.Transportador);
    });

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoTransporteFrete/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoTransporteFrete/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback));
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NomeContrato.val(knout.val());
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