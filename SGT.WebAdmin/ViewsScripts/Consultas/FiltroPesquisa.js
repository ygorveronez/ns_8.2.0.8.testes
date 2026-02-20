/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarFiltroPesquisa = function (knout, callbackRetorno, tipoFiltro, basicGrid) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);
    if (tipoFiltro == null)
        tipoFiltro = 1;

    var OpcoesKnout = function () {
        this.Filtro = PropertyEntity({ col: 12, text: "Descrição: " });
        this.TipoFiltro = PropertyEntity({ visible: false, val: ko.observable(true) });
        this.Titulo = PropertyEntity({ text: "Buscar Filtro Pesquisa", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Filtros de pesquisa", type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    knoutOpcoes.TipoFiltro.val(tipoFiltro);

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };

        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SessaoRoteirizador/ObterPreFiltrosMontagemCarregamento", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SessaoRoteirizador/ObterPreFiltrosMontagemCarregamento", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Situacao.val(knout.val());
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    })
}

var BuscarModeloFiltroPesquisa = function (knout, callbackRetorno, tipoFiltro, basicGrid) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);
    if (tipoFiltro == null)
        tipoFiltro = 1;

    var OpcoesKnout = function () {
        this.Modelo = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.TipoFiltro = PropertyEntity({ visible: false, val: ko.observable(tipoFiltro), def: tipoFiltro });
        this.Titulo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.BuscarFIltroPesquisa, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };

        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ModeloFiltroPesquisa/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ModeloFiltroPesquisa/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Situacao.val(knout.val());
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    })

    this.AbrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };
}