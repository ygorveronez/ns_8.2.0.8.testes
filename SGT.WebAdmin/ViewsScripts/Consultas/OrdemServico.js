/// <reference path="../../ViewsScripts/Enumeradores/EnumSituacaoOrdemServicoFrota.js" />

var BuscarOrdemServico = function (knout, callbackRetorno, basicGrid, situacoes) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var situacao = [];
    if (situacoes != null)
        situacao = situacoes;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Pesquisa de Ordens de Serviço", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Ordens de Serviço", type: types.local });

        this.NumeroInicial = PropertyEntity({ col: 3, text: "Número Inicial:", getType: typesKnockout.string });
        this.NumeroFinal = PropertyEntity({ col: 3, text: "Número Final:", getType: typesKnockout.int });
        this.Veiculo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
        this.Situacao = PropertyEntity({ col: 6, text: "Situação:", val: ko.observable(situacao), def: situacao, getType: typesKnockout.selectMultiple, options: EnumSituacaoOrdemServicoFrota.ObterOpcoes(), visible: true });
        this.Ativo = PropertyEntity({ val: ko.observable(true), def: true, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    if (situacoes != null)
        knoutOpcoes.Situacao.visible = false;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarVeiculos(knoutOpcoes.Veiculo);
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

    var url = "OrdemServico/Pesquisa";

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroInicial.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};