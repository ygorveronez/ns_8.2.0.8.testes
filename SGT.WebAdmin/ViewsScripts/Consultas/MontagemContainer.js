/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="TipoContainer.js" />
/// <reference path="Container.js" />

var BuscarMontagemContainer = function (knout, callbackRetorno, status) {
    var idDiv = guid();
    var gridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.MontagemContainer.BuscarMontagemDeContainer, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.MontagemContainer.MontagensDeContainer, type: types.local });
        this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Consultas.MontagemContainer.NumeroBooking.getFieldDescription(), col: 12 });
        this.Status = PropertyEntity({ text: Localization.Resources.Consultas.MontagemContainer.Situacao.getFieldDescription(), col: 12, visible: false });
        this.TipoContainer = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.MontagemContainer.TipoContainer.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Container = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.MontagemContainer.Container.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), text: Localization.Resources.Consultas.MontagemContainer.Ativo.getFieldDescription(), visible: false });
        
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.MontagemContainer.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametroDinamico = null;

    if (status != null) {
        funcaoParametroDinamico = function () {
            knoutOpcoes.Status.val(status);
        };
    }
    
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, null, function () {
        new BuscarTiposContainer(knoutOpcoes.TipoContainer);
        new BuscarContainers(knoutOpcoes.Container);
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

    gridConsulta = new GridView(idDiv + "_tabelaEntidades", "MontagemContainer/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroBooking.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}