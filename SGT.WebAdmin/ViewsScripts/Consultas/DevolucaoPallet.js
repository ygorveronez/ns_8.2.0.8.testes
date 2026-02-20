/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarDevolucaoSemValePallet = function (knout, callbackRetorno, knoutCarga) {
    var idDiv = guid();
    var GridConsulta;
    var focarNoPrimeiroInput = null;
    var multiplaEscolha = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Devoluções de Pallets", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Devoluções de Pallets", type: types.local });
        this.CargaOuNotaFiscal = PropertyEntity({ visible: false });
        this.NotaFiscal = PropertyEntity({ text: "Nota Fiscal:", col: 4, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
        this.DataEmissaoInicial = PropertyEntity({ text: "Emissão NF-e Início: ", col: 4, getType: typesKnockout.date });
        this.DataEmissaoFinal = PropertyEntity({ text: "Emissão NF-e Fim: ", col: 4, dateRangeInit: this.DataEmissaoInicial, getType: typesKnockout.date });
        this.Carga = PropertyEntity({ col: 4, text: "Carga" });
        this.Transportador = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid() });

        this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
        this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

        this.Pesquisar = PropertyEntity({ eventClick: function () { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar" });
    }

    var funcaoParamentroDinamico = null;
    var knoutOpcoes = new OpcoesKnout();

    if (knoutCarga) {
        knoutOpcoes.Carga.visible = false;

        funcaoParamentroDinamico = function () {
            knoutOpcoes.Carga.val(knoutCarga.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, focarNoPrimeiroInput, multiplaEscolha, function () {
        new BuscarTransportadores(knoutOpcoes.Transportador);
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Devolucao/PesquisaDevolucaoSemValePallet", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CargaOuNotaFiscal.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }

            knoutOpcoes.CargaOuNotaFiscal.val("");
        });
    });
}