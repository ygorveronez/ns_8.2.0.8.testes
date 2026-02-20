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
/// <reference path="../../../Enumeradores/EnumTipoImpressaoDiarioBordo.js" />

//*******EVENTOS*******

var CargaCanhotoAvulso = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function verificarEtapaImpressaoClick(e) {
    ocultarTodasAbas(e);
    PreencherGridCanhotosAvulsos(e);
    verificaAnexosCarga(e);

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ConfirmarImpressao, _PermissoesPersonalizadasCarga))
        e.ConfirmarImpressao.enable(true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && e.PossuiConfiguracaoImpressora.val()) {
        //e.EnviarParaImpressao.visible(true);
        e.EnviarCTes.visible(true);
        e.EnviarMDFes.visible(true);
    }

    if (_CONFIGURACAO_TMS.ExisteIntegracaoLoggi)
        e.RelatorioPedidoPacote.visible(true);

    if (_CONFIGURACAO_TMS.HabilitarRelatorioDeTroca) {
        e.EnviarNotasBoletos.visible(true);
        e.RelatorioDeTroca.visible(true);
        e.RelatorioDeEntrega.visible(true);
        e.DownloadNFeBoleto.visible(true);
    }

    if (_CONFIGURACAO_TMS.HabilitarRelatorioBoletimViagem)
        e.RelatorioBoletimViagem.visible(true);

    if (_CONFIGURACAO_TMS.HabilitarRelatorioDeEmbarque)
        e.RelatorioDeEmbarque.visible(true);

    if (_CONFIGURACAO_TMS.HabilitarRelatorioDiarioBordo && e.TipoOperacao.tipoImpressaoDiarioBordo !== EnumTipoImpressaoDiarioBordo.Nenhum) {
        e.RelatorioDiarioBordo.visible(true);
        if (e.TipoOperacao.tipoImpressaoDiarioBordo === EnumTipoImpressaoDiarioBordo.MinutaFreteBovino)
            e.RelatorioDiarioBordo.text(Localization.Resources.Cargas.Carga.MinutaDeFreteBovino);
        else
            e.RelatorioDiarioBordo.text(Localization.Resources.Cargas.Carga.DiarioDeBordo);
    }

    if (e.TipoOperacao.utilizarPlanoViagem)
        e.RelatorioPlanoViagem.visible(true);

    e.RelatorioDeRomaneio.visible(e.TipoOperacao.imprimirRelatorioRomaneioEtapaImpressaoCarga)
    e.ImprimirCRT.visible(e.TipoOperacao.imprimirCRT);
}

function verificaAnexosCarga(e) {
    executarReST("CargaAnexo/ContarAnexosCarga", { Codigo: e.Codigo.val() }, function (arg) {
        if (arg.Success) {
            e.AnexosCarga.visible(arg.Data > 0);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function confirmarImpressaoClick(e, sender) {
    executarReST("CargaImpressaoDocumentos/InformarImpressaoRealizada", { Carga: e.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                EtapaImpressaoAprovada(e)
                $("#" + e.EtapaImpressao.idGrid + " .DivImpressao").html("<p>" + Localization.Resources.Cargas.Carga.ConfirmacaoDeImpressaoDosDocumentosDeTransporteJaFoiRealizada + "</p>");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function DownloadLotePDFClick(e, sender) {
    executarDownload("CargaImpressaoDocumentos/DownloadLotePDF", { Carga: e.Codigo.val() });
}

function DownloadDocumentosZIPClick(e) {
    executarDownload("CargaImpressaoDocumentos/DownloadLoteDocumentoCompactado", { Carga: e.Codigo.val() });
}

//function enviarParaImpressaoClick(e, sender) {
//    exibirConfirmacao("Confirmação", "Você realmente deseja enviar para impressão?", function () {
//        executarReST("CargaImpressaoDocumentos/EnviarDocumentosParaImpressao", { Carga: e.Codigo.val() }, function (arg) {
//            if (arg.Success) {
//                if (arg.Data) {
//                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso");
//                } else {
//                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
//                }
//            } else {
//                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
//            }
//        });
//    })
//}

function EnviarNotasBoletosClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.VoceRealmenteDesejaEnviarNotasBoletosParaImpressao, function () {
        executarReST("CargaImpressaoDocumentos/EnviarNotasBoletosParaImpressao", { Carga: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    })
}

function EnviarCTesClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.VoceRealmenteDesejaEnviarOsCTesParaImpressao, function () {
        executarReST("CargaImpressaoDocumentos/EnviarCTesParaImpressao", { Carga: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    })
}

function EnviarMDFesClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.VoceRealmenteDesejaEnviarOsMDFesParaImpressao, function () {
        executarReST("CargaImpressaoDocumentos/EnviarMDFesParaImpressao", { Carga: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    })
}

function AnexosCargaClick(e, sender) {
    if (_knoutCargaAnexo == undefined) {
        loadCargaAnexo();
    }
    _knoutCargaAnexo.Codigo.val(e.Codigo.val());
    _knoutCargaAnexo.Adicionar.visible(e.PermiteAdicionarAnexosGuarita.val());
    recarregarGridCargaAnexo();
}

function RelatorioDeTrocaClick(e, sender) {
    executarDownload("CargaImpressaoDocumentos/RelatorioDeTroca", { Carga: e.Codigo.val(), Recolhimento: true });
}

function RelatorioDeEntregaClick(e, sender) {
    executarDownload("CargaImpressaoDocumentos/RelatorioDeTroca", { Carga: e.Codigo.val(), Recolhimento: false });
}

function RelatorioBoletimViagem(e, sender) {
    executarDownload("CargaImpressaoDocumentos/RelatorioBoletimViagem", { Carga: e.Codigo.val(), Recolhimento: false });
}

function relatorioDiarioBordoClick(e) {
    executarDownload("CargaImpressaoDocumentos/RelatorioDiarioBordo", { Carga: e.Codigo.val() });
}

function relatorioPlanoViagemClick(e) {
    executarDownload("CargaImpressaoDocumentos/RelatorioPlanoViagem", { Carga: e.Codigo.val() });
}

function DownloadNFeBoletoClick(e, sender) {
    executarDownload("CargaImpressaoDocumentos/DownloadNFeBoleto", { Carga: e.Codigo.val() });
}

function RelatorioDeRomaneioClick(e) {
    executarDownload("CargaImpressaoDocumentos/RelatorioDeRomaneio", { Carga: e.Codigo.val() });
}

function RelatorioPedidoPacoteClick(e) {
    executarDownload("CargaImpressaoDocumentos/RelatorioPedidoPacote", { Carga: _cargaAtual.Codigo.val() });
}

function ImprimirCRTClick(e) {
    executarDownload("CargaImpressaoDocumentos/ImprimirCRT", { Carga: e.Codigo.val() });
}

function RelatorioDeEmbarqueClick(e) {
    executarDownload("CargaImpressaoDocumentos/RelatorioDeEmbarque", { Carga: e.Codigo.val() });
}

function PreencherGridCanhotosAvulsos(knoutCarga) {
    if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) || (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)) {
        var cargaCanhotoAvulso = new CargaCanhotoAvulso();
        cargaCanhotoAvulso.Carga.val(knoutCarga.Codigo.val());
        var imprimirCanhotoAvuso = { descricao: Localization.Resources.Cargas.Carga.ImprimirCanhoto, id: guid(), metodo: imprimirCanhotoAvulsoClick };
        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [imprimirCanhotoAvuso] };
        var gridCanhotoAvulso = new GridView(knoutCarga.CanhotoAvulso.idGrid, "Canhoto/BuscarCanhotosAvulsosPorCarga", cargaCanhotoAvulso, menuOpcoes, null, 2);
        gridCanhotoAvulso.CarregarGrid(function (retornoGrid) {
            if (retornoGrid != null && retornoGrid.data.length > 0) {
                knoutCarga.CanhotoAvulso.visible(true);
            } else {
                knoutCarga.CanhotoAvulso.visible(false);
            }
        });
    }
}

function imprimirCanhotoAvulsoClick(row) {
    var data = { Codigo: row.Codigo };
    executarDownload("Canhoto/DownloadCanhotoAvulso", data);
}

//*******MÉTODOS*******

function EtapaImpressaoDesabilitada(e) {
    $("#" + e.EtapaImpressao.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaImpressao.idTab + " .step").attr("class", "step");
    e.EtapaImpressao.eventClick = function (e) { };
}

function EtapaImpressaoAguardando(e) {
    $("#" + e.EtapaImpressao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaImpressao.idTab + " .step").attr("class", "step yellow");
    e.EtapaImpressao.eventClick = verificarEtapaImpressaoClick;
    EtapaIntegracaoAprovada(e);
}

function EtapaImpressaoLiberada(e) {
    $("#" + e.EtapaImpressao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaImpressao.idTab + " .step").attr("class", "step yellow");
    e.EtapaImpressao.eventClick = verificarEtapaImpressaoClick;
    EtapaIntegracaoAprovada(e);
}

function EtapaImpressaoAprovada(e) {
    $("#" + e.EtapaImpressao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaImpressao.idTab + " .step").attr("class", "step green");
    e.EtapaImpressao.eventClick = verificarEtapaImpressaoClick;
    EtapaIntegracaoAprovada(e);
}

function EtapaImpressaoProblema(e) {
    $("#" + e.EtapaImpressao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaImpressao.idTab + " .step").attr("class", "step red");
    e.EtapaImpressao.eventClick = verificarEtapaImpressaoClick;
    EtapaIntegracaoAprovada(e);
}

function EtapaImpressaoEdicaoDesabilitada(e) {
    e.EtapaImpressao.enable(false);
    EtapaIntegracaoEdicaoDesabilitada(e);
}
