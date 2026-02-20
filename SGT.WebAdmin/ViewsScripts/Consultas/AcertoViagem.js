/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Motorista.js" />



var BuscarAcertoViagem = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Acertos de Viagem", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Acertos de Viagens", type: types.local });
        this.NumeroAcerto = PropertyEntity({ col: 2, text: "Número: ", maxlength: 8, type: typesKnockout.int });
        this.Motorista = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
        this.DataAcerto = PropertyEntity({ col: 4, text: "Data Emissão: ", type: typesKnockout.date, visible: true });
        this.DataInicial = PropertyEntity({ col: 6, text: "Data Inicial: ", type: typesKnockout.date, visible: true });
        this.DataFinal = PropertyEntity({ col: 6, text: "Data Final: ", type: typesKnockout.date, visible: true });        
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {        
        new BuscarMotoristas(knoutOpcoes.Motorista);        
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "AcertoViagem/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroAcerto.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}