/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarTipoContratoFrete = function (knout, apenasAditivos, callbackRetorno) {

    var idDiv = guid();
    var gridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Tipos de Contratos de Frete", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Tipo de Contrato de Frete", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição: " });
        this.Aditivos = PropertyEntity({ col: 0, visible: false, text: "", val: ko.observable(apenasAditivos) });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: "Situação: " });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    gridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoContratoFrete/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}