/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Consultas/CentroCarregamento.js" />
/// <reference path="../Enumeradores/EnumTipoAreaVeiculo.js" />

var BuscarAreaVeiculo = function (knout, callbackRetorno, knoutCentroCarregamento, basicGrid) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = Boolean(basicGrid);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.AreaVeiculo.BuscarAreaDeVeículo, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.AreaVeiculo.AreasDeVeiculos, type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Consultas.AreaVeiculo.Descricao, col: 8 });
        this.Tipo = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.AreaVeiculo.Tipo.getFieldDescription(), val: ko.observable(EnumTipoAreaVeiculo.Todos), options: EnumTipoAreaVeiculo.obterOpcoesPesquisa(), def: EnumTipoAreaVeiculo.Todos });
        this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.AreaVeiculo.CentroDeCarregamento.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.AreaVeiculo.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametrosDinamicos = null;

    if (knoutCentroCarregamento) {
        knoutOpcoes.CentroCarregamento.visible = false;

        funcaoParametrosDinamicos = function () {
            knoutOpcoes.CentroCarregamento.codEntity(knoutCentroCarregamento.codEntity());
            knoutOpcoes.CentroCarregamento.val(knoutCentroCarregamento.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, multiplaEscolha, function () {
        new BuscarCentrosCarregamento(knoutOpcoes.CentroCarregamento);
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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "AreaVeiculo/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "AreaVeiculo/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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