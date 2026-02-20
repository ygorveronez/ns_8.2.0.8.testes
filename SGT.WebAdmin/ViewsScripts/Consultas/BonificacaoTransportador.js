/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarBonificacaoTransportador = function (knout, callbackRetorno, basicGrid, knoutFilial) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Bonificação Transportador", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Bonificação Transportador", type: types.local });

        this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid() });
        this.Empresa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
        this.TipoDeCarga = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
        this.DataInicial = PropertyEntity({ col: 3, text: "Data Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
        this.DataFinal = PropertyEntity({ col: 3, text: "Data Final:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
        this.DataInicial.dateRangeLimit = this.DataFinal;
        this.DataFinal.dateRangeInit = this.DataInicial;

        this.Ativo = PropertyEntity({ visible: false, def: 1, val: ko.observable(1) });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutFilial != null) {
        funcaoParametroDinamico = function () {
            if (knoutFilial != null) {
                knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
                knoutOpcoes.Filial.val(knoutFilial.val());
            }
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, function () {
        new BuscarFilial(knoutOpcoes.Filial);
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarTiposdeCarga(knoutOpcoes.TipoDeCarga);
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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "BonificacaoTransportador/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "BonificacaoTransportador/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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
    })
}