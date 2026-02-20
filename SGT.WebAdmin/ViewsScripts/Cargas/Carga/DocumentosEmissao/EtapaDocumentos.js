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

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _HTMLDetalhesDocumentoEmissao;
var _gridCartaCorrecao, _pesquisaCartaCorrecao;
/*
 * Declaração das Classes
 */

var NFs = function () {
    this.Cliente = PropertyEntity({ type: types.local });
    this.ListaNF = PropertyEntity({ list: new Array, type: types.local });
};

var PesquisaCartaCorrecao = function () {
    this.Chave = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.string });
};

/*
 * 
 * 
 * Declaração das Funções Associadas a Eventos
 */

function buscarDocumentosClick(e, sender, mercosul, efetuouPesquisa) {
    if (!efetuouPesquisa) {
        ocultarTodasAbas(e);

        e.ExibirFiltrosEtapaNFe.visibleFade(false);
        e.PedidoComNFe.val("");
    }
    _cargaAtual = e;

    verificarTipoDeImportacaoManual(_cargaAtual.Codigo.val()).then(() => {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || e.PermiteImportarDocumentosManualmente.val()) {

            if (mercosul === true)
                $("#" + e.EtapaDocumentosMercosul.idGrid).html("");
            else
                $("#" + e.EtapaNotaFiscal.idGrid).html("");

            CarregarHTMLDocumentosParaEmissao().then(function () {
                loadInfoDocumentosParaEmissao(e, true, mercosul);
            });
        }
        else {
            var data = {
                Carga: e.Codigo.val(),
                PedidoComNFe: e.PedidoComNFe.val()
            };

            executarReST("CargaNotasFiscais/BuscarNFeCarga", data, function (arg) {
                if (arg.Success) {

                    var data = arg.Data;

                    $("#" + e.EtapaNotaFiscal.idGrid + " .NFsEnviadas").off('click', '**');
                    $("#" + e.EtapaNotaFiscal.idGrid + " .NFsEnviadas").html("");

                    let html = '';

                    if (e.EmissaoLiberada.val() && !e.AutorizarEmissaoDocumentos.visibleCTeProcessamento() && !e.AguardarIntegracaoEtapaTransportador.val()) {
                        html += '<div class="alert alert-info fw-bold">';
                        html += Localization.Resources.Cargas.Carga.EmissaoSoEstaProgramadaParaOcorrerEm + ' ' + e.DataInicioEmissao.val();
                        html += '</div>'
                    }

                    let idAccordion = 'accordion_' + e.EtapaNotaFiscal.idGrid;

                    html += '<div class="accordion" id="' + idAccordion + '">';

                    $.each(data.Pedidos, function (i, pedido) {
                        var bgColor = "";
                        var textoAguardando = "";

                        if (pedido.AgEmissaoCTeAnteriorTrecho) {
                            bgColor = "bg-warning";
                            textoAguardando = Localization.Resources.Cargas.Carga.AguardandoEmissaoDoCTeAnterior;
                        } else if (pedido.AgValorFreteRedespacho) {
                            bgColor = "bg-warning";
                            textoAguardando = Localization.Resources.Cargas.Carga.AguardandoValorDoFreteDaCargaDeRedespacho;
                        }

                        let idAccordionHeader = 'accordionHeader_' + pedido.Codigo;
                        let idAccordionCollapse = 'accordionCollapse_' + pedido.Codigo;

                        html += '   <div class="accordion-item">';
                        html += '       <h2 class="accordion-header" id="' + idAccordionHeader + '">';
                        html += '           <button class="accordion-button ' + bgColor + '" type="button" data-bs-toggle="collapse" data-bs-target="#' + idAccordionCollapse + '" aria-expanded="true" aria-controls="' + idAccordionCollapse + '">';

                        if (pedido.ReentregaSolicitada)
                            html += '<b class="icone-reentrega me-3"><i class="fal fa-sync"></i></b>&nbsp;';

                        html += pedido.Cliente + ' (' + Localization.Resources.Cargas.Carga.PedidoNumero + ' ' + pedido.CodigoPedidoEmbarcador + ')<b class="ms-3">' + textoAguardando + '</b>';

                        html += '           </button>';
                        html += '       </h2>';

                        if (pedido.NFs.length > 0 || pedido.NotasParciais.length > 0) {
                            html += '       <div id="' + idAccordionCollapse + '" class="accordion-collapse collapse show" aria-labelledby="' + idAccordionHeader + '">';
                            html += '           <div class="accordion-body">';
                            html += _HTMLDetalhesDocumentoEmissao.replace(/#DocumentosEmissao/g, pedido.Codigo);
                            html += '           </div>';
                            html += '       </div>';
                        }

                        html += '   </div>';
                    });

                    html += '</div>';

                    $("#" + e.EtapaNotaFiscal.idGrid + " .NFsEnviadas").html(html);

                    var numeroNF = 0;
                    $.each(data.Pedidos, function (i, pedido) {
                        numeroNF += pedido.NFs.length;

                        if (pedido.NFs.length > 0 || pedido.NotasParciais.length > 0) {
                            let header = [
                                { data: "CargaPedido", visible: false },
                                { data: "Codigo", visible: false },
                                { data: "Chave", title: Localization.Resources.Cargas.Carga.Chave, width: "50%", orderable: false },
                                { data: "SituacaoNaoConformidade", title: "Não Conformidade", width: "20%", className: "text-align-center", orderable: false, visible: data.PossuiNaoConformidade },
                                { data: "Numero", title: Localization.Resources.Cargas.Carga.Numero, width: "20%", className: "text-align-center", orderable: false },
                                { data: "DataEmissao", title: Localization.Resources.Cargas.Carga.Emissao, width: "15%", className: "text-align-center", orderable: false },
                                { data: "Peso", title: Localization.Resources.Cargas.Carga.Peso, width: "15%", className: "text-align-right" },
                                { data: "ValorTotal", title: Localization.Resources.Cargas.Carga.Valor, width: "15%", className: "text-align-right" }
                            ];

                            let opcoes = [];
                            let menuOpcoes = null;

                            if (_CONFIGURACAO_TMS.HabilitarRelatorioDeTroca) {
                                opcoes.push({ descricao: Localization.Resources.Cargas.Carga.EnviarBoletosParaImpressão, id: guid(), evento: "onclick", metodo: enviarBoletosParaImpressaoClick, tamanho: "10", icone: "" });
                                opcoes.push({ descricao: Localization.Resources.Cargas.Carga.EnviarNotaParaImpressao, id: guid(), evento: "onclick", metodo: enviarNotaParaImpressaoClick, tamanho: "10", icone: "" });
                            }

                            if (_CONFIGURACAO_TMS.PermitirDownloadDANFE) {
                                opcoes.push({ descricao: Localization.Resources.Cargas.Carga.DownloadDANFE, id: guid(), evento: "onclick", metodo: downloadDANFENotaClick, tamanho: "10", icone: "" });
                                opcoes.push({ descricao: Localization.Resources.Cargas.Carga.VisualizarDANFE, id: guid(), evento: "onclick", metodo: visualizarDANFENotaClick, tamanho: "10", icone: "" });
                            }

                            if (_CONFIGURACAO_TMS.PermitirDownloadXmlEtapaNfe) {
                                opcoes.push({ descricao: Localization.Resources.Cargas.Carga.DownloadXML, id: guid(), evento: "onclick", metodo: downloadXmlNotaClick, tamanho: "10", icone: "" });
                                opcoes.push({ descricao: Localization.Resources.Cargas.Carga.CartaDeCorrecao, id: guid(), evento: "onclick", metodo: exibirGridCartaCorrecao, tamanho: "10", icone: "", visibilidade: visibilidadeGridCCe });
                            }

                            opcoes.push({ descricao: "Detalhes das Não Conformidades", id: guid(), evento: "onclick", metodo: exibirDetalhesNaoConformidadesClick, tamanho: "10", icone: "", visibilidade: visibilidadeOpcaoDetalhesNaoConformidades });



                            if (opcoes.length > 0)
                                menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: opcoes, tamanho: 10, };

                            let gridNF = new BasicDataTable(pedido.Codigo + '_grid', header, menuOpcoes);

                            gridNF.CarregarGrid(pedido.NFs);

                            if ((pedido.TipoContratacaoCarga == EnumTipoContratacaoCarga.SVMTerceiro || pedido.TipoContratacaoCarga == EnumTipoContratacaoCarga.SVMProprio || pedido.TipoContratacaoCarga == EnumTipoContratacaoCarga.SubContratada || pedido.TipoContratacaoCarga == EnumTipoContratacaoCarga.Redespacho || pedido.TipoContratacaoCarga == EnumTipoContratacaoCarga.RedespachoIntermediario) && !pedido.CargaPedidoFilialEmissora) {
                                header = [
                                    { data: "Chave", title: Localization.Resources.Cargas.Carga.Chave, width: "50%", orderable: false },
                                    { data: "SituacaoNaoConformidade", title: "Não Conformidade", width: "15%", className: "text-align-right", orderable: false, visible: data.PossuiNaoConformidade },
                                    { data: "Numero", title: Localization.Resources.Cargas.Carga.Numero, width: "20%", className: "text-align-center", orderable: false },
                                    { data: "DataEmissao", title: Localization.Resources.Cargas.Carga.Emissao, width: "15%", className: "text-align-center", orderable: false },
                                    { data: "Emissor", title: Localization.Resources.Cargas.Carga.Emitente, width: "30%", className: "text-align-left", orderable: false },
                                    { data: "ValorReceber", title: Localization.Resources.Cargas.Carga.Valor, width: "15%", className: "text-align-right" }
                                ];

                                let gridCTe = new BasicDataTable(pedido.Codigo + '_gridCTe', header, null);

                                gridCTe.CarregarGrid(pedido.CTes);

                                $("#tabCTe_" + pedido.Codigo + "_li").show();
                            }

                            $("#divDocumentosEmissao_" + pedido.Codigo).show();
                        }
                        if (pedido.NotasParciais.length > 0) {
                            header = [
                                { data: "Chave", title: Localization.Resources.Cargas.Carga.Chave, width: "50%", orderable: false },
                                { data: "Numero", title: Localization.Resources.Cargas.Carga.NumeroFatura, width: "30%", className: "text-align-center", orderable: false },
                                { data: "Status", title: Localization.Resources.Gerais.Geral.Status, width: "20%", className: "text-align-center", orderable: false },
                            ];

                            let gridNotasParciais = new BasicDataTable(pedido.Codigo + '_gridNotasParciais', header, null);

                            gridNotasParciais.CarregarGrid(pedido.NotasParciais);

                            $("#tabNotasParciais_" + pedido.Codigo + "_li").show();
                            $("#divDocumentosEmissao_" + pedido.Codigo).show();
                        }

                    });



                    $("#" + e.EtapaNotaFiscal.idGrid + " .NFsEnviadas").on('click', '.dd-handle', function (e) {
                        e.stopPropagation();
                        $(e.currentTarget).parent().toggleClass('dd-collapsed');
                    });

                    e.QuantidadePedidosEtapaNFe.val(data.QuantidadePedidosComNF + "/" + data.QuantidadePedidos);

                    e.RetornarEtapaNFe.visible((_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && _CONFIGURACAO_TMS.PermitirRetornoAgNotasFiscais && numeroNF == 0));
                    e.LiberarEmissaoSemIntegracaoEtapaTransportador.visible(e.AguardarIntegracaoEtapaTransportador.val());

                    loadAgrupamentoNotasPrechekin(e.EtapaNotaFiscal.idGrid, "#liTabEtapaNFeStage_" + e.EtapaInicioTMS.idGrid, "#tabEtapaNFeStage_" + e.EtapaInicioTMS.idGrid);

                    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_LiberarEmissaoSemNF, _PermissoesPersonalizadasCarga)
                        && (!_cargaAtual.CargaDePreCarga.val() || _cargaAtual.CargaAgrupada.val()) && ((_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe)
                            && !Boolean(_cargaAtual.DataInicioEmissao.val()) || _cargaAtual.CargaEmitidaParcialmente) && _CONFIGURACAO_TMS.PermitirLiberarCargaSemNFe)
                        e.LiberarEmissaoSemNF.visible(true);
                    else
                        e.LiberarEmissaoSemNF.visible(false);

                    _pesquisaCartaCorrecao = new PesquisaCartaCorrecao();
                    loadGridCartaCorrecao();
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            });
        }
    });
}

function loadGridCartaCorrecao() {

    var opcaoDownloadDANFE = { descricao: Localization.Resources.Cargas.Carga.DownloadDANFECCe, id: guid(), evento: "onclick", metodo: downloadDACCeNotaClick, tamanho: "20", icone: "" };
    var opcaoDownloadXML = { descricao: Localization.Resources.Cargas.Carga.DownloadXMLCCe, id: guid(), evento: "onclick", metodo: downloadXmlNotaCCeClick, tamanho: "20", icone: "" };
    var menuOpcoes = { descricao: Localization.Resources.Gerais.Geral.Opcoes, tipo: TypeOptionMenu.list, opcoes: [opcaoDownloadDANFE, opcaoDownloadXML] };
    _gridCartaCorrecao = new GridView("gridCartaCorrecao", "DocumentoDestinadoEmpresa/ObterCartaCorrecoes", _pesquisaCartaCorrecao, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function visibilidadeGridCCe(dados) {
    return _CONFIGURACAO_TMS.PermitirDownloadXmlEtapaNfe && dados.PossuiCartaCorrecao == true;
}

function downloadXmlNotaClick(e) {
    executarDownload("DocumentoNF/DownloadXml", { CargaPedido: e.CargaPedido, Codigo: e.Codigo });
}

function downloadXmlNotaCCeClick(e) {
    executarDownload("DocumentoNF/DownloadXmlCCe", { CargaPedido: e.CargaPedido, Codigo: e.Codigo });
}

function downloadDACCeNotaClick(e) {
    executarDownload("DocumentoNF/DownloadDACCe", { CargaPedido: e.CargaPedido, Codigo: e.Codigo });
}

function exibirDetalhesNaoConformidadesClick(e) {
    exibirDetalhesNaoConformidadePorNotaFiscal(e.Codigo);
}

function downloadDANFENotaClick(e) {
    executarDownload("DocumentoNF/DownloadDANFE", { CargaPedido: e.CargaPedido, Codigo: e.Codigo });
}

function visualizarDANFENotaClick(e) {
    exibirModalVisualizarDANFE(e);
}

function downloadTodosXmlNotasFiscaisClick(e) {
    executarDownloadArquivo("DocumentoNF/DownloadTodosXmlPorCarga", { Carga: e.Codigo.val() });
}

function enviarBoletosParaImpressaoClick(notaSelecionada) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.VoceRealmenteDesejaEnviarOsBoletosParaImpressao, function () {
        executarReST("CargaImpressaoDocumentos/EnviarBoletosParaImpressao", { CargaPedido: notaSelecionada.CargaPedido }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    })
}

function enviarNotaParaImpressaoClick(notaSelecionada) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.VoceRealmenteDesejaEnviarNotaParaImpressao, function () {
        executarReST("CargaImpressaoDocumentos/EnviarNotaParaImpressao", { CargaPedido: notaSelecionada.CargaPedido, Nota: notaSelecionada.Numero }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    })
}

function LiberarEmissaoSemIntegracaoEtapaTransportadorClick(e, sender) {
    /**
    * Essa função é chamada em dois locais.
    * Cuidado ao usar atributos de "e"
    */
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaLiberarEmissaoSemIntegracaoNaEtapaTransportador, function () {
        var data = { Carga: _cargaAtual.Codigo.val() };
        executarReST("CargaNotasFiscais/LiberarEmissaoSemIntegracaoEtapaTransportador", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    e.LiberarEmissaoSemIntegracaoEtapaTransportador.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function LiberarEmissaoSemNFClick(e) {
    liberarEmissaoSemNF(e, e.Codigo.val());
}

function BuscarNFesEmilleniumClick(e) {
    var data = { Carga: e.Codigo.val() };

    executarReST("CargaIntegracao/BuscarNFesEmillenium", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, (arg.Data.Msg != null && arg.Data.Msg != '') ? arg.Data.Msg : Localization.Resources.Cargas.Carga.PedidosProcessadosComSucesso);
                idTab = _cargaAtual.EtapaNotaFiscal.idTab;
                $("#" + idTab).trigger("click");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function PesquisarEtapaNFeClick(e, sender) {
    e.ExibirFiltrosEtapaNFe.visibleFade(false);
    buscarDocumentosClick(e, sender, false, true);
}

function RetornarEtapaNFeClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaRetornarEstaCargaParaEtapaAnterior, function () {
        retornarEtapaNFe(e);
    });
};

function retornarEtapaNFe(e) {
    var data = { Carga: e.Codigo.val() };

    executarReST("CargaNotasFiscais/RetornarEtapa", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                var carga = arg.Data;
                var idTab;
                $.each(_listaKnoutsCarga, function (i, knoutCarga) {
                    if (carga.Codigo == knoutCarga.Codigo.val()) {
                        preencherDadosCarga(knoutCarga, carga);
                        idTab = knoutCarga.EtapaDadosTransportador.idTab;
                        return false;
                    }
                });
                EtapaDadosTransportadorLiberada(e);
                EtapaNotaFiscalDesabilitada(e);
                $("#" + idTab).trigger("click");
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.EtapaRetornadaComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function solicitarNFsClick(e) {
    var mensagem = Localization.Resources.Cargas.Carga.RealmenteDesejaSolicitarAsNotasFiscaisParaEstaCargaLembrandoQueAposFazerSolicitacaoNaoMaisPossivelAlterarAsEtapasAnteriores;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        mensagem = Localization.Resources.Cargas.Carga.RealmenteDesejaComecarEnvioDasNotasFiscaisParaEssaCarga;

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, mensagem, function () {
        solicitarNotasFiscais(e);
    });
}

/*
 * Declaração das Funções Públicas
 */

function EtapaNotaFiscalAguardando(e) {

    $("#" + e.EtapaNotaFiscal.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaNotaFiscal.idTab + " .step").attr("class", "step yellow");
    e.EtapaNotaFiscal.eventClick = buscarDocumentosClick;


    if (!e.ExigeNotaFiscalParaCalcularFrete.val()) {
        EtapaDadosTransportadorAprovada(e);

        if (!_CONFIGURACAO_TMS.PermitirInformarDadosTransportadorCargaEtapaNFe)
            EtapaDadosTransportadorEdicaoDesabilitada(e);
    }
    else if (_CONFIGURACAO_TMS.PossuiIntegracaoBRKVeiculoEMotorista && (!e.MotoristaValidadoBrasilRisk.val() || !e.PlacaValidadoBrasilRisk.val()))
        EtapaInicioTMSAlerta(e);
    else
        EtapaInicioTMSAprovada(e);

    if (_CONFIGURACAO_TMS.BloquearCamposTransportadorQuandoEtapaNotas)
        EtapaDadosTransportadorEdicaoDesabilitada(e);

    if (e.TipoOperacao.exibirFiltroDePedidosEtapaNotaFiscal)
        $("#liFiltrosPesquisaEtapaNotaFiscal_" + e.EtapaNotaFiscal.idGrid).show();

    if (e.Mercosul.val() && !e.EmitindoCRT.val()) {
        EtapaNotaFiscalAprovada(e);
        EtapaNotaFiscalMercosulAguardando(e);
        EtapaCTeNFsDesabilitada(e);
    }

    if (e.PossuiOperacaoContainer) {
        EtapaContainerAprovada(e);
    }
}

function EtapaNotaFiscalAprovada(e) {

    $("#" + e.EtapaNotaFiscal.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaNotaFiscal.idTab + " .step").attr("class", "step green");
    e.EtapaNotaFiscal.eventClick = buscarDocumentosClick;

    if (!e.ExigeNotaFiscalParaCalcularFrete.val()) {
        EtapaDadosTransportadorAprovada(e);
        EtapaDadosTransportadorEdicaoDesabilitada(e);
    }
    else if (_CONFIGURACAO_TMS.PossuiIntegracaoBRKVeiculoEMotorista && (!e.MotoristaValidadoBrasilRisk.val() || !e.PlacaValidadoBrasilRisk.val()))
        EtapaInicioTMSAlerta(e);
    else
        EtapaInicioTMSAprovada(e);

    if (e.PossuiFacturaFake.val())
        EtapaNotaFiscalComFacturaFake(e);

    if (e.TipoOperacao.exibirFiltroDePedidosEtapaNotaFiscal)
        $("#liFiltrosPesquisaEtapaNotaFiscal_" + e.EtapaNotaFiscal.idGrid).show();

    if (e.PossuiOperacaoContainer) {
        EtapaContainerAprovada(e);
    }

}

function EtapaNotaFiscalDesabilitada(e) {
    if (!e.EtapaNotaFiscal)
        return;

    $("#" + e.EtapaNotaFiscal.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaNotaFiscal.idTab + " .step").attr("class", "step");
    e.EtapaNotaFiscal.eventClick = function (e) { };
}

function EtapaNotaFiscalEdicaoDesabilitada(e) {
    e.EtapaNotaFiscal.enable(false);

    if (!e.ExigeNotaFiscalParaCalcularFrete.val())
        EtapaDadosTransportadorEdicaoDesabilitada(e);
    else if (e.PermiteImportarDocumentosManualmente.val())
        EtapaInicioTMSEdicaoDesabilitada(e);
}

function EtapaNotaFiscalLiberada(e) {
    $("#" + e.EtapaNotaFiscal.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaNotaFiscal.idTab + " .step").attr("class", "step yellow");
    e.EtapaNotaFiscal.eventClick = buscarDocumentosClick;

    if (!e.ExigeNotaFiscalParaCalcularFrete.val()) {
        EtapaDadosTransportadorAprovada(e);

        if (!_CONFIGURACAO_TMS.PermitirInformarDadosTransportadorCargaEtapaNFe)
            EtapaDadosTransportadorEdicaoDesabilitada(e);
    }
    else if (_CONFIGURACAO_TMS.PossuiIntegracaoBRKVeiculoEMotorista && (!e.MotoristaValidadoBrasilRisk.val() || !e.PlacaValidadoBrasilRisk.val()))
        EtapaInicioTMSAlerta(e);
    else
        EtapaInicioTMSAprovada(e);

    if (e.TipoOperacao.exibirFiltroDePedidosEtapaNotaFiscal)
        $("#liFiltrosPesquisaEtapaNotaFiscal_" + e.EtapaNotaFiscal.idGrid).show();
}

function EtapaNotaFiscalProblema(e) {
    $("#" + e.EtapaNotaFiscal.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaNotaFiscal.idTab + " .step").attr("class", "step red");
    e.EtapaNotaFiscal.eventClick = buscarDocumentosClick;

    EtapaDadosTransportadorAprovada(e);

    if (!_CONFIGURACAO_TMS.PermitirInformarDadosTransportadorCargaEtapaNFe)
        EtapaDadosTransportadorEdicaoDesabilitada(e);

    if (_CONFIGURACAO_TMS.PossuiIntegracaoBRKVeiculoEMotorista && (!e.MotoristaValidadoBrasilRisk.val() || !e.PlacaValidadoBrasilRisk.val()))
        EtapaInicioTMSAlerta(e);
    else
        EtapaInicioTMSAprovada(e);

    if (e.TipoOperacao.exibirFiltroDePedidosEtapaNotaFiscal)
        $("#liFiltrosPesquisaEtapaNotaFiscal_" + e.EtapaNotaFiscal.idGrid).show();

    if (e.PossuiOperacaoContainer) {
        EtapaContainerAprovada(e);
    }
}

function EtapaNotaFiscalComFacturaFake(e) {
    e.EtapaNotaFiscal.text("Factura");
    e.EtapaNotaFiscal.tooltip("Factura");
    e.EtapaNotaFiscal.tooltipTitle("Factura");

    $("#" + e.EtapaNotaFiscal.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaNotaFiscal.idTab + " .step").attr("class", "step cyan");
    e.EtapaNotaFiscal.eventClick = function (e, sender) { buscarDocumentosClick(e, sender, true) };

}

function EtapaNotaFiscalMercosulAguardando(e) {
    $("#" + e.EtapaDocumentosMercosul.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaDocumentosMercosul.idTab + " .step").attr("class", "step yellow");
    e.EtapaDocumentosMercosul.eventClick = function (e, sender) { buscarDocumentosClick(e, sender, true) };

    EtapaCTeFilialEmissoraAprovada(e);

    $("#" + e.EtapaFreteTMS.idTab).removeClass("active");
    $("#" + e.EtapaFreteEmbarcador.idTab).removeClass("active");
}

function EtapaNotaFiscalMercosulAprovada(e) {
    $("#" + e.EtapaDocumentosMercosul.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaDocumentosMercosul.idTab + " .step").attr("class", "step green");
    e.EtapaDocumentosMercosul.eventClick = function (e, sender) { buscarDocumentosClick(e, sender, true) };

    EtapaCTeFilialEmissoraAprovada(e);
}

function EtapaNotaFiscalMercosulDesabilitada(e) {
    $("#" + e.EtapaDocumentosMercosul.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaDocumentosMercosul.idTab + " .step").attr("class", "step");
    e.EtapaDocumentosMercosul.eventClick = function (e) { };
}

function EtapaNotaFiscalMercosulLiberada(e) {
    $("#" + e.EtapaDocumentosMercosul.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaDocumentosMercosul.idTab + " .step").attr("class", "step green");
    e.EtapaDocumentosMercosul.eventClick = function (e, sender) { buscarDocumentosClick(e, sender, true) };

    EtapaCTeFilialEmissoraAprovada(e);
}

function EtapaNotaFiscalMercosulProblema(e) {
    $("#" + e.EtapaDocumentosMercosul.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaDocumentosMercosul.idTab + " .step").attr("class", "step red");
    e.EtapaDocumentosMercosul.eventClick = function (e, sender) { buscarDocumentosClick(e, sender, true) };

    EtapaCTeFilialEmissoraAprovada(e);
}

function liberarEmissaoSemNF(e, codigoCarga) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaConfirmarEstaEtapaSemQueTodosPedidosPossuamNFe, function () {
        executarReST("CargaNotasFiscais/LiberarEmissaoSemNF", { Carga: codigoCarga }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    e.LiberarEmissaoSemNF.visible(false);
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function solicitarNotasFiscais(e) {
    executarReST("CargaNotasFiscais/SolicitarNotasFiscais", { Carga: e.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != false) {
                if (retorno.Data.PercursoMDFeValido) {
                    e.SolicitarNFEsCarga.visible(false);
                    EtapaNotaFiscalAguardando(e);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.SolicitacaoRealizada);
                }
                else {
                    _cargaAtual = e;
                    BuscarPercursoParaMDFe();
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.Carga.AntesDeSolicitarAsNotasFiscaisNecessarioConfigurarPercursoValidoParaGerarOsMDFes);
                }

                _cargaAtual.SituacaoCarga.val(EnumSituacoesCarga.AgNFe);

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                    $("#" + _cargaAtual.EtapaNotaFiscal.idTab).trigger("click");

                if (_CONFIGURACAO_TMS.BloquearCamposTransportadorQuandoEtapaNotas)
                    e.Empresa.enable(false);

                BuscarAgrupamentosNotas(e);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

/*
 * Declaração das Funções Privadas
 */

function CarregarHTMLDocumentosParaEmissao() {
    var p = new promise.Promise();

    if (_HTMLDocumentosParaEmissao == null || _HTMLDocumentosParaEmissao.length == 0) {
        iniciarRequisicao();
        $.get("Content/Static/Carga/DocumentosParaEmissao.html?dyn=" + guid(), function (data) {
            finalizarRequisicao();
            _HTMLDocumentosParaEmissao = data;
            p.done();
        });
    } else {
        p.done();
    }

    return p;
}

function visibilidadeOpcaoDetalhesNaoConformidades(registroSelecionado) {
    return Boolean(registroSelecionado.SituacaoNaoConformidade);
}

function verificarTipoDeImportacaoManual(codigoCarga) {
    return new Promise((resolve) => {
        executarReST("CargaDadosTransporte/VerificarTipoImportacaoDocumentos", { Codigo: codigoCarga }, (arg) => {
            _cargaAtual.PermiteImportarDocumentosManualmente.val(arg.Data);
            resolve();
        });
    });
}