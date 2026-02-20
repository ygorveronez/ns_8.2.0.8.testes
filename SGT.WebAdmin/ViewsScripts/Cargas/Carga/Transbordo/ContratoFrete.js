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
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
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
/// <reference path="../../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoTransbordo.js" />
/// <reference path="Transbordo.js" />

//*******MAPEAMENTO*******

function loadTransbordoContratoFrete() {
    _cargaTransbordoSubContratacao = new CargasSubContratacao();
    _cargaTransbordoSubContratacao.AlterarContrato.eventClick = salvarContratoFreteSubContratacao;
    _cargaTransbordoSubContratacao.AutorizarContrato.eventClick = autorizarContratoFreteTerceiroClick;

    var strKnoutCargaTransbordoSubContratacao = "knoutTransbordo" + strKnoutCargaTransbordo;
    $("#tab_contrato" + strKnoutCargaTransbordo).html(_HTMLCargaContratoFrete.replace("#knoutCargaSubContratacao", strKnoutCargaTransbordoSubContratacao));
    KoBindings(_cargaTransbordoSubContratacao, strKnoutCargaTransbordoSubContratacao);
    LocalizeCurrentPage();
}

function autorizarContratoFreteTerceiroClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaLiberarEsseContratoDeFrete, function () {
        Salvar(e, "ContratoFrete/AprovarContrato", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ContratoAprovadoComSucesso);
                    _cargaTransbordoSubContratacao.SituacaoContratoFrete.val(EnumSituacaoContratoFrete.Aprovado);
                    buscarCargasPreCTe(_cargaTransbordoSubContratacao);
                    _cargaTransbordoSubContratacao.AutorizarContrato.visible(false);
                    _cargaTransbordoSubContratacao.PercentualAdiantamento.enable(false);
                    _cargaTransbordoSubContratacao.PercentualAbastecimento.enable(false);
                    _cargaTransbordoSubContratacao.Descontos.enable(false);
                    _cargaTransbordoSubContratacao.Observacao.enable(false);
                    _cargaTransbordoSubContratacao.AlterarContrato.visible(false);
                    _cargaTransbordoSubContratacao.ImprimirContrato.visible(true);

                    buscarPorCodigoTransbordo(function () {
                        _gridTransbordos.CarregarGrid(function () {
                            preecherDadosContrato(arg.Data, _cargaTransbordoSubContratacao);
                        });
                    });
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function preecherDadosTransbordoContrato(contratoFrete) {
    _cargaTransbordoSubContratacao.ImprimirContrato.visible(false);
    if (contratoFrete == null || contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.AgAprovacao) {
        _cargaTransbordoSubContratacao.PercentualAdiantamento.enable(true);
        _cargaTransbordoSubContratacao.PercentualAbastecimento.enable(true);
        _cargaTransbordoSubContratacao.Descontos.enable(true);
        _cargaTransbordoSubContratacao.Observacao.enable(true);
        _cargaTransbordoSubContratacao.ValorFreteSubcontratacao.enable(true);
        _cargaTransbordoSubContratacao.ValorOutrosAdiantamento.enable(true);
        _cargaTransbordoSubContratacao.AutorizarContrato.visible(true);
        _cargaTransbordoSubContratacao.AlterarContrato.visible(true);
        _cargaTransbordoSubContratacao.PercentualAdiantamento.visible(true);
        _cargaTransbordoSubContratacao.PercentualAbastecimento.visible(true);
        _cargaTransbordoSubContratacao.Descontos.visible(true);
        _cargaTransbordoSubContratacao.Observacao.visible(true);
        _cargaTransbordoSubContratacao.ValorOutrosAdiantamento.visible(true);
    }

    if (contratoFrete != null) {
        preecherDadosContrato(contratoFrete, _cargaTransbordoSubContratacao);
        if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Aprovado || contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Finalizada) {
            _cargaTransbordoSubContratacao.ImprimirContrato.visible(true);
        }
        buscarCargasPreCTe(_cargaTransbordoSubContratacao);
    } else {
        _cargaTransbordoSubContratacao.AutorizarContrato.visible(false);
    }
}

function salvarContratoFreteSubContratacao(e, sender) {

    if (ValidarCamposObrigatorios(e)) {
        var data = RetornarObjetoPesquisa(e);
        data.Transbordo = _cargaTransbordo.Codigo.val();
        executarReST("Transbordo/SalvarFreteTerceiroTransbordo", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ContratoSalvoComSucesso);
                    PreencherTransbordo(arg.Data);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }

}