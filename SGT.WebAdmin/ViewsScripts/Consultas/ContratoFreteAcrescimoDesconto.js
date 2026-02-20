/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

let BuscarContratoFreteAcrescimoDesconto = function (knout, callbackRetorno, basicGrid, tipoJustificativa, codigoTransportadorContratoFreteOrigem) {

    let idDiv = guid();
    let GridConsulta;

    let multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    let OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: ko.observable(tipoJustificativa.val() == EnumTipoJustificativa.Acrescimo ? "Busca de acréscimos" : "Busca de descontos"), type: types.local });
        this.TituloGrid = PropertyEntity({ text: ko.observable(tipoJustificativa.val() == EnumTipoJustificativa.Acrescimo ? "Acréscimo" : "Desconto"), type: types.local });
        this.TipoJustificativa = PropertyEntity({ val: ko.observable(tipoJustificativa.val()), options: EnumTipoJustificativa.obterOpcoesPesquisa(), def: tipoJustificativa.val(), visible: false });
        this.CodigoTransportadorContratoFreteOrigem = PropertyEntity({ val: ko.observable(codigoTransportadorContratoFreteOrigem.val()), def: 0, getType: typesKnockout.int, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    let knoutOpcoes = new OpcoesKnout();
    let funcaoParamentroDinamico = function () {
        knoutOpcoes.TipoJustificativa.val(tipoJustificativa.val());
        knoutOpcoes.CodigoTransportadorContratoFreteOrigem.val(codigoTransportadorContratoFreteOrigem.val());
    }

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    let callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteAcrescimoDesconto/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteAcrescimoDesconto/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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
