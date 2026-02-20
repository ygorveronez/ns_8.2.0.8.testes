/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Globais.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/Rest.js" />

var BuscarPeriodoCarregamento = function (knout, callbackRetorno, knoutData, knoutCentroCarregamento, knoutFilial, knoutTipoCarga, exibirFiltroDataCarregamento) {
    var idDiv = guid();
    var GridConsulta;
     
    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.PeriodoCarregamento.BuscarPeriodoDeCarregamento, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.PeriodoCarregamento.PeriodosDeCarregamento, type: types.local });
        this.CentroCarregamento = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.PeriodoCarregamento.CentroDeCarregamento.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Filial = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.PeriodoCarregamento.Filial.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.TipoCarga = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.PeriodoCarregamento.TipoDeCarga.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Data = PropertyEntity({ col: 4, getType: typesKnockout.date, text: Localization.Resources.Consultas.PeriodoCarregamento.DataDeCarregamento.getRequiredFieldDescription(), visible: Boolean(exibirFiltroDataCarregamento) });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.PeriodoCarregamento.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = function () {
        knoutOpcoes.Data.val(knoutData ? knoutData.val() : Global.DataAtual());
        
        if (knoutCentroCarregamento) {
            knoutOpcoes.CentroCarregamento.codEntity(knoutCentroCarregamento.codEntity());
            knoutOpcoes.CentroCarregamento.val(knoutCentroCarregamento.val());
        }

        if (Boolean(knoutFilial) && Boolean(knoutTipoCarga)) {
            knoutOpcoes.TipoCarga.codEntity(knoutTipoCarga.codEntity());
            knoutOpcoes.TipoCarga.val(knoutTipoCarga.val());
            knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
            knoutOpcoes.Filial.val(knoutFilial.val());
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CentroCarregamento/PesquisaPeriodoCarregamento", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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

    this.buscarPrimeiro = function (dataCarregamentoFiltrarPeriodo) {
        funcaoParamentroDinamico();

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 0) {
                callback(undefined);
                return;
            }

            if ((lista.data.length == 1) || !Boolean(dataCarregamentoFiltrarPeriodo) || (dataCarregamentoFiltrarPeriodo.length <= 10)) {
                callback(lista.data[0]);
                return;
            }

            var dataBase = dataCarregamentoFiltrarPeriodo.substring(0, 10);
            var dataComparar = moment(Global.criarData(dataCarregamentoFiltrarPeriodo));

            for (var i = 0; i < lista.data.length; i++) {
                var periodo = lista.data[i];
                var dataInicio = moment(Global.criarData(dataBase + " " + periodo.HoraInicio));
                var dataTermino = moment(Global.criarData(dataBase + " " + periodo.HoraTermino));

                if ((dataInicio.diff(dataComparar) <= 0) && (dataTermino.diff(dataComparar) >= 0)) {
                    callback(periodo);
                    return;
                }
            }

            callback(lista.data[0]);
        });
    };

    this.Destroy = function () {
        divBusca.Destroy();
    };
}
