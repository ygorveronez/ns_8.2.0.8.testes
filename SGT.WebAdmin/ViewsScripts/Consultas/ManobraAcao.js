/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumTipoManobraAcao.js" />

var BuscarManobraAcao = function (knout, callbackRetorno, knoutCentroCarregamento, tipoManobraAcao) {
    var idDiv = guid();
    var GridConsulta;
    var tipo = tipoManobraAcao ? tipoManobraAcao : EnumTipoManobraAcao.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ManobraAcao.BuscarAcaoDeManobra, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ManobraAcao.AcoesDeManobra, type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: 12 });
        this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ManobraAcao.CentroDeCarregamento.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Tipo = PropertyEntity({ val: ko.observable(tipo), options: EnumTipoManobraAcao.obterOpcoesPesquisa(), def: tipo, text: Localization.Resources.Consultas.ManobraAcao.Tipo.getFieldDescription(), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametrosDinamicos = null;

    if (knoutCentroCarregamento) {
        funcaoParametrosDinamicos = function () {
            knoutOpcoes.CentroCarregamento.codEntity(knoutCentroCarregamento.codEntity());
            knoutOpcoes.CentroCarregamento.val(knoutCentroCarregamento.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ManobraAcao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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