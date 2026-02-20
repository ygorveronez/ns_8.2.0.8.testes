/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarBoletoConfiguracao = function (knout, callbackRetorno, utilizaConfiguracaoPagamentoEletronico) {

    var idDiv = guid();
    var GridConsulta;

    if (utilizaConfiguracaoPagamentoEletronico === null || utilizaConfiguracaoPagamentoEletronico === undefined)
        utilizaConfiguracaoPagamentoEletronico = false;

    var OpcoesKnout = function () {
        this.Descricao = PropertyEntity({ text: Localization.Resources.Consultas.BoletoConfiguracao.Agencia, col: 12 });
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.BoletoConfiguracao.BuscarConfiguracoes, type: types.local });
        this.UtilizaConfiguracaoPagamentoEletronico = PropertyEntity({ visible: false, val: ko.observable(utilizaConfiguracaoPagamentoEletronico), def: utilizaConfiguracaoPagamentoEletronico });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.BoletoConfiguracao.Configuracoes, type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "BoletoConfiguracao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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