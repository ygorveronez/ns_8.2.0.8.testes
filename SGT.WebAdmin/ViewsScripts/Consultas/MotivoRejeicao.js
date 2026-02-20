/// <reference path="../../wwwroot/js/Global/Buscas.js" />
/// <reference path="../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../wwwroot/js/Global/CRUD.js" />

var BuscarMotivos = function (knout, callbackRetorno, basicGrid) {
    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Motivos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motivos", type: types.local });
        this.Descricao = PropertyEntity({ col: 6, val: ko.observable(""), text: "Descrição:" });
        this.Ativo = PropertyEntity({ col: 6, val: ko.observable(1), options: _statusPesquisa, text: "Situação", def: 1 });
        this.TipoMotivo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        console.log(e);
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motivo/Pesquisa", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "Motivo/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback, 20), { column: 0, dir: orderDir.desc });
    }


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
};
