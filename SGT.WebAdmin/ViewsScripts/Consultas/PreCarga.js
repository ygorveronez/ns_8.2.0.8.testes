/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Consultas/Filial.js" />
/// <reference path="../Consultas/ModeloVeicularCarga.js" />

var BuscarPreCarga = function (knout, callbackRetorno, knoutFilial, somenteSemCarregamento) {
    var idDiv = guid();
    var GridConsulta;
    var FiltrosDinamicos = {};

    if (somenteSemCarregamento == null)
        somenteSemCarregamento = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.PreCarga.BuscarPreCarga, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.PreCarga.PreCargas, type: types.local });
        this.Filial = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.PreCarga.Filial.getFieldDescription(), idBtnSearch: guid(), visible: !knoutFilial });
        this.ModeloVeicularCarga = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.PreCarga.ModeloVeicular.getFieldDescription(), idBtnSearch: guid() });
        this.PreCarga = PropertyEntity({ text: Localization.Resources.Consultas.PreCarga.PreCargaRota, col: 4 });
        this.SemCarregamento = PropertyEntity({ visible: false, col: 0, val: ko.observable(somenteSemCarregamento) });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = function () {
        if (knoutFilial) {
            knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
            knoutOpcoes.Filial.val(knoutFilial.val());
        }

        for (var prop in FiltrosDinamicos) {
            knoutOpcoes[prop] = PropertyEntity({ visible: false, col: 0, val: ko.observable(FiltrosDinamicos[prop]) });
        }
    };
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {
        new BuscarFilial(knoutOpcoes.Filial);
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PreCarga/PesquisaPreCarga", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.PreCarga.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    return {
        SetFiltro: function (nome, valor) {
            FiltrosDinamicos[nome] = valor;
        }
    }
}