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
/// <reference path="../../Enumeradores/EnumSituacaoProvisao.js" />
/// <reference path="../../Enumeradores/EnumTipoProvisao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaProvisao;

var EtapaProvisao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable(_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? "50%" : "33%") });

    this.Etapa1 = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção se documentos."),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? false : true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa onde a provisão é confirmada antes da integração."),
        tooltipTitle: ko.observable("Fechamento")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? 2 : 3),
        tooltip: ko.observable("Caso tenha integração, nessa etapa é possível acompanhar."),
        tooltipTitle: ko.observable("Integração")
    });

    this.TipoProvisao = PropertyEntity({ text: "Tipo Provisão", val: ko.observable(_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? EnumTipoProvisao.ProvisaoPorCTe : EnumTipoProvisao.ProvisaoPorNotaFiscal), options: EnumTipoProvisao.obterOpcoes(), def: _CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? EnumTipoProvisao.ProvisaoPorCTe : EnumTipoProvisao.ProvisaoPorNotaFiscal, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.PermitirProvisionamentoDeNotasCTesNaTelaProvisao) });

    this.TipoProvisao.val.subscribe(function (valorEnumerador) {
        AtualizacaoEtapasPorEnumerador(valorEnumerador);
    });
}

//*******EVENTOS*******

function loadEtapasProvisao() {
    _etapaProvisao = new EtapaProvisao();
    KoBindings(_etapaProvisao, "knockoutEtapaProvisao");

    Etapa1Liberada();

    AtualizacaoEtapasPorEnumerador(_etapaProvisao.TipoProvisao.val());
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
}

function SetarEtapasProvisao() {
    var situacao = _provisao.Situacao.val();

    //if (situacao == EnumSituacaoProvisao.Cancelada)
    //situacao = _etapaProvisao.SituacaoNoCancelamento.val();

    if (situacao === EnumSituacaoProvisao.Todos)
        Etapa1Liberada();
    else if (situacao === EnumSituacaoProvisao.EmAlteracao)
        Etapa1Liberada();
    else if (situacao === EnumSituacaoProvisao.EmFechamento)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoProvisao.PendenciaFechamento)
        Etapa2Reprovada();
    else if (situacao === EnumSituacaoProvisao.AgIntegracao)
        Etapa3Aguardando();
    else if (situacao === EnumSituacaoProvisao.Finalizado)
        Etapa3Aprovada();
    else if (situacao === EnumSituacaoProvisao.FalhaIntegracao)
        Etapa3Reprovada();
    else if (situacao === EnumSituacaoProvisao.EmIntegracao)
        Etapa3Aguardando();
    else if (situacao === EnumSituacaoProvisao.Cancelado)
        Etapa3Cancelada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaProvisao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaProvisao.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaProvisao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaProvisao.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaProvisao.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaProvisao.Etapa2.idTab + " .step").attr("class", "step");
    _etapaProvisao.Etapa2.eventClick = function () { };
    Etapa3Desabilitada();
}

function Etapa2Aguardando() {
    $("#" + _etapaProvisao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaProvisao.Etapa2.idTab + " .step").attr("class", "step yellow");
    _etapaProvisao.Etapa2.eventClick = buscarDadosFechamentoProvisao;
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaProvisao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaProvisao.Etapa2.idTab + " .step").attr("class", "step red");
    _etapaProvisao.Etapa2.eventClick = buscarDadosFechamentoProvisao;
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaProvisao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaProvisao.Etapa2.idTab + " .step").attr("class", "step green");
    _etapaProvisao.Etapa2.eventClick = buscarDadosFechamentoProvisao;
    Etapa1Aprovada();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaProvisao.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaProvisao.Etapa3.idTab + " .step").attr("class", "step");
    _etapaProvisao.Etapa3.eventClick = function () { };
}

function Etapa3Aguardando() {
    $("#" + _etapaProvisao.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaProvisao.Etapa3.idTab + " .step").attr("class", "step yellow");
    _etapaProvisao.Etapa3.eventClick = CarregaIntegracao;
    Etapa2Aprovada();
    ocultarBotoesFechamento();
}

function Etapa3Reprovada() {
    $("#" + _etapaProvisao.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaProvisao.Etapa3.idTab + " .step").attr("class", "step red");
    _etapaProvisao.Etapa3.eventClick = CarregaIntegracao;
    Etapa2Aprovada();
    ocultarBotoesFechamento();
}

function Etapa3Aprovada() {
    $("#" + _etapaProvisao.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaProvisao.Etapa3.idTab + " .step").attr("class", "step green");
    _etapaProvisao.Etapa3.eventClick = CarregaIntegracao;
    Etapa2Aprovada();
    ocultarBotoesFechamento();
}

function Etapa3Cancelada() {
    $("#" + _etapaProvisao.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaProvisao.Etapa3.idTab + " .step").attr("class", "step red");
    _etapaProvisao.Etapa3.eventClick = CarregaIntegracao;
    Etapa2Aprovada();
    ocultarBotoesFechamento();
}

function AtualizacaoEtapasPorEnumerador(valorEnumerador) {
    if (valorEnumerador == EnumTipoProvisao.ProvisaoPorNotaFiscal) {
        _etapaProvisao.Etapa2.visible(true);
        _etapaProvisao.Etapa3.step(3);
        _etapaProvisao.TamanhoEtapa.val("33%");
        _selecaoDocumentos.TipoEtapasDocumentoProvisao.val(false);
    } else if (valorEnumerador == EnumTipoProvisao.ProvisaoPorCTe) {
        _etapaProvisao.Etapa2.visible(false);
        _etapaProvisao.Etapa3.step(2);
        _etapaProvisao.TamanhoEtapa.val("50%");
        _selecaoDocumentos.TipoEtapasDocumentoProvisao.val(true);
    }
}