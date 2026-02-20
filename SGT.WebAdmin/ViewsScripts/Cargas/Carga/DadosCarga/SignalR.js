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
/// <reference path="../../../Global/Notificacoes/Notificacao.js" />
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
/// <reference path="Carga.js" />
/// <reference path="DataCarregamento.js" />
/// <reference path="Leilao.js" />
/// <reference path="Operador.js" />
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
/// <reference path="../../../Enumeradores/EnumTipoAcaoCarga.js" />

function LoadConexaoSignalRCarga() {
    SignalRCargaAlteradaEvent = VerificarCargaAlteradaEvent;
    SignalRCargaQuantidadeDocumentosGerados = InformarQuantidadeDocumentosGeradosCargaEvent;
    SignalRCargaQuantidadeDocumentosEmitidos = InformarQuantidadeDocumentosEmitidosCargaEvent;
    SignalRTransbordoCargaAtualizadaEvent = InformarTransbordoCargaAtualizadaEvent;
    SignalRCargaInformarRetornoCalculoFreteEvent = InformarRetornoCalculoFreteEvent;
    SignalRCargaInformarMensagemAlertaEvent = InformarMensagemAlertaEvent;
    SignalRCargaRetornoProcessamentoDocumentosFiscaisEvent = InformarRetornoProcessamentoDocumentosFiscaisEvent;
    SignalRCargaDadosTransporteAtualizadoEvent = InformarRetornoIntegracaoCargaDadosTransporteEvent;
}

function VerificarCargaAlteradaEvent(retorno) {
    if (_listaKnoutsCarga != null) {
        if (retorno.TipoAcao == EnumTipoAcaoCarga.Inserida) {

        } else {
            var podeAtualizar = false;
            var indiceKnout;

            $.each(_listaKnoutsCarga, function (i, knoutCarga) {
                if (knoutCarga.Codigo.val() == retorno.CodigoCarga) {
                    podeAtualizar = true;
                    indiceKnout = i;
                    return false;
                }
            });

            if (podeAtualizar) {
                var data = { Carga: retorno.CodigoCarga };
                _RequisicaoIniciada = true;
                executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
                    if (arg.Success) {
                        var carga = arg.Data;
                        var knoutCarga = _listaKnoutsCarga[indiceKnout];

                        atualizarDadosCarga(knoutCarga, carga);
                    }
                    _RequisicaoIniciada = false;
                });
            }
        }
    }
}

function InformarQuantidadeDocumentosGeradosCargaEvent(retorno) {
    if (_listaKnoutsCarga != null) {
        $.each(_listaKnoutsCarga, function (i, knoutCarga) {
            if (knoutCarga.Codigo.val() == retorno.CodigoCarga) {
                knoutCarga.QuantidadeDocumentosGerados.val(retorno.QuantidadeDocumentosGerados);
                knoutCarga.QuantidadeDocumentosTotal.val(retorno.QuantidadeDocumentosTotal);
                return false;
            }
        });
    }
}

function InformarQuantidadeDocumentosEmitidosCargaEvent(retorno) {
    if (_cargaCTe != null && _cargaCTe.Carga.val() == retorno.CodigoCarga) {
        if (retorno.Erro) {
            _cargaCTe.EmitindoCTes.visible(false);
            _cargaCTe.ErroEmissaoCTes.visible(true);
            _cargaCTe.ErroEmissaoCTes.val(retorno.Mensagem);
        } else if (retorno.QuantidadeDocumentosEmitidos == retorno.QuantidadeDocumentosTotal) {
            _cargaCTe.ErroEmissaoCTes.visible(false);
            _cargaCTe.EmitindoCTes.visible(false);
            _cargaAtual.EmitindoCTes.val(false);
            _cargaAtual.CTesEmDigitacao.val(false);
            _cargaCTe.SucessoEmissaoCTes.visible(true);

            if (_gridCargaCTe != null)
                _gridCargaCTe.CarregarGrid();
        } else {
            _cargaCTe.ErroEmissaoCTes.visible(false);
            _cargaCTe.EmitindoCTes.visible(true);
            _cargaCTe.QuantidadeDocumentosEmitidos.val(retorno.QuantidadeDocumentosEmitidos);
            _cargaCTe.QuantidadeDocumentosTotal.val(retorno.QuantidadeDocumentosTotal);
        }
    }
}

function InformarRetornoCalculoFreteEvent(retorno) {
    if (_cargaAtual != null && _cargaAtual.Codigo.val() == retorno.CodigoCarga) {
        if (preecherRetornoFrete && typeof preecherRetornoFrete === 'function' && retorno.Retorno != null) {
            preecherRetornoFrete(_cargaAtual, retorno.Retorno);
            atualizarResumoCargaTerceiro();
        }

        if (obterDadosEmissaoGeralCarga && typeof obterDadosEmissaoGeralCarga === 'function') {
            loadCargaDadosEmissaoGeral();
            obterDadosEmissaoGeralCarga();
        }
    }
}

function InformarMensagemAlertaEvent(retorno) {
    if (_cargaAtual != null && _cargaAtual.Codigo.val() == retorno.CodigoCarga) {
        if (preecherRetornoMensagemAlerta && typeof preecherRetornoMensagemAlerta === 'function' && retorno.Retorno != null)
            preecherRetornoMensagemAlerta(_cargaAtual, retorno.Retorno);
    }
}

function InformarRetornoProcessamentoDocumentosFiscaisEvent(retorno) {
    if (retorno != null) {
        if (retorno.Retorno != null && retorno.Retorno.Sucesso === true) {
            var podeAtualizar = false;
            var indiceKnout;
            $.each(_listaKnoutsCarga, function (i, knoutCarga) {
                if (knoutCarga.Codigo.val() == retorno.CodigoCarga) {
                    podeAtualizar = true;
                    indiceKnout = i;
                    return false;
                }
            });

            if (podeAtualizar) {
                var knoutCarga = _listaKnoutsCarga[indiceKnout];
                IniciarBindKnoutCarga(knoutCarga, retorno.Retorno.Carga);
                InformarEstadosDasEtapas(knoutCarga, retorno.Retorno.Carga, knoutCarga.DivCarga.id);
            }
        }

        if (_cargaAtual != null && _cargaAtual.Codigo.val() == retorno.CodigoCarga) {
            if (retorno.Retorno.Sucesso) {

                var carga = retorno.Retorno.Carga;

                if ((_documentoEmissao != null) && !_cargaAtual.CargaEmitidaParcialmente.val()) {
                    limparCamposDocumentosParaEmissao();

                    _documentoEmissao.Pedido.enable(false);
                    _documentoEmissao.Dropzone.visible(false);
                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {

                    $("#" + _cargaAtual.EtapaNotaFiscal.idGrid).attr("class", "tab-pane");
                    $("#" + _cargaAtual.EtapaFreteEmbarcador.idGrid).attr("class", "tab-pane active");

                    loadCargaDadosEmissao(function () {
                        carregarDadosPedido(0, function () {
                            //preecherRetornoFrete(_cargaAtual, retorno);
                            verificarFrete(_cargaAtual); //adicionado desta maneira para verificar se não fica travado como frete sendo calculado
                        });
                    });
                }
            } else {
                _cargaAtual.ProcessandoDocumentosFiscais.val(false);

                if (_documentoEmissao != null) {
                    _documentoEmissao.ProcessandoDocumentosFiscais.val(false);
                    _documentoEmissao.ConfirmarEnvioDocumentos.visible(true);
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Retorno.Mensagem);
                }
            }
        }
    }
}

function InformarRetornoIntegracaoCargaDadosTransporteEvent(retorno) {
    if (retorno != null && typeof RecarregarIntegracaoCargaDadosTransporteViaSignalR == 'function') {
        var podeAtualizar = false;
        var indiceKnout;
        $.each(_listaKnoutsCarga, function (i, knoutCarga) {
            if (knoutCarga.Codigo.val() == retorno.CodigoCarga) {
                podeAtualizar = true;
                indiceKnout = i;
                return false;
            }
        });

        if (podeAtualizar) {
            var knoutCarga = _listaKnoutsCarga[indiceKnout];

            RecarregarIntegracaoCargaDadosTransporteViaSignalR(knoutCarga);
        }
    }
}
