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
/// <reference path="../../../Creditos/ControleSaldo/ControleSaldo.js" />
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
/// <reference path="Complemento.js" />
/// <reference path="Componente.js" />
/// <reference path="Frete.js" />
/// <reference path="SemTabela.js" />
/// <reference path="TabelaCliente.js" />
/// <reference path="TabelaComissao.js" />
/// <reference path="TabelaRota.js" />
/// <reference path="TabelaSubContratacao.js" />
/// <reference path="TabelaTerceiros.js" />
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
/// <reference path="../../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="../../../Consultas/ComponenteFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoFreteCliente.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoFreteComissao.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoComplementoFrete.js" />

//*******EVENTOS*******

function retornarParaEtapaNFeClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaRetornarEnvioDosDocumentos, function () {
        var data = { Carga: e.Codigo.val() };
        executarReST("CargaFrete/RetornarEtapaNotaFiscaTMS", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    e.SituacaoCarga.val(EnumSituacoesCarga.AgNFe);
                    $("#" + e.EtapaNotaFiscal.idTab).trigger("click");
                    EtapaFreteTMSDesabilitada(e);
                    EtapaNotaFiscalAguardando(e);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function liberarSemConfirmacaoERPClick(e) {
    executarReST("CargaFrete/LiberarSemConfirmacaoERP", { Carga: _cargaAtual.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                e.AutorizarEmissaoDocumentos.visibleBTN(false);
                e.AutorizarEmissaoDocumentos.visibleCTeProcessamento(true);
                e.AguardandoEmissaoDocumentoAnterior.visible(false);
                e.LiberarSemConfirmacaoERP.visible(false);

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.LiberacaoConcedidaComSucesso);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function validarFreteIniciarEmissaoClick(e) {
    if (_cargaAtual.ExigeInformarIsca.val() && _iscas.length <= 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.NecessarioInformarPeloMenosUmaIscaParaCarga);
        return;
    }

    if (!_cargaAtual.ExigeNotaFiscalParaCalcularFrete.val() && _cargaAtual.TipoOperacao.ExigeConformacaoFreteAntesEmissao && _CONFIGURACAO_TMS.ExigirConfirmacaoEtapaFreteNoFluxoNotaAposFrete) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaAvancarEtapaTransportadorMesmoAssim, function () { confirmarFreteIniciarEtapaTransportadorClick(e); });

        return;
    }

    executarReST("CargaFrete/ValidarInicioEmissaoDocumentos", { Carga: _cargaAtual.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RoteirizacaoPendente)
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Data.Mensagem);
                else if (retorno.Data.PossuiPendencia)
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.Mensagem + " " + Localization.Resources.Cargas.Carga.DesejaIniciarEmissaoDosDocumentosMesmoAssim, function () { confirmarFreteIniciarEmissaoClick(e); });
                else if (!retorno.Data.PermiteGerarGNRE)
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.Mensagem + " " + Localization.Resources.Cargas.Carga.DesejaProsseguir, function () { confirmarFreteIniciarEmissaoClick(e); });
                else if (retorno.Data.OcorreuErroEPossuiPermissao)
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.Mensagem + " " + Localization.Resources.Cargas.Carga.DesejaProsseguir, function () { confirmarFreteIniciarEmissaoClick(e); });
                else
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteIniciarEmissaoDosDocumentos, function () { confirmarFreteIniciarEmissaoClick(e); });
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function confirmarFreteIniciarEtapaTransportadorClick(e) {
    let data = { Carga: e.Codigo.val() };

    executarReST("CargaFrete/ConfirmarFreteIniciarEtapaTransportador", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != false) {
                return;
            }

            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 30000);
            return;
        }

        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function confirmarFreteIniciarEmissaoClick(e) {
    var data = { Carga: e.Codigo.val() };

    executarReST("CargaFrete/ConfirmarFreteIniciarEmissao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                EtapaFreteTMSAprovada(e);
                EtapaFreteTMSEdicaoDesabilitada(e);

                if (_cargaDadosEmissaoGeral != null) {
                    _cargaDadosEmissaoGeral.Pedido.enable(false);

                    if (_cargaDadosEmissaoObservacao != null)
                        _cargaDadosEmissaoObservacao.Pedido.enable(false);

                    if (_cargaDadosEmissaoRota != null)
                        _cargaDadosEmissaoRota.Pedido.enable(false);

                    if (_cargaDadosEmissaoSeguro != null)
                        _cargaDadosEmissaoSeguro.Pedido.enable(false);

                    _cargaDadosEmissaoConfiguracao.Pedido.enable(false);
                }

                e.AutorizarEmissaoDocumentos.visibleCTeProcessamento(true);
                e.AutorizarEmissaoDocumentos.visibleBTN(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 30000);
            }
        } else {
            if (arg.Data != null) {
                if (arg.Data.RecarregarDadosEmissao) {
                    carregarDadosPedido(0);
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 10000);
                } else {
                    if (!arg.Data.PercursoMDFeValido) {
                        BuscarPercursoParaMDFe();
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.Carga.AntesDeIniciarEmissaoNecessarioConfigurarUmPercursoValidoParaGerarOsMDFes);
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }
    });
}



//*******ETAPA NOTAS POS CALCULO*******

function EtapaFreteEmbarcadorDesabilitada(e) {
    $("#" + e.EtapaFreteEmbarcador.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaFreteEmbarcador.idTab + " .step").attr("class", "step");
    e.EtapaFreteEmbarcador.eventClick = function (e) { };
    EtapaFreteTMSDesabilitada(e);
}

function EtapaFreteEmbarcadorAguardando(e) {
    $("#" + e.EtapaFreteEmbarcador.idTab).attr("data-bs-toggle", "tab");

    if (e.AgConfirmacaoUtilizacaoCredito.val())
        $("#" + e.EtapaFreteEmbarcador.idTab + " .step").attr("class", "step orange");
    else if (e.LiberadaEtapaFaturamentoBloqueada.val())
        $("#" + e.EtapaFreteEmbarcador.idTab + " .step").attr("class", "step lightsalmon");
    else
        $("#" + e.EtapaFreteEmbarcador.idTab + " .step").attr("class", "step yellow");

    e.EtapaFreteEmbarcador.eventClick = verificarFreteClick;
    EtapaInicioEmbarcadorAprovada(e);
}

function EtapaFreteEmbarcadorLiberada(e) {
    $("#" + e.EtapaFreteEmbarcador.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaFreteEmbarcador.idTab + " .step").attr("class", "step yellow");
    e.EtapaFreteEmbarcador.eventClick = verificarFreteClick;
    EtapaInicioEmbarcadorAprovada(e);
}

function EtapaFreteEmbarcadorAprovada(e) {
    $("#" + e.EtapaFreteEmbarcador.idTab).attr("data-bs-toggle", "tab");

    if (e.AgConfirmacaoUtilizacaoCredito.val())
        $("#" + e.EtapaFreteEmbarcador.idTab + " .step").attr("class", "step orange");
    else if (e.LiberadaEtapaFaturamentoBloqueada.val())
        $("#" + e.EtapaFreteEmbarcador.idTab + " .step").attr("class", "step lightsalmon");
    else
        $("#" + e.EtapaFreteEmbarcador.idTab + " .step").attr("class", "step green");

    e.EtapaFreteEmbarcador.eventClick = verificarFreteClick;
    EtapaInicioEmbarcadorAprovada(e);
}

function EtapaFreteEmbarcadorProblema(e) {
    $("#" + e.EtapaFreteEmbarcador.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaFreteEmbarcador.idTab + " .step").attr("class", "step red");
    EtapaDadosTransportadorDesabilitada(e);
    EtapaInicioEmbarcadorAprovada(e);
    e.EtapaFreteEmbarcador.eventClick = verificarFreteClick;
}

function EtapaFreteEmbarcadorEdicaoDesabilitada(e) {
    e.EtapaFreteEmbarcador.enable(false);

    if (e.ExigeNotaFiscalParaCalcularFrete.val())
        EtapaInicioTMSEdicaoDesabilitada(e);

    e.AutorizarEmissaoDocumentos.enable(false);
    EtapaInicioEmbarcadorEdicaoDesabilitada(e);
}

//*******ETAPA NOTAS PRE CALCULO*******

function EtapaFreteTMSDesabilitada(e) {
    $("#" + e.EtapaFreteTMS.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaFreteTMS.idTab + " .step").attr("class", "step");
    e.EtapaFreteTMS.eventClick = function (e) { };
}

function EtapaFreteTMSLiberada(e) {
    $("#" + e.EtapaFreteTMS.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaFreteTMS.idTab + " .step").attr("class", "step yellow");
    e.EtapaFreteTMS.eventClick = verificarFreteClick;
    EtapaNotaFiscalAprovada(e);
    EtapaNotaFiscalEdicaoDesabilitada(e);
}

function EtapaFreteTMSAguardando(e) {
    $("#" + e.EtapaFreteTMS.idTab).attr("data-bs-toggle", "tab");

    if (e.PendenteGerarCargaDistribuidor.val())
        $("#" + e.EtapaFreteTMS.idTab + " .step").attr("class", "step orange");
    else if (e.LiberadaEtapaFaturamentoBloqueada.val())
        $("#" + e.EtapaFreteTMS.idTab + " .step").attr("class", "step lightsalmon");
    else
        $("#" + e.EtapaFreteTMS.idTab + " .step").attr("class", "step yellow");

    e.EtapaFreteTMS.eventClick = verificarFreteClick;
    EtapaNotaFiscalAprovada(e);
    EtapaNotaFiscalEdicaoDesabilitada(e);
}

function EtapaFreteTMSAprovada(e) {
    $("#" + e.EtapaFreteTMS.idTab).attr("data-bs-toggle", "tab");

    if (e.AgConfirmacaoUtilizacaoCredito.val())
        $("#" + e.EtapaFreteTMS.idTab + " .step").attr("class", "step orange");
    else if (e.LiberadaEtapaFaturamentoBloqueada.val())
        $("#" + e.EtapaFreteTMS.idTab + " .step").attr("class", "step lightsalmon");
    else
        $("#" + e.EtapaFreteTMS.idTab + " .step").attr("class", "step green");

    e.EtapaFreteTMS.eventClick = verificarFreteClick;
    EtapaNotaFiscalAprovada(e);
    EtapaNotaFiscalEdicaoDesabilitada(e);
}

function EtapaFreteTMSProblema(e) {
    $("#" + e.EtapaFreteTMS.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaFreteTMS.idTab + " .step").attr("class", "step red");
    EtapaDadosTransportadorDesabilitada(e);
    EtapaNotaFiscalAprovada(e);
    EtapaNotaFiscalEdicaoDesabilitada(e);
    e.EtapaFreteTMS.eventClick = verificarFreteClick;
}

function EtapaFreteTMSEdicaoDesabilitada(e) {
    e.EtapaFreteEmbarcador.enable(false); // Desabilita a etapa Embarcador pois é o html utilizado referencia essa etapa independente do módulo (embarcador ou TMS)
    e.AutorizarEmissaoDocumentos.enable(false);
    e.AtualizarValorFrete.enable(false);
    e.RecalcularFrete.enable(false);
    e.RetornarParaEtapaNFeTMS.enable(false);
    e.ComponenteFrete.enable(false);
    e.ConferenciaDeFrete.enable(false);

    EtapaNotaFiscalEdicaoDesabilitada(e);

    if (e.ExigeNotaFiscalParaCalcularFrete.val())
        EtapaInicioTMSEdicaoDesabilitada(e);

    if (_configuracaoEmissaoCTe != null)
        _configuracaoEmissaoCTe.DesabilitarEmissaoCTe();

    if (_cargaDadosEmissaoGeral != null)
        _cargaDadosEmissaoGeral.Atualizar.enable(false);

    if (_cargaDadosEmissaoConfiguracao != null)
        _cargaDadosEmissaoConfiguracao.Atualizar.enable(false);

    if (_cargaDadosEmissaoLacre != null)
        _cargaDadosEmissaoLacre.Adicionar.enable(false);

    if (_cargaDadosEmissaoObservacao != null)
        _cargaDadosEmissaoObservacao.Atualizar.enable(false);

    if (_cargaDadosEmissaoPassagem != null)
        _cargaDadosEmissaoPassagem.AlterarPercursoMDFe.enable(false);

    if (_ordemPercursoCarga != null)
        _ordemPercursoCarga.AlterarPercurso.enable(false);

    if (_cargaDadosEmissaoValePedagio != null)
        _cargaDadosEmissaoValePedagio.Adicionar.enable(false);

    if (_cargaDadosEmissaoSeguro != null)
        _cargaDadosEmissaoSeguro.Atualizar.enable(false);

    if (_gridCompentesDeFrete != null)
        _gridCompentesDeFrete.CarregarGrid(_gridCompentesDeFrete.BuscarRegistros(), false);
}
