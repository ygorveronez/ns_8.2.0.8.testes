/// <reference path="../../Enumeradores/EnumSituacaoOrdemServicoFrota.js" />
/// <reference path="OrdemServico.js" />
/// <reference path="Servico.js" />
/// <reference path="Orcamento.js" />
/// <reference path="Fechamento.js" />
/// <reference path="Aprovacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaOrdemServico;

var EtapaOrdemServico = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("20%") });

    this.Etapa1 = PropertyEntity({
        text: "Ordem", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se gera uma OS."),
        tooltipTitle: ko.observable("Ordem de Serviço")
    });
    this.Etapa2 = PropertyEntity({
        text: IsMobile() ? "Serv." : "Serviços", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de seleção dos serviços da OS."),
        tooltipTitle: ko.observable("Serviços")
    });
    this.Etapa3 = PropertyEntity({
        text: IsMobile() ? "Orç." : "Orçamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("É a etapa de orçamento da OS."),
        tooltipTitle: ko.observable("Orçamento")
    });
    this.Etapa4 = PropertyEntity({
        text: IsMobile() ? "Aprov." : "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("É a etapa de aprovação da OS."),
        tooltipTitle: ko.observable("Aprovação")
    });
    this.Etapa5 = PropertyEntity({
        text: IsMobile() ? "Fecha." : "Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(5),
        tooltip: ko.observable("É a etapa de fechamento da OS."),
        tooltipTitle: ko.observable("Fechamento")
    });
};

//*******EVENTOS*******

function LoadEtapaOrdemServico() {
    _etapaOrdemServico = new EtapaOrdemServico();
    KoBindings(_etapaOrdemServico, "knockoutEtapaOrdemServico");
    Etapa1Liberada();
}

function SetarEtapaInicioOrdemServico() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaOrdemServico.Etapa1.idTab).click();
    $("#" + _etapaOrdemServico.Etapa1.idTab).tab("show")
}

function SetarEtapaOrdemServico() {
    var situacaoOrdemServico = _ordemServico.Situacao.val();

    if (situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.Cancelada)
        situacaoOrdemServico = _ordemServico.SituacaoAnteriorCancelamento.val();

    if (situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.EmDigitacao)
        Etapa2Aguardando();
    else if (situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.AgAutorizacao)
        Etapa3Aguardando();
    else if (situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.DivergenciaOrcadoRealizado)
        Etapa3Reprovada();
    else if (situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.EmManutencao || situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.AgNotaFiscal)
        Etapa5Aguardando();
    else if (situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.Finalizada)
        Etapa5Aprovada();
    else if (situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.Rejeitada)
        Etapa3Reprovada();
    else if (situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.AguardandoAprovacao || situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.SemRegraAprovacao)
        Etapa4Aguardando();
    else if (situacaoOrdemServico === EnumSituacaoOrdemServicoFrota.AprovacaoRejeitada)
        Etapa4Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
    Etapa5Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaOrdemServico.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaOrdemServico.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaOrdemServico.Etapa2.eventClick = function () { };
    $("#" + _etapaOrdemServico.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOrdemServico.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada();
}

function Etapa2Liberada() {
    _etapaOrdemServico.Etapa2.eventClick = BuscarManutencoesOrdemServico;
    $("#" + _etapaOrdemServico.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaOrdemServico.Etapa2.eventClick = BuscarManutencoesOrdemServico;
    $("#" + _etapaOrdemServico.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaOrdemServico.Etapa2.idTab).click();
    $("#" + _etapaOrdemServico.Etapa2.idTab).tab("show")
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaOrdemServico.Etapa2.eventClick = BuscarManutencoesOrdemServico;
    $("#" + _etapaOrdemServico.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaOrdemServico.Etapa2.eventClick = BuscarManutencoesOrdemServico;
    $("#" + _etapaOrdemServico.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    _etapaOrdemServico.Etapa3.eventClick = function () { };
    $("#" + _etapaOrdemServico.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOrdemServico.Etapa3.idTab + " .step").attr("class", "step");
    Etapa4Desabilitada();
}

function Etapa3Liberada() {
    _etapaOrdemServico.Etapa3.eventClick = BuscarServicosOrcamento;
    $("#" + _etapaOrdemServico.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaOrdemServico.Etapa3.eventClick = BuscarServicosOrcamento;
    $("#" + _etapaOrdemServico.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaOrdemServico.Etapa3.idTab).click();
    $("#" + _etapaOrdemServico.Etapa3.idTab).tab("show")
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    _etapaOrdemServico.Etapa3.eventClick = BuscarServicosOrcamento;
    $("#" + _etapaOrdemServico.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    _etapaOrdemServico.Etapa3.eventClick = BuscarServicosOrcamento;
    $("#" + _etapaOrdemServico.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa3.idTab + " .step").attr("class", "step red");
    $("#" + _etapaOrdemServico.Etapa3.idTab).click();
    $("#" + _etapaOrdemServico.Etapa3.idTab).tab("show")
    Etapa2Aprovada();
}

//*******Etapa 4*******

function Etapa4Desabilitada() {
    _etapaOrdemServico.Etapa4.eventClick = function () { };
    $("#" + _etapaOrdemServico.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOrdemServico.Etapa4.idTab + " .step").attr("class", "step");
    Etapa5Desabilitada();
}

function Etapa4Liberada() {
    _etapaOrdemServico.Etapa4.eventClick = CarregarAprovacaoOrdemServico;
    $("#" + _etapaOrdemServico.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa4.idTab + " .step").attr("class", "step yellow");
    Etapa3Aprovada();
}

function Etapa4Aguardando() {
    _etapaOrdemServico.Etapa4.eventClick = CarregarAprovacaoOrdemServico;
    $("#" + _etapaOrdemServico.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa4.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaOrdemServico.Etapa4.idTab).click();
    $("#" + _etapaOrdemServico.Etapa4.idTab).tab("show")
    Etapa3Aprovada();
}

function Etapa4Aprovada() {
    _etapaOrdemServico.Etapa4.eventClick = CarregarAprovacaoOrdemServico;
    $("#" + _etapaOrdemServico.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa4.idTab + " .step").attr("class", "step green");
    Etapa3Aprovada();
}

function Etapa4Reprovada() {
    _etapaOrdemServico.Etapa4.eventClick = CarregarAprovacaoOrdemServico;
    $("#" + _etapaOrdemServico.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa4.idTab + " .step").attr("class", "step red");
    $("#" + _etapaOrdemServico.Etapa4.idTab).click();
    $("#" + _etapaOrdemServico.Etapa4.idTab).tab("show")
    Etapa3Aprovada();
}

//*******Etapa 5*******

function Etapa5Desabilitada() {
    _etapaOrdemServico.Etapa5.eventClick = function () { };
    $("#" + _etapaOrdemServico.Etapa5.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOrdemServico.Etapa5.idTab + " .step").attr("class", "step");
}

function Etapa5Liberada() {
    _etapaOrdemServico.Etapa5.eventClick = CarregarFechamentoOrcamento;
    $("#" + _etapaOrdemServico.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa5.idTab + " .step").attr("class", "step yellow");
    Etapa4Aprovada();
}

function Etapa5Aguardando() {
    _etapaOrdemServico.Etapa5.eventClick = CarregarFechamentoOrcamento;
    $("#" + _etapaOrdemServico.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa5.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaOrdemServico.Etapa5.idTab).click();
    $("#" + _etapaOrdemServico.Etapa5.idTab).tab("show")
    Etapa4Aprovada();
}

function Etapa5Aprovada() {
    _etapaOrdemServico.Etapa5.eventClick = CarregarFechamentoOrcamento;
    $("#" + _etapaOrdemServico.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa5.idTab + " .step").attr("class", "step green");
    $("#" + _etapaOrdemServico.Etapa5.idTab).click();
    $("#" + _etapaOrdemServico.Etapa1.idTab).tab("show")
    Etapa4Aprovada();
}

function Etapa5Reprovada() {
    _etapaOrdemServico.Etapa5.eventClick = CarregarFechamentoOrcamento;
    $("#" + _etapaOrdemServico.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServico.Etapa5.idTab + " .step").attr("class", "step red");
    Etapa4Aprovada();
}