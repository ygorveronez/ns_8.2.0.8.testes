/// <reference path="../Enumeradores/EnumSituacaoFechamentoFrete.js" />
/// <reference path="ContratoFreteTransportador.js" />
/// <reference path="Tranportador.js" />

var BuscarFechamentoFrete = function (knout, callbackRetorno, basicGrid) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Fechamentos de Frete", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Fechamentos de Frete", type: types.local });
        this.Numero = PropertyEntity({ text: "Número: ", col: 3 });
        this.DataInicio = PropertyEntity({ text: "Data Inicial: ", col: 3, getType: typesKnockout.date, visible: true });
        this.DataFim = PropertyEntity({ text: "Data Final: ", col: 3, getType: typesKnockout.date, visible: true });
        this.Situacao = PropertyEntity({ text: "Situação: ", col: 3, val: ko.observable(EnumSituacaoFechamentoFrete.Todas), options: EnumSituacaoFechamentoFrete.obterOpcoesPesquisa(), def: EnumSituacaoFechamentoFrete.Todas });
        this.Transportador = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? "Empresa/Filial:" : "Transportador:"), idBtnSearch: guid(), visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros) });
        this.Contrato = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Contrato de Frete", visible: true });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarContratoFreteTransportador(knoutOpcoes.Contrato);
        new BuscarTransportadores(knoutOpcoes.Transportador);
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

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "FechamentoFrete/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "FechamentoFrete/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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
