/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoOcorrencia.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCancelamentoOcorrencia;

var EtapaCancelamentoOcorrencia = function () {
    this.Etapa1 = PropertyEntity({
        text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Dados, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OndeSeInformaDadosIniciaisParaCancelamentoOcorrencia),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Documentos, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OndeSeFazAcompanhamentoCancelamentoDocumentos),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa3 = PropertyEntity({
        text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Integracao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.CasoTenhaIntegracaoNessaEtapaPossivelAcompanhar),
        tooltipTitle: ko.observable("Integração")
    });

    var totalEtapas = 0;
    for (var k in this) if (k.substr(0, 5).toLowerCase() == "etapa") totalEtapas++;
    var pcEtapa = (100 / totalEtapas).toFixed(2);

    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable(pcEtapa + "%") });
}


//*******EVENTOS*******

function loadEtapasCancelamentoOcorrencia() {
    _etapaCancelamentoOcorrencia = new EtapaCancelamentoOcorrencia();
    KoBindings(_etapaCancelamentoOcorrencia, "knockoutEtapaCancelamentoOcorrencia");
    Etapa1Liberada();
}

function SetarEtapaInicioCancelamento() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCancelamentoOcorrencia.Etapa1.idTab).click();
}

function SetarEtapasCancelamento() {
    var situacao = _cancelamentoOcorrencia.Situacao.val();

    if (situacao == EnumSituacaoCancelamentoOcorrencia.Todas)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoCancelamentoOcorrencia.EmCancelamento)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoCancelamentoOcorrencia.Cancelada)
        Etapa3Aprovada();
    else if (situacao == EnumSituacaoCancelamentoOcorrencia.RejeicaoCancelamento)
        Etapa2Reprovada();
    else if (situacao == EnumSituacaoCancelamentoOcorrencia.AguardandoIntegracao)
        Etapa3Aguardando();
    else if (situacao == EnumSituacaoCancelamentoOcorrencia.FalhaIntegracao)
        Etapa3Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCancelamentoOcorrencia.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoOcorrencia.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCancelamentoOcorrencia.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoOcorrencia.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaCancelamentoOcorrencia.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamentoOcorrencia.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Aprovada() {
    $("#" + _etapaCancelamentoOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoOcorrencia.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaCancelamentoOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoOcorrencia.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
} 

function Etapa2Aguardando() {
    $("#" + _etapaCancelamentoOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoOcorrencia.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaCancelamentoOcorrencia.Etapa2.idTab).click();
    Etapa1Aprovada();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaCancelamentoOcorrencia.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamentoOcorrencia.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Aguardando() {
    $("#" + _etapaCancelamentoOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoOcorrencia.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaCancelamentoOcorrencia.Etapa3.idTab).click();
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapaCancelamentoOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoOcorrencia.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    $("#" + _etapaCancelamentoOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoOcorrencia.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}