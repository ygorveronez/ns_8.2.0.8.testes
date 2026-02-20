/// <reference path="FluxoPatio.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLEtapasFluxoGestaoPatio;

var EtapaFluxoGestaoPatio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Carga.getFieldDescription(), getType: typesKnockout.int, eventClick: exibirMenuFluxoPatioClick, OcultarFluxoCarga: _configuracaoGestaoPatio.OcultarFluxoCarga && !_configuracaoGestaoPatio.PermiteCancelarFluxoPatioAtual });
    this.NumeroCarga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.PreCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.PreCarga.getFieldDescription(), getType: typesKnockout.int });
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });
    this.CargaCancelada = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CargaDePreCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EtapaAtual = PropertyEntity({});
    this.EtapaFluxoGestaoPatioAtual = PropertyEntity({});
    this.PossuiCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnvioNotificacaoSMS = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteEnviarNotificacaoSuperApp = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.InformarEquipamentoFluxoPatio = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SituacaoEtapaFluxoGestaoPatio = PropertyEntity({});
    this.Tipo = PropertyEntity({});
    this.NomeMotoristas = PropertyEntity({ visible: ko.observable(true) });
    this.CPFMotoristas = PropertyEntity({ visible: ko.observable(true) });

    // Configurações da Sequencia Gestão Patio
    this.ChegadaVeiculoPermiteImprimirRelacaoDeProdutos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DeslocamentoPatioPermiteInformacoesPesagem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DeslocamentoPatioPermiteInformarQuantidade = PropertyEntity({  getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DeslocamentoPatioPermiteInformacoesLoteInterno = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GuaritaEntradaPermiteInformacoesPesagem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GuaritaEntradaPermiteInformacoesProdutor = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GuaritaSaidaPermiteInformacoesPesagem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    // Campos exibidos no fluxo de pátio
    this.NumeroCarregamento = PropertyEntity({});
    this.Placas = PropertyEntity({ visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ visible: ko.observable(true) });
    this.CategoriaAgendamento = PropertyEntity({ visible: ko.observable(true) });
    this.TipoCarregamento = PropertyEntity({ visible: ko.observable(true), val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ visible: ko.observable(false) });
    this.DataCarga = PropertyEntity({ visible: ko.observable(false) });
    this.Doca = PropertyEntity({ visible: ko.observable(false) });
    this.ModeloVeicularCargaVeiculo = PropertyEntity({ visible: ko.observable(false) });
    this.AreaVeiculo = PropertyEntity({ visible: ko.observable(false) });
    this.Equipamento = PropertyEntity({ visible: ko.observable(false) });
    this.AntecipacaoICMS = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.AntecipacaoICMS, visible: ko.observable(false) });
    this.PercentualSeparacaoMercadoria = PropertyEntity({ visible: ko.observable(false), cssClass: ko.observable("progress-bar bg-color-yellow") });
    this.Remetente = PropertyEntity({ visible: ko.observable(true) });
    this.KnoutEtapas = PropertyEntity({ def: [], val: ko.observableArray([]), ToTime: DecimalToTime });

    // Mensagens de alerta
    this.ExibirMensagensAlerta = PropertyEntity({ eventClick: exibirMensagensAlertaFluxoPatio, type: types.event, idModal: guid() });
    this.MensagemAlertaComBloqueio = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.MensagensAlerta = PropertyEntity({ val: ko.observableArray() });

    this.AlturaFluxo = PropertyEntity({ type: types.local, val: ko.computed(AlturaFluxo, this) });
    this.Remetente.valsubstring = ko.computed(CortarDescricaoRemetente, this);

    // Botões
    this.Auditar = PropertyEntity({ eventClick: AuditarEtapaFluxoGestaoPatio, visible: ko.observable(PermiteAuditar()) }); // OK
    this.DetalhesCarga = PropertyEntity({ eventClick: detalhesCargaPedidoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VerDetalhes, visible: ko.observable(true) }); // OK
    this.AtualizarPerguntasChecklist = PropertyEntity({ eventClick: atualizarPerguntasChecklistClick, type: types.event, text: "Gerar perguntas do checklist", visible: false });
};

//*******EVENTOS*******

function loadEtapasFluxoGestao() {
    return $.get("Content/Static/GestaoPatio/FluxoGestaoPatio.html?dyn=" + guid(), function (data) {
        _HTMLEtapasFluxoGestaoPatio = data;
    });
}

function AuditarEtapaFluxoGestaoPatio(e) {
    var _fn = OpcaoAuditoria("FluxoGestaoPatio", "Codigo", e);

    _fn({ Codigo: e.Codigo.val() });
}

function SetarEtapaFluxoDesabilitada(etapa) {
    etapa.cssClass("step");
    etapa.eventClick = function () { };
}

function SetarEtapaFluxoAprovada(etapa) {
    etapa.cssClass("step green");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoPermanenciaMaximaExcedida(etapa) {
    etapa.cssClass("step orange blink");
}

function SetarEtapaFluxoAprovadaComAtraso(etapa) {
    etapa.cssClass("step orange");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoAprovadaComPendencia(etapa) {
    etapa.cssClass("step cyan");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoAguardando(etapa) {
    etapa.cssClass("step yellow");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoAguardandoAlertaVisual(etapa) {
    etapa.cssClass("step yellow blink");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoDesbloqueada(etapa) {
    etapa.cssClass("step yellow");
    etapa.eventClick = etapa.eventClickDesbloqueada;
}

function SetarEtapaFluxoProblema(etapa) {
    etapa.cssClass("step red");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoLiberada(etapa) {
    etapa.cssClass("step yellow");
    etapa.eventClick = etapa.backEventClick;
}

function _RetornaDataExibicao(realizado, previsto) {
    return function () {
        var data = realizado.val();

        if (data == "") return previsto.val();

        return data;
    }
}

function AlturaFluxo() {
    // Altura minima para que fluxo seja exibido sem quebras
    var alturaMinima = 75;

    // Altura necessária com outros elementos que não os passíveis de serem ocultos
    //var alturaComplementar = 27;
    // tarefa 23785, Luiz Felipe
    var alturaComplementar = 20;

    // Cada span, requer xx de altura
    var alturaPorLinha = 20;

    // Quantidade de elementos visíveis
    var qtdLinhasVisiveis = 0;

    if (this.Remetente.visible())
        qtdLinhasVisiveis++;

    if (this.TipoOperacao.visible())
        qtdLinhasVisiveis++;

    if (this.Transportador.visible())
        qtdLinhasVisiveis += 2;

    if (this.CategoriaAgendamento.visible())
        qtdLinhasVisiveis ++;

    if (this.Placas.visible())
        qtdLinhasVisiveis++;

    if (this.DataCarga.visible())
        qtdLinhasVisiveis++;

    if (this.Doca.visible())
        qtdLinhasVisiveis++;

    if (this.ModeloVeicularCargaVeiculo.visible())
        qtdLinhasVisiveis++;

    if (this.AreaVeiculo.visible())
        qtdLinhasVisiveis++;

    if (this.AntecipacaoICMS.visible())
        qtdLinhasVisiveis++;

    if (this.PercentualSeparacaoMercadoria.val() != '0%')
        qtdLinhasVisiveis += 3;

    var alturaFluxo = alturaComplementar + (alturaPorLinha * qtdLinhasVisiveis);

    return (alturaFluxo < alturaMinima ? alturaMinima : alturaFluxo) + "px";
}

function CortarDescricaoRemetente() {
    var remetente = this.Remetente.val();
    var destinatario = this.Destinatario.val();
    var origemdestino = remetente + (destinatario != "" ? " x " : "") + destinatario;
    var cortado = origemdestino.substring(0, 15);

    if (origemdestino.length >= 18)
        cortado += "...";

    return cortado;
}

function DecimalToTime(val) {
    var num = Math.abs(val);
    var horas = Math.floor(num / 60);
    var minutos = (num % 60);

    return (horas > 9 ? horas : '0' + horas) + ':' + (minutos > 9 ? minutos : '0' + minutos);
}

function detalhesCargaPedidoClick(fluxoPatio) {
    exibirDetalhesPedidos(fluxoPatio.Carga.val());
}

function atualizarPerguntasChecklistClick(fluxoPatio) {
    executarReST("CheckList/AtualizarPerguntas", { FluxoGestaoPatio: fluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso.");
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirMensagensAlertaFluxoPatio(fluxoPatio, e) {
    e.stopPropagation();

    Global.abrirModal(fluxoPatio.ExibirMensagensAlerta.idModal);
}

function confirmarMensagemAlertaFluxoPatioClick(fluxoPatio, mensagemAlerta) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja realmente confirmar a leitura da mensagem de alerta?", function () {
        executarReST("FluxoPatio/ConfirmarMensagemAlerta", { Codigo: mensagemAlerta.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    var mensagensAlerta = fluxoPatio.MensagensAlerta.val();

                    for (var i = 0; i < mensagensAlerta.length; i++) {
                        if (mensagensAlerta[i].Codigo == mensagemAlerta.Codigo)
                            mensagensAlerta.splice(i, 1);
                    }

                    fluxoPatio.MensagensAlerta.val(mensagensAlerta);
                    fluxoPatio.MensagemAlertaComBloqueio.val(isPossuiMensagemAlertaFluxoPatioComBloqueio(mensagensAlerta));

                    if (mensagensAlerta.length == 0)
                        Global.fecharModal(fluxoPatio.ExibirMensagensAlerta.idModal);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function isPossuiMensagemAlertaFluxoPatioComBloqueio(mensagensAlerta) {
    for (var i = 0; i < mensagensAlerta.length; i++) {
        if (mensagensAlerta[i].Bloquear)
            return true;
    }

    return false;
}
