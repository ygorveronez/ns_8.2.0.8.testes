/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarControleImportacao = function (knout, codigoControleImportacao, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ControleImportacao.BuscarConfiguracaoDeColuna, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ControleImportacao.ConfiguracoesDeColunas, type: types.local });

        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.CodigoControleImportacao = PropertyEntity({ val: ko.observable(codigoControleImportacao), def: codigoControleImportacao, visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    this.pesquisa = knoutOpcoes;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null);

    var callback = function (e) {
        callbackRetorno(e);
        divBusca.DefCallback(e);
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ImportacaoArquivo/PesquisaConfiguracao", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}