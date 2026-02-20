/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />


var BuscarAcrescimoDescontoAutomatico = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;
    var buscaJustificativa;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.AcrescimoDescontoAutomatico.BuscarAcrescimosDescontosAutomaticos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.AcrescimoDescontoAutomatico.AcrescimosDescontosAutomaticos, type: types.local });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.AcrescimoDescontoAutomatico.Descricao, getType: typesKnockout.string });
        this.Justificativa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.AcrescimoDescontoAutomatico.Justificativa, idBtnSearch: guid() });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.AcrescimoDescontoAutomatico.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        buscaJustificativa = new BuscarJustificativas(knoutOpcoes.Justificativa, null, null, null, null);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteAcrescimoDescontoAutomatico/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContratoFreteAcrescimoDescontoAutomatico/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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

    this.Destroy = function () {
        if (buscaJustificativa)
            buscaJustificativa.Destroy();

        divBusca.Destroy();
    };
}