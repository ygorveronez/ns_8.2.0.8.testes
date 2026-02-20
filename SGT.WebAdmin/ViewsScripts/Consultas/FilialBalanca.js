/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarFilialBalanca = function (knout, callbackRetorno, basicGrid, knoutCodigoFilial, knoutFilial) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = basicGrid != null;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Balanças da Filial", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Balanças da Filial", type: types.local });

        this.Descricao = PropertyEntity({ col: 12, text: "Modelo: " });

        this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCodigoFilial != null || knoutFilial != null) {
        funcaoParamentroDinamico = function () {
            if (knoutCodigoFilial != null) {
                knoutOpcoes.Filial.codEntity(knoutCodigoFilial.val());
                knoutOpcoes.Filial.val(knoutCodigoFilial.val());
            }

            if (knoutFilial != null) {
                knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
                knoutOpcoes.Filial.val(knoutFilial.val());
            }
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Filial/PesquisaFilialBalanca", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Filial/PesquisaFilialBalanca", knoutOpcoes, divBusca.OpcaoPadrao(callback));
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
};