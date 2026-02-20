/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../ViewsScripts/Enumeradores/EnumStatusPedidoVenda.js" />
/// <reference path="../../ViewsScripts/Enumeradores/EnumTipoPedidoVenda.js" />
/// <reference path="Cliente.js" />

var BuscarPedidosVendas = function (knout, callbackRetorno, menuOpcoes, basicGrid, status) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    if (status == null)
        status = EnumStatusPedidoVenda.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pedidos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pedidos", type: types.local });

        this.NumeroInicial = PropertyEntity({ col: 2, text: "Número Inicial: ", getType: typesKnockout.int, maxlength: 16 });
        this.NumeroFinal = PropertyEntity({ col: 2, text: "Número Final: ", getType: typesKnockout.int, maxlength: 16 });
        this.Pessoa = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: true });
        this.Status = PropertyEntity({ val: ko.observable(status), def: status, visible: false });
        this.Tipo = PropertyEntity({ val: ko.observable(-1), def: -1, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarClientes(knoutOpcoes.Pessoa);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    var opcoes = divBusca.OpcaoPadrao(callback)
    if (menuOpcoes != null) {
        opcoes = menuOpcoes;
    }
    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PedidoVenda/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PedidoVenda/Pesquisa", knoutOpcoes, opcoes, null);

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
    });
};