/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Globais.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/Rest.js" />

var BuscarPeriodoDescarregamentoSugerido = function (knout, callbackRetorno, knoutDestinatario, knoutData) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Período de Descarregamento", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Períodos de Descarregamento", type: types.local });
        this.Destinatario = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Centro de Descarregamento:", idBtnSearch: guid(), visible: false });
        this.Data = PropertyEntity({ col: 4, getType: typesKnockout.date, text: "*Data de Descarregamento:", visible: true });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = function () {
        knoutOpcoes.Data.val(knoutData ? knoutData.val() : Global.DataAtual());

        if (knoutDestinatario) {
            knoutOpcoes.Destinatario.codEntity(knoutDestinatario.codEntity());
            knoutOpcoes.Destinatario.val(knoutDestinatario.val());
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CentroDescarregamento/PesquisaPeriodoDescarregamentoSugerido", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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
