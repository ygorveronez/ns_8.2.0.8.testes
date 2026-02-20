/// <reference path="Sistema.js" />
/// <reference path="Modulo.js" />
/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarTipoCarregamento = function (knout, callbackRetorno, menuOpcoes) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Tipo de Carregamento", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Tipo de Carregamento", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição: " });
        this.CodigoDeIntegracao = PropertyEntity({ col: 12, text: "Codigo de Integração: " });
        this.Observacao = PropertyEntity({ col: 12, text: "Observação: " });
        this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(1), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: 1, visible: true });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;


    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        }
    }

    var opcoes = divBusca.OpcaoPadrao(callback)
    if (menuOpcoes != null) {
        opcoes = menuOpcoes;
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoCarregamento/Pesquisa", knoutOpcoes, opcoes, null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
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