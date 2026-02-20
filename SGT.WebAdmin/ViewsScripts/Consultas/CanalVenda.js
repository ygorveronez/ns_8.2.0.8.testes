/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

const BuscarCanaisVenda = function (knout, callbackRetorno, basicGrid, callbackRetornoMultiplaEscolha, knoutFilial) {

    const idDiv = guid();
    let gridConsulta;

    let multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    const OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Canais de Venda", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Canal de Venda", type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: "Descrição:" });
        this.CodigoIntegracao = PropertyEntity({ col: 4, text: "Código de Integração:" });
        this.Ativo = PropertyEntity({ text: "Status:", val: ko.observable(1), options: _statusPesquisa, def: 1, visible: false });
        this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(false) });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    };

    let knoutOpcoes = new OpcoesKnout();

    if (knoutFilial != null) {
        knoutOpcoes.Filial.visible = false;
    }

    const funcaoParametroDinamico = function () {
        if (knoutFilial) {
            knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
            knoutOpcoes.Filial.val(knoutFilial.val());
        }
    }

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, null, null, null, null, callbackRetornoMultiplaEscolha);

    let callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        const objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "CanalVenda/Pesquisa", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "CanalVenda/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });
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