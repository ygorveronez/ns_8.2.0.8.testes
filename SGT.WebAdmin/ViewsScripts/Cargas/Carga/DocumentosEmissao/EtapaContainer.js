/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../../js/plugin/dropzone/dropzone.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="CargaPedidoDocumentoCTe.js" />
/// <reference path="ConsultaReceita.js" />
/// <reference path="CTe.js" />
/// <reference path="Documentos.js" />
/// <reference path="DropZone.js" />
/// <reference path="Documentos.js" />
/// <reference path="NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoConsultaPortalFazenda.js" />
/// <reference path="../../../Enumeradores/EnumPermissoesEdicaoCTe.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoMinutaAvon.js" />


function EtapaContainerAprovada(e) {
    $("#" + e.EtapaContainer.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaContainer.idTab + " .step").attr("class", "step green");
    e.EtapaContainer.eventClick = LoadEtapaContainer;

    if (!e.ExigeNotaFiscalParaCalcularFrete.val()) {
        EtapaDadosTransportadorAprovada(e);
        EtapaDadosTransportadorEdicaoDesabilitada(e);
    }
    else if (_CONFIGURACAO_TMS.PossuiIntegracaoBRKVeiculoEMotorista && (!e.MotoristaValidadoBrasilRisk.val() || !e.PlacaValidadoBrasilRisk.val()))
        EtapaInicioTMSAlerta(e);
    else
        EtapaInicioTMSAprovada(e);

}

function EtapaContainerDesabilitada(e) {

    if (!e.EtapaContainer)
        return;
    
    $("#" + e.EtapaContainer.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaContainer.idTab + " .step").attr("class", "step");
    e.EtapaContainer.eventClick = function (e) { };

    e.EtapaContainer.enable(false);
}

function EtapaContainerAguardando(e) {
    $("#" + e.EtapaContainer.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaContainer.idTab + " .step").attr("class", "step yellow");
    e.EtapaContainer.eventClick = LoadEtapaContainer;

    EtapaNotaFiscalDesabilitada(e);

    if (!e.ExigeNotaFiscalParaCalcularFrete.val()) {
        EtapaDadosTransportadorAprovada(e);
        EtapaDadosTransportadorEdicaoDesabilitada(e);
    }
    else if (_CONFIGURACAO_TMS.PossuiIntegracaoBRKVeiculoEMotorista && (!e.MotoristaValidadoBrasilRisk.val() || !e.PlacaValidadoBrasilRisk.val()))
        EtapaInicioTMSAlerta(e);
    else
        EtapaInicioTMSAprovada(e);
}

function EtapaContainerProblema(e) {
    $("#" + e.EtapaContainer.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaContainer.idTab + " .step").attr("class", "step red");
    e.EtapaContainer.eventClick = LoadEtapaContainer;

    EtapaNotaFiscalDesabilitada(e);

    if (!e.ExigeNotaFiscalParaCalcularFrete.val()) {
        EtapaDadosTransportadorAprovada(e);
        EtapaDadosTransportadorEdicaoDesabilitada(e);
    }
    else if (_CONFIGURACAO_TMS.PossuiIntegracaoBRKVeiculoEMotorista && (!e.MotoristaValidadoBrasilRisk.val() || !e.PlacaValidadoBrasilRisk.val()))
        EtapaInicioTMSAlerta(e);
    else
        EtapaInicioTMSAprovada(e);

}