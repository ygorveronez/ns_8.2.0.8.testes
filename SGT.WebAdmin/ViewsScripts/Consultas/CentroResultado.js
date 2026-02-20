/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarCentroResultado = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno, knoutTipoCentroResultado, knoutTipoMovimento, basicGrid, fnAfterDefaultCallback, somenteDoUsuario) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = basicGrid != null;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : Localization.Resources.Consultas.CentroResultado.PesquisaDeCentroDeResultado, type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : Localization.Resources.Consultas.CentroResultado.CentrosDeResultados, type: types.local });

        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.Plano = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.CentroResultado.Plano.getFieldDescription(), visible: !IsMobile() });
        this.Ativo = PropertyEntity({ col: 2, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), options: _statusPesquisa, def: 1, val: ko.observable(1), visible: !IsMobile() });

        this.Tipo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.TipoMovimento = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.CentroResultado.TipoMovimento, idBtnSearch: guid(), visible: false });
        this.SomenteDoUsuario = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(somenteDoUsuario != null ? somenteDoUsuario : false), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutTipoCentroResultado != null || knoutTipoMovimento != null) {
        funcaoParametroDinamico = function () {
            if (knoutTipoCentroResultado != null) {
                knoutOpcoes.Tipo.visible = false;
                knoutOpcoes.Tipo.codEntity(knoutTipoCentroResultado);
                knoutOpcoes.Tipo.val(knoutTipoCentroResultado);
            }

            if (knoutTipoMovimento != null) {
                knoutOpcoes.TipoMovimento.visible = false;
                knoutOpcoes.TipoMovimento.codEntity(knoutTipoMovimento.codEntity());
                knoutOpcoes.TipoMovimento.val(knoutTipoMovimento.val());
            }
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha);

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CentroResultado/Pesquisa", knoutOpcoes, null, { column: 2, dir: orderDir.asc }, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CentroResultado/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback, 22), { column: 2, dir: orderDir.asc });
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
};