/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarRaca = function (knout, callbackRetorno, knockoutEspecie) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar raça", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Raças", type: types.local });
        this.DescricaoRaca = PropertyEntity({ text: "Raça:", col: 8 });
        this.AtivoRaca = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Situação: ", col: 4 });
        this.Especie = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (knockoutEspecie != null) {
        knoutOpcoes.Especie.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Especie.codEntity(knockoutEspecie.codEntity());
            knoutOpcoes.Especie.val(knockoutEspecie.val());
        }
    }

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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "EspecieRaca/Pesquisar", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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