/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarTipoGrot = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.TipoGrot.TiposGrot, type: types.local });
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.TipoGrot.BuscaTiposGrot, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.TipoGrot.Descricao.getFieldDescription() });
        this.Ativo = PropertyEntity({ text: Localization.Resources.Consultas.TipoGrot.Status.getFieldDescription(), val: ko.observable(1), options: _statusPesquisa, def: 1, visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.TipoGrot.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha);

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
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "ConfigCheckListUsuario/Pesquisa", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "ConfigCheckListUsuario/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });
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