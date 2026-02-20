/// <reference path="CentroCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridConfiguracao;
var _centroCarregamentoConfiguracao;
var _configuracao;

/*
 * Declaração das Classes
 */

var CentroCarregamentoConfiguracao = function () {
    //Campos bool
    this.JanelaCarregamentoAbaPendentes = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.ExibirAbaPendentesNaJanelaDeCarregamento, val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.JanelaCarregamentoAbaExcedentes = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.ExibirAbaExcedentesNaJanelaDeCarregamento, val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.JanelaCarregamentoAbaReservas = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.ExibirAbaReservasNaJanelaDeCarregamento, val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.JanelaCarregamentoExibirSituacaoPatio = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.ExibirSituacaoPatioJanelaDeCarregamentoGrid, val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.EscolherHorarioCarregamentoPorLista = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EscolherHorarioDeCarregamentoObrigatoriaMontagemCarga, val: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.DataCarregamentoObrigatoriaMontagemCarga = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.DataCarregamentoObrigatoriaMontagemCarga, val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.NaoPermitirAlterarDataCarregamentoCarga = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.NaoPermitirAlterarDataDeCarreagamentoNaCarga, val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ExibirVisualizacaoDosTiposDeOperacao = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.ExibirVisualizacaoDostiposDeOperacao, val: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PermiteTransportadorVisualizarMenorLanceLeilao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PermiteTransportadorVisualizarMenorLanceDoLeilao, visible: ko.observable(true) });
    this.RepassarCargaCasoNaoExistaVeiculoDisponivel = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.EmCasoDeVitoriaNoLeilaoMasNaoTerVeiculosDisponiveisCargaDeveSerReapassadaAoProximoColocado, visible: ko.observable(false) });
    this.PermiteTransportadorInformarSomenteVeiculosRestritosAosClientes = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PermiteTransportadorInformarSomenteVeiculosRestritosAosClientes, visible: ko.observable(true) });
    this.NaoPermiteTransportadorSugerirHorarioSeClientePossuirJanelaDescarga = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.NaoPermiteTransportadorSugerirHorarioSeClientePossuirJanelaDeDescarga, visible: ko.observable(true) });
    this.PermiteTransportadorSelecionarHorarioCarregamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PermiteTransprotadorSelecionarHorarioDeCarregamento });
    this.BloquearComponentesDeFreteJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.BloquearComponentesDeFreteNaJanelaDeCarregamentoTransportador });
    this.ExibirNotasFiscaisJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.ExibirNotasFiscaisNaJanelaDeCarregamentoTransportador });
    this.ExibirDadosAvancadosJanelaCarregamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.ExibirDadosAvancadosNaJanelaDeCarregamento });
    this.PermitirTransportadorImprimirOrdemColeta = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PermitirQueTransportadorImprimaOrdemDaColeta });
    this.BloquearTrocaDataListaHorarios = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.BloquearTrocaDeDataNaListaDeHorariosTransportador });
    this.RetornarJanelaCarregamentoParaAgLiberacaoParaTransportadoresAposRejeicaoDoTransportador = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.RetornarCargaParaAguardandoLiberacaoParaTransportadoresAposRejeicaoDoTransportador });
    this.ExigirTermoAceiteTransporte = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.ExigirTermoDeAceiteParaTransporte });
    this.PermiteTransportadorInformarObservacoesJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.PermitirTransportadorInformarObservacoesNaJanelaDeCarregamentoDoTransportador });
    this.ExigirTransportadorConfirmarMDFeNaoEncerradoForaDoSistema = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.ExigirTransportadorConfirmarMDFeNaoEncerradoForaDoSistema, val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ExigirTransportadorInformarMotivoAoRejeitarCarga = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.ExigirTransportadorInformarMotivoAoRejeitarCarga, val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ExibirFilialJanelaCarregamentoTransportador = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.ExibirFilialJanelaCarregamentoTransportador, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.NaoPermitirGerarCarregamentosQuandoExistirPedidosAtrasadosAgendamentoPedidos = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.NaoPermitirGerarCarregamentosQuandoExistirPedidosAtrasadosAgendamentoPedidos, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteTransportadorVisualizarColocacaoDentreLancesLeilao = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.PermiteTransportadorVisualizarColocacaoDentreLancesLeilao, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigirConfirmacaoParticipacaoLeilao = PropertyEntity({ text: "Exigir confirmação na participação do leilão", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirQueTransportadorAltereHorarioDoCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.PermitirQueTransportadorAltereHorarioDoCarregamento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EscolherPrimeiroTransportadorAoMarcarInteresseAutomaticamente = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EscolherPrimeiroTransportadorAoMarcarInteresseAutomaticamente, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteTransportadorEnviarIntegracaoGRDocumentosReprovados = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.PermiteTransportadorEnviarIntegracaoGRDocumentosReprovados, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirAgendarCargasNoMesmoDia = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.NaoPermitirAgendarCargasNoMesmoDia, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigirChecklistAoConfirmarDadosTransporteMultiTransportador = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.ExigirChecklistAoConfirmarDadosTransporteMultiTransportador, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarControleVisualizacaoTransportadorasTerceiros = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.GerarControleVisualizacao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.ExigirTermoAceiteTransporte.val.subscribe((novoValor) => {
        habilitarAbaTermoAceite(novoValor);
    });

    this.ExigirConfirmacaoParticipacaoLeilao.val.subscribe((novoValor) => {
        if (novoValor) {
            _centroCarregamentoConfiguracao.MensagemConfirmacaoLeilao.visible(novoValor);
            _centroCarregamentoConfiguracao.MensagemConfirmacaoLeilao.required(novoValor);
        }
        else {
            _centroCarregamentoConfiguracao.MensagemConfirmacaoLeilao.visible(novoValor);
            _centroCarregamentoConfiguracao.MensagemConfirmacaoLeilao.required(novoValor);
        }
    });

    //Campos int
    this.IntervaloSelecaoHorarioCarregamentoTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.IntervaloSelecaoHorarioDeCarregamentoTransportadorMinutos.getFieldDescription() });
    this.TempoMaximoModificarHorarioCarregamentoTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoMaximoModificarHorarioDeCarregamentoTransportadorHoras.getFieldDescription() });
    this.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoPadraoParaTerminoDoCarregamentoParaValidarDisponibilidadeDe.getFieldDescription() });
    this.ToleranciaDataRetroativa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.ToleranciaEmMinutosParaDataRetroativa.getFieldDescription() });
    this.LimiteAlteracoesHorarioTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.LimiteDeAlteracoesDeHorarioPeloTransportador.getFieldDescription(), visible: ko.observable(false) });
    this.DiasAtrasoPermitidosPedidosAgendamentoPedidos = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.DiasAtrasoPermitidosPedidosAgendamentoPedidos.getFieldDescription(), val: ko.observable(""), getType: typesKnockout.int, def: 0, visible: ko.observable(true), required: ko.observable(false), maxlength: 3 });

    //campos string
    this.MensagemConfirmacaoLeilao = PropertyEntity({ text: "*Mensagem de confirmação do leilão:", val: ko.observable(""), getType: typesKnockout.string, def: "", visible: ko.observable(false), required: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadCentroCarregamentoConfiguracao() {
    _centroCarregamentoConfiguracao = new CentroCarregamentoConfiguracao();
    KoBindings(_centroCarregamentoConfiguracao, "knockoutConfiguracoes");

    $("#liTermoAceite").hide();
}

/*
 * Declaração das Funções
 */

function limparCamposCentroCarregamentoConfiguracao() {
    LimparCampos(_centroCarregamentoConfiguracao);
}

function preencherCentroCarregamentoConfiguracao(dadosConfiguracao) {
    PreencherObjetoKnout(_centroCarregamentoConfiguracao, { Data: dadosConfiguracao });
}

function preencherCentroCarregamentoConfiguracaoSalvar(centroCarregamento) {
    for (var key in _centroCarregamentoConfiguracao) {
        centroCarregamento[key] = _centroCarregamentoConfiguracao[key].val();
    }
}

function habilitarAbaTermoAceite(novoValor) {
    if (novoValor)
        $("#liTermoAceite").show();
    else
        $("#liTermoAceite").hide();
}
