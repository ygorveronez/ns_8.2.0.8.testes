/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarArmazem = function (knout, callbackRetorno, basicGrid, knoutFilial) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Armazéns", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Armazém", type: types.local });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Gerais.Geral.Descricao });
        this.CodigoIntegracao = PropertyEntity({ col: 6, text: "Código Integração" });
        this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.Filiais = PropertyEntity({ type: types.multiplesEntities, val: ko.observable([]), visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutFilial != null) {
        funcaoParametroDinamico = function () {
            if (knoutFilial != null) {
                if (knoutFilial.type == types.entity) {
                    knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
                    knoutOpcoes.Filial.val(knoutFilial.val());
                } else if (knoutFilial.type == types.multiplesEntities) {
                    knoutOpcoes.Filiais.multiplesEntities(knoutFilial.multiplesEntities());
                }
            }
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, null);

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Filial/PesquisaFilialArmazem", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Filial/PesquisaFilialArmazem", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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