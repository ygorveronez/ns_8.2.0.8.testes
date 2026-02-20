/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="MotivoCompra.js" />

var BuscarRequisicaoCompra = function (knout, callbackRetorno, menuOpcoes, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Requisição Compra", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Requisições", type: types.local });

        this.NumeroInicial = PropertyEntity({ col: 6, text: "Número Inicial: ", getType: typesKnockout.int, maxlength: 16 });
        this.NumeroFinal = PropertyEntity({ col: 6, text: "Número Final: ", getType: typesKnockout.int, maxlength: 16 });
        this.Motivo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid(), visible: true });
        this.Modo = PropertyEntity({ val: ko.observable(2), def: 2, text: "*Modo: ", visible: false });        
        this.Situacao = PropertyEntity({ val: ko.observable(1), def: 1, text: "*Situacao: ", visible: false });        
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarMotivoCompra(knoutOpcoes.Motivo);
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
    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "RequisicaoMercadoria/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "RequisicaoMercadoria/Pesquisa", knoutOpcoes, opcoes, null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
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