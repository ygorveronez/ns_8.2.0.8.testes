/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

const BuscarSituacaoComercialPedido = function (knout, callbackRetorno, basicGrid) {

    const idDiv = guid();
    let GridConsulta;

    let multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    const OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.SituacaoComercialPedido.BuscarSituacaoComercialPedido, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.SituacaoComercialPedido.SituacaoComercialDoPedido, type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao });
        this.CodigoIntegracao = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.SituacaoComercialPedido.CodigoIntegracao });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    const knoutOpcoes = new OpcoesKnout();
    let funcaoParametroDinamico = null;

    const divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, null);

    let callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        const objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SituacaoComercialPedido/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SituacaoComercialPedido/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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