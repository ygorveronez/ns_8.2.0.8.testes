/// <reference path="Cliente.js" />

var BuscarOrdemCompra = function (knout, callbackRetorno, basicGrid, situacao) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Pesquisa de Ordens de Compra", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Ordens de Compra", type: types.local });

        this.Numero = PropertyEntity({ col: 4, text: "Número:", getType: typesKnockout.int });
        this.Fornecedor = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
        this.NumeroCotacao = PropertyEntity({ col: 4, text: "Nº da Cotação:", getType: typesKnockout.int });

        this.Situacao = PropertyEntity({ val: ko.observable(situacao), def: situacao, visible: false });
        this.Ativo = PropertyEntity({ val: ko.observable(true), def: true, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarClientes(knoutOpcoes.Fornecedor);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    var url = "OrdemCompra/Pesquisa";

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.desc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Numero.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};

var BuscarOrdemCompraMercadoria = function (knout, callbackRetorno, basicGrid, knoutProduto, knoutOrdemCompra, situacao) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Pesquisa de Mercadoria na Ordem de Compra", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Mercadorias", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Produto:", getType: typesKnockout.string });
        this.Produto = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), visible: false });
        this.Codigo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Ordem de Compra:", idBtnSearch: guid(), visible: false });
        this.Situacao = PropertyEntity({ val: ko.observable(situacao), def: situacao, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutProduto != null && knoutOrdemCompra != null) {
        knoutOpcoes.Produto.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Produto.codEntity(knoutProduto.codEntity());
            knoutOpcoes.Produto.val(knoutProduto.codEntity());
            knoutOpcoes.Codigo.codEntity(knoutOrdemCompra.codEntity());
            knoutOpcoes.Codigo.val(knoutOrdemCompra.codEntity());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        //new BuscarVeiculos(knoutOpcoes.Veiculo);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    var url = "OrdemCompra/PesquisaMercadorias";

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 3, dir: orderDir.asc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};