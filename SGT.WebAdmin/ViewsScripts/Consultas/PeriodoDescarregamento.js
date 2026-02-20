/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Globais.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/Rest.js" />

var BuscarPeriodoDescarregamento = function (knout, callbackRetorno, knoutData, knoutCentroDescarregamento, exibirFiltroDataDescarregamento) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Período de Descarregamento", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Períodos de Descarregamento", type: types.local });
        this.CentroDescarregamento = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Centro de Descarregamento:", idBtnSearch: guid(), visible: false });
        this.Data = PropertyEntity({ col: 4, getType: typesKnockout.date, text: "*Data de Descarregamento:", visible: Boolean(exibirFiltroDataDescarregamento)});
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = function () {
        LimparCampos(knoutOpcoes);

        knoutOpcoes.Data.val(knoutData ? knoutData.val() : Global.DataAtual());

        if (knoutOpcoes.Data.updateValue instanceof Function)
            knoutOpcoes.Data.updateValue();

        if (knoutCentroDescarregamento) {
            knoutOpcoes.CentroDescarregamento.codEntity(knoutCentroDescarregamento.codEntity());
            knoutOpcoes.CentroDescarregamento.val(knoutCentroDescarregamento.val());
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, false);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CentroDescarregamento/PesquisaPeriodoDescarregamento", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}
