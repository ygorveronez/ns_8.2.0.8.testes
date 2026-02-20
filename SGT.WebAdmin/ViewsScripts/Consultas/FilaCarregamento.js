/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Consultas/CentroCarregamento.js" />

var BuscarFilaCarregamentoVeiculo = function (knout, callbackRetorno, basicGrid, knoutCentroCarregamento) {

    var idDiv = guid();
    var GridConsulta;
    var funcaoParamentroDinamico = null;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Fila de Carregamento", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Fila de Carregamento", type: types.local });
        
        this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento", idBtnSearch: guid(), visible: true });

       
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    if (knoutCentroCarregamento ) {
        knoutOpcoes.CentroCarregamento.visible = !knoutCentroCarregamento;

        funcaoParamentroDinamico = function () {
            if (knoutCentroCarregamento) {
                knoutOpcoes.CentroCarregamento.codEntity(knoutCentroCarregamento.codEntity());
                knoutOpcoes.CentroCarregamento.val(knoutCentroCarregamento.val());
            }
        }
    }
    
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarCentrosCarregamento(knoutOpcoes.CentroCarregamento);
    });
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Tracao);
        knout.requiredClass("form-control");

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "FilaCarregamento/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "FilaCarregamento/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Placa.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};
