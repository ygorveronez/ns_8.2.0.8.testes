/// <reference path="../../Enumeradores/EnumEtapaAgendamentoColeta.js" />

var _etapaAgendamentoColeta;

var EtapasAgendamentoColeta = function () {
    if (_CONFIGURACAO_TMS.ControlarAgendamentoSKU) {
        if (_CONFIGURACAO_TMS.RemoverEtapaAgendamentoAgendamentoColeta)
            this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });
        else
            this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    }
    else if (_CONFIGURACAO_TMS.RemoverEtapaAgendamentoAgendamentoColeta)
        this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });
    else
        this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.Etapa1 = PropertyEntity({
        text: "Carga", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Nessa etapa são preenchidas as informações sobre a carga."),
        tooltipTitle: ko.observable("Carga")
    });
    this.Etapa2 = PropertyEntity({
        text: "Agendamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é atualizado os dados de transporte."),
        tooltipTitle: ko.observable("Dados Transporte/Agendamento")
    });
    this.Etapa3 = PropertyEntity({
        text: "NF-e", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: CarregarEtapaCarga,
        step: ko.observable(3),
        tooltip: ko.observable("NF-es."),
        tooltipTitle: ko.observable("NF-e")
    });
    this.Etapa4 = PropertyEntity({
        text: "Emissão", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("Nessa etapa é mostrado os CT-es e MDF-es."),
        tooltipTitle: ko.observable("Emissão")
    });
    this.Etapa5 = PropertyEntity({
        text: "Documento para Transporte", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(5),
        tooltip: ko.observable("Nesta etapa são adicionar os Notas e CTe"),
        tooltipTitle: ko.observable("Documento para Transporte")
    });
}

function LoadEtapasAgendamentoColeta() {
    _etapaAgendamentoColeta = new EtapasAgendamentoColeta();
    KoBindings(_etapaAgendamentoColeta, "knockoutEtapasAgendamentoColeta");
    VerificarVisibilidadeEtapaNFe();
    EtapaCargaAguardando();
}

function SetarEtapasRequisicao(status, removerEtapaAgendamento) {
    if (status == EnumEtapaAgendamentoColeta.AguardandoAceite) {
        EtapaCargaAguardandoAceite();
    }
    else if (status == EnumEtapaAgendamentoColeta.DadosTransporte) {
        EtapaDadosTransporteLiberada();
    }
    else if (status == EnumEtapaAgendamentoColeta.NFe) {
        EtapaNFeLiberada();
    }
    else if (status == EnumEtapaAgendamentoColeta.NFeCancelada) {
        EtapaNFeRejeitada();
    }
    else if (status == EnumEtapaAgendamentoColeta.Emissao) {
        if (_retornoAgendamento.SituacaoCodigo.val() === EnumSituacaoAgendamentoColeta.AguardandoCTes)
            EtapaEmissaoAguardando();
        else
            EtapaEmissaoFinalizada();
    } else if (status == EnumEtapaAgendamentoColeta.DocumentoParaTransporte) {
        EtapaDocumentoParaTransporteAguardando();
    }
    else
        EtapaCargaAguardando();

    if (removerEtapaAgendamento) {
        if (_CONFIGURACAO_TMS.ControlarAgendamentoSKU)
            _etapaAgendamentoColeta.TamanhoEtapa.val("50%");
        else
            _etapaAgendamentoColeta.TamanhoEtapa.val("33%");

        _etapaAgendamentoColeta.Etapa2.visible(false);
        _etapaAgendamentoColeta.Etapa3.step(2);
        _etapaAgendamentoColeta.Etapa4.step(3);
    }
    else if (!_CONFIGURACAO_TMS.RemoverEtapaAgendamentoAgendamentoColeta) {
        if (_CONFIGURACAO_TMS.ControlarAgendamentoSKU)
            _etapaAgendamentoColeta.TamanhoEtapa.val("33%");
        else
            _etapaAgendamentoColeta.TamanhoEtapa.val("25%");

        _etapaAgendamentoColeta.Etapa2.visible(true);
        _etapaAgendamentoColeta.Etapa3.step(3);
        _etapaAgendamentoColeta.Etapa4.step(4);
    }
}

function EtapaCargaAguardando() {
    $("#" + _etapaAgendamentoColeta.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaAgendamentoColeta.Etapa1.idTab + " .step").attr("class", "step yellow");
    if (_CONFIGURACAO_TMS.ControlarAgendamentoSKU)
        $("#divPedidosPendentes").show();
    $("#divPedidos").removeClass("col-md-12 col-lg-12");
    $("#divPedidos").addClass("col-md-6 col-lg-6");

    _etapaCarga.Adicionar.visible(true);
    _CRUDDadosTransporte.InformarNotasFiscais.visible(false);
    _dadosTransporte.Atualizar.visible(false);
    _CRUDEtapaNFe.Encaminhar.visible(false);
    _etapaCarga.Cancelar.visible(false);

    SetarEnableCamposKnockout(_etapaCarga, true);

    EtapaDadosTransporteDesabilitada();
    EtapaNFeDesabilitada();
    EtapaEmissaoDesabilitada();
    EtapaDocumentoParaTransporteDesabilitada();
    FocarEtapaCargaAguardando();
}

function FocarEtapaCargaAguardando() {
    $("#" + _etapaAgendamentoColeta.Etapa1.idTab).tab("show");
    $("#tabCarga").click();
}

function EtapaCargaAguardandoAceite() {
    $("#" + _etapaAgendamentoColeta.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaAgendamentoColeta.Etapa1.idTab + " .step").attr("class", "step yellow");
    $("#divPedidosPendentes").hide();
    $("#divPedidos").removeClass("col-md-6 col-lg-6");
    $("#divPedidos").addClass("col-md-12 col-lg-12");

    _CRUDDadosTransporte.InformarNotasFiscais.visible(false);
    _dadosTransporte.Atualizar.visible(false);
    _CRUDEtapaNFe.Encaminhar.visible(false);

    SetarEnableCamposKnockout(_etapaCarga, false);

    _etapaCarga.Adicionar.visible(false);
    _etapaCarga.Cancelar.visible(_configuracaoAgendamentoColeta.HabilitarCancelamentoAgendamentoColeta);
    $("#tabCarga").click();
    EtapaDadosTransporteDesabilitada();
    EtapaNFeDesabilitada();
    EtapaEmissaoDesabilitada();
    EtapaDocumentoParaTransporteDesabilitada();
}

function EtapaCargaAceita() {
    $("#" + _etapaAgendamentoColeta.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaAgendamentoColeta.Etapa1.idTab + " .step").attr("class", "step green");
}

function EtapaDadosTransporteLiberada() {
    _etapaCarga.Adicionar.visible(false);
    EtapaCargaAceita();
    EtapaNFeDesabilitada();
    EtapaEmissaoDesabilitada();
    $("#" + _etapaAgendamentoColeta.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaAgendamentoColeta.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#divPedidosPendentes").hide();
    $("#divPedidos").removeClass("col-md-6 col-lg-6");
    $("#divPedidos").addClass("col-md-12 col-lg-12");

    SetarEnableCamposKnockout(_etapaCarga, false);
    SetarEnableCamposKnockout(_dadosTransporte, true);
    SetarEnableCamposKnockout(_dadosAgendamento, true);
    SetarEnableCamposKnockout(_lacreAgendamento, true);

    _etapaCarga.Adicionar.visible(false);
    _etapaCarga.Cancelar.visible(_configuracaoAgendamentoColeta.HabilitarCancelamentoAgendamentoColeta);
    _CRUDDadosTransporte.Cancelar.visible(_configuracaoAgendamentoColeta.HabilitarCancelamentoAgendamentoColeta);
    _CRUDDadosTransporte.InformarNotasFiscais.visible(true);
    _dadosTransporte.Atualizar.visible(true);
    _CRUDEtapaNFe.Encaminhar.visible(false);

    Global.ExibirStep(_etapaAgendamentoColeta.Etapa2.idTab);
}

function EtapaDadosTransporteAceita() {
    $("#" + _etapaAgendamentoColeta.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaAgendamentoColeta.Etapa2.idTab + " .step").attr("class", "step green");
}

function EtapaNFeLiberada() {
    EtapaCargaAceita();
    EtapaDadosTransporteAceita();
    EtapaEmissaoDesabilitada();
    $("#" + _etapaAgendamentoColeta.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaAgendamentoColeta.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#divPedidosPendentes").hide();
    $("#divPedidos").removeClass("col-md-6 col-lg-6");
    $("#divPedidos").addClass("col-md-12 col-lg-12");

    SetarEnableCamposKnockout(_etapaCarga, false);
    SetarEnableCamposKnockout(_dadosTransporte, false);
    SetarEnableCamposKnockout(_dadosAgendamento, false);
    SetarEnableCamposKnockout(_lacreAgendamento, false);

    _etapaCarga.Adicionar.visible(false);
    _etapaCarga.Cancelar.visible(_configuracaoAgendamentoColeta.HabilitarCancelamentoAgendamentoColeta);
    _CRUDDadosTransporte.Cancelar.visible(_configuracaoAgendamentoColeta.HabilitarCancelamentoAgendamentoColeta);
    _CRUDEtapaNFe.Cancelar.visible(_configuracaoAgendamentoColeta.HabilitarCancelamentoAgendamentoColeta);
    _CRUDDadosTransporte.InformarNotasFiscais.visible(false);
    _dadosTransporte.Atualizar.visible(false);
    _CRUDEtapaNFe.Encaminhar.visible(true);
    _NFeAgendamento.Dropzone.visible(true);
    _lacreAgendamento.Adicionar.enable(true);

    Global.ExibirStep(_etapaAgendamentoColeta.Etapa3.idTab);
}

function EtapaNFeRejeitada() {
    EtapaCargaAceita();
    EtapaDadosTransporteAceita();
    EtapaEmissaoDesabilitada();
    $("#" + _etapaAgendamentoColeta.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaAgendamentoColeta.Etapa3.idTab + " .step").attr("class", "step red");
    $("#divPedidosPendentes").hide();
    $("#divPedidos").removeClass("col-md-6 col-lg-6");
    $("#divPedidos").addClass("col-md-12 col-lg-12");

    SetarEnableCamposKnockout(_etapaCarga, false);
    SetarEnableCamposKnockout(_dadosTransporte, false);
    SetarEnableCamposKnockout(_dadosAgendamento, false);
    SetarEnableCamposKnockout(_lacreAgendamento, false);

    _etapaCarga.Adicionar.visible(false);
    _etapaCarga.Cancelar.visible(false);
    _CRUDDadosTransporte.Cancelar.visible(false);
    _CRUDEtapaNFe.Cancelar.visible(false);
    _CRUDDadosTransporte.InformarNotasFiscais.visible(false);
    _dadosTransporte.Atualizar.visible(false);
    _CRUDEtapaNFe.Encaminhar.visible(false);
    _NFeAgendamento.Dropzone.visible(false);

    Global.ExibirStep(_etapaAgendamentoColeta.Etapa3.idTab);
}

function EtapaNFeAceita() {
    $("#" + _etapaAgendamentoColeta.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaAgendamentoColeta.Etapa3.idTab + " .step").attr("class", "step green");
}

function EtapaEmissaoAguardando() {
    EtapaCargaAceita();
    EtapaDadosTransporteAceita();
    EtapaNFeAceita();
    $("#" + _etapaAgendamentoColeta.Etapa4.idTab).prop("disabled", false);
    $("#" + _etapaAgendamentoColeta.Etapa4.idTab + " .step").attr("class", "step yellow");
    $("#divPedidosPendentes").hide();
    $("#divPedidos").removeClass("col-md-6 col-lg-6");
    $("#divPedidos").addClass("col-md-12 col-lg-12");

    SetarEnableCamposKnockout(_etapaCarga, false);
    SetarEnableCamposKnockout(_dadosTransporte, false);
    SetarEnableCamposKnockout(_dadosAgendamento, false);
    SetarEnableCamposKnockout(_lacreAgendamento, false);

    _etapaCarga.Cancelar.visible(false);
    _CRUDDadosTransporte.Cancelar.visible(false);
    _CRUDEtapaNFe.Cancelar.visible(false);
    _etapaCarga.Adicionar.visible(false);
    _CRUDDadosTransporte.InformarNotasFiscais.visible(false);
    _dadosTransporte.Atualizar.visible(false);
    _CRUDEtapaNFe.Encaminhar.visible(false);

    Global.ExibirStep(_etapaAgendamentoColeta.Etapa4.idTab);
}

function EtapaEmissaoFinalizada() {
    EtapaCargaAceita();
    EtapaDadosTransporteAceita();
    EtapaNFeAceita();

    $("#" + _etapaAgendamentoColeta.Etapa4.idTab).prop("disabled", false);
    $("#" + _etapaAgendamentoColeta.Etapa4.idTab + " .step").attr("class", "step green");
    $("#divPedidosPendentes").hide();
    $("#divPedidos").removeClass("col-md-6 col-lg-6");
    $("#divPedidos").addClass("col-md-12 col-lg-12");

    SetarEnableCamposKnockout(_etapaCarga, false);
    SetarEnableCamposKnockout(_dadosTransporte, false);
    SetarEnableCamposKnockout(_dadosAgendamento, false);
    SetarEnableCamposKnockout(_lacreAgendamento, false);

    _etapaCarga.Cancelar.visible(false);
    _CRUDDadosTransporte.Cancelar.visible(false);
    _CRUDEtapaNFe.Cancelar.visible(false);
    _etapaCarga.Adicionar.visible(false);
    _CRUDDadosTransporte.InformarNotasFiscais.visible(false);
    _dadosTransporte.Atualizar.visible(false);
    _CRUDEtapaNFe.Encaminhar.visible(false);

    Global.ExibirStep(_etapaAgendamentoColeta.Etapa4.idTab);
}

function EtapaDadosParaTransporteAceita() {
    $("#" + _etapaAgendamentoColeta.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAgendamentoColeta.Etapa5.idTab + " .step").attr("class", "step green");
}

function EtapaDocumentoParaTransporteAguardando() {
    EtapaCargaAceita();
    EtapaDadosTransporteAceita();
    $("#" + _etapaAgendamentoColeta.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAgendamentoColeta.Etapa5.idTab + " .step").attr("class", "step yellow");
    $("#divPedidosPendentes").hide();
    $("#divPedidos").removeClass("col-md-6 col-lg-6");
    $("#divPedidos").addClass("col-md-12 col-lg-12");

    SetarEnableCamposKnockout(_etapaCarga, false);
    SetarEnableCamposKnockout(_dadosTransporte, false);
    SetarEnableCamposKnockout(_dadosAgendamento, false);

    _etapaCarga.Cancelar.visible(false);
    _CRUDDadosTransporte.Cancelar.visible(false);
    _CRUDEtapaNFe.Cancelar.visible(false);
    _etapaCarga.Adicionar.visible(false);
    _CRUDDadosTransporte.InformarNotasFiscais.visible(false);
    _dadosTransporte.Atualizar.visible(false);
    _CRUDEtapaNFe.Encaminhar.visible(false);

    Global.ExibirStep(_etapaAgendamentoColeta.Etapa5.idTab);
}

function EtapaDadosTransporteDesabilitada() {
    $("#" + _etapaAgendamentoColeta.Etapa2.idTab).prop("disabled", true);
    $("#" + _etapaAgendamentoColeta.Etapa2.idTab + " .step").attr("class", "step");
}

function EtapaNFeDesabilitada() {
    $("#" + _etapaAgendamentoColeta.Etapa3.idTab).prop("disabled", true);
    $("#" + _etapaAgendamentoColeta.Etapa3.idTab + " .step").attr("class", "step");
}

function EtapaEmissaoDesabilitada() {
    $("#" + _etapaAgendamentoColeta.Etapa4.idTab).prop("disabled", true);
    $("#" + _etapaAgendamentoColeta.Etapa4.idTab + " .step").attr("class", "step");
}

function EtapaDocumentoParaTransporteDesabilitada() {
    $("#" + _etapaAgendamentoColeta.Etapa5.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAgendamentoColeta.Etapa5.idTab + " .step").attr("class", "step");
}

function VerificarVisibilidadeEtapaNFe() {
    if (_CONFIGURACAO_TMS.ControlarAgendamentoSKU) {
        _etapaAgendamentoColeta.Etapa3.visible(false);
        _etapaAgendamentoColeta.Etapa4.step(3);
    }
    else {
        _etapaAgendamentoColeta.Etapa3.visible(true);

        if (_CONFIGURACAO_TMS.RemoverEtapaAgendamentoAgendamentoColeta)
            _etapaAgendamentoColeta.Etapa4.step(3);
        else
            _etapaAgendamentoColeta.Etapa4.step(4);
    }

    if (_CONFIGURACAO_TMS.RemoverEtapaAgendamentoAgendamentoColeta) {
        _etapaAgendamentoColeta.Etapa2.visible(false);
        _etapaAgendamentoColeta.Etapa3.step(2);
    }
}

function VerificarSePossuiEtapaInformarNotaCte(possui, situacao) {
    _etapaAgendamentoColeta.Etapa3.visible(!possui);
    _etapaAgendamentoColeta.Etapa4.visible(!possui);
    _etapaAgendamentoColeta.Etapa5.visible(possui);

    if (situacao == "Finalizado") {
        _documentoParaTransporte.Avancar.visible(false);
        _documentoParaTransporte.Salvar.enable(false);
        $("#" + _etapaAgendamentoColeta.Etapa5.idTab + " .step").removeClass("yellow");
        $("#" + _etapaAgendamentoColeta.Etapa5.idTab + " .step").addClass("green");
    }

}