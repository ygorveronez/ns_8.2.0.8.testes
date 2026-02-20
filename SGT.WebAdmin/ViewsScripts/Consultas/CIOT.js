/// <reference path="Sistema.js" />
/// <reference path="Modulo.js" />
/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../Enumeradores/EnumSituacaoCIOT.js" />

var BuscarCIOT = function (knout, callbackRetorno, menuOpcoes, situacoes, tiposTransportador) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar CIOT", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "CIOT", type: types.local });
        this.Numero = PropertyEntity({ col: 2, text: "Número: " });
        this.Transportador = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
        this.Veiculo = PropertyEntity({ col: 2, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
        this.Motorista = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
        this.Situacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(JSON.stringify(situacoes)), idGrid: guid(), visible: false });
        this.TiposTransportador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(JSON.stringify(tiposTransportador)), idGrid: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;


    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {
        BuscarClientes(knoutOpcoes.Transportador, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);
        BuscarVeiculos(knoutOpcoes.Veiculo, null, null, null, knoutOpcoes.Motorista, null, null, null, null, null, null, null, null, null, null, null, null, null, knoutOpcoes.Transportador, "0");
        BuscarMotoristas(knoutOpcoes.Motorista);
    });
    
    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        }
    }

    var opcoes = divBusca.OpcaoPadrao(callback)
    if (menuOpcoes != null) {
        opcoes = menuOpcoes;
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CIOT/Pesquisa", knoutOpcoes, opcoes, null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
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