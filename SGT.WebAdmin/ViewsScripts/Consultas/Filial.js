/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarFilial = function (knout, callbackRetorno, basicGrid, isFiltrarPorConfiguracaoOperadorLogistica, isFiltrarSomenteFilialComSolicitacaoDeGas, isFiltrarPorLiberadaParaFilaCarregamento, isFiltrarPorConfiguracaoOperadorLogisticaFilialVenda) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.DescricaoOuCodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao, col: 12 });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao });
        this.FiltrarPorConfiguracaoOperadorLogistica = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarPorConfiguracaoOperadorLogistica != false), visible: false });
        this.FiltrarPorConfiguracaoOperadorLogisticaFilialVenda = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarPorConfiguracaoOperadorLogisticaFilialVenda), visible: false });
        this.SomenteFiliaisComSolicitacaoDeGas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarSomenteFilialComSolicitacaoDeGas == true), visible: false });
        this.SomenteLiberadasParaFilaCarregamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarPorLiberadaParaFilaCarregamento == true), visible: false });
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Filial.PesquisarFiliais, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Filial.Filiais, type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

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

        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Filial/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Filial/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.DescricaoOuCodigoIntegracao.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });

    this.abrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };
}