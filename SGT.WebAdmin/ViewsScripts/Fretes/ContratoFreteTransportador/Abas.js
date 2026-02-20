/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />

function LoadAbas() {
    _valoresVeiculos.ValoresVeiculos.val.subscribe(GerenciarAbasContratoFrete);
    
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        $(".hide-when-multitms").addClass("tab-hide");
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $(".hide-when-multiembarcador").addClass("tab-hide");
    }

    if (_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorKm)
        $("#liFranquiaFaixaKm").addClass("tab-hide");
    else
        $("#liFranquia").addClass("tab-hide");
}


function GerenciarAbasContratoFrete() {
    var possuiCadastroValoresVeiculo = _valoresVeiculos.ValoresVeiculos.val().length > 0;
    var possuiCadastroAcordos = _contratoFreteTransportador.Acordos.list.length > 0;
    var possuiCadastroOcorrencias = _tiposOcorrencia.TiposOcorrencia.val().length > 0;

    if (possuiCadastroValoresVeiculo) {
        $("#liAcordo").hide();
        $("#liValorFreteMinimo").hide();
    }
    else {
        $("#liAcordo").show();
        $("#liValorFreteMinimo").show();
    }

    if (possuiCadastroAcordos)
        $("#liValoresVeiculos").hide();
    else if (possuiCadastroOcorrencias)
        $("#liValoresVeiculos").show();

    var evt = $.Event("abaschange");
    $("body").trigger(evt, {
        PossuiCadastroValoresVeiculo: possuiCadastroValoresVeiculo,
        PossuiCadastroAcordos: possuiCadastroAcordos,
        PossuiCadastroOcorrencias: possuiCadastroOcorrencias,
    });
}

function GerenciarVisibilidadeAbas() {

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaDadosContrato, _PermissoesPersonalizadas)) 
        controlarCamposHabilitadosPorKnockout(_contratoFreteTransportador, false);

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaOcorrencia, _PermissoesPersonalizadas))
        $("#TiposOcorrencia").hide();

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaClientes, _PermissoesPersonalizadas))
        $("#liClientes").hide();

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaAcordo, _PermissoesPersonalizadas))
        $("#liAcordo").hide();

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaAnexos, _PermissoesPersonalizadas))
        $("#liAnexos").hide();
    
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaValoresVeiculos, _PermissoesPersonalizadas)) 
        $("#liValoresVeiculos").hide();
    
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaVeiculos, _PermissoesPersonalizadas))
        $("#liVeiculos").hide();
    
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaOutrosValores, _PermissoesPersonalizadas))
        $("#liOutrosValores").hide();
    
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaFranquia, _PermissoesPersonalizadas)) {
        $("#liFranquia").hide();
        $("#liFranquiaFaixaKm").hide();
    }
    
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaFiliais, _PermissoesPersonalizadas))
        $("#liFiliais").hide();
    
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaTipoOperacao, _PermissoesPersonalizadas))
        $("#liTipoOperacao").hide();
    
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_HabilitarAbaValoresFreteMinimo, _PermissoesPersonalizadas))
        $("#liValorFreteMinimo").hide();
    
}