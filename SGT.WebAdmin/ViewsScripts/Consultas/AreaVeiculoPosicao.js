/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Consultas/AreaVeiculo.js" />

var BuscarAreaVeiculoPosicao = function (knout, callbackRetorno, knoutCentroCarregamento, knoutPreCarga, knoutTipoRetornoCarga, tipoAreaVeiculo) {
    var idDiv = guid();
    var GridConsulta;

    if (!tipoAreaVeiculo)
        tipoAreaVeiculo = EnumTipoAreaVeiculo.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.AreaVeiculoPosicao.BuscarPosicaoDeAreaDeVeiculo, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.AreaVeiculoPosicao.PosicoesDeAreasDeVeiculos, type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Consultas.AreaVeiculoPosicao.Descricao.getFieldDescription(), col: 12 });
        this.AreaVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.AreaVeiculoPosicao.AreaDeVeiculo.getFieldDescription(), idBtnSearch: guid() });
        this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.PreCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
        this.TipoAreaVeiculo = PropertyEntity({ val: ko.observable(tipoAreaVeiculo), options: EnumTipoAreaVeiculo.obterOpcoes(), def: tipoAreaVeiculo, visible: false });
        this.TipoRetornoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.AreaVeiculoPosicao.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametrosDinamicos = null;

    if (knoutCentroCarregamento || knoutPreCarga) {
        funcaoParametrosDinamicos = function () {
            if (knoutCentroCarregamento) {
                knoutOpcoes.CentroCarregamento.codEntity(knoutCentroCarregamento.codEntity());
                knoutOpcoes.CentroCarregamento.val(knoutCentroCarregamento.val());
            }

            if (knoutTipoRetornoCarga) {
                knoutOpcoes.TipoRetornoCarga.codEntity(knoutTipoRetornoCarga.codEntity());
                knoutOpcoes.TipoRetornoCarga.val(knoutTipoRetornoCarga.val());
            }

            if (knoutPreCarga)
                knoutOpcoes.PreCarga.val(knoutPreCarga.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, null, function () {
        new BuscarAreaVeiculo(knoutOpcoes.AreaVeiculo, null, knoutCentroCarregamento);
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "AreaVeiculoPosicao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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