/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../ViewsScripts/Consultas/ModeloPneu.js" />
/// <reference path="../../ViewsScripts/Consultas/BandaRodagemPneu.js" />
/// <reference path="../../ViewsScripts/Enumeradores/EnumVidaPneu.js" />

var BuscarConfiguracaoPalletizacao = function (knout, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Palletização", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Palletização", type: types.local });

        this.Descricao = PropertyEntity({ text: "Descrição:", col: 6, maxlength: 500 });
        this.CodigoIntegracao = PropertyEntity({ text: "Código integração:", col: 6, maxlength: 500 });
        this.Ativo = PropertyEntity({ val: ko.observable(1), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inatvo"), def: "", visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, false, function () {

    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Palletizacao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}