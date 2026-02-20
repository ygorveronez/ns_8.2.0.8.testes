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
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="CTeComplementar.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaOcorrencia;

var EtapaOcorrencia = function () {
    var _this = this;

    this.Etapa1 = PropertyEntity({
        text: Localization.Resources.Ocorrencias.Ocorrencia.DescricaoOcorrencia, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.OndeSeInformaOsDadosIniciais),
        tooltipTitle: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.DescricaoOcorrencia)
    });
    this.Etapa1.visible.subscribe(EtapaAlternada);

    this.Etapa2 = PropertyEntity({
        text: Localization.Resources.Ocorrencias.Ocorrencia.Aprovacao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.CasoOcorrenciaNecessiteAprovacaoSeraRealizada),
        tooltipTitle: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.Aprovacao)
    });
    this.Etapa2.visible.subscribe(EtapaAlternada);

    this.Etapa3FilialEmissora = PropertyEntity({
        text: Localization.Resources.Ocorrencias.Ocorrencia.EmissaoFilialEmissora, type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.EstaEtapaOndeDocumentosComplementaresFilial),
        tooltipTitle: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.EmissaoFilialEmissora)
    });
    this.Etapa3FilialEmissora.visible.subscribe(EtapaAlternada);

    this.Etapa3IntegracaoFilialEmissora = PropertyEntity({
        text: Localization.Resources.Ocorrencias.Ocorrencia.IntegracaoFilialEmissora, type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.NestaEtapaEResposanvelPelaGeracaoAqruivos),
        tooltipTitle: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.IntegracaoFilialEmissora)
    });
    this.Etapa3IntegracaoFilialEmissora.visible.subscribe(EtapaAlternada);

    this.Etapa3 = PropertyEntity({
        text: Localization.Resources.Ocorrencias.Ocorrencia.Emissao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.EstaEtapaOndeOsDocumentosComplementaresEmitidos),
        tooltipTitle: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.Emissao)
    });
    this.Etapa3.visible.subscribe(EtapaAlternada);

    this.Etapa4 = PropertyEntity({
        text: Localization.Resources.Ocorrencias.Ocorrencia.Aceite, type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.NesataEtapaEPossivelAcompanharAceitoTransportador),
        tooltipTitle: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.Aceite)
    });
    this.Etapa4.visible.subscribe(EtapaAlternada);

    this.Etapa5 = PropertyEntity({
        text: Localization.Resources.Ocorrencias.Ocorrencia.Integracao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(5),
        tooltip: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.NestaEtapaResposavelPelaGeracaoArquivos),
        tooltipTitle: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.Integracao)
    });
    this.Etapa5.visible.subscribe(EtapaAlternada);

    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("") });
}

//*******EVENTOS*******

function loadEtapaOcorrencia() {
    _etapaOcorrencia = new EtapaOcorrencia();
    KoBindings(_etapaOcorrencia, "knockoutEtapaOcorrencia");
    Etapa1Liberada();
    //$("#" + _etapaOcorrencia.Etapa1.idTab).click();

    EtapaAlternada();
}

function AjustarEtapasOcorrencia() {
    if (_ocorrencia.EmiteComplementoFilialEmissora.val()) {
        //_etapaOcorrencia.TamanhoEtapa.val("20%");
        _etapaOcorrencia.Etapa3FilialEmissora.visible(true);
        _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.visible(true);
        //_etapaOcorrencia.Etapa3.step(4);
        //_etapaOcorrencia.Etapa4.step(5);
    } else {
        //_etapaOcorrencia.TamanhoEtapa.val("25%");
        _etapaOcorrencia.Etapa3FilialEmissora.visible(false);
        _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.visible(false);
        //_etapaOcorrencia.Etapa3.step(3);
        //_etapaOcorrencia.Etapa4.step(4);
    }
}

function setarEtapaInicioOcorrencia() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    AlternaEtapas();
    $("#" + _etapaOcorrencia.Etapa1.idTab).click();
    $("#" + _etapaOcorrencia.Etapa1.idTab).tab("show");
}

function setarEtapasOcorrencia() {
    AjustarEtapasOcorrencia();

    var ocorrenciaCancelada = false;
    var situacaoOcorrencia = _ocorrencia.SituacaoOcorrencia.val();
    var emiteComplementoFilialEmissora = _ocorrencia.EmiteComplementoFilialEmissora.val();
    var liberadaParaEmissaoCTeSubContratacaoFilialEmissora = _ocorrencia.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val();
    var integrandoFilialEmissora = _ocorrencia.IntegrandoFilialEmissora.val();
    var reenviouIntegracaoFilialEmissora = _ocorrencia.ReenviouIntegracaoFilialEmissora.val();

    if (situacaoOcorrencia == EnumSituacaoOcorrencia.Cancelada) {
        situacaoOcorrencia = _ocorrencia.SituacaoOcorrenciaNoCancelamento.val();
        ocorrenciaCancelada = true;
    }

    if (situacaoOcorrencia == EnumSituacaoOcorrencia.AgAprovacao || situacaoOcorrencia == EnumSituacaoOcorrencia.AgConfirmacaoUso)
        Etapa2Aguardando();
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.Rejeitada)
        Etapa2Reprovada();
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.RejeitadaEtapaEmissao)
        if (!emiteComplementoFilialEmissora || liberadaParaEmissaoCTeSubContratacaoFilialEmissora)
            Etapa3Reprovada();
        else
            Etapa3FilialEmissoraReprovada();
    //else if (situacaoOcorrencia == EnumSituacaoOcorrencia.AgEmissaoCTeComplementar || situacaoOcorrencia == EnumSituacaoOcorrencia.EmEmissaoCTeComplementar) {
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.EmEmissaoCTeComplementar) {
        if (_ocorrencia.EmiteNFSeFora.val() && _ocorrencia.ErroIntegracaoComGPA.val())
            if (!emiteComplementoFilialEmissora || liberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                Etapa3Problema();
            else
                Etapa3FilialEmissoraProblema();
        else if (!_ocorrencia.AgImportacaoCTe.val()) {
            if (!emiteComplementoFilialEmissora || liberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                Etapa3Aguardando();
            else
                Etapa3FilialEmissoraAguardando();
        }
        else {
            if (!emiteComplementoFilialEmissora || liberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                Etapa3AguardandoImportacao();
            else
                Etapa3FilialEmissoraAguardandoImportacao();
        }

    }
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.AgAutorizacaoEmissao) {
        if (!emiteComplementoFilialEmissora || liberadaParaEmissaoCTeSubContratacaoFilialEmissora)
            Etapa3AguardandoAutorizacaoEmissao();
        else
            Etapa3FilialEmissoraAguardandoAutorizacaoEmissao();
    }
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.PendenciaEmissao) {
        if (!emiteComplementoFilialEmissora || liberadaParaEmissaoCTeSubContratacaoFilialEmissora)
            Etapa3Problema();
        else
            Etapa3FilialEmissoraProblema();
    }
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.AgIntegracao) {
        if (reenviouIntegracaoFilialEmissora) {
            Etapa5Aprovada();
            Etapa3IntegracaoFilialEmissoraAguardando();
        } else {
            if (integrandoFilialEmissora && !liberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                Etapa3IntegracaoFilialEmissoraAguardando();
            else
                Etapa5Aguardando();
        }
    }
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.FalhaIntegracao) {
        if (integrandoFilialEmissora && !liberadaParaEmissaoCTeSubContratacaoFilialEmissora)
            Etapa3IntegracaoFilialEmissoraProblema();
        else
            Etapa5Problema();
    }
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.Finalizada) {
        Etapa5Aprovada();
    }
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.AgInformacoes)
        Etapa1Aguardando();
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.SemRegraAprovacao)
        Etapa2SemRegra(ocorrenciaCancelada);
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.SemRegraEmissao) {
        if (!emiteComplementoFilialEmissora || liberadaParaEmissaoCTeSubContratacaoFilialEmissora)
            Etapa3SemRegra(ocorrenciaCancelada);
        else
            Etapa3FilialEmissoraSemRegra(ocorrenciaCancelada);
    }
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.AgAceiteTransportador)
        Etapa4Aguardando();
    else if (situacaoOcorrencia == EnumSituacaoOcorrencia.DebitoRejeitadoTransportador)
        Etapa4Reprovado();


}

function EtapaAlternada() {
    var etapas_visiveis = [];

    for (var i in _etapaOcorrencia) {
        var match = i.match(/^Etapa([0-9]+)/);
        if (match != null && _etapaOcorrencia[i].visible()) {
            etapas_visiveis.push({
                num: parseInt(match[1]),
                prop: _etapaOcorrencia[i]
            });
        }
    }

    etapas_visiveis
        // Ordena pelo nome da propriedade
        .sort(function (a, b) {
            return a.num - b.num;
        })
        // seta o numero da etapa
        .map(function (c, i) {
            c.prop.step(i + 1);
        });

    _etapaOcorrencia.TamanhoEtapa.val(Math.floor(100 / etapas_visiveis.length) + "%");
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    //Etapa3Desabilitada();
    //Etapa3FilialEmissoraDesabilitada();
    //Etapa4Desabilitada();

    AjustarEtapasOcorrencia();
}


function EtapaSemRegra(ocorrenciaCancelada) {
    /* Quando a ocorrência esta cancelada, a situação para a etapa é a mesma de que quando ela foi cancelada
     * Mas para evitar que apareca mensagens de aviso de SEM REGRA e que apareca o botão de reprocessar regra
     * A flag de ocorrencia cancelada é enviada
     */
    if (!ocorrenciaCancelada) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.Ocorrencia.RegrasEtapa, Localization.Resources.Ocorrencias.Ocorrencia.NenhumaRegraEncontradaOcorrenciaPermanece);
        _CRUDOcorrencia.ReprocessarRegras.visible(true);
    }

    _ocorrenciaAutorizacao.MensagemEtapaSemRegra.visible(true);
    _ocorrenciaAutorizacaoEmissao.MensagemEtapaSemRegra.visible(true);
}

function AlternaEtapas() {
    if (!_CONFIGURACAO_TMS.NaoExigeAceiteTransportadorParaNFDebito && configuracaoTipoOcorrencia != null && configuracaoTipoOcorrencia.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == EnumTipoDocumentoCreditoDebito.Debito)
        _etapaOcorrencia.Etapa4.visible(true);
    else
        _etapaOcorrencia.Etapa4.visible(false);
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaOcorrencia.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaOcorrencia.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Aguardando() {
    $("#" + _etapaOcorrencia.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa1.idTab + " .step").attr("class", "step yellow");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaOcorrencia.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOcorrencia.Etapa2.idTab + " .step").attr("class", "step");
    if (!_etapaOcorrencia.Etapa3FilialEmissora.visible())
        Etapa3Desabilitada();
    else
        Etapa3IntegracaoFilialEmissoraDesabilitada();
}

function Etapa2Liberada() {
    $("#" + _etapaOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2SemRegra(ocorrenciaCancelada) {
    $("#" + _etapaOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
    EtapaSemRegra(ocorrenciaCancelada);
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaOcorrencia.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOcorrencia.Etapa3.idTab + " .step").attr("class", "step");
    _etapaOcorrencia.Etapa3.eventClick = function () { };
    if (!_ocorrencia.EmiteComplementoFilialEmissora.val())
        Etapa4Desabilitada();
    else
        Etapa3IntegracaoFilialEmissoraDesabilitada();
}

function Etapa3Liberada() {
    $("#" + _etapaOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3.idTab + " .step").attr("class", "step yellow");
    _etapaOcorrencia.Etapa3.eventClick = function () { BuscarDocumentosComplementares(false); };
    if (!_ocorrencia.EmiteComplementoFilialEmissora.val())
        Etapa2Aprovada();
    else
        Etapa3IntegracaoFilialEmissoraAprovada();
}

function Etapa3AguardandoImportacao() {
    $("#" + _etapaOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3.idTab + " .step").attr("class", "step orange");
    _etapaOcorrencia.Etapa3.eventClick = function () { BuscarDocumentosComplementares(false); };
    if (!_ocorrencia.EmiteComplementoFilialEmissora.val())
        Etapa2Aprovada();
    else
        Etapa3IntegracaoFilialEmissoraAprovada();
}

function Etapa3Aguardando() {
    $("#" + _etapaOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3.idTab + " .step").attr("class", "step yellow");
    _etapaOcorrencia.Etapa3.eventClick = function () { BuscarDocumentosComplementares(false); };
    if (!_ocorrencia.EmiteComplementoFilialEmissora.val())
        Etapa2Aprovada();
    else
        Etapa3IntegracaoFilialEmissoraAprovada();
}

function Etapa3AguardandoAutorizacaoEmissao() {
    $("#" + _etapaOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3.idTab + " .step").attr("class", "step yellow");
    _etapaOcorrencia.Etapa3.eventClick = AtualizarAprovadoresEmissao;
    if (!_ocorrencia.EmiteComplementoFilialEmissora.val())
        Etapa2Aprovada();
    else
        Etapa3IntegracaoFilialEmissoraAprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapaOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3.idTab + " .step").attr("class", "step green");
    _etapaOcorrencia.Etapa3.eventClick = function () { BuscarDocumentosComplementares(false); };
    if (!_ocorrencia.EmiteComplementoFilialEmissora.val())
        Etapa2Aprovada();
    else
        Etapa3IntegracaoFilialEmissoraAprovada();
}

function Etapa3Problema() {
    $("#" + _etapaOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3.idTab + " .step").attr("class", "step red");
    _etapaOcorrencia.Etapa3.eventClick = function () { BuscarDocumentosComplementares(false); };
    if (!_ocorrencia.EmiteComplementoFilialEmissora.val())
        Etapa2Aprovada();
    else
        Etapa3IntegracaoFilialEmissoraAprovada();
}

function Etapa3SemRegra(ocorrenciaCancelada) {
    $("#" + _etapaOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3.idTab + " .step").attr("class", "step red");
    if (!_ocorrencia.EmiteComplementoFilialEmissora.val())
        Etapa2Aprovada();
    else
        Etapa3IntegracaoFilialEmissoraAprovada();
    EtapaSemRegra(ocorrenciaCancelada);
}

function Etapa3Reprovada() {
    $("#" + _etapaOcorrencia.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3.idTab + " .step").attr("class", "step red");
    if (!_ocorrencia.EmiteComplementoFilialEmissora.val())
        Etapa2Aprovada();
    else
        Etapa3IntegracaoFilialEmissoraAprovada();
}


//*******Etapa 3 Filial Emissora*******

function Etapa3FilialEmissoraDesabilitada() {
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab + " .step").attr("class", "step");
    _etapaOcorrencia.Etapa3FilialEmissora.eventClick = function () { };
    Etapa4Desabilitada();
}

function Etapa3FilialEmissoraLiberada() {
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab + " .step").attr("class", "step yellow");
    _etapaOcorrencia.Etapa3FilialEmissora.eventClick = function () { BuscarDocumentosComplementares(true); };
    Etapa2Aprovada();
}

function Etapa3FilialEmissoraAguardandoImportacao() {
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab + " .step").attr("class", "step orange");
    _etapaOcorrencia.Etapa3FilialEmissora.eventClick = function () { BuscarDocumentosComplementares(true); };
    Etapa2Aprovada();
}

function Etapa3FilialEmissoraAguardando() {
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab + " .step").attr("class", "step yellow");
    _etapaOcorrencia.Etapa3FilialEmissora.eventClick = function () { BuscarDocumentosComplementares(true); };
    Etapa2Aprovada();
}

function Etapa3FilialEmissoraAguardandoAutorizacaoEmissao() {
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab + " .step").attr("class", "step yellow");
    _etapaOcorrencia.Etapa3FilialEmissora.eventClick = function () { BuscarDocumentosComplementares(true); };
    Etapa2Aprovada();
}

function Etapa3FilialEmissoraAprovada() {
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab + " .step").attr("class", "step green");
    _etapaOcorrencia.Etapa3FilialEmissora.eventClick = function () { BuscarDocumentosComplementares(true); };
    Etapa2Aprovada();
}

function Etapa3FilialEmissoraProblema() {
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab + " .step").attr("class", "step red");
    _etapaOcorrencia.Etapa3FilialEmissora.eventClick = function () { BuscarDocumentosComplementares(true); };
    Etapa2Aprovada();
}

function Etapa3FilialEmissoraSemRegra(ocorrenciaCancelada) {
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
    EtapaSemRegra(ocorrenciaCancelada);
}

function Etapa3FilialEmissoraReprovada() {
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3FilialEmissora.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}

//*******Etapa 3 Integração Filial Emissora*******

function Etapa3IntegracaoFilialEmissoraAguardando() {
    _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.eventClick = function () { BuscarDadosIntegracoesOcorrencia(true) };
    $("#" + _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.idTab + " .step").attr("class", "step yellow");
    if (!_ocorrencia.EmiteComplementoFilialEmissora.val())
        Etapa2Aprovada();
    else
        Etapa3FilialEmissoraAprovada();
}

function Etapa3IntegracaoFilialEmissoraDesabilitada() {
    _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.eventClick = function () { };
    $("#" + _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.idTab + " .step").attr("class", "step");
    Etapa3FilialEmissoraDesabilitada();
}

function Etapa3IntegracaoFilialEmissoraLiberada() {
    _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.eventClick = function () { BuscarDadosIntegracoesOcorrencia(true) };
    $("#" + _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.idTab + " .step").attr("class", "step yellow");
    Etapa4Aprovada();
}

function Etapa3IntegracaoFilialEmissoraAprovada() {
    _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.eventClick = function () { BuscarDadosIntegracoesOcorrencia(true) };
    $("#" + _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.idTab + " .step").attr("class", "step green");
    Etapa3FilialEmissoraAprovada();
}

function Etapa3IntegracaoFilialEmissoraProblema() {
    _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.eventClick = function () { BuscarDadosIntegracoesOcorrencia(true) };
    $("#" + _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa3IntegracaoFilialEmissora.idTab + " .step").attr("class", "step red");
    Etapa3FilialEmissoraAprovada();
}


//*******Etapa 4*******
function Etapa4Desabilitada() {
    $("#" + _etapaOcorrencia.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOcorrencia.Etapa4.idTab + " .step").attr("class", "step");
    Etapa5Desabilitada();
}

function Etapa4Aguardando() {
    $("#" + _etapaOcorrencia.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa4.idTab + " .step").attr("class", "step yellow");
    Etapa3Aprovada();
}

function Etapa4Aprovada() {
    $("#" + _etapaOcorrencia.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa4.idTab + " .step").attr("class", "step green");

    if (_ocorrencia.NFSManualPendenteGeracao.val()) {
        Etapa3Aguardando();
    } else {
        Etapa3Aprovada();
    }
}

function Etapa4Reprovado() {
    $("#" + _etapaOcorrencia.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa4.idTab + " .step").attr("class", "step red");
    Etapa3Aprovada();
}


//*******Etapa 5*******

function Etapa5Desabilitada() {
    _etapaOcorrencia.Etapa5.eventClick = function () { };
    $("#" + _etapaOcorrencia.Etapa5.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOcorrencia.Etapa5.idTab + " .step").attr("class", "step");
}

function Etapa5Liberada() {
    _etapaOcorrencia.Etapa5.eventClick = function () { BuscarDadosIntegracoesOcorrencia(false) };
    $("#" + _etapaOcorrencia.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa5.idTab + " .step").attr("class", "step yellow");
    Etapa4Aprovada();
}

function Etapa5Aguardando() {
    _etapaOcorrencia.Etapa5.eventClick = function () { BuscarDadosIntegracoesOcorrencia(false) };
    $("#" + _etapaOcorrencia.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa5.idTab + " .step").attr("class", "step yellow");
    Etapa4Aprovada();
}

function Etapa5Aprovada() {
    _etapaOcorrencia.Etapa5.eventClick = function () { BuscarDadosIntegracoesOcorrencia(false) };
    $("#" + _etapaOcorrencia.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa5.idTab + " .step").attr("class", "step green");
    Etapa4Aprovada();
}

function Etapa5Problema() {
    _etapaOcorrencia.Etapa5.eventClick = function () { BuscarDadosIntegracoesOcorrencia(false) };
    $("#" + _etapaOcorrencia.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrencia.Etapa5.idTab + " .step").attr("class", "step red");
    Etapa4Aprovada();
}
