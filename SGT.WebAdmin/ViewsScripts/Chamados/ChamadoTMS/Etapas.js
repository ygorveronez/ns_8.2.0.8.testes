/// <reference path="ChamadoTMS.js" />
/// <reference path="ChamadoTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamadoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapa;

var Etapas = function () {
    this.Etapa1 = PropertyEntity({
        text: "Lançamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à abertura do chamado."),
        tooltipTitle: ko.observable("Lançamento")
    });
    this.Etapa2 = PropertyEntity({
        text: "Cliente/Motorista", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É onde informa dados para o Cliente ou Motorista."),
        tooltipTitle: ko.observable("Cliente/Motorista")
    });
    this.Etapa3 = PropertyEntity({
        text: ko.observable("Análise"), type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("É onde ocorre a análise dos operadores."),
        tooltipTitle: ko.observable("Análise")
    });
    this.Etapa4 = PropertyEntity({
        text: ko.observable("Cobrança"), type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("Quando necessário, é criado uma cobrança."),
        tooltipTitle: ko.observable("Cobrança")
    });

    var etapas = 0;
    for (var i in this) if (/(Etapa)[0-9]+/.test(i)) etapas++;
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable((100 / etapas).toFixed(2) + "%") });
}


//*******EVENTOS*******

function loadEtapasChamado() {
    _etapa = new Etapas();
    KoBindings(_etapa, "knockoutEtapaChamado");
    Etapa1LiberadaChamado();
}

function SetarEtapaInicioChamado() {
    DesabilitarTodasEtapasChamado();
    Etapa1LiberadaChamado();
    $("#" + _etapa.Etapa1.idTab).click();
}

function SetarEtapaChamado() {
    var situacao = _chamadoTMS.Situacao.val();
    if (situacao === EnumSituacaoChamadoTMS.Todos)
        Etapa1LiberadaChamado();
    else if (situacao === EnumSituacaoChamadoTMS.Aberto)
        Etapa2LiberadaChamado();
    else if (situacao === EnumSituacaoChamadoTMS.EmAnalise)
        Etapa3LiberadaChamado();
    else if (situacao === EnumSituacaoChamadoTMS.AguardandoAutorizacao)
        Etapa3AguardandoChamados();
    else if (situacao === EnumSituacaoChamadoTMS.PagamentoNaoAutorizado)
        Etapa3DesabilitadaReprovada();
    else if (situacao === EnumSituacaoChamadoTMS.Finalizado)
        Etapa4AprovadaChamado();
    else if (situacao === EnumSituacaoChamadoTMS.LiberadaOcorrencia)
        Etapa3AprovadaChamado();
    else if (situacao === EnumSituacaoChamadoTMS.Cancelado)
        Etapa3DesabilitadaReprovada();
}

function DesabilitarTodasEtapasChamado() {
    Etapa2DesabilitadaChamado();
    Etapa3DesabilitadaChamado();
    Etapa4DesabilitadaChamado();
}

//*******Etapa 1*******

function Etapa1LiberadaChamado() {
    $("#" + _etapa.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2DesabilitadaChamado();
}

function Etapa1AprovadaChamado() {
    $("#" + _etapa.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2DesabilitadaChamado() {
    $("#" + _etapa.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3DesabilitadaChamado();
}

function Etapa2AprovadaChamado() {
    $("#" + _etapa.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1AprovadaChamado();
}

function Etapa2LiberadaChamado() {
    $("#" + _etapa.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapa.Etapa2.idTab).click();
    Etapa1AprovadaChamado();
}

function Etapa2AguardandoChamados() {
    $("#" + _etapa.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1AprovadaChamado();
}

//*******Etapa 3*******

function Etapa3DesabilitadaChamado() {
    $("#" + _etapa.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step");
    Etapa4DesabilitadaChamado();
}

function Etapa3AguardandoChamados() {
    $("#" + _etapa.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapa.Etapa3.idTab).click();
    Etapa2AprovadaChamado();
}

function Etapa3AprovadaChamado() {
    $("#" + _etapa.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2AprovadaChamado();
}

function Etapa3DesabilitadaReprovada() {
    $("#" + _etapa.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2AprovadaChamado();
}

function Etapa3LiberadaChamado() {
    $("#" + _etapa.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapa.Etapa3.idTab).click();
    Etapa2AprovadaChamado();
}

//*******Etapa 4*******

function Etapa4DesabilitadaChamado() {
    $("#" + _etapa.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step");
}

function Etapa4AguardandoChamados() {
    $("#" + _etapa.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step yellow");
    Etapa3AprovadaChamado();
}

function Etapa4AprovadaChamado() {
    $("#" + _etapa.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step green");
    Etapa3AprovadaChamado();
}

function Etapa4DesabilitadaReprovada() {
    $("#" + _etapa.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step red");
    Etapa3AprovadaChamado();
}

function Etapa4LiberadaChamado() {
    $("#" + _etapa.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step blue");
    $("#" + _etapa.Etapa4.idTab).click();
    Etapa3AprovadaChamado();
}