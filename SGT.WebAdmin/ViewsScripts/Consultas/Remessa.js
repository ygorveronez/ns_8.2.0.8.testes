/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="BoletoConfiguracao.js" />

var BuscarRemessa = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : "Pesquisa de Remessa de Boletos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : "Remessas", type: types.local });
        this.NumeroSequencial = PropertyEntity({ col: 4, text: "Número Sequencial: ", getType: typesKnockout.int });
        this.BoletoConfiguracao = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Configuração Boleto (Banco):", idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {
        new BuscarBoletoConfiguracao(knoutOpcoes.BoletoConfiguracao, RetornoConfiguracaoBancoRemessa);
    });

    function RetornoConfiguracaoBancoRemessa(data) {
        knoutOpcoes.BoletoConfiguracao.codEntity(data.Codigo);
        knoutOpcoes.BoletoConfiguracao.val(data.DescricaoBanco);
    }

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "BoletoRemessa/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 2, dir: orderDir.desc });
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroSequencial.val(knout.val());
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