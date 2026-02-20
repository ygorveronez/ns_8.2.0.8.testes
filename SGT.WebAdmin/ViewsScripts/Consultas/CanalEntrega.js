/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarCanaisEntrega = function (knout, callbackRetorno, basicGrid, callbackRetornoMultiplaEscolha, isCanalEntregaPrincipal, knoutFilial) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.CanalEntrega.BuscaDeCanaisDeEntrega, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.CanalEntrega.CanalDeEntrega, type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.CodigoIntegracao = PropertyEntity({ col: 4, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription() });
        this.Ativo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Status.getFieldDescription(), val: ko.observable(1), options: _statusPesquisa, def: 1, visible: false });
        this.FiltrarCanaisEntregaPrincipal = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isCanalEntregaPrincipal === true), visible: false });
        this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.CanalEntrega.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();

    if (knoutFilial != null) {
        knoutOpcoes.Filial.visible = false;
    }

    var funcaoParametroDinamico = function () {
        if (knoutFilial) {
            knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
            knoutOpcoes.Filial.val(knoutFilial.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, null, null, null, null, callbackRetornoMultiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "CanalEntrega/Pesquisa", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "CanalEntrega/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length === 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};