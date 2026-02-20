/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumTipoComponenteFrete.js" />

var BuscarComponentesDeFrete = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: 12 });
        this.TipoComponenteFrete = PropertyEntity({ val: ko.observable(EnumTipoComponenteFrete.TODOS), def: EnumTipoComponenteFrete.TODOS, visible: false, text: Localization.Resources.Consultas.ComponenteFrete.TipoComplemento.getFieldDescription() });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ComponenteFrete.BuscarComponentesDeFrete, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ComponenteFrete.ComponentesDeFrete, type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);
    

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ComponenteFrete/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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