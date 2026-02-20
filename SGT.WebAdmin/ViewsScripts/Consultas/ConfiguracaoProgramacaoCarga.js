/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../Consultas/Filial.js" />
/// <reference path="../Consultas/TipoCarga.js" />
/// <reference path="../Consultas/TipoOperacao.js" />

var BuscarConfiguracaoProgramacaoCarga = function (knout, callbackRetorno, basicGrid, knoutFilial) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = Boolean(basicGrid);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ConfiguracaoDeProgramacaoDeCarga.BuscarConfiguracaoProgramacaoCarga, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ConfiguracaoDeProgramacaoDeCarga.ConfiguracoesProgramacaoCarga, type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Consultas.ConfiguracaoDeProgramacaoDeCarga.Descricao, col: 6 });
        this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ConfiguracaoDeProgramacaoDeCarga.Filial, col: 6, idBtnSearch: guid(), visible: !Boolean(knoutFilial) });
        this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ConfiguracaoDeProgramacaoDeCarga.TipoCarga, col: 6, idBtnSearch: guid(), visible: !Boolean(knoutFilial) });
        this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ConfiguracaoDeProgramacaoDeCarga.TipoOperacao, col: 6, idBtnSearch: guid(), visible: !Boolean(knoutFilial) });
        this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ConfiguracaoDeProgramacaoDeCarga.ModeloVeicular, col: 6, idBtnSearch: guid(), visible: !Boolean(knoutFilial) });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.ConfiguracaoDeProgramacaoDeCarga.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametrosDinamicos = function () {
        if (knoutFilial) {
            knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
            knoutOpcoes.Filial.val(knoutFilial.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, multiplaEscolha, function () {
        new BuscarFilial(knoutOpcoes.Filial);
        new BuscarTiposdeCarga(knoutOpcoes.TipoCarga);
        new BuscarTiposOperacao(knoutOpcoes.TipoOperacao);
        new BuscarModelosVeicularesCarga(knoutOpcoes.ModeloVeicularCarga);
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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ConfiguracaoProgramacaoCarga/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ConfiguracaoProgramacaoCarga/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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
}
