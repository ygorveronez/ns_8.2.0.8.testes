$(document).ready(function () {
    $("#txtChaveAcessoCTeEmitidoTomador").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");
    $("#txtChaveAcessoNFeEmitidaTomador").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");
    $("#txtChaveAcessoCTeAnulacao").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");
    $("#txtCNPJNFouCTEmitidoTomador").mask("99.999.999/9999-99");
    $("#txtNumeroNFouCTEmitidoTomador").mask("9?999999999");
    $("#txtValorNFouCTEmitidoTomador").priceFormat({ prefix: '' });

    FormatarCampoDate("txtDataEmissaoNFouCTEmitidoTomador");
    FormatarCampoDate("txtDataEmissaoDeclaracaoCTeAnulado");

    $("#selTipoDocumentoTomadorContribuinte").change(function () {
        TrocarTipoDocumentoTomador();
    });

    $("#selTomadorContribuinteICMS").change(function () {
        if ($(this).val() == "1") {
            $("#divTomadorContribuinte").show();
            $("#divTomadorNaoContribuinte").hide();
        } else {
            $("#divTomadorContribuinte").hide();
            $("#divTomadorNaoContribuinte").show();
        }
    });

    CarregarConsultadeClientes("btnBuscarEmitenteDocumentoSubstituicao", "btnBuscarEmitenteDocumentoSubstituicao", RetornoConsultaEmitenteDocumentoAnulacao, true, false);
});

function RetornoConsultaCTeDocumentoAnulacao(cte) {
    $("#txtChaveAcessoCTeAnulacao").val(cte.Chave);
}

function RetornoConsultaEmitenteDocumentoAnulacao(cliente) {
    $("#txtCNPJNFouCTEmitidoTomador").val(cliente.CPFCNPJ);
}

function TrocarTipoDocumentoTomador() {
    switch ($("#selTipoDocumentoTomadorContribuinte").val()) {
        case "0":
            $("#divChaveAcessoCTeTomador").hide();
            $("#divChaveAcessoNFeTomador").hide();
            $("#divInformacaoNFouCTEmitidoTomador").hide();
            break;
        case "1":
            $("#divChaveAcessoCTeTomador").show();
            $("#divChaveAcessoNFeTomador").hide();
            $("#divInformacaoNFouCTEmitidoTomador").hide();
            break;
        case "2":
            $("#divChaveAcessoCTeTomador").hide();
            $("#divChaveAcessoNFeTomador").show();
            $("#divInformacaoNFouCTEmitidoTomador").hide();
            break;
        case "3":
            $("#divChaveAcessoCTeTomador").hide();
            $("#divChaveAcessoNFeTomador").hide();
            $("#divInformacaoNFouCTEmitidoTomador").show();
            break;
    }
}

function BloquearCamposCTeComplementar() {
    BloquearCamposCTeOutros();

    $("#txtDataEmissao").prop("disabled", false);
    $("#txtHoraEmissao").prop("disabled", false);
    $("#txtValorFreteContratado").prop("disabled", false);
    $("#selAliquotaICMS").prop("disabled", false);
    $("#txtValorBaseCalculoICMS").prop("disabled", false);
    $("#chkIncluirICMSNoFrete").prop("disabled", false);
    $("#txtPercentualICMSRecolhido").prop("disabled", false);
    $("#selICMS").prop("disabled", false);
    $("#selFormaImpressao").prop("disabled", false);
    $("#ddlUFInicioPrestacao").prop("disabled", false);
    $("#ddlMunicipioInicioPrestacao").prop("disabled", false);
    $("#ddlUFTerminoPrestacao").prop("disabled", false);
    $("#ddlMunicipioTerminoPrestacao").prop("disabled", false);

    $("#selPIS").prop("disabled", false);
    $("#txtValorBaseCalculoPIS").prop("disabled", false);
    $("#selAliquotaPIS").prop("disabled", false);
    $("#txtValorPIS").prop("disabled", false);

    $("#selCOFINS").prop("disabled", false);
    $("#txtValorBaseCalculoCOFINS").prop("disabled", false);
    $("#selAliquotaCOFINS").prop("disabled", false);
    $("#txtValorCOFINS").prop("disabled", false);

    $("#txtValorBaseCalculoIR").prop("disabled", false);
    $("#txtAliquotaIR").prop("disabled", false);
    $("#txtValorIR").prop("disabled", false);

    $("#txtValorBaseCalculoINSS").prop("disabled", false);
    $("#txtAliquotaINSS").prop("disabled", false);
    $("#txtValorINSS").prop("disabled", false);

    $("#txtValorBaseCalculoCSLL").prop("disabled", false);
    $("#txtAliquotaCSLL").prop("disabled", false);
    $("#txtValorCSLL").prop("disabled", false);

    $("#txtIBSEstadualAliquota").prop("disabled", false);
    $("#txtIBSEstadualReducao").prop("disabled", false);
    $("#txtIBSEstadualEfetiva").prop("disabled", false);
    $("#txtIBSEstadualValor").prop("disabled", false);

    $("#txtIBSMunAliquota").prop("disabled", false);
    $("#txtIBSMunReducao").prop("disabled", false);
    $("#txtIBSMunEfetiva").prop("disabled", false);
    $("#txtIBSMunValor").prop("disabled", false);

    $("#txtCBSAliquota").prop("disabled", false);
    $("#txtCBSReducao").prop("disabled", false);
    $("#txtCBSEfetiva").prop("disabled", false);
    $("#txtCBSValor").prop("disabled", false);

    $("#txtRazaoSocialRemetente").prop("disabled", false);
    $("#txtRGIERemetente").prop("disabled", false);
    $("#txtRazaoSocialDestinatario").prop("disabled", false);
    $("#txtRGIEDestinatario").prop("disabled", false);
    $("#txtRGIETomador").prop("disabled", false);
    $("#txtRGIEExpedidor").prop("disabled", false);
    $("#txtRGIERecebedor").prop("disabled", false);
    $("#selIndIEToma").prop("disabled", false);
    $("#txtValorTotalPrestacaoServico").prop("disabled", false);
    $("#txtValorAReceber").prop("disabled", false);
    $("#selCFOP").prop("disabled", false);
    $("#txtDescricaoComponentePrestacaoServico").prop("disabled", false);
    $("#txtCaracteristicaAdicionalTransporte").prop("disabled", false);

    $("#tabsEmissaoCTe a[href='#tabsResumo']").hide();
    $("#tabsEmissaoCTe a[href='#tabDados']").tab("show");
}

function BloquearCamposCTeAnulacao() {
    BloquearCamposCTeOutros();
    $("#txtHoraEmissao").prop("disabled", false);
    $("#txtDataEmissaoDeclaracaoCTeAnulado").prop("disabled", false);
    $("#selCFOP").prop("disabled", false);
    $("#ddlNaturezaOperacao").prop("disabled", false);

    $("#txtRazaoSocialRemetente").prop("disabled", false);
    $("#txtRazaoSocialDestinatario").prop("disabled", false);
    $("#ddlSerie").prop("disabled", false);

    $("#tabsEmissaoCTe a[href='#tabsResumo']").hide();
    $("#tabsEmissaoCTe a[href='#tabDados']").tab("show");
}

function BloquearCamposCTeSubstituicao() {
    BloquearCamposCTeOutros();
    $("#txtHoraEmissao").prop("disabled", false);
    $("#selTomadorContribuinteICMS").prop("disabled", false);
    $("#txtChaveAcessoCTeAnulacao").prop("disabled", false);
    $("#selTipoDocumentoTomadorContribuinte").prop("disabled", false);
    $("#txtChaveAcessoCTeEmitidoTomador").prop("disabled", false);
    $("#txtChaveAcessoNFeEmitidaTomador").prop("disabled", false);
    $("#txtCNPJNFouCTEmitidoTomador").prop("disabled", false);
    $("#ddlModeloNFouCTEmitidoTomador").prop("disabled", false);
    $("#txtSerieNFouCTEmitidoTomador").prop("disabled", false);
    $("#txtSubserieNFouCTEmitidoTomador").prop("disabled", false);
    $("#txtNumeroNFouCTEmitidoTomador").prop("disabled", false);
    $("#txtValorNFouCTEmitidoTomador").prop("disabled", false);
    $("#txtDataEmissaoNFouCTEmitidoTomador").prop("disabled", false);
    $("#btnBuscarChaveAcessoCTeAnulacao").prop("disabled", false);
    $("#btnBuscarEmitenteDocumentoSubstituicao").prop("disabled", false);
    $("#txtValorFreteContratado").prop("disabled", false);

    $("#selICMS").prop("disabled", false);
    $("#txtReducaoBaseCalculoICMS").prop("disabled", false);
    $("#txtValorBaseCalculoICMS").prop("disabled", false);
    $("#selAliquotaICMS").prop("disabled", false);
    $("#txtValorICMS").prop("disabled", false);
    $("#txtValorICMSDesoneracao").prop("disabled", false);
    $("#txtCodigoBeneficio").prop("disabled", false);
    $("#txtValorCreditoICMS").prop("disabled", false);
    $("#chkExibirICMSNaDACTE").prop("disabled", false);

    $("#txtValorTotalPrestacaoServico").prop("disabled", false);
    $("#txtValorAReceber").prop("disabled", false);
    $("#chkIncluirICMSNoFrete").prop("disabled", false);
    $("#txtDescricaoComponentePrestacaoServico").prop("disabled", false);
    $("#txtValorComponentePrestacaoServico").prop("disabled", false);
    $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("disabled", false);
    $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("disabled", false);
    $("#btnSalvarComponentePrestacaoServico").prop("disabled", false);
    $("#btnExcluirComponentePrestacaoServico").prop("disabled", false);
    $("#btnCancelarComponentePrestacaoServico").prop("disabled", false);

    $("#txtRazaoSocialRemetente").prop("disabled", false);
    $("#txtRazaoSocialDestinatario").prop("disabled", false);
    $("#ddlSerie").prop("disabled", false);
    $("#selSubTomador").prop("disabled", false);
    $("#selTomadorServico").prop("disabled", false);
    DesbloquearCamposTomadorExportacao();

    $("#tabsEmissaoCTe a[href='#tabsResumo']").hide();
    $("#tabsEmissaoCTe a[href='#tabDados']").tab("show");
}

function BloquearAbasCTeOutros() {
    $("#selSubTomador").prop("disabled", true);
    switch ($("#selTipoCTE").val()) {
        case "1":
            $("#tabsEmissaoCTe a[href='#tabCTeOutros']").show();
            $("#tabsCTeOutros a[href='#tabCTeComlementar']").show();
            $("#tabsCTeOutros a[href='#tabCTeAnulacao']").hide();
            $("#tabsCTeOutros a[href='#tabCTeSubstituicao']").hide();

            $("#tabsCTeOutros a[href='#tabCTeComlementar']").tab("show");
            break;
        case "2":
            $("#tabsEmissaoCTe a[href='#tabCTeOutros']").show();
            $("#tabsCTeOutros a[href='#tabCTeComlementar']").hide();
            $("#tabsCTeOutros a[href='#tabCTeAnulacao']").show();
            $("#tabsCTeOutros a[href='#tabCTeSubstituicao']").hide();

            $("#tabsCTeOutros a[href='#tabCTeAnulacao']").tab("show");
            break;
        case "3":
            $("#tabsEmissaoCTe a[href='#tabCTeOutros']").show();
            $("#tabsCTeOutros a[href='#tabCTeComlementar']").hide();
            $("#tabsCTeOutros a[href='#tabCTeAnulacao']").hide();
            $("#tabsCTeOutros a[href='#tabCTeSubstituicao']").show();

            $("#tabsCTeOutros a[href='#tabCTeSubstituicao']").tab("show");
            break;
        default:
            $("#tabsEmissaoCTe a[href='#tabCTeOutros']").hide();
            $("#tabsCTeOutros a[href='#tabCTeComlementar']").hide();
            $("#tabsCTeOutros a[href='#tabCTeAnulacao']").hide();
            $("#tabsCTeOutros a[href='#tabCTeSubstituicao']").hide();
            break;
    }
}

function BloquearCamposCTeOutros() {
    $("#divEmissaoCTe .modal-body :input, #divEmissaoCTe .modal-body :button").prop("disabled", true);
    $("#btnSalvarCTe").prop("disabled", false);
    $("#btnEmitirCTe").prop("disabled", false);
    $("#btnCancelarCTe").prop("disabled", false);
    $("#txtObservacaoGeral").prop("disabled", false);
    $("#btnBuscarObservacaoGeral").prop("disabled", false);
    $("#txtIdentificadorObservacaoContribuinte").prop("disabled", false);
    $("#txtDescricaoObservacaoContribuinte").prop("disabled", false);
    $("#btnBuscarObservacaoContribuinte").prop("disabled", false);
    $("#btnSalvarObservacaoContribuinte").prop("disabled", false);
    $("#btnExcluirObservacaoContribuinte").prop("disabled", false);
    $("#btnCancelarObservacaoContribuinte").prop("disabled", false);
    $("#txtIdentificadorObservacaoFisco").prop("disabled", false);
    $("#txtDescricaoObservacaoFisco").prop("disabled", false);
    $("#btnSalvarObservacaoFisco").prop("disabled", false);
    $("#btnExcluirObservacaoFisco").prop("disabled", false);
    $("#btnCancelarObservacaoFisco").prop("disabled", false);
}

function EmitirCTeComplementar(cte) {
    executarRest("/ConhecimentoDeTransporteEletronico/ObterDetalhes?callback=?", { CodigoCTe: cte.data.Codigo }, function (r) {
        if (r.Sucesso) {
            $.ajaxSetup({ async: false });
            if (!r.Objeto.PossuiCTeAnulacao) {
                if (!r.Objeto.PossuiCTeSubstituicao) {
                    //if (r.Objeto.TipoCTE == 0) {
                    if (r.Objeto.Status == "A") {
                        var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
                        var dataEmissao = Globalize.parseDate(r.Objeto.DataEmissao, "dd/MM/yyyy");
                        var hoje = new Date();
                        dataEmissao.setDate(configuracaoEmpresa != null && configuracaoEmpresa.DiasParaEmissaoDeCTeComplementar > 0 ? dataEmissao.getDate() + configuracaoEmpresa.DiasParaEmissaoDeCTeComplementar : dataEmissao.getDate());
                        if (dataEmissao >= hoje) {

                            // Seta como tipo do cte complementar
                            $("body").data("ctecomplementar", "complementar");

                            // Mas limpa quando fechar a div
                            $("#divEmissaoCTe").one('hide.bs.modal', function () {
                                $("body").data("ctecomplementar", "");
                            });
                            PreencherCTe(r.Objeto, 1, true, true);

                            BloquearCamposCTeComplementar();
                        } else {
                            jAlert("Não é possível emitir um CT-e Complementar pois a data limite (" + (configuracaoEmpresa != null && configuracaoEmpresa.DiasParaEmissaoDeCTeComplementar > 0 ? configuracaoEmpresa.DiasParaEmissaoDeCTeComplementar : 1) + " dia(s)) foi ultrapassada.", "Atenção");
                        }
                    } else {
                        jAlert("O status do CT-e é inválido para a emissão de um CT-e Complementar.", "Atenção");
                    }
                    //} else {
                    //    jAlert("Não é possível emitir um CT-e Complementar para um CT-e com esta finalidade.", "Atenção");
                    //}
                } else {
                    jAlert("Não é possível emitir um CT-e Complementar para este CT-e pois ele já foi substituído.", "Atenção");
                }
            } else {
                jAlert("Não é possível emitir um CT-e Complementar para este CT-e pois ele já foi anulado.", "Atenção");
            }
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function EmitirCTeAnulacao(cte) {
    executarRest("/ConhecimentoDeTransporteEletronico/ObterDetalhes?callback=?", { CodigoCTe: cte.data.Codigo }, function (r) {
        if (r.Sucesso) {
            if (!r.Objeto.PossuiCTeAnulacao) {
                if (!r.Objeto.PossuiCTeSubstituicao) {
                    //if (r.Objeto.TipoCTE == 0) {
                    if (r.Objeto.Status == "A" || r.Objeto.Status == "Z") {
                        var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
                        var dataEmissao = Globalize.parseDate(r.Objeto.DataEmissao, "dd/MM/yyyy");
                        var hoje = new Date();
                        dataEmissao.setDate(configuracaoEmpresa != null && configuracaoEmpresa.DiasParaEmissaoDeCTeAnulacao > 0 ? dataEmissao.getDate() + configuracaoEmpresa.DiasParaEmissaoDeCTeAnulacao : dataEmissao.getDate());
                        if (dataEmissao >= hoje) {
                            PreencherCTe(r.Objeto, 2, true);
                            BloquearCamposCTeAnulacao();
                        } else {
                            jAlert("Não é possível emitir um CT-e de Anulação pois a data limite (" + (configuracaoEmpresa != null && configuracaoEmpresa.DiasParaEmissaoDeCTeAnulacao > 0 ? configuracaoEmpresa.DiasParaEmissaoDeCTeAnulacao : 1) + " dia(s)) foi ultrapassada.", "Atenção");
                        }
                    } else {
                        jAlert("O status do CT-e é inválido para a emissão de um CT-e de Anulação.", "Atenção");
                    }
                    //} else {
                    //    jAlert("Não é possível emitir um CT-e de Anulação para um CT-e com esta finalidade.", "Atenção");
                    //}
                } else {
                    jAlert("Não é possível emitir um CT-e de Anulação para este CT-e pois ele já foi substituído.", "Atenção");
                }
            } else {
                jAlert("Não é possível emitir um CT-e de Anulação para este CT-e pois ele já foi anulado.", "Atenção");
            }
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function EmitirCTeSubstituicao(cte) {
    executarRest("/ConhecimentoDeTransporteEletronico/ObterDetalhes?callback=?", { CodigoCTe: cte.data.Codigo }, function (r) {
        if (r.Sucesso) {
            if (!r.Objeto.PossuiCTeSubstituicao) {
                //if (r.Objeto.TipoCTE == 0) {
                if (r.Objeto.Status == "A" || r.Objeto.Status == "Z") {
                    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
                    var dataEmissao = Globalize.parseDate(r.Objeto.DataEmissao, "dd/MM/yyyy");
                    var hoje = new Date();
                    dataEmissao.setDate(configuracaoEmpresa != null && configuracaoEmpresa.DiasParaEmissaoDeCTeSubstituicao > 0 ? dataEmissao.getDate() + configuracaoEmpresa.DiasParaEmissaoDeCTeSubstituicao : dataEmissao.getDate());
                    if (dataEmissao >= hoje) {
                        PreencherCTe(r.Objeto, 3, true);
                        BloquearCamposCTeSubstituicao();
                    } else {
                        jAlert("Não é possível emitir um CT-e de Substituição pois a data limite (" + (configuracaoEmpresa != null && configuracaoEmpresa.DiasParaEmissaoDeCTeSubstituicao > 0 ? configuracaoEmpresa.DiasParaEmissaoDeCTeSubstituicao : 1) + " dia(s)) foi ultrapassada.", "Atenção");
                    }
                } else {
                    jAlert("O status do CT-e é inválido para a emissão de um CT-e de Substituição.", "Atenção");
                }
                //} else {
                //    jAlert("Não é possível emitir um CT-e de Substituição para um CT-e com esta finalidade.", "Atenção");
                //}
            } else {
                jAlert("Não é possível emitir um CT-e de Substituição para este CT-e pois ele já foi substituído.", "Atenção");
            }
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function DownloadDacte(cte) {
    executarDownload("/ConhecimentoDeTransporteEletronico/DownloadDacte", { CodigoCTe: cte.data.Codigo });
}

function DownloadXML(cte) {
    executarDownload("/ConhecimentoDeTransporteEletronico/DownloadXML", { CodigoCTe: cte.data.Codigo });
}

function DownloadXMLCancelamento(cte) {
    executarDownload("/ConhecimentoDeTransporteEletronico/DownloadXMLCancelamento", { CodigoCTe: cte.data.Codigo });
}

function DownloadPreCTe(cte) {
    executarDownload("/ConhecimentoDeTransporteEletronico/DownloadPreCTe", { CodigoCTe: cte.data.Codigo });
}

function EditarCTe(codigo) {
    var cte = {};

    if (codigo != null && typeof (codigo) == "number" && codigo > 0)
        cte.Codigo = codigo;
    else
        cte = codigo.data;

    executarRest("/ConhecimentoDeTransporteEletronico/ObterDetalhes?callback=?", { CodigoCTe: cte.Codigo }, function (r) {
        if (r.Sucesso) {
            $.ajaxSetup({ async: false });

            var versaoCte = r.Objeto.Versao != null ? r.Objeto.Versao : "2.00";
            ClassesVersao({ VersaoCTe: versaoCte });

            $("body").data("editandoCTe", true);
            PreencherCTe(r.Objeto);
            $("body").data("editandoCTe", false);

            $.ajaxSetup({ async: true });

            ControlarCamposCTeOS((r.Objeto.Status == "S" || r.Objeto.Status == "R"));
            if (r.Objeto.Status == "S" || r.Objeto.Status == "R") {
                var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());

                if (janelaDicas != null)
                    janelaDicas.close();
                AbrirDicaParaEmissaoDeCTe(true, configuracaoEmpresa);
                $("#divEmissaoCTe").off("shown.bs.modal");
            }

        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function ValidarAliquotaICMSCTeComplementar(aliquota) {
    $("#selAliquotaICMS option").each(function () {
        var aliq = Globalize.parseFloat($(this).val());
        //if (aliquota > aliq)
        //    $(this).prop("disabled", true);
    });
}

function PreencherCTe(cte, tipo, fromCancelamento, fromDuplicado) {
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    var hoje = new Date();
    var fromDuplicado = (fromDuplicado != null && fromDuplicado == true);

    // Fix #5330 
    $("body").data("cteDuplicado", fromDuplicado);
    LimparDadosCTe();
    $("body").data("cteDuplicado", fromDuplicado);
    // End Fix #5330

    $("#ddlSerie").val(cte.Serie);
    $("#selPago_APagar").val(cte.TipoPagamento);
    $("#selTipoServico").val(cte.TipoServico);
    $("#selFormaImpressao").val(cte.TipoImpressao);
    //if (tipo != null && tipo == 1)
    //    $("#selGlobalizado").val("0");
    //else
    $("#selGlobalizado").val(cte.IndicadorGlobalizado);
    $("#selIndIEToma").val(cte.IndicadorIETomador);
    $("#selSubTomador").val(cte.SubstituicaoTomador);


    $("#ddlNaturezaOperacao").val(cte.NaturezaDaOperacao);
    BuscarCFOPs(cte.CFOP);

    if (cte.Remetente != null) {
        if (!cte.Remetente.Exterior)
            BuscarRemetente(false, null, cte.Remetente, fromDuplicado, false);
        else
            PreencherCamposRemetenteExportacao(cte.Remetente);

        //if ($("#txtRazaoSocialRemetente").val() == "DIVERSOS")
        //    $("#txtRazaoSocialRemetente").val("DIVERSOS COMPLEMENTAR");
    }

    if (cte.Expedidor != null)
        if (!cte.Expedidor.Exterior)
            BuscarExpedidor(false, null, cte.Expedidor, false);
        else
            PreencherCamposExpedidorExportacao(cte.Expedidor);

    if (cte.Recebedor != null)
        if (!cte.Recebedor.Exterior)
            BuscarRecebedor(false, null, cte.Recebedor, false);
        else
            PreencherCamposRecebedorExportacao(cte.Recebedor);

    if (cte.Destinatario != null) {
        if (!cte.Destinatario.Exterior)
            BuscarDestinatario(false, null, cte.Destinatario, fromDuplicado, false);
        else
            PreencherCamposDestinatarioExportacao(cte.Destinatario);

        //if ($("#txtRazaoSocialDestinatario").val() == "DIVERSOS")
        //    $("#txtRazaoSocialDestinatario").val("DIVERSOS COMPLEMENTAR");
    }

    if (cte.Tomador != null)
        if (!cte.Tomador.Exterior)
            BuscarTomador(false, null, cte.Tomador, false);
        else
            PreencherCamposTomadorExportacao(cte.Tomador);

    BuscarLocalEntregaDiferenteDestinatario(cte.CPF_CNPJ_Cliente_Entrega);

    if (cte.Retira == 1) {
        $("#chkRecebedorRetiraDestino").prop('checked', true);
    } else {
        $("#chkRecebedorRetiraDestino").prop("checked", false);
    }

    $("#txtDetalhesRetiradaRecebedor").val(cte.DetalhesRetira);
    $("#ddlModalTransporte").val(cte.ModalTransporte);
    $("#selTomadorServico").val(cte.TipoTomador);
    $("#selTomadorServicoResumo").val(cte.TipoTomador);
    $("#txtValorFreteContratado").val(Globalize.format(cte.ValorFrete, "n2"));
    $("#txtValorTotalPrestacaoServico").val(Globalize.format(cte.ValorPrestacaoServico, "n2"));
    $("#txtValorAReceber").val(Globalize.format(cte.ValorAReceber, "n2"));

    if (cte.IncluirICMSNoFrete == 1) {
        $("#chkIncluirICMSNoFrete").prop("checked", true);
        $("#chkIncluirICMSNoFreteResumo").prop("checked", true);
        $("#divPercentualICMSRecolhido").show();
        $("#txtPercentualICMSRecolhido").val(Globalize.format(cte.PercentualICMSIncluirNoFrete, "n2"));
    } else {
        $("#chkIncluirICMSNoFrete").prop("checked", false);
        $("#chkIncluirICMSNoFreteResumo").prop("checked", false);
        $("#divPercentualICMSRecolhido").hide();
        $("#txtPercentualICMSRecolhido").val("100,00");
    }

    $("#selICMS").val(cte.CST.toString());
    $("#selICMSResumo").val(cte.CST.toString());
    TrocarICMS(cte.CST.toString(), false);

    $("#txtReducaoBaseCalculoICMS").val(Globalize.format(cte.PercentualReducaoBaseCalculoICMS, "n2"));
    $("#txtValorBaseCalculoICMS").val(Globalize.format(cte.BaseCalculoICMS, "n2"));
    $("#selAliquotaICMS").val(Globalize.format(cte.AliquotaICMS, "n2"));
    $("#selAliquotaICMSResumo").val(Globalize.format(cte.AliquotaICMS, "n2"));
    $("#txtValorICMS").val(Globalize.format(cte.ValorICMS, "n2"));
    $("#txtValorICMSDesoneracao").val(Globalize.format(cte.ValorICMSDesoneracao, "n2"));
    $("#txtCodigoBeneficio").val(cte.CodigoBeneficio);
    $("#txtValorCreditoICMS").val(Globalize.format(cte.ValorPresumido, "n2"));
    $("#chkExibirICMSNaDACTE").prop("checked", cte.ExibeICMSNaDACTE);
    $("#selPIS").val(cte.CSTPIS);
    $("#txtValorBaseCalculoPIS").val(Globalize.format(cte.BasePIS, "n2"));
    $("#selAliquotaPIS").val(cte.AliquotaPIS <= 0 ? $("#selAliquotaPIS option:first").val() : Globalize.format(cte.AliquotaPIS, "n2"));
    $("#txtValorPIS").val(Globalize.format(cte.ValorPIS, "n2"));
    $("#selCOFINS").val(cte.CSTCOFINS);
    $("#txtValorBaseCalculoCOFINS").val(Globalize.format(cte.BaseCOFINS, "n2"));
    $("#selAliquotaCOFINS").val(cte.AliquotaCOFINS <= 0 ? $("#selAliquotaCOFINS option:first").val() : Globalize.format(cte.AliquotaCOFINS, "n2"));
    $("#txtValorCOFINS").val(Globalize.format(cte.ValorCOFINS, "n2"));
    $("#txtProdutoPredominante").val(cte.ProdutoPredominante);
    $("#txtOutrasCaracteristicasCarga").val(cte.OutrasCaracteristicasDaCarga);
    $("#txtConteiner").val(cte.Container);
    $("#txtDataPrevistaEntregaConteiner").val(cte.DataPrevistaContainer);
    $("#txtNumeroLacre").val(cte.LacreContainer);
    //if (fromDuplicado != null && fromDuplicado == true)
    $("#txtRNTRC").val(cte.RNTRC);
    $("#txtDataPrevistaEntregaCargaRecebedor").val(cte.DataPrevistaEntrega);
    $("#txtCaracteristicaAdicionalTransporte").val(cte.CaracteristicaTransporte);
    $("#txtDescricaoComponentePrestacaoServico").val(cte.DescricaoComplemento);

    $("#txtCaracteristicaAdicionalServico").val(cte.CaracteristicaServico);

    if (cte.Lotacao == 1) {
        $("#chkIndicadorLotacao").prop("checked", true);
    } else {
        $("#chkIndicadorLotacao").prop("checked", false);
    }

    $("#txtSerieCTRB").val("");
    $("#txtNumeroCTRB").val("");
    $("#txtCIOT").val(cte.CIOT);
    $("#txtObservacaoGeral").val(cte.ObservacoesGerais);
    $("#txtInformacaoAdicionalFisco").val(cte.InformacaoAdicionalFisco);
    $("#ddlModelo").val(cte.ModeloDocumentoFiscal);
    $("#txtValorTotalCarga").val(Globalize.format(cte.ValorTotalMercadoria, "n2"));
    $("#txtValorCargaAverbacao").val(Globalize.format(cte.ValorCargaAverbacao, "n2"));

    $("#selTipoFretamento").val(cte.TipoFretamento);
    if (!fromDuplicado && !fromCancelamento)
        $("#txtDataHoraViagem").val(cte.DataHoraViagem);
    else
        $("#txtDataHoraViagem").val(Globalize.format(hoje, "dd/MM/yyyy") + " " + Globalize.format(hoje, "HH:mm"));

    $("#txtValorBaseCalculoINSS").val(Globalize.format(cte.ValorBaseCalculoINSS, "n2"));
    $("#txtAliquotaINSS").val(Globalize.format(cte.AliquotaINSS, "n2"));
    $("#txtValorINSS").val(Globalize.format(cte.ValorINSS, "n2"));

    $("#txtValorBaseCalculoIR").val(Globalize.format(cte.ValorBaseCalculoIR, "n2"));
    $("#txtAliquotaIR").val(Globalize.format(cte.AliquotaIR, "n2"));
    $("#txtValorIR").val(Globalize.format(cte.ValorIR, "n2"));

    $("#txtValorBaseCalculoCSLL").val(Globalize.format(cte.ValorBaseCalculoCSLL, "n2"));
    $("#txtAliquotaCSLL").val(Globalize.format(cte.AliquotaCSLL, "n2"));
    $("#txtValorCSLL").val(Globalize.format(cte.ValorCSLL, "n2"));

    codigoOutrasAliquotas = cte.CodigoOutrasAliquotas;
    $("#txtIBSCBS_CST").val(cte.CSTIBSCBS);
    $("#txtIBSCBS_Class").val(cte.ClassificacaoTributariaIBSCBS);
    $("#txtIBSCBSBaseCalculo").val(Globalize.format(cte.BaseCalculoIBSCBS, "n2"));

    $("#txtIBSEstadualAliquota").val(Globalize.format(cte.AliquotaIBSEstadual, "n4"));
    $("#txtIBSEstadualReducao").val(Globalize.format(cte.PercentualReducaoIBSEstadual, "n4"));
    $("#txtIBSEstadualEfetiva").val(Globalize.format(cte.AliquotaIBSEstadualEfetiva, "n4"));
    $("#txtIBSEstadualValor").val(Globalize.format(cte.ValorIBSEstadual, "n2"));

    $("#txtIBSMunAliquota").val(Globalize.format(cte.AliquotaIBSMunicipal, "n4"));
    $("#txtIBSMunReducao").val(Globalize.format(cte.PercentualReducaoIBSMunicipal, "n4"));
    $("#txtIBSMunEfetiva").val(Globalize.format(cte.AliquotaIBSMunicipalEfetiva, "n4"));
    $("#txtIBSMunValor").val(Globalize.format(cte.ValorIBSMunicipal, "n2"));

    $("#txtCBSAliquota").val(Globalize.format(cte.AliquotaCBS, "n4"));
    $("#txtCBSReducao").val(Globalize.format(cte.PercentualReducaoCBS, "n4"));
    $("#txtCBSEfetiva").val(Globalize.format(cte.AliquotaCBSEfetiva, "n4"));
    $("#txtCBSValor").val(Globalize.format(cte.ValorCBS, "n2"));

    $("#hddCodigoCTEReferenciado").val(cte.CTeReferencia);

    if (tipo != null) {

        $("#txtDataEmissao").val(Globalize.format(hoje, "dd/MM/yyyy"));
        $("#txtHoraEmissao").val(Globalize.format(hoje, "HH:mm"));
        $("#hddChaveCTEOriginal").val(cte.Chave);
        $("#hddCodigoCTE").val('0');
        $("#txtNumero").val("Automático");
        $("#selTipoCTE").val(tipo);

        if (tipo == 1) { //Complementar
            $("#txtChaveCTeComplementado").val(cte.Chave);
            ValidarAliquotaICMSCTeComplementar(cte.AliquotaICMS);
            $("#txtValorTotalCarga").val(Globalize.format(0, "n2"));
            $("#txtValorCargaAverbacao").val(Globalize.format(0, "n2"));
            $("#txtValorFreteContratado").val(Globalize.format(0, "n2"));
            $("#txtValorTotalPrestacaoServico").val(Globalize.format(0, "n2"));
            $("#txtValorAReceber").val(Globalize.format(0, "n2"));
            $("#txtValorBaseCalculoICMS").val(Globalize.format(0, "n2"));
            $("#txtValorICMS").val(Globalize.format(0, "n2"));
            $("#txtValorICMSDesoneracao").val(Globalize.format(0, "n2"));
            $("#txtValorCreditoICMS").val(Globalize.format(0, "n2"));
        } else if (tipo == 2) { //Anulação
            $("#txtChaveCTeAnulado").val(cte.Chave);

            var obsGeral = configuracaoEmpresa != null ? configuracaoEmpresa.ObservacaoCTeAnulacao : "";
            obsGeral = typeof obsGeral == "string" ? obsGeral : "";

            obsGeral = obsGeral.replace(/#NumeroCTe#/g, cte.Numero);
            obsGeral = obsGeral.replace(/#ChaveCTe#/g, cte.Chave);
            obsGeral = obsGeral.replace(/#DataEmissaoCTe#/g, cte.DataEmissao);
            $("#txtObservacaoGeral").val(obsGeral);
        } else if (tipo == 3) { //Substituição
            $("#txtChaveCTeSubstituido").val(cte.Chave);
            $("#txtChaveAcessoCTeAnulacao").val(cte.ChaveCTeAnulacao);
            $("#hddValorFreteContratadoOriginal").val(cte.ValorFrete);

            var obsGeral = configuracaoEmpresa != null ? configuracaoEmpresa.ObservacaoCTeSubstituicao : "";
            obsGeral = typeof obsGeral == "string" ? obsGeral : "";

            obsGeral = obsGeral.replace(/#NumeroCTe#/g, cte.Numero);
            obsGeral = obsGeral.replace(/#ChaveCTe#/g, cte.Chave);
            obsGeral = obsGeral.replace(/#DataEmissaoCTe#/g, cte.DataEmissao);
            $("#txtObservacaoGeral").val(obsGeral);
        } else {
            CarregarComponentesDaPrestacao(cte.Codigo);
        }
    } else {
        if (cte.TipoCTE == 1) { //Complementar
            $("#txtChaveCTeComplementado").val(cte.ChaveCTESubComp);
            ValidarAliquotaICMSCTeComplementar(cte.AliquotaICMSCTeSubComp);
            BloquearCamposCTeComplementar();
        } else if (cte.TipoCTE == 2) { //Anulação
            $("#txtDataEmissaoDeclaracaoCTeAnulado").val(cte.DataAnulacao);
            $("#txtChaveCTeAnulado").val(cte.ChaveCTESubComp);
            BloquearCamposCTeAnulacao();
        } else if (cte.TipoCTE == 3) {
            PreencherCamposCTeSubstituicao(cte);
            BloquearCamposCTeSubstituicao();
        } else {
            DesbloquearCamposCTe();
        }

        if (!fromCancelamento) {
            $("#hddCodigoCTE").val(cte.Codigo);
            $("#txtNumero").val(cte.Numero);
            $("#txtDataEmissao").val(cte.DataEmissao);
            $("#txtHoraEmissao").val(cte.HoraEmissao);
            $("#txtNumeroProtocoloCancelamento").val(cte.ProtocoloCancelamentoInutilizacao);
            $("#txtJustificativaCancelamento").val(cte.ObservacaoCancelamento);
            $("#txtNumeroProtocoloAutorizacao").val(cte.Protocolo);
            $("#txtChaveCTe").val(cte.Chave);
            $("#txtObservacaoDigitacaoCTe").val(cte.ObservacoesDigitacao);
            $("#lblIdCTe").text("Cod." + cte.Codigo);
        } else {
            var dataEntrega = new Date();
            dataEntrega.setDate(configuracaoEmpresa != null && configuracaoEmpresa.DiasParaEntrega > 0 ? hoje.getDate() + configuracaoEmpresa.DiasParaEntrega : hoje.getDate() + 1);
            $("#txtDataPrevistaEntregaCargaRecebedor").val(Globalize.format(dataEntrega, "dd/MM/yyyy"));
            $("#txtDataEmissao").val(Globalize.format(hoje, "dd/MM/yyyy"));
            $("#txtHoraEmissao").val(Globalize.format(hoje, "HH:mm"));
            $("#txtDataEmissaoNFeRemetente").val(Globalize.format(hoje, "dd/MM/yyyy"));
            $("#txtDataEmissaoNotaFiscalRemetente").val(Globalize.format(hoje, "dd/MM/yyyy"));
            $("#txtDataEmissaoOutrosRemetente").val(Globalize.format(hoje, "dd/MM/yyyy"));
            $("#lblIdCTe").text("");
        }

        $("#selTipoCTE").val(cte.TipoCTE);
        $("#lblLogCTe").text(cte.Log);

        CarregarObservacoesContribuinte(cte.Codigo, fromCancelamento);
        CarregarObservacoesFisco(cte.Codigo, fromCancelamento);
        CarregarComponentesDaPrestacao(cte.Codigo, fromCancelamento);

        $("#btnEmitirCTe").prop("disabled", false);
        $("#btnSalvarCTe").prop("disabled", false);

        if (cte.Status != "S" && cte.Status != "R" && !fromCancelamento) {
            BloquearCamposCTe();
            $("#tabsEmissaoCTe a[href='#tabsResumo']").hide();
        }
        else {
            if ((fromDuplicado == null || fromDuplicado == false) && cte.Status == "R" && cte.CodigoRetornoSefaz == 105) {
                BloquearCamposCTe();
                $("#btnEmitirCTe").prop("disabled", false);
                $("#btnSalvarCTe").prop("disabled", true);
            }
            else if ((fromDuplicado == null || fromDuplicado == false) && cte.Status == "R" && cte.CodigoRetornoSefaz == 8888) {
                BloquearCamposCTe();
                $("#btnEmitirCTe").prop("disabled", false);
                $("#btnSalvarCTe").prop("disabled", true);
            }
            else if ((fromDuplicado == null || fromDuplicado == false) && cte.Status == "R" && cte.CodigoRetornoSefaz == 678) {
                BloquearCamposCTe();
                $("#btnEmitirCTe").prop("disabled", false);
                $("#btnSalvarCTe").prop("disabled", true);
            }
            else if ((fromDuplicado == null || fromDuplicado == false) && cte.Status == "R" && cte.CodigoRetornoSefaz == 223) {
                BloquearCamposCTe();
                $("#btnEmitirCTe").prop("disabled", false);
                $("#btnSalvarCTe").prop("disabled", true);
            }
            else if (cte.CTeReferencia > 0) { //(fromDuplicado == null || fromDuplicado == false) && 
                ControlarCamposResumo(cte.Status == "S" || cte.Status == "R" || fromCancelamento || fromDuplicado);
            }
        }

        if (cte.Status == "I" || cte.Status == "C")
            $("#tabsEmissaoCTe a[href='#tabsCancelamento']").show();
        else
            $("#tabsEmissaoCTe a[href='#tabsCancelamento']").hide();
    }

    $("#ddlUFLocalEmissaoCTe").val(cte.UFEmissao);
    BuscarLocalidades(cte.UFEmissao, "ddlMunicipioLocalEmissaoCTe", cte.LocalidadeEmissao);

    $("#ddlUFInicioPrestacao").val(cte.UFInicioPrestacao);
    BuscarLocalidades(cte.UFInicioPrestacao, "ddlMunicipioInicioPrestacao", cte.LocalidadeInicioPrestacao);

    $("#ddlUFTerminoPrestacao").val(cte.UFTerminoPrestacao);
    BuscarLocalidades(cte.UFTerminoPrestacao, "ddlMunicipioTerminoPrestacao", cte.LocalidadeTerminoPrestacao);

    CarregarPercursos(cte);
    CarregarDocumentos(cte, tipo, fromCancelamento);
    CarregarInformacoesDaCarga(cte.Codigo, tipo, fromCancelamento);
    CarregarInformacoesSeguro(cte.Codigo, fromCancelamento);
    CarregarVeiculos(cte.Codigo, fromCancelamento);
    CarregarMotoristas(cte.Codigo, fromCancelamento);
    CarregarDocumentosDeTransporteAnterioresEletronicos(cte.Codigo, fromCancelamento);
    CarregarDocumentosDeTransporteAnterioresPapel(cte.Codigo, fromCancelamento);
    CarregarProdutosPerigosos(cte.Codigo, fromCancelamento);
    CarregarDadosDeCobranca(cte.Codigo, fromCancelamento);

    CarregarConsultaApoliceSeguro(cte);

    CopiarValoresParaResumo();

    AbrirTelaEmissaoCTe(!(cte.Status != "S" && cte.Status != "R" && !fromCancelamento));

    if ((fromDuplicado == null || fromDuplicado == false) && cte.Status == "R" && cte.CodigoRetornoSefaz == 105) {
        jAlert("CT-e está em processamento no Sefaz, somente é possível emitir novamente sem alterar os dados.", "Atenção");
    }
    else if ((fromDuplicado == null || fromDuplicado == false) && cte.Status == "R" && cte.CodigoRetornoSefaz == 8888) {
        jAlert("CT-e foi enviado para o Sefaz porém o mesmo está oscilando, somente é possível emitir novamente sem alterar os dados.", "Atenção");
    }
    else if ((fromDuplicado == null || fromDuplicado == false) && cte.Status == "R" && cte.CodigoRetornoSefaz == 678) {
        jAlert("CT-e foi enviado para o Sefaz porém o mesmo está oscilando, somente é possível emitir novamente sem alterar os dados.", "Atenção");
    }
}

function CarregarConsultaApoliceSeguro(cte) {
    $("#btnBuscarApoliceSeguro").off();
    var cpfCnpjTomador = "";
    switch (cte.TipoTomador.toString()) {
        case "0":
            cpfCnpjTomador = cte.CPF_CNPJ_Remetente;
            break;
        case "1":
            cpfCnpjTomador = cte.CPF_CNPJ_Expedidor;
            break;
        case "2":
            cpfCnpjTomador = cte.CPF_CNPJ_Recebedor;
            break;
        case "3":
            cpfCnpjTomador = cte.CPF_CNPJ_Destinatario;
            break;
        case "4":
            cpfCnpjTomador = cte.CPF_CNPJ_Tomador;
            break;
        default:
            cpfCnpjTomador = "";
            break;
    }
    CarregarConsultaDeApolicesDeSegurosPorCliente("btnBuscarApoliceSeguro", "btnBuscarApoliceSeguro", cpfCnpjTomador, RetornoConsultaApoliceSeguro, true, false);
}

function PreencherCamposCTeSubstituicao(cte) {
    $("#txtChaveCTeSubstituido").val(cte.ChaveCTESubComp);
    $("#hddValorFreteContratadoOriginal").val(cte.ValorFreteOriginal);
    if (cte.DocumentoAnulacao.ContribuinteICMS == 1) {
        $("#selTomadorContribuinteICMS").val("1");
        $("#divTomadorContribuinte").show();
        $("#divTomadorNaoContribuinte").hide();
        $("#selTipoDocumentoTomadorContribuinte").val(cte.DocumentoAnulacao.Tipo);
        if (cte.DocumentoAnulacao.Tipo == 1) { //CT-e
            $("#txtChaveAcessoCTeEmitidoTomador").val(cte.DocumentoAnulacao.Chave);
            $("#divChaveAcessoCTeTomador").show();
        } else if (cte.DocumentoAnulacao.Tipo == 2) { //NF-e
            $("#txtChaveAcessoNFeEmitidaTomador").val(cte.DocumentoAnulacao.Chave);
            $("#divChaveAcessoNFeTomador").show();
        } else if (cte.DocumentoAnulacao.Tipo == 3) { // CT ou NF
            $("#divInformacaoNFouCTEmitidoTomador").show();
            $("#txtCNPJNFouCTEmitidoTomador").val(cte.DocumentoAnulacao.CNPJ);
            $("#ddlModeloNFouCTEmitidoTomador").val(cte.DocumentoAnulacao.Modelo);
            $("#txtSerieNFouCTEmitidoTomador").val(cte.DocumentoAnulacao.Serie);
            $("#txtSubserieNFouCTEmitidoTomador").val(cte.DocumentoAnulacao.Subserie);
            $("#txtNumeroNFouCTEmitidoTomador").val(cte.DocumentoAnulacao.Numero);
            $("#txtValorNFouCTEmitidoTomador").val(Globalize.format(cte.DocumentoAnulacao.Valor, "n2"));
            $("#txtDataEmissaoNFouCTEmitidoTomador").val(cte.DocumentoAnulacao.DataEmissao);
        }
    } else {
        $("#selTomadorContribuinteICMS").val("0");
        $("#txtChaveAcessoCTeAnulacao").val(cte.DocumentoAnulacao.Chave);
        $("#divTomadorContribuinte").hide();
        $("#divTomadorNaoContribuinte").show();
    }
}

function LimparCamposCTeSubstituicao() {
    $("#txtChaveCTeSubstituido").val("");
    $("#hddValorFreteContratadoOriginal").val("");
    $("#selTomadorContribuinteICMS").val($("#selTomadorContribuinteICMS option:first").val());
    $("#selTipoDocumentoTomadorContribuinte").val($("#selTipoDocumentoTomadorContribuinte option:first").val());
    $("#txtChaveAcessoCTeEmitidoTomador").val("");
    $("#txtChaveAcessoNFeEmitidaTomador").val("");
    $("#txtCNPJNFouCTEmitidoTomador").val("");
    $("#ddlModeloNFouCTEmitidoTomador").val($("#ddlModeloNFouCTEmitidoTomador option:first").val());
    $("#txtSerieNFouCTEmitidoTomador").val("");
    $("#txtSubserieNFouCTEmitidoTomador").val("");
    $("#txtNumeroNFouCTEmitidoTomador").val("");
    $("#txtValorNFouCTEmitidoTomador").val("0,00");
    $("#txtDataEmissaoNFouCTEmitidoTomador").val("");
    $("#txtChaveAcessoCTeAnulacao").val("");
    $("#divTomadorNaoContribuinte").show();
    $("#divTomadorContribuinte").hide();
    $("#divChaveAcessoCTeTomador").hide();
    $("#divChaveAcessoNFeTomador").hide();
    $("#divInformacaoNFouCTEmitidoTomador").hide();
}

function CarregarObservacoesContribuinte(codigoCTe, fromCancelamento) {
    executarRest("/ObservacaoContribuinteCTE/BuscarPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento)
                for (var i = 0; i < r.Objeto.length; i++)
                    r.Objeto[i].Codigo = -(i + 1);
            $("#hddObservacoesContribuinte").val(JSON.stringify(r.Objeto));
            RenderizarObservacoesContribuinte();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarObservacoesFisco(codigoCTe, fromCancelamento) {
    executarRest("/ObservacaoFiscoCTE/BuscarPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento)
                for (var i = 0; i < r.Objeto.length; i++)
                    r.Objeto[i].Codigo = -(i + 1);
            $("#hddObservacoesFisco").val(JSON.stringify(r.Objeto));
            RenderizarObservacoesFisco();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarMotoristas(codigoCTe, fromCancelamento) {
    executarRest("/MotoristaCTe/BuscarPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento)
                for (var i = 0; i < r.Objeto.length; i++)
                    r.Objeto[i].Codigo = -(i + 1);
            $("#hddMotoristas").val(JSON.stringify(r.Objeto));
            RenderizarMotoristas();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarVeiculos(codigoCTe, fromCancelamento) {
    executarRest("/VeiculoCTe/BuscarPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento)
                for (var i = 0; i < r.Objeto.length; i++)
                    r.Objeto[i].Id = -(i + 1);
            $("#hddVeiculos").val(JSON.stringify(r.Objeto));
            RenderizarVeiculos();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarComponentesDaPrestacao(codigoCTe, fromCancelamento) {
    executarRest("/ComponentePrestacaoCTe/BuscarPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento) {

                var ocorrencias = 0;

                for (var i = 0; i < r.Objeto.length; i++) {

                    r.Objeto[i].Id = -(i + 1);

                    if (r.Objeto[i].Descricao == "VALOR FRETE" || r.Objeto[i].Descricao == "FRETE VALOR") {
                        if (ocorrencias > 0) {
                            r.Objeto.splice(i, 1);
                            i--;
                        }
                        ocorrencias += 1;
                    }

                }
            }
            $("#hddComponentesDaPrestacao").val(JSON.stringify(r.Objeto));
            RenderizarComponentesDaPrestacao();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarPercursos(cte) {
    executarRest("/PercursoCTe/BuscarPorCTe?callback=?", { CodigoCTe: cte.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("percursos", r.Objeto);
            RenderizarPercursos((cte.Status != "R" && cte.Status != "S" ? true : false));
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarDocumentos(cte, tipo, fromCancelamento) {
    executarRest("/DocumentosCTe/BuscarPorCTe?callback=?", { CodigoCTe: cte.Codigo }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento)
                for (var i = 0; i < r.Objeto.length; i++)
                    r.Objeto[i].Codigo = -(i + 1);
            if (tipo != null && tipo == 1) {
                var outroDocumento = {
                    Codigo: 0,
                    Modelo: $("#ddlTipoDocumentoOutrosRemetente option:first").val(),
                    DescricaoModelo: $("#ddlTipoDocumentoOutrosRemetente option:first").text(),
                    Descricao: "Complementar",
                    Numero: "0",
                    DataEmissao: Globalize.format(new Date(), "dd/MM/yyyy"),
                    ValorTotal: 0,
                    Excluir: false
                };
                var outrosDocumentos = new Array();
                outrosDocumentos.push(outroDocumento);
                $("#hddOutrosDocumentosRemetente").val(JSON.stringify(outrosDocumentos));
                $("#selTipoDocumentoRemetente").val("3");
                RenderizarOutrosDocumentosRemetente();
                var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
                var textoObs = configuracaoEmpresa != null ? configuracaoEmpresa.ObservacaoCTeComplementar : "";
                textoObs = typeof textoObs == "string" ? textoObs : "";

                textoObs = textoObs.replace(/#NumeroCTe#/g, cte.Numero);
                textoObs = textoObs.replace(/#ChaveCTe#/g, cte.Chave);
                textoObs = textoObs.replace(/#DataEmissaoCTe#/g, cte.DataEmissao);
                $("#txtObservacaoGeral").val(textoObs);
            } else {
                if (r.Objeto.length > 0) {
                    if (r.Objeto[0].NumeroModelo == null || r.Objeto[0].NumeroModelo == "55") {
                        $("#hddNotasFiscaisEletronicasRemetente").val(JSON.stringify(r.Objeto));
                        $("#selTipoDocumentoRemetente").val("1");
                        RenderizarNFesRemetente();
                    } else if (r.Objeto[0].NumeroModelo == "01" || r.Objeto[0].NumeroModelo == "04") {
                        $("#hddNotasFiscaisRemetente").val(JSON.stringify(r.Objeto));
                        $("#selTipoDocumentoRemetente").val("2");
                        RenderizarNotasFiscaisRemetente();
                    } else if (r.Objeto[0].NumeroModelo == "00" || r.Objeto[0].NumeroModelo == "99") {
                        $("#hddOutrosDocumentosRemetente").val(JSON.stringify(r.Objeto));
                        $("#selTipoDocumentoRemetente").val("3");
                        RenderizarOutrosDocumentosRemetente();
                    }
                }
            }
            BloquearAbasRemetente();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarInformacoesDaCarga(codigoCTe, tipo, fromCancelamento) {
    if (tipo == 1) {
        var informacaoQuantidade = {
            Id: -1,
            UnidadeMedida: 1,
            DescricaoUnidadeMedida: "KG",
            TipoUnidade: "Kilograma",
            Quantidade: 0,
            Excluir: false
        };
        var infomacoesQuantidadeCarga = new Array();
        infomacoesQuantidadeCarga.push(informacaoQuantidade);
        $("#hddInformacoesQuantidadeCarga").val(JSON.stringify(infomacoesQuantidadeCarga));
        RenderizarInformacaoQuantidadeCarga();
    } else {
        executarRest("/InformacaoCargaCTe/BuscarPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
            if (r.Sucesso) {
                if (fromCancelamento)
                    for (var i = 0; i < r.Objeto.length; i++)
                        r.Objeto[i].Id = -(i + 1);
                $("#hddInformacoesQuantidadeCarga").val(JSON.stringify(r.Objeto));
                RenderizarInformacaoQuantidadeCarga();
            } else {
                jAlert(r.Erro, "Atenção");
            }
        });
    }
}

function CarregarInformacoesSeguro(codigoCTe, fromCancelamento) {
    executarRest("/SeguroCTe/BuscarPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento)
                for (var i = 0; i < r.Objeto.length; i++)
                    r.Objeto[i].Id = -(i + 1);
            $("#hddInformacoesSeguro").val(JSON.stringify(r.Objeto));
            RenderizarInformacaoSeguro();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarDocumentosDeTransporteAnterioresEletronicos(codigoCTe, fromCancelamento) {
    executarRest("/DocumentoDeTransporteAnteriorCTe/ObterDocumentosEletronicosPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento)
                for (var i = 0; i < r.Objeto.length; i++)
                    r.Objeto[i].Codigo = -(i + 1);
            $("#hddDocsTranspAntEletronico").val(JSON.stringify(r.Objeto));
            RenderizarDocTranspAntEletronico();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarDocumentosDeTransporteAnterioresPapel(codigoCTe, fromCancelamento) {
    executarRest("/DocumentoDeTransporteAnteriorCTe/ObterDocumentosPapelPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento)
                for (var i = 0; i < r.Objeto.length; i++)
                    r.Objeto[i].Codigo = -(i + 1);
            $("#hddDocsTranspAntPapel").val(JSON.stringify(r.Objeto));
            RenderizarDocTranspAntPapel();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarProdutosPerigosos(codigoCTe, fromCancelamento) {
    executarRest("/ProdutoPerigosoCTe/BuscarPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento)
                for (var i = 0; i < r.Objeto.length; i++)
                    r.Objeto[i].Codigo = -(i + 1);
            $("#hddProdutosPerigosos").val(JSON.stringify(r.Objeto));
            RenderizarProdutoPerigoso();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarDadosDeCobranca(codigoCTe, fromCancelamento) {
    executarRest("/CobrancaCTe/BuscarPorCTe?callback=?", { CodigoCTe: codigoCTe }, function (r) {
        if (r.Sucesso) {
            if (fromCancelamento)
                for (var i = 0; i < r.Objeto.length; i++)
                    r.Objeto[i].Codigo = -(i + 1);
            $("#hddDuplicatas").val(JSON.stringify(r.Objeto));
            if (r.Objeto.length > 0 && !fromCancelamento)
                $("#txtNumeroDuplicata").val(r.Objeto[0].Numero);
            else
                BuscarProximoNumeroFatura();
            RenderizarDuplicata();
            SetarProximaParcelaFatura();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function DuplicarCTe(cte) {
    jConfirm("Deseja realmente duplicar o CT-e nº " + cte.data.Numero + "?", "Atenção", function (ret) {
        if (ret) {
            executarRest("/ConhecimentoDeTransporteEletronico/ObterDetalhes?callback=?", { CodigoCTe: cte.data.Codigo }, function (r) {
                if (r.Sucesso) {
                    $.ajaxSetup({ async: false });
                    PreencherCTe(r.Objeto, null, true, true);
                    ControlarCamposCTeOS(true);
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }
    });
}

function EmitirCTe_Menu(cte, formaEmissao) {
    if (cte.data != null)
        cte = cte.data;

    executarRest("/ConhecimentoDeTransporteEletronico/Emitir?callback=?", { CodigoCTe: cte.Codigo, FormaEmissao: (formaEmissao != null ? formaEmissao : 1) }, function (r) {
        if (r.Sucesso) {
            jAlert("CT-e emitido com sucesso!", "Sucesso");
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function EmitirCCe(cte) {
    if (cte.data.Status == "A") {
        location.href = 'CartaDeCorrecaoEletronica.aspx?x=' + cte.data.CodigoCriptografado;
    } else {
        jAlert("É necessário que o CT-e esteja autorizado para realizar a emissão de uma Carta de Correção Eletrônica.", "Atenção");
    }
}

function BuscarOcorrenciasDoCTe(cte) {
    CriarGridView("/OcorrenciaDeCTe/BuscarPorCTe?callback=?", { CodigoCTe: cte.data.Codigo }, "tbl_ocorrencias_cte_table", "tbl_ocorrencias", "tbl_paginacao_ocorrencias", null, [0]);
    $("#tituloOcorrenciaCTe").text("Ocorrências do CT-e " + cte.data.Numero + "-" + cte.data.Serie);
    $('#divOcorrenciasCTe').modal("show");
}

function RetornosSefaz(cte) {
    var opcoes = new Array();
    opcoes.push({ Descricao: "Visualizar", Evento: VisualizarMensagemRetornoSefaz });

    CriarGridView("/ConhecimentoDeTransporteEletronico/ConsultarRetornosSefaz?callback=?", { CodigoCTe: cte.data.Codigo }, "tbl_retornossefaz_table", "tbl_retornossefaz", "tbl_paginacao_retornossefaz", opcoes, [0]);
    $("#tituloRetornosSefaz").text("Retornos Sefaz do CT-e " + cte.data.Numero + "-" + cte.data.Serie);
    $('#divRetornosSefaz').modal("show");
}

function VisualizarMensagemRetornoSefaz(retornoSefaz) {
    jAlert(retornoSefaz.data.RetornoSefaz, "Mensagem retorno Sefaz");
}
