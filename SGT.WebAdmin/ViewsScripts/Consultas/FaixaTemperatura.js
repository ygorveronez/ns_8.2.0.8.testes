/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Consultas/TipoOperacao.js" />

var BuscarFaixaTemperatura = function (knout, callbackRetorno, knoutTipoOperacao, carga, basicGrid) {
    var idDiv = guid();
    var GridConsulta;

    if (carga == null)
        carga = 0

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.FaixaTemperatura.BuscarFaixaTemperatura, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.FaixaTemperatura.FaixasDeTemperatura, type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: 12 });
        this.TipoOperacao = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.FaixaTemperatura.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
        this.Carga = PropertyEntity({ visible: false, val: ko.observable(carga) });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametrosDinamicos = null;

    if (knoutTipoOperacao) {
        knoutOpcoes.TipoOperacao.visible = false;

        funcaoParametrosDinamicos = function () {
            knoutOpcoes.TipoOperacao.codEntity(knoutTipoOperacao.codEntity());
            knoutOpcoes.TipoOperacao.val(knoutTipoOperacao.val());
            knoutOpcoes.Carga.val(carga);
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, multiplaEscolha, function () {
        new BuscarTiposOperacao(knoutOpcoes.TipoOperacao);
    });

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "FaixaTemperatura/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "FaixaTemperatura/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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

    this.AbrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };
}