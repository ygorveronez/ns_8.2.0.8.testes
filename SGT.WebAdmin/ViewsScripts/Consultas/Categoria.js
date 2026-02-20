/// <reference path="../../wwwroot/js/Global/Buscas.js" />
/// <reference path="../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../wwwroot/js/libs/jquery-2.1.1.js" />

var BuscarCategoria = function (knout, callbackRetorno, basicGrid, basicGrid, fnAfterDefaultCallback) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = basicGrid != null;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Categorias", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Categorias Cadastradas", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição"});
        this.Situacao = PropertyEntity({ text: "Situação", val: ko.observable(1), def: 1, visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar, afterDefaultCallback: fnAfterDefaultCallback };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Categoria/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Categoria/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback));
    }

    GridConsulta.CarregarGrid(function (response) {
        const temCategorias = response && response.data && response.data.length > 0;
        knout.required = temCategorias;
        knout.text(temCategorias ? '*Categoria' : 'Categoria');
    });

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
};