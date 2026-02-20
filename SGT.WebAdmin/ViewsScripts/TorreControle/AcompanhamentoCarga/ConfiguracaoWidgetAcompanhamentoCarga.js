/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumBotoesDetalheAcompanhamentoCarga.js" />
/// <reference path="../../Enumeradores/EnumOpcoesOrdenacaoCardsAcompanhamentoCarga.js" />

var _configWidgetAcompanhamentoCarga;

var ConfigWidgetAcompanhamentoCarga = function () {

    this.ExibirVeiculoTracao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Tracao, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.ExibirReboques = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Reboque, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.ExibirNumeroFrotaReboques = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirNumeroFrotaReboques, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirMotorista = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirMotorista, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.ExibirAlertas = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirAlertas, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.ExibirDescricaoTipoOperacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirDescricaoTipoOperacao, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.ExibirPedidosEmMaisCargas = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirPedidosOutrasCargas, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.ExibirProximoDestino = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirProximoDestino, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.ExibirProximaEntregaPrevisao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirPrevisaoEntrega, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.ExibirPrevisaoReprogramada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirPrevisaoEntregaReprogramada, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirDataInicioViagem = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirInicioViagem, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.ExibirTendenciaAtrasoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirTendenciaAtraso, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });

    this.ExibirTendenciaAtrasoColeta = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirTendenciaAtrasoColeta, getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(true) });
    this.ExibirCidadeProximoDestino = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirCidadeProximoDestino, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirValorTotalNFe = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirValorTotalNFe, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirPesoTotalNFe = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirPesoTotalNFe, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirPesoBrutoNFe = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirPesoBruto, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirAnotacoes = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirAnotacoes, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.DesativarAtualizacaoNovasCargas = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DesativarAtualizacaoNovasCargas, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirDataPrevisaoInicioViagem = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirDataPrevisaoInicioViagem, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirFilial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Filial, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirLeadTimeTransportador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.LeadTimeTransportador, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirModalTransporte = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirModalTransporte, val: ko.observable(false) });
    this.ExibirCanalVenda = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CanalVenda, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.ExibirDataAgendamentoPedido = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataAgendamento, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirDataColeta = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataColeta, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirMatrizComplementar = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Matriz, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirEscritorioVendasComplementar = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EscritorioDeVenda, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirNumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.NumeroPedidoCliente, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirMesorregiao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Mesoregiao, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.ExibirRegiao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Regiao, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.BotaoPrimario = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.BotaoPrimario, options: EnumBotoesDetalheAcompanhamentoCarga.obterOpcoes(), val: ko.observable([]), def: [], visible: ko.observable(true) });
    this.BotaoSecundario = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.BotaoSecundario, options: EnumBotoesDetalheAcompanhamentoCarga.obterOpcoes(), val: ko.observable([]), def: [], visible: ko.observable(true) });
    this.OpcaoOrdenacaoCardsAcompanhamentoCarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.OpcoesOrdenacaoCardsAcompanhamentoCarga, options: EnumOpcoesOrdenacaoCardsAcompanhamentoCarga.obterOpcoes(), val: ko.observable([]), def: EnumOpcoesOrdenacaoCardsAcompanhamentoCarga.DataCriacaoCarga, visible: ko.observable(true) });

    this.Salvar = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            SalvarConfiguracoesWidgetAcompanhamento();

        }, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, idGrid: guid(), visible: ko.observable(true)
    });

    this.VoltarPadrao = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;

            VoltarConfigPadrao();

        }, type: types.event, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirVeiculoTracao.val.subscribe((value) => {
        this.ExibirNumeroFrotaReboques.visible(value);
    });
};

function LoadConfiguracaoWidgets() {
    _configWidgetAcompanhamentoCarga = new ConfigWidgetAcompanhamentoCarga();
    KoBindings(_configWidgetAcompanhamentoCarga, "knockoutConfigWidgetAcompanhamentoCarga", false, _configWidgetAcompanhamentoCarga.Salvar.id);

    ObterConfiguracaoWidget();
}


function ObterConfiguracaoWidget() {
    executarReST("AcompanhamentoCarga/ObterConfiguracaoWidget", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                PreencherObjetoKnout(_configWidgetAcompanhamentoCarga, arg);
                AplicarConfigWidget();
                ControlarVisibilidadeBotoesAcessoRapido(arg.Data.BotaoPrimarioDetalheAcompanhamentoCarga, arg.Data.BotaoSecundarioDetalheAcompanhamentoCarga);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}


function SalvarConfiguracoesWidgetAcompanhamento() {
    var data = RetornarObjetoPesquisa(_configWidgetAcompanhamentoCarga);

    executarReST("AcompanhamentoCarga/SalvarConfiguracaoWidgetAcompanhamentoCarga", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                Global.fecharModal("knockoutConfigWidgetAcompanhamentoCarga");

                ControlarVisibilidadeBotoesAcessoRapido(_configWidgetAcompanhamentoCarga.BotaoPrimario.val(), _configWidgetAcompanhamentoCarga.BotaoSecundario.val());
                BuscarCargasAcompanhamento(1, false, false, false, false, false).then(function () {
                    AplicarConfigWidget();
                    ObterMensagensNaoLidasCards();
                    loadCargasNoMapa();
                });

                setTimeout(function () {
                    $("[rel=popover-hover]").popover({ trigger: "hover", container: "body", delay: { "show": 1000, "hide": 0 } });
                }, 1000);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

}

function VoltarConfigPadrao() {

    _configWidgetAcompanhamentoCarga.ExibirDataInicioViagem.val(true);
    _configWidgetAcompanhamentoCarga.ExibirDescricaoTipoOperacao.val(true);
    _configWidgetAcompanhamentoCarga.ExibirVeiculoTracao.val(true);
    _configWidgetAcompanhamentoCarga.ExibirReboques.val(true);
    _configWidgetAcompanhamentoCarga.ExibirProximaEntregaPrevisao.val(true);
    _configWidgetAcompanhamentoCarga.ExibirPrevisaoReprogramada.val(false);
    _configWidgetAcompanhamentoCarga.ExibirProximoDestino.val(true);
    _configWidgetAcompanhamentoCarga.ExibirCidadeProximoDestino.val(false);
    _configWidgetAcompanhamentoCarga.DesativarAtualizacaoNovasCargas.val(false);
    _configWidgetAcompanhamentoCarga.ExibirAlertas.val(true);
    _configWidgetAcompanhamentoCarga.ExibirPedidosEmMaisCargas.val(true);
    _configWidgetAcompanhamentoCarga.ExibirTendenciaAtrasoEntrega.val(false);
    _configWidgetAcompanhamentoCarga.ExibirTendenciaAtrasoColeta.val(false);
    _configWidgetAcompanhamentoCarga.ExibirNumeroFrotaReboques.val(false);
    _configWidgetAcompanhamentoCarga.ExibirValorTotalNFe.val(false);
    _configWidgetAcompanhamentoCarga.ExibirPesoTotalNFe.val(false);
    _configWidgetAcompanhamentoCarga.ExibirPesoBrutoNFe.val(false);
    _configWidgetAcompanhamentoCarga.ExibirAnotacoes.val(false);
    _configWidgetAcompanhamentoCarga.ExibirFilial.val(false);
    _configWidgetAcompanhamentoCarga.ExibirLeadTimeTransportador.val(false);
    _configWidgetAcompanhamentoCarga.ExibirDataColeta.val(false);
    _configWidgetAcompanhamentoCarga.ExibirDataAgendamentoPedido.val(false);
    _configWidgetAcompanhamentoCarga.ExibirDataPrevisaoInicioViagem.val(false);
    _configWidgetAcompanhamentoCarga.ExibirCanalVenda.val(false);
    _configWidgetAcompanhamentoCarga.ExibirModalTransporte.val(false);
    _configWidgetAcompanhamentoCarga.ExibirMesorregiao.val(false);
    _configWidgetAcompanhamentoCarga.ExibirRegiao.val(false);
}

function AplicarConfigWidget() {

    if (_cardAcompanhamentoCarga.Cargas() != undefined) {
        for (var i = 0; i < _cardAcompanhamentoCarga.Cargas().length; i++) {
            _cardAcompanhamentoCarga.Cargas()[i].VeiculoTracao.visible(_configWidgetAcompanhamentoCarga.ExibirVeiculoTracao.val());
            _cardAcompanhamentoCarga.Cargas()[i].Reboques.visible(_configWidgetAcompanhamentoCarga.ExibirReboques.val());
            _cardAcompanhamentoCarga.Cargas()[i].NumeroFrotaReboques.visible(_configWidgetAcompanhamentoCarga.ExibirNumeroFrotaReboques.val());
            _cardAcompanhamentoCarga.Cargas()[i].PesoTotalNF.visible(_configWidgetAcompanhamentoCarga.ExibirPesoTotalNFe.val());
            _cardAcompanhamentoCarga.Cargas()[i].PesoBruto.visible(_configWidgetAcompanhamentoCarga.ExibirPesoBrutoNFe.val());
            _cardAcompanhamentoCarga.Cargas()[i].AnotacoesCard.visible(_configWidgetAcompanhamentoCarga.ExibirAnotacoes.val());
            _cardAcompanhamentoCarga.Cargas()[i].Filial.visible(_configWidgetAcompanhamentoCarga.ExibirFilial.val());
            _cardAcompanhamentoCarga.Cargas()[i].DataPrevisaoInicioViagem.visible(_configWidgetAcompanhamentoCarga.ExibirDataPrevisaoInicioViagem.val());
            _cardAcompanhamentoCarga.Cargas()[i].ValorTotalNF.visible(_configWidgetAcompanhamentoCarga.ExibirValorTotalNFe.val());
            _cardAcompanhamentoCarga.Cargas()[i].DataInicioViagemFormatada.visible(_configWidgetAcompanhamentoCarga.ExibirDataInicioViagem.val());
            _cardAcompanhamentoCarga.Cargas()[i].TipoOperacao.visible(_configWidgetAcompanhamentoCarga.ExibirDescricaoTipoOperacao.val());
            _cardAcompanhamentoCarga.Cargas()[i].Motoristas.visible(_configWidgetAcompanhamentoCarga.ExibirMotorista.val());
            _cardAcompanhamentoCarga.Cargas()[i].DataPrevistaProximaEntrega.visible(_configWidgetAcompanhamentoCarga.ExibirProximaEntregaPrevisao.val());
            _cardAcompanhamentoCarga.Cargas()[i].DataReprogramadaProximaEntrega.visible(_configWidgetAcompanhamentoCarga.ExibirPrevisaoReprogramada.val());
            _cardAcompanhamentoCarga.Cargas()[i].LeadTimeTransportador.visible(_configWidgetAcompanhamentoCarga.ExibirLeadTimeTransportador.val());
            _cardAcompanhamentoCarga.Cargas()[i].DataCarregamentoPedidoFormatada.visible(_configWidgetAcompanhamentoCarga.ExibirDataColeta.val());
            _cardAcompanhamentoCarga.Cargas()[i].DataAgendamentoPedidoFormatada.visible(_configWidgetAcompanhamentoCarga.ExibirDataAgendamentoPedido.val());
            _cardAcompanhamentoCarga.Cargas()[i].ProximoDestino.visible(_configWidgetAcompanhamentoCarga.ExibirProximoDestino.val());
            _cardAcompanhamentoCarga.Cargas()[i].ProximaCidadeDestino.visible(_configWidgetAcompanhamentoCarga.ExibirCidadeProximoDestino.val());
            _cardAcompanhamentoCarga.Cargas()[i].CanalVenda.visible(_configWidgetAcompanhamentoCarga.ExibirCanalVenda.val());
            _cardAcompanhamentoCarga.Cargas()[i].ModalTransporte.visible(_configWidgetAcompanhamentoCarga.ExibirModalTransporte.val());
            _cardAcompanhamentoCarga.Cargas()[i].Mesoregiao.visible(_configWidgetAcompanhamentoCarga.ExibirMesorregiao.val());
            _cardAcompanhamentoCarga.Cargas()[i].Regiao.visible(_configWidgetAcompanhamentoCarga.ExibirRegiao.val());

            _cardAcompanhamentoCarga.Cargas()[i].PossuiAlerta.visible(_configWidgetAcompanhamentoCarga.ExibirAlertas.val());
            _cardAcompanhamentoCarga.Cargas()[i].PedidoEmOutrasCargas.visible(_configWidgetAcompanhamentoCarga.ExibirPedidosEmMaisCargas.val());
            _cardAcompanhamentoCarga.Cargas()[i].TendenciaEntrega.visible(_configWidgetAcompanhamentoCarga.ExibirTendenciaAtrasoEntrega.val());
            _cardAcompanhamentoCarga.Cargas()[i].TendenciaColeta.visible(_configWidgetAcompanhamentoCarga.ExibirTendenciaAtrasoColeta.val());
            _cardAcompanhamentoCarga.Cargas()[i].UltimaColetaRealizadaNoPrazoDescricao.visible(_configWidgetAcompanhamentoCarga.ExibirTendenciaAtrasoColeta.val());
            _cardAcompanhamentoCarga.Cargas()[i].UltimaEntregaRealizadaNoPrazoDescricao.visible(_configWidgetAcompanhamentoCarga.ExibirTendenciaAtrasoEntrega.val());
        }
    }
}