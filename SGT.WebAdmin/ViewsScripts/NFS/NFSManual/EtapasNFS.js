/// <reference path="SolicitacaoAvaria.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoLancamentoNFSManual.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaNFS;

var EtapaNFS = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.Etapa1 = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção se documentos."),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Dados NFS", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É onde se informa os dados iniciais para a emissão da NFS."),
        tooltipTitle: ko.observable("Dados NFS")
    });
    this.Etapa3 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Nessa etapa é possível acompanhar a aprvação da emissão."),
        tooltipTitle: ko.observable("Aprovação")
    });
    this.Etapa4 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("Caso tenha integração, nessa etapa é possível acompanhar."),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function loadEtapasNFS() {
    _etapaNFS = new EtapaNFS();
    KoBindings(_etapaNFS, "knockoutEtapaLancamentoNFS");
    Etapa1Liberada();
}

function SetarEtapaInicioLancamento() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaNFS.Etapa1.idTab).click();
    $("#" + _etapaNFS.Etapa1.idTab).tab("show");
}

function SetarEtapasNFS() {
    var situacao = _nfsManual.Situacao.val();

    if (situacao == EnumSituacaoLancamentoNFSManual.Cancelada || situacao == EnumSituacaoLancamentoNFSManual.Cancelada)
        situacao = _nfsManual.SituacaoNoCancelamento.val();

    if (situacao == EnumSituacaoLancamentoNFSManual.Todas)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoLancamentoNFSManual.DadosNota)
        Etapa2Liberada();
    else if (situacao == EnumSituacaoLancamentoNFSManual.AgAprovacao)
        Etapa3Aguardando();
    else if (situacao == EnumSituacaoLancamentoNFSManual.Reprovada)
        Etapa3Reprovada();
    else if (situacao == EnumSituacaoLancamentoNFSManual.SemRegra)
        Etapa3Reprovada();
    else if (situacao == EnumSituacaoLancamentoNFSManual.EmEmissao)
        Etapa3Aprovada();
    else if (situacao == EnumSituacaoLancamentoNFSManual.FalhaEmissao)
        Etapa3Etapa3Reprovada();
    else if (situacao == EnumSituacaoLancamentoNFSManual.AgIntegracao)
        Etapa4Aguardando();
    else if (situacao == EnumSituacaoLancamentoNFSManual.FalhaIntegracao)
        Etapa4Reprovada();
    else if (situacao == EnumSituacaoLancamentoNFSManual.Finalizada)
        Etapa4Aprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaNFS.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaNFS.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaNFS.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaNFS.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Aprovada() {
    $("#" + _etapaNFS.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Liberada() {
    $("#" + _etapaNFS.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaNFS.Etapa2.idTab).click();
    $("#" + _etapaNFS.Etapa2.idTab).tab("show");
    Etapa1Aprovada();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaNFS.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaNFS.Etapa3.idTab + " .step").attr("class", "step");
    Etapa4Desabilitada();
}

function Etapa3Aguardando() {
    $("#" + _etapaNFS.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaNFS.Etapa3.idTab).click();
    $("#" + _etapaNFS.Etapa3.idTab).tab("show");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapaNFS.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Etapa3Reprovada() {
    $("#" + _etapaNFS.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}

function Etapa3SemRegra() {
    $("#" + _etapaNFS.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
    EtapaSemRegra();
}

function Etapa3Reprovada() {
    $("#" + _etapaNFS.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}

function Etapa3Liberada() {
    $("#" + _etapaNFS.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa3.idTab + " .step").attr("class", "step blue");
    Etapa2Aprovada();
}


//*******Etapa 4*******

function Etapa4Desabilitada() {
    $("#" + _etapaNFS.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaNFS.Etapa4.idTab + " .step").attr("class", "step");
    _etapaNFS.Etapa4.eventClick = function () { };
    
}
function Etapa4Aguardando() {
    $("#" + _etapaNFS.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa4.idTab + " .step").attr("class", "step yellow");
    _etapaNFS.Etapa4.eventClick = BuscarDadosIntegracoesLancamentoNFSManual;
    Etapa3Aprovada();
}

function Etapa4Reprovada() {
    $("#" + _etapaNFS.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa4.idTab + " .step").attr("class", "step red");
    _etapaNFS.Etapa4.eventClick = BuscarDadosIntegracoesLancamentoNFSManual;
   
    Etapa3Aprovada();
}

function Etapa4Aprovada() {
    $("#" + _etapaNFS.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaNFS.Etapa4.idTab + " .step").attr("class", "step green");
    _etapaNFS.Etapa4.eventClick = BuscarDadosIntegracoesLancamentoNFSManual;
    
    Etapa3Aprovada();
}