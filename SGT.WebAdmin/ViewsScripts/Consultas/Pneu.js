/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../ViewsScripts/Consultas/ModeloPneu.js" />
/// <reference path="../../ViewsScripts/Consultas/BandaRodagemPneu.js" />
/// <reference path="../../ViewsScripts/Enumeradores/EnumVidaPneu.js" />

var BuscarPneu = function (knout, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pneu", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pneus", type: types.local });
        this.NumeroFogo = PropertyEntity({ text: "Número de Fogo:", col: 6, maxlength: 500 });
        this.VidaAtual = PropertyEntity({ text: "Vida Atual: ", col: 6, val: ko.observable(EnumVidaPneu.Todas), options: EnumVidaPneu.obterOpcoesPesquisa(), def: EnumVidaPneu.Todas });
        this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo:", col: 6, idBtnSearch: guid() });
        this.BandaRodagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banda de Rodagem:", col: 6, idBtnSearch: guid() });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, false, function () {
        new BuscarModeloPneu(knoutOpcoes.Modelo);
        new BuscarBandaRodagemPneu(knoutOpcoes.BandaRodagem);
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pneu/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroFogo.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}