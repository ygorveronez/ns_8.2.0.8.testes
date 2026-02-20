/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />

var BuscarPrevisaoCarregamento = function (knout, koCentroCarregamento, koDataReserva, callbackRetorno) {

    var idDiv = guid();
    var gridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Previsão de Carregamento", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Previsões de Carregamento", type: types.local });
        this.Rota = PropertyEntity({ col: 6, text: "Rota: " });
        this.Descricao = PropertyEntity({ col: 6, text: "Descrição: " });
        this.CentroCarregamento = PropertyEntity({ visible: false, type: types.entity, codEntity: ko.observable(0) });
        this.DataReserva = PropertyEntity({ visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var funcaoParamentroDinamico = function () {
        if (koCentroCarregamento != null) {
            knoutOpcoes.CentroCarregamento.codEntity(koCentroCarregamento.codEntity());
            knoutOpcoes.CentroCarregamento.val(koCentroCarregamento.val());
        }
        if (koDataReserva != null) {
            knoutOpcoes.DataReserva.val(koDataReserva.val());
        }
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.DefCallback(e);
            callbackRetorno(e);
        }
    }

    gridConsulta = new GridView(idDiv + "_tabelaEntidades", "ReservaCargaGrupoPessoa/PrevisoesDisponiveis", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
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