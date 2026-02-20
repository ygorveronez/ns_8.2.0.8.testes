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
/// <reference path="../../Enumeradores/EnumSituacaoLote.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaLote;

var EtapaLote = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ? "25%" : "33%") });

    this.Etapa1 = PropertyEntity({
        text: "Lote", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde é feito a manutenção das avarias do lote."),
        tooltipTitle: ko.observable("Lote")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aceite Transportador", type: types.local, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiTMS), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Situação da aprovação do transportador."),
        tooltipTitle: ko.observable("Aceite Transportador")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ? 2 : 3),
        tooltip: ko.observable("Esta etapa é onde a integração dos dados são feitas."),
        tooltipTitle: ko.observable("Integração")
    });
    this.Etapa4 = PropertyEntity({
        text: "Ocorrência", type: types.local, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Quando necessário, é criado uma ocorrência.."),
        tooltipTitle: ko.observable("Ocorrência")
    });
    this.Etapa5 = PropertyEntity({
        text: "Destino Avaria", type: types.local, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("É onde poderá aplicar um destino nos produtos"),
        tooltipTitle: ko.observable("Destino Avaria")
    });
};

//*******EVENTOS*******

function loadEtapasLote() {
    _etapaLote = new EtapaLote();
    KoBindings(_etapaLote, "knockoutEtapas");
    Etapa1LoteLiberada();
}

function setarEtapaInicioLote() {
    DesabilitarTodasEtapasLote();
    Etapa1LoteLiberada();
    $("#" + _etapaLote.Etapa1.idTab).click();
}

function setarEtapasAvaria() {
    var situacao = _lote.Situacao.val();

    if (situacao == EnumSituacaoLote.Todas)
        Etapa1LoteLiberada();
    else if (situacao == EnumSituacaoLote.EmCriacao)
        Etapa1LoteLiberada();
    else if (situacao == EnumSituacaoLote.AgAprovacao)
        Etapa2LoteAguardando();
    else if (situacao == EnumSituacaoLote.EmCorrecao)
        Etapa2LoteAguardando();
    else if (situacao == EnumSituacaoLote.Reprovacao)
        Etapa2LoteReprovada();
    else if (situacao == EnumSituacaoLote.AgIntegracao)
        Etapa3LoteAguardando();
    else if (situacao == EnumSituacaoLote.AgAprovacaoIntegracao)
        Etapa3LoteAguardando();
    else if (situacao == EnumSituacaoLote.Finalizada) {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
            Etapa5LoteAguardando();
        else
            Etapa3LoteAprovada();
    }
    else if (situacao == EnumSituacaoLote.IntegracaoReprovada)
        Etapa3LoteReprovada();
    else if (situacao == EnumSituacaoLote.FalhaIntegracao)
        Etapa3LoteReprovada();
    else if (situacao == EnumSituacaoLote.EmIntegracao)
        Etapa3LoteAguardando();
    else if (situacao == EnumSituacaoLote.FinalizadaComDestino)
        Etapa5LoteAprovada();
}

function DesabilitarTodasEtapasLote() {
    Etapa2LoteDesabilitada();
    Etapa3LoteDesabilitada();
    Etapa4LoteDesabilitada();
    Etapa5LoteDesabilitada();
}

//*******Etapa 1*******

function Etapa1LoteLiberada() {
    $("#" + _etapaLote.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2LoteDesabilitada();
}

function Etapa1LoteAprovada() {
    $("#" + _etapaLote.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2LoteDesabilitada() {
    $("#" + _etapaLote.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLote.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3LoteDesabilitada();
}

function Etapa2LoteAguardando() {
    $("#" + _etapaLote.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1LoteAprovada();
}

function Etapa2LoteAprovada() {
    $("#" + _etapaLote.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1LoteAprovada();
}

function Etapa2LoteReprovada() {
    $("#" + _etapaLote.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1LoteAprovada();
    Etapa3LoteDesabilitada();
}

//*******Etapa 3*******

function Etapa3LoteDesabilitada() {
    $("#" + _etapaLote.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLote.Etapa3.idTab + " .step").attr("class", "step");
    Etapa4LoteDesabilitada();
}

function Etapa3LoteAguardando() {
    $("#" + _etapaLote.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2LoteAprovada();
}

function Etapa3LoteAprovada() {
    $("#" + _etapaLote.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2LoteAprovada();
}

function Etapa3LoteReprovada() {
    $("#" + _etapaLote.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2LoteAprovada();
}

//*******Etapa 4*******

function Etapa4LoteDesabilitada() {
    _etapaLote.Etapa4.eventClick = function () { };

    $("#" + _etapaLote.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLote.Etapa4.idTab + " .step").attr("class", "step");
    Etapa5LoteDesabilitada();
}

function Etapa4LoteAguardando() {
    _etapaLote.Etapa4.eventClick = EtapaOcorrenciaLoteAvariaClick;
    $("#" + _etapaLote.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa4.idTab + " .step").attr("class", "step yellow");
    Etapa3LoteAprovada();
}

function Etapa4LoteAprovada() {
    _etapaLote.Etapa4.eventClick = EtapaOcorrenciaLoteAvariaClick;
    $("#" + _etapaLote.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa4.idTab + " .step").attr("class", "step green");
    Etapa3LoteAprovada();
}

function Etapa4LoteReprovada() {
    _etapaLote.Etapa4.eventClick = EtapaOcorrenciaLoteAvariaClick;
    $("#" + _etapaLote.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa4.idTab + " .step").attr("class", "step red");
    Etapa3LoteAprovada();
}

//*******Etapa 5*******

function Etapa5LoteDesabilitada() {
    _etapaLote.Etapa5.eventClick = function () { };
    $("#" + _etapaLote.Etapa5.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLote.Etapa5.idTab + " .step").attr("class", "step");
}

function Etapa5LoteAguardando() {
    _etapaLote.Etapa5.eventClick = CarregarDestinoAvarias;
    $("#" + _etapaLote.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa5.idTab + " .step").attr("class", "step yellow");
    Etapa4LoteAprovada();
}

function Etapa5LoteAprovada() {
    _etapaLote.Etapa5.eventClick = CarregarDestinoAvarias;
    $("#" + _etapaLote.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa5.idTab + " .step").attr("class", "step green");
    Etapa4LoteAprovada();
}

function Etapa5LoteReprovada() {
    _etapaLote.Etapa5.eventClick = CarregarDestinoAvarias;
    $("#" + _etapaLote.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLote.Etapa5.idTab + " .step").attr("class", "step red");
    Etapa4LoteAprovada();
}