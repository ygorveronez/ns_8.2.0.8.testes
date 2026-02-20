/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarEnderecosProduto = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Endereços de Produtos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Endereços de Produtos", type: types.local });
        this.Descricao = PropertyEntity({ col: 6, text: "Descrição: " });
        this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarFilial(knoutOpcoes.Filial);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "EnderecoProduto/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "EnderecoProduto/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

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