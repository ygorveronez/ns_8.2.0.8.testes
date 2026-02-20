/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarPerfilAcesso = function (knout, callbackRetorno, ativo, basicGrid) {
    var idDiv = guid();
    var GridConsulta;

    if (ativo == null)
        ativo = 1;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;
    

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.PerfilAcesso.BuscarPerfilDeAcesso, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.PerfilAcesso.PerfilDeAceso, type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: 12 });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(ativo), text: Localization.Resources.Gerais.Geral.Ativo.getFieldDescription(), visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);

        divBusca.CloseModal();
        if (callbackRetorno != null)
            callbackRetorno(e); 
    }

    var url = "PerfilAcesso/Pesquisa";

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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
    });
}

var BuscarPerfilAcessoChecklist = function (knout, callbackRetorno, ativo, basicGrid) {
    var idDiv = guid();
    var GridConsulta;

    if (ativo == null)
        ativo = 1;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;


    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.PerfilAcesso.BuscarPerfilDeAcesso, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.PerfilAcesso.PerfilDeAceso, type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: 12 });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(ativo), text: Localization.Resources.Gerais.Geral.Ativo.getFieldDescription(), visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout,  null, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PerfilAcesso/Pesquisa", knoutOpcoes, null, { column: 1, dir: orderDir.asc }, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PerfilAcesso/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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
    });
}