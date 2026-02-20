$(document).ready(function () {
    HeaderAuditoria("Empresa");

    var uploader;
    $("#txtDiasParaEntrega").mask("9?99");
    $("#txtDiasParaEmissaoCTeAnulacao").mask("9?99");
    $("#txtDiasParaEmissaoCTeComplementar").mask("9?99");
    $("#txtDiasParaEmissaoCTeSubstituicao").mask("9?99");
    $("#txtPrazoCancelamentoCTe").mask("9?99");
    $("#txtPrazoCancelamentoMDFe").mask("9?99");
    $("#txtPercentualImpostoSimplesNacional").priceFormat();
    $("#txtTamanhoTag").mask("?9");
    $("#txtPrimeiroNumeroMDFe").mask("9?99999");
    $("#txtCPFNFSe").mask("9?9999999999");
    $("#txtSeguradoraCNPJ").mask("99.999.999/9999-99");
    $("#txtTamanhoTagObservacaoNFe").mask("?99");
    $("#txtValorLimiteFrete").priceFormat();
    $("#txtAliquotaIR").priceFormat();
    $("#txtAliquotaCSLL").priceFormat();
    $("#txtAliquotaINSS").priceFormat();
    $("#txtPercentualBaseINSS").priceFormat();
    $("#txtCNPJMatrizCIOT").mask("99.999.999/9999-99");

    CarregarConsultaDeAtividades("txtAtividadePadrao", "btnBuscarAtividade", RetornoConsultaAtividades, true, false);
    CarregarConsultaDeSeriesPorTipo("btnBuscarSerieIntraestadual", "btnBuscarSerieIntraestadual", "A", "0", RetornoConsultaSerieIntraestadual, true, false);
    CarregarConsultaDeSeriesPorTipo("btnBuscarSerieInterestadual", "btnBuscarSerieInterestadual", "A", "0", RetornoConsultaSerieInterestadual, true, false);
    CarregarConsultaDeSeriesPorTipo("btnBuscarSerieComplementar", "btnBuscarSerieComplementar", "A", "0", RetornoConsultaSerieComplementar, true, false);
    CarregarConsultaDeSeriesPorTipo("btnBuscarSeriePorUF", "btnBuscarSeriePorUF", "A", "0", RetornoConsultaSeriePorUF, true, false);
    CarregarConsultaDeSeriesPorTipo("btnBuscarSeriePorCliente", "btnBuscarSeriePorCliente", "A", "0", RetornoConsultaSeriePorCliente, true, false);
    CarregarConsultaDeSeriesPorTipo("btnBuscarSerieMDFe", "btnBuscarSerieMDFe", "A", "1", RetornoConsultaSerieMDFe, true, false);
    CarregarConsultaDeSeriesPorTipo("btnBuscarSerieNFSe", "btnBuscarSerieNFSe", "A", "2", RetornoConsultaSerieNFSe, true, false);
    CarregarConsultaDeApolicesDeSeguros("btnBuscarApoliceSeguro", "btnBuscarApoliceSeguro", "A", RetornoConsultaApoliceSeguro, true, false);
    CarregarConsultaDeServicosNFSe("btnBuscarServicoNFSe", "btnBuscarServicoNFSe", "A", RetornoConsultaServicoNFSe, true, false);
    CarregarConsultaDeServicosNFSe("btnBuscarServicoNFSeFora", "btnBuscarServicoNFSeFora", "A", RetornoConsultaServicoNFSeFora, true, false);
    CarregarConsultaDeNaturezasNFSe("btnBuscarNaturezaNFSe", "btnBuscarNaturezaNFSe", "A", RetornoConsultaNaturezaNFSe, true, false);
    CarregarConsultaDeNaturezasNFSe("btnBuscarNaturezaForaNFSe", "btnBuscarNaturezaForaNFSe", "A", RetornoConsultaNaturezaForaNFSe, true, false);
    CarregarConsultaDeLayoutEDI('btnBuscarNaturaLayoutEDI', 'btnBuscarNaturaLayoutEDI', 2, RetornoConsultaNaturaLayoutEDI, true, false);
    CarregarConsultaDeLayoutEDI('btnBuscarNaturaLayoutEDIOcoren', 'btnBuscarNaturaLayoutEDIOcoren', 3, RetornoConsultaNaturaLayoutEDIOcoren, true, false);

    CarregarConsultaDeLocalidades("btnBuscarCidadeNFe", "btnBuscarCidadeNFe", RetornoConsultaCidadeNFe, true, false);
    CarregarConsultaDeServicosNFSe("btnBuscarServicoPorCidade", "btnBuscarServicoPorCidade", "A", RetornoConsultaServicoPorCidade, true, false);
    CarregarConsultaDeNaturezasNFSe("btnBuscarNaturezaPorCidade", "btnBuscarNaturezaPorCidade", "A", RetornoConsultaNaturezaPorCidade, true, false);


    BuscarDados();
    BuscarUFs("selUF");
    BuscarUFs("selEstadoBloqueado");

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnVisualizarLogDicas").click(function () {
        AbrirDivLogDicas();
    });

    RemoveConsulta("#txtNaturaLayoutEDI, #txtNaturaLayoutEDIOcoren", function ($this) {
        $this.val('');
        $this.data('codigo', '');
    });

    $("#txtServicoNFSe").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtServicoNFSe").data("codigo", 0);
            }
            e.preventDefault();
        }
    });

    $("#txtServicoNFSeFora").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtServicoNFSeFora").data("codigo", 0);
            }
            e.preventDefault();
        }
    });

    $("#txtNaturezaNFSe").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtNaturezaNFSe").data("codigo", 0);
            }
            e.preventDefault();
        }
    });

    $("#txtNaturezaForaNFSe").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtNaturezaForaNFSe").data("codigo", 0);
            }
            e.preventDefault();
        }
    });

    $("#txtServicoNFSe").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtServicoNFSe").data("codigo", 0);
            }
            e.preventDefault();
        }
    });

    $("#txtServicoPorCidade").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtServicoPorCidade").data("codigo", 0);
            }
            e.preventDefault();
        }
    });

    $("#txtNaturezaPorCidade").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtNaturezaPorCidade").data("codigo", 0);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtCidadeNFe").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtCidadeNFe").data("codigo", 0);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtSerieInterestadual").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtSerieInterestadual").data("codigo", 0);
            } else {
                e.preventDefault();
            }
        }
    });


    $("#txtSerieComplementar").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtSerieComplementar").data("codigo", 0);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtSerieMDFe").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtSerieMDFe").data("codigo", 0);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtSerieNFSe").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtSerieNFSe").data("codigo", 0);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtApoliceSeguro").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#txtApoliceSeguro").data("codigo", 0);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#btnAdicionarServicoPorCidade").click(function () {
        if (ValidarServicoPorCidade())
            AdicionarServicoCidade();
    });

    $("#btnAdicionarCodigoServico").click(function () {
        if (ValidarCodigoServico())
            AdicionarCodigoServico();
    });

    $("#btnAdicionarUFSerie").click(function () {
        if (ValidarSerieUF())
            AdicionarSerieUF();
    });

    $("#btnAdicionarEstadoBloquado").click(function () {
        if (ValidarEstadoBloqueado())
            AdicionarEstadoBloqueado();
    });

    $("#btnAdicionarSerieCliente").click(function () {
        if (ValidarSerieCliente())
            AdicionarSerieCliente();
    });

    $("#selTipoIntegradoraCIOT").change(function () {
        TrocarIntegradoraCIOT($(this).val());
    });

    $("#selTipoIntegradoraSM").change(function () {
        TrocarIntegradoraSM($(this).val());
    });

    //$("#selAverbaAutomaticoATM").change(function () {
    //    PainelAverbacao();
    //});
    //PainelAverbacao();

    $("#selSeguradoraAverbacao").change(function () {
        ParametrosAverbacao();
    });

    $("#btnSalvarAverbacao").click(function () {
        SalvarAverbacao();
    });
    $("#btnCancelarAverbacao").click(function () {
        LimparAverbacao();
    });

    $("#btnExcluirAverbacao").click(function () {
        ExcluirAverbacao();
    });

    $("#btnSalvarAverbacaoSerie").click(function () {
        SalvarAverbacaoSerie();
    });
    $("#btnCancelarAverbacaoSerie").click(function () {
        LimparAverbacaoSerie();
    });

    $("#btnExcluirAverbacaoSerie").click(function () {
        ExcluirAverbacaoSerie();
    });

    $("#selUtilizaENotasNFSe").change(function () {
        TrocarIntegradorEnotas($(this).val());
    });

    CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoConsultaCliente, true, false);
    CarregarConsultadeClientes("btnBuscarClienteSerie", "btnBuscarClienteSerie", RetornoConsultaClienteSerie, true, false);

    TrocarIntegradoraCIOT($("#selTipoPagamentoCIOT").val());
    TrocarIntegradorEnotas($("#selUtilizaENotasNFSe").val());
    TrocarIntegradoraSM($("#selTipoIntegradoraSM").val());

    uploader = new plupload.Uploader({
        runtimes: 'html5,flash,gears,silverlight,browserplus',
        unique_names: true,
        browse_button: 'btnImportarPreNFSItupega',
        multi_selection: false,
        filters: {
            max_file_size: '10mb',
            mime_types: ".xml,.XML",
        },
        url: ObterPath() + "/ConfiguracaoEmpresa/ImportarArquivoPreNFS",
        init: {
            FilesAdded: function (up, files) {
                //$(".processar-arquivo").addClass('disabled').attr('disabled', true);
                //ArquivoSelecionado(files[0].name);                
                uploader.start();
            },

            UploadProgress: function (up, file) {
                //$(".file-percent").text(' - ' + file.percent + '%');
                iniciarRequisicao();
            },

            FileUploaded: function (up, file, info) {
                try {
                    var retorno = info.response;
                    retorno = retorno.substr(1, (retorno.length - 3));
                    var retorno_json = JSON.parse(retorno);

                    if (retorno_json.Sucesso) {
                        ExibirMensagemSucesso("Importação realizada com sucesso.", "Sucesso");
                        //DATA = retorno_json.Objeto;
                    } else {
                        ExibirMensagemErro(retorno_json.Erro, "Atenção");
                        //ExibirMensagemErro(retorno_json.Erro, 'Atenção!', 'messages-placeholder-upload');
                        DATA = null;
                    }
                    finalizarRequisicao();
                    //VerificaProcessar();
                } catch (e) {
                    ExibirMensagemErro("Erro ao processar " + file.name, 'Atenção!');
                }
            },

            Error: function (up, err) {
                finalizarRequisicao();
                ExibirMensagemErro(err.message, 'Atenção!');
            }
        }
    });

    uploader.init();
});


function PainelAverbacao(tipo) {
    if ($("#selAverbaAutomaticoATM").val() == "0") {
        $("#listaDeAverbacoes").collapse('show');
    } else {
        $("#listaDeAverbacoes").collapse('hide');
    }
}

function AbrirDivLogDicas() {
    AtualizarLogDicas();
    $("#divLogDicas").modal({ keyboard: false, backdrop: 'static' });
}

function AtualizarLogDicas(log) {
    var dados = {
        CodigoEmpresa: $("#hddCodigoEmpresa").val()
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Visualizar Dica", Evento: VisualizarDica });

    CriarGridView("/ConfiguracaoEmpresa/BuscarLogDicas?callback=?", dados, "tbl_log_dicas_table", "tbl_log_dicas", "tbl_paginacao_log_dicas", opcoes, [0], null);
}

function VisualizarDica(log) {
    executarRest("/ConfiguracaoEmpresa/BuscarDicaDeLog?callback=?", { Codigo: log.data.Codigo }, function (r) {
        $("#txtLogDicasCTe").html(r.Objeto);
        $("#divDicas").modal({ keyboard: false, backdrop: 'static' });
    });
}

function RetornoConsultaNaturezaForaNFSe(natureza) {
    $("#txtNaturezaForaNFSe").data("codigo", natureza.Codigo);
    $("#txtNaturezaForaNFSe").val(natureza.Descricao);
}

function RetornoConsultaNaturezaNFSe(natureza) {
    $("#txtNaturezaNFSe").data("codigo", natureza.Codigo);
    $("#txtNaturezaNFSe").val(natureza.Descricao);
}

function RetornoConsultaNaturezaPorCidade(natureza) {
    $("#txtNaturezaPorCidade").data("codigo", natureza.Codigo);
    $("#txtNaturezaPorCidade").val(natureza.Descricao);
}

function RetornoConsultaNaturaLayoutEDI(layout) {
    var $this = $("#txtNaturaLayoutEDI");
    $this.val(layout.Descricao);
    $this.data('codigo', layout.Codigo);
}

function RetornoConsultaNaturaLayoutEDIOcoren(layout) {
    var $this = $("#txtNaturaLayoutEDIOcoren");
    $this.val(layout.Descricao);
    $this.data('codigo', layout.Codigo);
}

function RetornoConsultaServicoNFSeFora(servico) {
    $("#txtServicoNFSeFora").data("codigo", servico.Codigo);
    $("#txtServicoNFSeFora").val(servico.Descricao);
}

function RetornoConsultaServicoNFSe(servico) {
    $("#txtServicoNFSe").data("codigo", servico.Codigo);
    $("#txtServicoNFSe").val(servico.Descricao);
}

function RetornoConsultaSerieNFSe(serie) {
    $("#txtSerieNFSe").data("codigo", serie.Codigo);
    $("#txtSerieNFSe").val(serie.Numero);
}

function RetornoConsultaServicoPorCidade(servico) {
    $("#txtServicoPorCidade").data("codigo", servico.Codigo);
    $("#txtServicoPorCidade").val(servico.Descricao);
}

function RetornoConsultaCidadeNFe(localidade) {
    $("#txtCidadeNFe").data("codigo", localidade.Codigo);
    $("#txtCidadeNFe").val(localidade.Descricao + " - " + localidade.UF);
}

function RetornoConsultaSerieMDFe(serie) {
    $("#txtSerieMDFe").data("codigo", serie.Codigo);
    $("#txtSerieMDFe").val(serie.Numero);
}

function RetornoConsultaSerieIntraestadual(serie) {
    $("#txtSerieIntraestadual").data("codigo", serie.Codigo);
    $("#txtSerieIntraestadual").val(serie.Numero);
}

function RetornoConsultaSerieInterestadual(serie) {
    $("#txtSerieInterestadual").data("codigo", serie.Codigo);
    $("#txtSerieInterestadual").val(serie.Numero);
}

function RetornoConsultaSerieComplementar(serie) {
    $("#txtSerieComplementar").data("codigo", serie.Codigo);
    $("#txtSerieComplementar").val(serie.Numero);
}

function RetornoConsultaSeriePorUF(serie) {
    $("#txtSeriePorUF").data("codigo", serie.Codigo);
    $("#txtSeriePorUF").val(serie.Numero);
}

function RetornoConsultaSeriePorCliente(serie) {
    $("#txtSeriePorCliente").data("codigo", serie.Codigo);
    $("#txtSeriePorCliente").val(serie.Numero);
}

function RetornoConsultaAtividades(atividade) {
    $("#hddCodigoAtividade").val(atividade.Codigo);
    $("#txtAtividadePadrao").val(atividade.Codigo + " - " + atividade.Descricao);
}

function RetornoConsultaApoliceSeguro(apolice) {
    $("#txtApoliceSeguro").data("codigo", apolice.Codigo);
    $("#txtApoliceSeguro").val(apolice.NumeroApolice + " - " + apolice.NomeSeguradora);
}

function ValidarAtividade() {
    if ($("#txtAtividadePadrao").val().trim() == "") {
        $("#txtAtividadePadrao").val("");
        $("#hddCodigoAtividade").val("0");
    }
}

function BuscarDados() {
    executarRest("/ConfiguracaoEmpresa/ObterDetalhes?callback=?", { Codigo: GetUrlParam("x") }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto) {
                HeaderAuditoriaCodigo(r.Objeto.CodigoEmpresa);
                $("#hddCodigoEmpresa").val(r.Objeto.CodigoEmpresa);
                $("#hddCodigoAtividade").val(r.Objeto.CodigoAtividade);
                $("#txtAtividadePadrao").val(r.Objeto.CodigoAtividade > 0 ? r.Objeto.CodigoAtividade + " - " + r.Objeto.DescricaoAtividade : "");
                $("#txtObservacaoPadraoNormal").val(r.Objeto.ObservacaoCTeNormal);
                $("#txtObservacaoPadraoComplementar").val(r.Objeto.ObservacaoCTeComplementar);
                $("#txtObservacaoPadraoAnulacao").val(r.Objeto.ObservacaoCTeAnulacao);
                $("#txtObservacaoPadraoSubstituicao").val(r.Objeto.ObservacaoCTeSubstituicao);
                $("#txtObservacaoSimplesNacional").val(r.Objeto.ObservacaoCTeSimplesNacional);
                $("#txtDiasParaEntrega").val(r.Objeto.DiasParaEntrega);
                $("#txtDiasParaEmissaoCTeAnulacao").val(r.Objeto.DiasParaEmissaoDeCTeAnulacao);
                $("#txtDiasParaEmissaoCTeComplementar").val(r.Objeto.DiasParaEmissaoDeCTeComplementar);
                $("#txtDiasParaEmissaoCTeSubstituicao").val(r.Objeto.DiasParaEmissaoDeCTeSubstituicao);
                $("#txtPrazoCancelamentoCTe").val(r.Objeto.PrazoCancelamentoCTe);
                $("#txtPrazoCancelamentoMDFe").val(r.Objeto.PrazoCancelamentoMDFe);
                $("#chkIndicadorLotacao").attr("checked", r.Objeto.IndicadorDeLotacao);
                $("#chkEmitirSemValorDaCarga").attr("checked", r.Objeto.EmitirSemValorDaCarga);
                $("#chkUtilizaNovaImportacaoEDI").attr("checked", r.Objeto.UtilizaNovaImportacaoEDI);
                $("#txtDicasEmissaoCTe").val(r.Objeto.DicasEmissaoCTe);
                $("#txtProdutoPredominante").val(r.Objeto.ProdutoPredominante);
                $("#txtOutrasCaracteristicas").val(r.Objeto.OutrasCaracteristicas);
                $("#selTipoImpressao").val(r.Objeto.TipoImpressao);
                $("#txtContaAbastecimento").val(r.Objeto.DescricaoContaAbastecimento);
                $("#hddCodigoPlanoAbastecimento").val(r.Objeto.CodigoContaAbastecimento);
                $("#txtContaPagamentoMotorista").val(r.Objeto.DescricaoContaPagamentoMotorista);
                $("#txtContaPagamentoMotorista").data("codigo", r.Objeto.CodigoContaPagamentoMotorista);
                $("#txtContaCTe").val(r.Objeto.DescricaoContaCTe);
                $("#hddCodigoPlanoCTe").val(r.Objeto.CodigoContaCTe);
                $("#chkUtilizaTabelaFrete").attr("checked", r.Objeto.UtilizaTabelaDeFrete);
                $("#chkAtualizaVeiculoImpXMLCTe").attr("checked", r.Objeto.AtualizaVeiculoImpXMLCTe);
                $("#chkPermiteVincularMesmaPlacaOutrosVeiculos").attr("checked", r.Objeto.PermiteVincularMesmaPlacaOutrosVeiculos);
                $("#chkNaoCopiarImpostosCTeAnterior").attr("checked", r.Objeto.NaoCopiarImpostosCTeAnterior);
                $("#txtDiasParaVencimento").val(r.Objeto.DiasParaVencimentoDasDuplicatas);
                $("#txtDiasParaAvisoVencimentos").val(r.Objeto.DiasParaAvisoVencimentos);
                $("#selGerarDuplicatasAutomaticamente").val(r.Objeto.GeraDuplicatasAutomaticamente);
                $("#txtNumeroParcelasDuplicatas").val(r.Objeto.NumeroDeParcelasDasDuplicatas);
                $("#txtSerieIntraestadual").data("codigo", r.Objeto.CodigoSerieIntraestadual);
                $("#txtSerieIntraestadual").val(r.Objeto.CodigoSerieIntraestadual > 0 ? r.Objeto.NumeroSerieIntraestadual : "");
                $("#txtSerieInterestadual").data("codigo", r.Objeto.CodigoSerieInterestadual);
                $("#txtSerieInterestadual").val(r.Objeto.CodigoSerieInterestadual > 0 ? r.Objeto.NumeroSerieInterestadual : "");
                $("#txtSerieComplementar").data("codigo", r.Objeto.CodigoSerieComplementar);
                $("#txtSerieComplementar").val(r.Objeto.CodigoSerieComplementar > 0 ? r.Objeto.NumeroSerieComplementar : "");
                $("#txtSerieMDFe").data("codigo", r.Objeto.CodigoSerieMDFe);
                $("#txtSerieMDFe").val(r.Objeto.CodigoSerieMDFe > 0 ? r.Objeto.NumeroSerieMDFe : "");
                $("#txtSerieNFSe").data("codigo", r.Objeto.CodigoSerieNFSe);
                $("#txtSerieNFSe").val(r.Objeto.CodigoSerieNFSe > 0 ? r.Objeto.NumeroSerieNFSe : "");
                $("#chkICMSIsento").attr("checked", r.Objeto.ICMSIsento);
                $("#selTipoGeracaoCTeWS").val(r.Objeto.TipoGeracaoCTeWS);
                $("#selIncluirICMS").val(r.Objeto.IncluirICMS);
                $("#chkCadastrarItemDocumentoEntrada").prop('checked', r.Objeto.CadastrarItemDocumentoEntrada);
                $("#chkBloquearDuplicidadeCTeAcerto").prop('checked', r.Objeto.BloquearDuplicidadeCTeAcerto);
                $("#selAcertoViagemMovimentoReceitas").val(r.Objeto.AcertoViagemMovimentoReceitas).trigger('change');
                $("#selAcertoViagemMovimentoDespesas").val(r.Objeto.AcertoViagemMovimentoDespesas).trigger('change');
                $("#selAcertoViagemMovimentoDespesasAbastecimentos").val(r.Objeto.AcertoViagemMovimentoDespesasAbastecimentos).trigger('change');
                $("#selAcertoViagemMovimentoDespesasAdiantamentosMotorista").val(r.Objeto.AcertoViagemMovimentoDespesasAdiantamentosMotorista).trigger('change');
                $("#selAcertoViagemMovimentoReceitasDevolucoesMotorista").val(r.Objeto.AcertoViagemMovimentoReceitasDevolucoesMotorista).trigger('change');
                $("#chkNaoCopiarSeguroCTeAnterior").attr("checked", r.Objeto.NaoCopiarSeguroCTeAnterior);
                $("#chkCopiarObservacaoFiscoContribuinteCTeAnterior").attr("checked", r.Objeto.CopiarObservacaoFiscoContribuinteCTeAnterior);

                if (r.Objeto.AcertoViagemContaReceitas != null)
                    $("#txtAcertoViagemContaReceitas").val(r.Objeto.AcertoViagemContaReceitas.Descricao).data("Codigo", r.Objeto.AcertoViagemContaReceitas.Codigo);
                if (r.Objeto.AcertoViagemContaDespesas != null)
                    $("#txtAcertoViagemContaDespesas").val(r.Objeto.AcertoViagemContaDespesas.Descricao).data("Codigo", r.Objeto.AcertoViagemContaDespesas.Codigo);
                if (r.Objeto.AcertoViagemContaDespesasAbastecimentos != null)
                    $("#txtAcertoViagemContaDespesasAbastecimentos").val(r.Objeto.AcertoViagemContaDespesasAbastecimentos.Descricao).data("Codigo", r.Objeto.AcertoViagemContaDespesasAbastecimentos.Codigo);
                if (r.Objeto.AcertoViagemContaDespesasAdiantamentosMotorista != null)
                    $("#txtAcertoViagemContaDespesasAdiantamentosMotorista").val(r.Objeto.AcertoViagemContaDespesasAdiantamentosMotorista.Descricao).data("Codigo", r.Objeto.AcertoViagemContaDespesasAdiantamentosMotorista.Codigo);
                if (r.Objeto.AcertoViagemContaReceitasDevolucoesMotorista != null)
                    $("#txtAcertoViagemContaReceitasDevolucoesMotorista").val(r.Objeto.AcertoViagemContaReceitasDevolucoesMotorista.Descricao).data("Codigo", r.Objeto.AcertoViagemContaReceitasDevolucoesMotorista.Codigo);
                if (r.Objeto.AcertoViagemContaDespesasPagamentosMotorista != null)
                    $("#txtAcertoViagemContaDespesasPagamentosMotorista").val(r.Objeto.AcertoViagemContaDespesasPagamentosMotorista.Descricao).data("Codigo", r.Objeto.AcertoViagemContaDespesasPagamentosMotorista.Codigo);

                $("#chkPermiteSelecionarCTeOutroTomador").prop('checked', r.Objeto.PermiteSelecionarCTeOutroTomador);
                $("#selPerfilSPED").val(r.Objeto.Perfil);
                $("#selCriterioEscrituracaoEApuracao").val(r.Objeto.CriterioEscrituracaoEApuracao);
                $("#selIncidenciaTributariaNoPeriodo").val(r.Objeto.IncidenciaTributariaNoPeriodo);
                $("#selCSTPIS").val(r.Objeto.CSTPIS);
                $("#selAliquotaPIS").val(r.Objeto.AliquotaPIS);
                $("#selCSTCOFINS").val(r.Objeto.CSTCOFINS);
                $("#selAliquotaCOFINS").val(r.Objeto.AliquotaCOFINS);
                $("#txtApoliceSeguro").data("codigo", r.Objeto.CodigoApoliceSeguro);
                $("#txtApoliceSeguro").val(r.Objeto.DescricaoApoliceSeguro);
                $("#txtObservacaoCTeAvancadaProprio").val(r.Objeto.ObservacaoCTeAvancadaProprio);
                $("#txtObservacaoCTeAvancadaTerceiros").val(r.Objeto.ObservacaoCTeAvancadaTerceiros);
                $("#txtCodigoATM").val(r.Objeto.CodigoATM);
                $("#txtUsuarioATM").val(r.Objeto.UsuarioATM);
                $("#txtSenhaATM").val(r.Objeto.SenhaATM);
                $("#selAverbaAutomaticoATM").val(r.Objeto.AverbaAutomaticoATM);
                $("#hddInformacoesAverbacao").val(JSON.stringify(r.Objeto.ListaAverbacoes));
                $("#hddInformacoesAverbacaoSerie").val(JSON.stringify(r.Objeto.ListaAverbacoesSerie));
                $("#selTipoEmpresa").val(r.Objeto.TipoEmpresaCIOT);
                $("#selTipoIntegradoraCIOT").val(r.Objeto.TipoIntegradoraCIOT).change();
                $("#selTipoPagamentoCIOT").val(r.Objeto.TipoPagamentoCIOT);
                $("#txtChaveCriptograficaSigaFacil").val(r.Objeto.ChaveCriptograficaSigaFacil);
                $("#txtCodigoContratanteSigaFacil").val(r.Objeto.CodigoContratanteSigaFacil);
                $("#txtCodigoIntegradorEFrete").val(r.Objeto.CodigoIntegradorEFrete);
                $("#txtSenhaEFrete").val(r.Objeto.SenhaEFrete);
                $("#txtUsuarioEFrete").val(r.Objeto.UsuarioEFrete);
                $("#selEmissaoGratuitaEFrete").val(r.Objeto.EmissaoGratuitaEFrete.toString());
                $("#selBloquearNaoEquiparadoEFrete").val(r.Objeto.BloquearNaoEquiparadoEFrete.toString());
                $("#txtTruckPadURL").val(r.Objeto.TruckPadURL);
                $("#txtTruckPadUser").val(r.Objeto.TruckPadUser);
                $("#txtTruckPadPassword").val(r.Objeto.TruckPadPassword);
                $("#txtCPFNFSe").val(r.Objeto.NFSeCPF);
                $("#txtSenhaNFSe").val(r.Objeto.SenhaNFSe);
                $("#txtFraseSecretaNFSe").val(r.Objeto.FraseSecretaNFSe);
                $("#txtSerieRPSNFSe").val(r.Objeto.SerieRPSNFSe);
                $("#txtServicoNFSe").data("codigo", r.Objeto.CodigoServicoNFSe);
                $("#txtServicoNFSe").val(r.Objeto.DescricaoServicoNFSe);
                $("#txtServicoNFSeFora").data("codigo", r.Objeto.CodigoServicoNFSeFora);
                $("#txtServicoNFSeFora").val(r.Objeto.DescricaoServicoNFSeFora);
                $("#txtNaturezaNFSe").data("codigo", r.Objeto.CodigoNaturezaNFSe);
                $("#txtNaturezaNFSe").val(r.Objeto.DescricaoNaturezaNFSe);
                $("#txtNaturezaForaNFSe").data("codigo", r.Objeto.CodigoNaturezaForaNFSe);
                $("#txtNaturezaForaNFSe").val(r.Objeto.DescricaoNaturezaForaNFSe);
                $("#txtNomePDFCTe").val(r.Objeto.NomePDFCTe);
                $("#txtTokenIntegracaoCTe").val(r.Objeto.TokenIntegracaoCTe);
                $("#txtTokenIntegracaoEnvioCTe").val(r.Objeto.TokenIntegracaoEnvioCTe);
                $("#txtWsIntegracaoEnvioCTe").val(r.Objeto.WsIntegracaoEnvioCTe);
                $("#txtURLPrefeituraNFSe").val(r.Objeto.URLPrefeituraNFSe);
                $("#txtLoginSitePrefeituraNFSe").val(r.Objeto.LoginSitePrefeituraNFSe);
                $("#txtSenhaSitePrefeituraNFSe").val(r.Objeto.SenhaSitePrefeituraNFSe);
                $("#txtObservacaoIntegracaoNFSe").val(r.Objeto.ObservacaoIntegracaoNFSe);
                $("#txtObservacaoPadraoNFSe").val(r.Objeto.ObservacaoPadraoNFSe);
                $("#txtPrimeiroNumeroMDFe").val(r.Objeto.PrimeiroNumeroMDFe);
                $("#txtNaturaMatriz").val(r.Objeto.NaturaMatriz);
                $("#txtNaturaFilial").val(r.Objeto.NaturaFilial);
                $("#txtNaturaUsuario").val(r.Objeto.NaturaUsuario);
                $("#txtNaturaSenha").val(r.Objeto.NaturaSenha);
                $("#txtNaturaLayoutEDI").val(r.Objeto.DescricaoNaturaLayoutEDI);
                $("#txtNaturaLayoutEDI").data('codigo', r.Objeto.LayoutEDINatura);
                $("#txtFTPNaturaHost").val(r.Objeto.FTPNaturaHost);
                $("#txtFTPNaturaPorta").val(r.Objeto.FTPNaturaPorta);
                $("#txtFTPNaturaUsuario").val(r.Objeto.FTPNaturaUsuario);
                $("#txtFTPNaturaSenha").val(r.Objeto.FTPNaturaSenha);
                $("#txtFTPNaturaDiretorio").val(r.Objeto.FTPNaturaDiretorio);
                $("#chkFTPNaturaPassivo").prop("checked", r.Objeto.FTPNaturaPassivo);
                $("#chkFTPNaturaSeguro").prop("checked", r.Objeto.FTPNaturaSeguro);
                $("#chkNaturaEnviaOcorrenciaEntreguePadrao").prop("checked", r.Objeto.NaturaEnviaOcorrenciaEntreguePadrao);
                $("#txtNaturaLayoutEDIOcoren").val(r.Objeto.DescricaoNaturaLayoutEDIOcoren);
                $("#txtNaturaLayoutEDIOcoren").data('codigo', r.Objeto.NaturaLayoutEDIOcoren);
                $("#hddInformacoesDocumento").val(JSON.stringify(r.Objeto.DocumentosXML));
                $("#txtAssinaturaEmail").val(r.Objeto.AssinaturaEmail);
                $("#selModeloPadrao").val(r.Objeto.ModeloPadrao);
                $("#selVersaoCTe").val(r.Objeto.VersaoCTe);
                $("#selUtilizaENotasNFSe").val(r.Objeto.NFSeIntegracaoENotas);
                $("#chkEmiteNFSeForaEmbarcador").prop("checked", r.Objeto.EmiteNFSeForaEmbarcador);
                $("#chkNFSeNacional").prop("checked", r.Objeto.NFSeNacional);
                $("#txtIDEnotas").val(r.Objeto.NFSeIDENotas);

                $("#selSeguradoraAverbacao").val(r.Objeto.SeguradoraAverbacao);
                $("#txtTokenSeguroBradesco").val(r.Objeto.TokenAverbacao);
                $("#txtWsdlQuorum").val(r.Objeto.WsdlQuorum);

                $("#chkEmailSemTexto").attr("checked", r.Objeto.EmailSemTexto);

                $("#txtSeguradoraCNPJ").val(r.Objeto.SeguradoraCNPJ).trigger('blur');
                $("#txtSeguradoraNome").val(r.Objeto.SeguradoraNome);
                $("#selSeguroResponsavel").val(r.Objeto.ResponsavelSeguro);
                $("#txtSeguradoraNApolice").val(r.Objeto.SeguradoraNApolice);
                $("#txtSeguradoraNAverbacao").val(r.Objeto.SeguradoraNAverbacao);
                $("#chkNaoUtilizarDadosSeguroEmpresaPai").attr("checked", r.Objeto.NaoUtilizarDadosSeguroEmpresaPai);
                $("#chkAguardarAverbacaoCTeParaEmitirMDFe").attr("checked", r.Objeto.AguardarAverbacaoCTeParaEmitirMDFe);
                $("#selTipoPesoNFe").val(r.Objeto.TipoPesoNFe);
                $("#chkBloquearEmissaoMDFeWS").attr("checked", r.Objeto.BloquearEmissaoMDFeWS);
                $("#chkGerarNFSeImportacoes").attr("checked", r.Objeto.GerarNFSeImportacoes);
                $("#selTipoCTeAverbacao").val(r.Objeto.TipoCTeAverbacao);
                $("#chkFormatarPlacaComHifenNaObservacao").attr("checked", r.Objeto.FormatarPlacaComHifenNaObservacao);
                $("#chkImportacaoNaoRateiaPedagio").attr("checked", r.Objeto.ImportacaoNaoRateiaPedagio);
                $("#chkGerarCTeIntegracaoDocumentosMunicipais").attr("checked", r.Objeto.GerarCTeIntegracaoDocumentosMunicipais);
                $("#txtTagParaImportarObservacaoNFe").val(r.Objeto.TagParaImportarObservacaoNFe);
                $("#txtTamanhoTagObservacaoNFe").val(r.Objeto.TamanhoTagObservacaoNFe);
                $("#txtDescricaoMedidaKgCTe").val(r.Objeto.DescricaoMedidaKgCTe);
                $("#chkAverbarMDFe").attr("checked", r.Objeto.AverbarMDFe);
                $("#chkNaoCalcularDIFALCTeOS").attr("checked", r.Objeto.NaoCalcularDIFALCTeOS);
                $("#chkNaoPermitirDuplciataMesmoDocumento").attr("checked", r.Objeto.NaoPermitirDuplciataMesmoDocumento);
                $("#chkBloquearEmissaoCTeCargaMunicipal").attr("checked", r.Objeto.BloquearEmissaoCTeCargaMunicipal);
                $("#chkBloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca").attr("checked", r.Objeto.BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca);
                $("#chkBloquearEmissaoCTeComUFDestinosDiferentes").attr("checked", r.Objeto.BloquearEmissaoCTeComUFDestinosDiferentes);
                $("#chkAverbarNFSe").attr("checked", r.Objeto.AverbarNFSe);
                $("#chkNaoCopiarValoresCTeAnterior").attr("checked", r.Objeto.NaoCopiarValoresCTeAnterior);
                $("#chkAdicionarResponsavelSeguroObsContribuinte").attr("checked", r.Objeto.AdicionarResponsavelSeguroObsContribuinte);
                $("#chkArmazenaNotasParaGerarPorPeriodo").attr("checked", r.Objeto.ArmazenaNotasParaGerarPorPeriodo);
                $("#chkPermiteImportarXMLNFSe").attr("checked", r.Objeto.PermiteImportarXMLNFSe);
                $("#chkNaoImportarValoresImportacaoCTe").attr("checked", r.Objeto.NaoImportarValoresImportacaoCTe);
                $("#chkExigirObservacaoContribuinteValorContainer").attr("checked", r.Objeto.ExigirObservacaoContribuinteValorContainer);
                $("#chkUsarRegraICMSParaCteDeSubcontratacao").attr("checked", r.Objeto.UsarRegraICMSParaCteDeSubcontratacao);
                $("#chkNaoImportarNotaDuplicadaEDINovaImportacao").attr("checked", r.Objeto.NaoImportarNotaDuplicadaEDINovaImportacao);
                $("#chkNaoSmarCreditoICMSNoValorDaPrestacao").attr("checked", r.Objeto.NaoSmarCreditoICMSNoValorDaPrestacao);

                $("#chkExibirHomeVencimentoCertificado").attr("checked", r.Objeto.ExibirHomeVencimentoCertificado);
                $("#chkExibirHomePendenciasEntrega").attr("checked", r.Objeto.ExibirHomePendenciasEntrega);
                $("#chkExibirHomeGraficosEmissoes").attr("checked", r.Objeto.ExibirHomeGraficosEmissoes);
                $("#chkExibirHomeServicosVeiculos").attr("checked", r.Objeto.ExibirHomeServicosVeiculos);
                $("#chkExibirHomeParcelaDuplicatas").attr("checked", r.Objeto.ExibirHomeParcelaDuplicatas);
                $("#chkExibirHomePagamentosMotoristas").attr("checked", r.Objeto.ExibirHomePagamentosMotoristas);
                $("#chkExibirHomeAcertoViagem").attr("checked", r.Objeto.ExibirHomeAcertoViagem);
                $("#chkExibirHomeMDFesPendenteEncerramento").attr("checked", r.Objeto.ExibirHomeMDFesPendenteEncerramento);

                $("#txtValorLimiteFrete").val(r.Objeto.ValorLimiteFrete);
                $("#txtCNPJMatrizCIOT").val(r.Objeto.CNPJMatrizCIOT);

                $("#txtTrafegusURL").val(r.Objeto.TrafegusURL);
                $("#txtTrafegusUsuario").val(r.Objeto.TrafegusUsuario);
                $("#txtTrafegusSenha").val(r.Objeto.TrafegusSenha);

                $("#txtBuonnyURL").val(r.Objeto.BuonnyURL);
                $("#txtBuonnyToken").val(r.Objeto.BuonnyToken);
                $("#txtBuonnyGerenciadora").val(r.Objeto.BuonnyGerenciadora);
                $("#txtBuonnyCodigoTipoProduto").val(r.Objeto.BuonnyCodigoTipoProduto);

                $("#selTipoIntegradoraCIOT").val(r.Objeto.TipoIntegradoraCIOT).change();
                $("#selTipoIntegradoraSM").val(r.Objeto.IntegradoraSM).change();

                $("#txtMercadoLivreURL").val(r.Objeto.URLMercadoLivre);
                $("#txtMercadoLivreID").val(r.Objeto.IDMercadoLivre);
                $("#txtMercadoLivreSecretKey").val(r.Objeto.SecretKeyMercadoLivre);

                $("#txtWsIntegracaoEnvioCTeEmbarcadorTMS").val(r.Objeto.WsIntegracaoEnvioCTeEmbarcadorTMS);
                $("#txtWsIntegracaoEnvioNFSeEmbarcadorTMS").val(r.Objeto.WsIntegracaoEnvioNFSeEmbarcadorTMS);
                $("#txtWsIntegracaoEnvioMDFeEmbarcadorTMS").val(r.Objeto.WsIntegracaoEnvioMDFeEmbarcadorTMS);
                $("#txtTokenIntegracaoEmbarcadorTMS").val(r.Objeto.TokenIntegracaoEmbarcadorTMS);

                arquivosDicas = $.isArray(r.Objeto.ArquivosDicas) && r.Objeto.ArquivosDicas.length > 0 ? r.Objeto.ArquivosDicas : [];

                // States
                if ("ListaFTP" in r.Objeto)
                    StateFTP.set(r.Objeto.ListaFTP);

                if (r.Objeto.JaEmitiuMDFe == false)
                    $("#txtPrimeiroNumeroMDFe").attr("disabled", false);
                else
                    $("#txtPrimeiroNumeroMDFe").attr("disabled", true);

                if (r.Objeto.OptanteSimplesNacional) {
                    $("#txtPercentualImpostoSimplesNacional").attr("disabled", false);
                    $("#txtPercentualImpostoSimplesNacional").val(r.Objeto.PercentualImpostoSimplesNacional);
                } else {
                    $("#txtPercentualImpostoSimplesNacional").attr("disabled", true);
                    $("#txtPercentualImpostoSimplesNacional").val("0,00");
                }

                $("#txtAliquotaIR").val(r.Objeto.AliquotaIR);
                $("#txtAliquotaINSS").val(r.Objeto.AliquotaINSS);
                $("#txtPercentualBaseINSS").val(r.Objeto.PercentualBaseINSS);
                $("#txtAliquotaCSLL").val(r.Objeto.AliquotaCSLL);
                $("#chkDescontarINSSValorReceber").attr("checked", r.Objeto.DescontarINSSValorReceber);

                $("body").data("seriesUF", r.Objeto.seriesUF);
                $("body").data("seriesCliente", r.Objeto.seriesCliente);
                $("body").data("servicosCidades", r.Objeto.servicosCidades);
                $("body").data("codigosServicos", r.Objeto.codigosServicos);
                $("body").data("estadosBloqueados", r.Objeto.estadosBloqueados);

                RenderizarSeriesUF();
                RenderizarSeriesCliente();
                RenderizarAverbacoes();
                RenderizarAverbacoesSerie();
                RenderizarCNPJCPFAutenticados();
                RenderizaRichEditor();
                RenderizarArquivosDicas();
                RenderizarServicosCidades();
                RenderizarCodigosServicos();
                ParametrosAverbacao();
                RenderizarEstadosBloqueados();
                TrocarIntegradorEnotas($("#selUtilizaENotasNFSe").val());
            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}


function Salvar() {
    executarRest("/ConfiguracaoEmpresa/Salvar?callback=?", {
        Codigo: GetUrlParam("x"),
        CodigoAtividade: $("#hddCodigoAtividade").val(),
        ObservacaoCTeNormal: $("#txtObservacaoPadraoNormal").val(),
        ObservacaoCTeComplementar: $("#txtObservacaoPadraoComplementar").val(),
        ObservacaoCTeSubstituicao: $("#txtObservacaoPadraoSubstituicao").val(),
        ObservacaoCTeAnulacao: $("#txtObservacaoPadraoAnulacao").val(),
        ObservacaoCTeSimplesNacional: $("#txtObservacaoSimplesNacional").val(),
        DiasParaEntrega: $("#txtDiasParaEntrega").val(),
        DiasParaEmissaoCTeAnulacao: $("#txtDiasParaEmissaoCTeAnulacao").val(),
        DiasParaEmissaoCTeComplementar: $("#txtDiasParaEmissaoCTeComplementar").val(),
        DiasParaEmissaoCTeSubstituicao: $("#txtDiasParaEmissaoCTeSubstituicao").val(),
        PrazoCancelamentoCTe: $("#txtPrazoCancelamentoCTe").val(),
        PrazoCancelamentoMDFe: $("#txtPrazoCancelamentoMDFe").val(),
        IndicadorDeLotacao: $("#chkIndicadorLotacao")[0].checked,
        EmitirSemValorDaCarga: $("#chkEmitirSemValorDaCarga")[0].checked,
        UtilizaNovaImportacaoEDI: $("#chkUtilizaNovaImportacaoEDI")[0].checked,
        DicasEmissaoCTe: $("#txtDicasEmissaoCTe").val(),
        ProdutoPredominante: $("#txtProdutoPredominante").val(),
        OutrasCaracteristicas: $("#txtOutrasCaracteristicas").val(),
        TipoImpressao: $("#selTipoImpressao").val(),
        CodigoPlanoAbastecimento: $("#hddCodigoPlanoAbastecimento").val(),
        CodigoPlanoCTe: $("#hddCodigoPlanoCTe").val(),
        CodigoPlanoPagamentoMotorista: $("#txtContaPagamentoMotorista").data("codigo"),
        UtilizaTabelaFrete: $("#chkUtilizaTabelaFrete")[0].checked,
        AtualizaVeiculoImpXMLCTe: $("#chkAtualizaVeiculoImpXMLCTe")[0].checked,
        PermiteVincularMesmaPlacaOutrosVeiculos: $("#chkPermiteVincularMesmaPlacaOutrosVeiculos")[0].checked,
        NaoCopiarImpostosCTeAnterior: $("#chkNaoCopiarImpostosCTeAnterior").prop('checked'),
        GeraDuplicatasAutomaticamente: $("#selGerarDuplicatasAutomaticamente").val(),
        DiasParaVencimentoDasDuplicatas: $("#txtDiasParaVencimento").val(),
        DiasParaAvisoVencimentos: $("#txtDiasParaAvisoVencimentos").val(),
        NumeroDeParcelasDasDuplicatas: $("#txtNumeroParcelasDuplicatas").val(),
        CodigoSerieIntraestadual: $("#txtSerieIntraestadual").data("codigo"),
        CodigoSerieInterestadual: $("#txtSerieInterestadual").data("codigo"),
        CodigoSerieComplementar: $("#txtSerieComplementar").data("codigo"),
        CodigoSerieMDFe: $("#txtSerieMDFe").data("codigo"),
        ICMSIsento: $("#chkICMSIsento")[0].checked,
        TipoGeracaoCTeWS: $("#selTipoGeracaoCTeWS").val(),
        IncluirICMS: $("#selIncluirICMS").val(),
        Perfil: $("#selPerfilSPED").val(),
        BloquearDuplicidadeCTeAcerto: $("#chkBloquearDuplicidadeCTeAcerto").prop('checked'),
        NaoCopiarSeguroCTeAnterior: $("#chkNaoCopiarSeguroCTeAnterior").prop('checked'),
        CopiarObservacaoFiscoContribuinteCTeAnterior: $("#chkCopiarObservacaoFiscoContribuinteCTeAnterior").prop('checked'),

        AcertoViagemMovimentoReceitas: $("#selAcertoViagemMovimentoReceitas").val(),
        AcertoViagemMovimentoDespesas: $("#selAcertoViagemMovimentoDespesas").val(),
        AcertoViagemMovimentoDespesasAbastecimentos: $("#selAcertoViagemMovimentoDespesasAbastecimentos").val(),
        AcertoViagemMovimentoDespesasAdiantamentosMotorista: $("#selAcertoViagemMovimentoDespesasAdiantamentosMotorista").val(),
        AcertoViagemMovimentoReceitasDevolucoesMotorista: $("#selAcertoViagemMovimentoReceitasDevolucoesMotorista").val(),

        AcertoViagemContaReceitas: $("#txtAcertoViagemContaReceitas").data("Codigo"),
        AcertoViagemContaDespesas: $("#txtAcertoViagemContaDespesas").data("Codigo"),
        AcertoViagemContaDespesasAbastecimentos: $("#txtAcertoViagemContaDespesasAbastecimentos").data("Codigo"),
        AcertoViagemContaDespesasAdiantamentosMotorista: $("#txtAcertoViagemContaDespesasAdiantamentosMotorista").data("Codigo"),
        AcertoViagemContaReceitasDevolucoesMotorista: $("#txtAcertoViagemContaReceitasDevolucoesMotorista").data("Codigo"),
        AcertoViagemContaDespesasPagamentosMotorista: $("#txtAcertoViagemContaDespesasPagamentosMotorista").data("Codigo"),

        CadastrarItemDocumentoEntrada: $("#chkCadastrarItemDocumentoEntrada").prop('checked'),
        PermiteSelecionarCTeOutroTomador: $("#chkPermiteSelecionarCTeOutroTomador").prop('checked'),
        CriterioEscrituracaoEApuracao: $("#selCriterioEscrituracaoEApuracao").val(),
        IncidenciaTributariaNoPeriodo: $("#selIncidenciaTributariaNoPeriodo").val(),
        CSTPIS: $("#selCSTPIS").val(),
        AliquotaPIS: $("#selAliquotaPIS").val(),
        CSTCOFINS: $("#selCSTCOFINS").val(),
        AliquotaCOFINS: $("#selAliquotaCOFINS").val(),
        CodigoApoliceSeguro: $("#txtApoliceSeguro").data("codigo"),
        ObservacaoCTeAvancadaProprio: $("#txtObservacaoCTeAvancadaProprio").val(),
        ObservacaoCTeAvancadaTerceiros: $("#txtObservacaoCTeAvancadaTerceiros").val(),
        PercentualImpostoSimplesNacional: $("#txtPercentualImpostoSimplesNacional").val(),
        CodigoATM: $("#txtCodigoATM").val(),
        UsuarioATM: $("#txtUsuarioATM").val(),
        SenhaATM: $("#txtSenhaATM").val(),
        AverbaAutomaticoATM: $("#selAverbaAutomaticoATM").val(),
        ListaAverbacoes: $("#hddInformacoesAverbacao").val(),
        ListaAverbacoesSerie: $("#hddInformacoesAverbacaoSerie").val(),
        ListaFTP: StateFTP.toJson(),
        TipoEmpresaCIOT: $("#selTipoEmpresa").val(),
        TipoIntegradoraCIOT: $("#selTipoIntegradoraCIOT").val(),
        TipoPagamentoCIOT: $("#selTipoPagamentoCIOT").val(),
        ChaveCriptograficaSigaFacil: $("#txtChaveCriptograficaSigaFacil").val(),
        CodigoContratanteSigaFacil: $("#txtCodigoContratanteSigaFacil").val(),
        CodigoIntegradorEFrete: $("#txtCodigoIntegradorEFrete").val(),
        SenhaEFrete: $("#txtSenhaEFrete").val(),
        UsuarioEFrete: $("#txtUsuarioEFrete").val(),
        EmissaoGratuitaEFrete: $("#selEmissaoGratuitaEFrete").val(),
        BloquearNaoEquiparadoEFrete: $("#selBloquearNaoEquiparadoEFrete").val(),
        TruckPadURL: $("#txtTruckPadURL").val(),
        TruckPadUser: $("#txtTruckPadUser").val(),
        TruckPadPassword: $("#txtTruckPadPassword").val(),
        CodigoSerieNFSe: $("#txtSerieNFSe").data("codigo"),
        NFSeCPF: $("#txtCPFNFSe").val(),
        SenhaNFSe: $("#txtSenhaNFSe").val(),
        FraseSecretaNFSe: $("#txtFraseSecretaNFSe").val(),
        SerieRPSNFSe: $("#txtSerieRPSNFSe").val(),
        CodigoNaturezaNFSe: $("#txtNaturezaNFSe").data("codigo"),
        CodigoNaturezaForaNFSe: $("#txtNaturezaForaNFSe").data("codigo"),
        CodigoServicoNFSe: $("#txtServicoNFSe").data("codigo"),
        CodigoServicoNFSeFora: $("#txtServicoNFSeFora").data("codigo"),
        NomePDFCTe: $("#txtNomePDFCTe").val(),
        SeriesPorUF: JSON.stringify($("body").data("seriesUF")),
        SeriesPorCliente: JSON.stringify($("body").data("seriesCliente")),
        EstadosBloqueados: JSON.stringify($("body").data("estadosBloqueados")),
        TokenIntegracaoCTe: $("#txtTokenIntegracaoCTe").val(),
        TokenIntegracaoEnvioCTe: $("#txtTokenIntegracaoEnvioCTe").val(),
        WsIntegracaoCTe: $("#txtWsIntegracaoEnvioCTe").val(),
        URLPrefeituraNFSe: $("#txtURLPrefeituraNFSe").val(),
        LoginSitePrefeituraNFSe: $("#txtLoginSitePrefeituraNFSe").val(),
        SenhaSitePrefeituraNFSe: $("#txtSenhaSitePrefeituraNFSe").val(),
        ObservacaoIntegracaoNFSe: $("#txtObservacaoIntegracaoNFSe").val(),
        ObservacaoPadraoNFSe: $("#txtObservacaoPadraoNFSe").val(),
        PrimeiroNumeroMDFe: $("#txtPrimeiroNumeroMDFe").val(),
        NaturaMatriz: $("#txtNaturaMatriz").val(),
        NaturaFilial: $("#txtNaturaFilial").val(),
        NaturaUsuario: $("#txtNaturaUsuario").val(),
        NaturaSenha: $("#txtNaturaSenha").val(),
        NaturaLayoutEDI: $("#txtNaturaLayoutEDI").data('codigo'),
        NaturaLayoutEDIOcoren: $("#txtNaturaLayoutEDIOcoren").data('codigo'),
        FTPNaturaHost: $("#txtFTPNaturaHost").val(),
        FTPNaturaPorta: $("#txtFTPNaturaPorta").val(),
        FTPNaturaUsuario: $("#txtFTPNaturaUsuario").val(),
        FTPNaturaSenha: $("#txtFTPNaturaSenha").val(),
        FTPNaturaDiretorio: $("#txtFTPNaturaDiretorio").val(),
        FTPNaturaPassivo: $("#chkFTPNaturaPassivo").prop("checked"),
        FTPNaturaSeguro: $("#chkFTPNaturaSeguro").prop("checked"),
        NaturaEnviaOcorrenciaEntreguePadrao: $("#chkNaturaEnviaOcorrenciaEntreguePadrao").prop("checked"),
        DocumentosXML: $("#hddInformacoesDocumento").val(),
        AssinaturaEmail: $("#txtAssinaturaEmail").val(),
        ModeloPadrao: $("#selModeloPadrao").val(),
        VersaoCTe: $("#selVersaoCTe").val(),
        NFSeIntegracaoENotas: $("#selUtilizaENotasNFSe").val(),
        EmiteNFSeForaEmbarcador: $("#chkEmiteNFSeForaEmbarcador").prop("checked"),
        NFSeNacional: $("#chkNFSeNacional").prop("checked"),
        NFSeIDENotas: $("#txtIDEnotas").val(),

        SeguradoraAverbacao: $("#selSeguradoraAverbacao").val(),
        TokenAverbacao: $("#txtTokenSeguroBradesco").val(),
        WsdlQuorum: $("#txtWsdlQuorum").val(),
        EmailSemTexto: $("#chkEmailSemTexto")[0].checked,
        SeguradoraCNPJ: $("#txtSeguradoraCNPJ").val(),
        SeguradoraNome: $("#txtSeguradoraNome").val(),
        ResponsavelSeguro: $("#selSeguroResponsavel").val(),
        SeguradoraNApolice: $("#txtSeguradoraNApolice").val(),
        SeguradoraNAverbacao: $("#txtSeguradoraNAverbacao").val(),
        NaoUtilizarDadosSeguroEmpresaPai: $("#chkNaoUtilizarDadosSeguroEmpresaPai")[0].checked,
        AguardarAverbacaoCTeParaEmitirMDFe: $("#chkAguardarAverbacaoCTeParaEmitirMDFe")[0].checked,
        TipoPesoNFe: $("#selTipoPesoNFe").val(),
        BloquearEmissaoMDFeWS: $("#chkBloquearEmissaoMDFeWS").prop('checked'),
        GerarNFSeImportacoes: $("#chkGerarNFSeImportacoes").prop('checked'),
        FormatarPlacaComHifenNaObservacao: $("#chkFormatarPlacaComHifenNaObservacao").prop('checked'),
        ImportacaoNaoRateiaPedagio: $("#chkImportacaoNaoRateiaPedagio").prop('checked'),
        GerarCTeIntegracaoDocumentosMunicipais: $("#chkGerarCTeIntegracaoDocumentosMunicipais").prop('checked'),
        TagParaImportarObservacaoNFe: $("#txtTagParaImportarObservacaoNFe").val(),
        TamanhoTagObservacaoNFe: $("#txtTamanhoTagObservacaoNFe").val(),
        TipoCTeAverbacao: $("#selTipoCTeAverbacao").val(),
        DescricaoMedidaKgCTe: $("#txtDescricaoMedidaKgCTe").val(),
        AverbarMDFe: $("#chkAverbarMDFe").prop('checked'),
        NaoCalcularDIFALCTeOS: $("#chkNaoCalcularDIFALCTeOS").prop('checked'),
        NaoPermitirDuplciataMesmoDocumento: $("#chkNaoPermitirDuplciataMesmoDocumento").prop('checked'),
        BloquearEmissaoCTeCargaMunicipal: $("#chkBloquearEmissaoCTeCargaMunicipal").prop('checked'),
        BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca: $("#chkBloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca").prop('checked'),
        BloquearEmissaoCTeComUFDestinosDiferentes: $("#chkBloquearEmissaoCTeComUFDestinosDiferentes").prop('checked'),
        AverbarNFSe: $("#chkAverbarNFSe").prop('checked'),
        NaoCopiarValoresCTeAnterior: $("#chkNaoCopiarValoresCTeAnterior").prop('checked'),
        AdicionarResponsavelSeguroObsContribuinte: $("#chkAdicionarResponsavelSeguroObsContribuinte").prop('checked'),
        ArmazenaNotasParaGerarPorPeriodo: $("#chkArmazenaNotasParaGerarPorPeriodo").prop('checked'),
        PermiteImportarXMLNFSe: $("#chkPermiteImportarXMLNFSe").prop('checked'),
        NaoImportarValoresImportacaoCTe: $("#chkNaoImportarValoresImportacaoCTe").prop('checked'),
        ExigirObservacaoContribuinteValorContainer: $("#chkExigirObservacaoContribuinteValorContainer").prop('checked'),
        UsarRegraICMSParaCteDeSubcontratacao: $("#chkUsarRegraICMSParaCteDeSubcontratacao").prop('checked'),
        NaoImportarNotaDuplicadaEDINovaImportacao: $("#chkNaoImportarNotaDuplicadaEDINovaImportacao").prop('checked'),
        NaoSmarCreditoICMSNoValorDaPrestacao: $("#chkNaoSmarCreditoICMSNoValorDaPrestacao").prop('checked'),

        ExibirHomeVencimentoCertificado: $("#chkExibirHomeVencimentoCertificado").prop('checked'),
        ExibirHomePendenciasEntrega: $("#chkExibirHomePendenciasEntrega").prop('checked'),
        ExibirHomeGraficosEmissoes: $("#chkExibirHomeGraficosEmissoes").prop('checked'),
        ExibirHomeServicosVeiculos: $("#chkExibirHomeServicosVeiculos").prop('checked'),
        ExibirHomeParcelaDuplicatas: $("#chkExibirHomeParcelaDuplicatas").prop('checked'),
        ExibirHomePagamentosMotoristas: $("#chkExibirHomePagamentosMotoristas").prop('checked'),
        ExibirHomeAcertoViagem: $("#chkExibirHomeAcertoViagem").prop('checked'),
        ExibirHomeMDFesPendenteEncerramento: $("#chkExibirHomeMDFesPendenteEncerramento").prop('checked'),

        ValorLimiteFrete: $("#txtValorLimiteFrete").val(),
        AliquotaIR: $("#txtAliquotaIR").val(),
        AliquotaINSS: $("#txtAliquotaINSS").val(),
        PercentualBaseINSS: $("#txtPercentualBaseINSS").val(),
        AliquotaCSLL: $("#txtAliquotaCSLL").val(),
        DescontarINSSValorReceber: $("#chkDescontarINSSValorReceber").prop('checked'),
        CNPJMatrizCIOT: $("#txtCNPJMatrizCIOT").val(),
        ServicosCidades: JSON.stringify($("body").data("servicosCidades")),
        CodigosServicos: JSON.stringify($("body").data("codigosServicos")),

        TrafegusURL: $("#txtTrafegusURL").val(),
        TrafegusUsuario: $("#txtTrafegusUsuario").val(),
        TrafegusSenha: $("#txtTrafegusSenha").val(),

        BuonnyURL: $("#txtBuonnyURL").val(),
        BuonnyToken: $("#txtBuonnyToken").val(),
        BuonnyGerenciadora: $("#txtBuonnyGerenciadora").val(),
        BuonnyCodigoTipoProduto: $("#txtBuonnyCodigoTipoProduto").val(),

        IntegradoraSM: $("#selTipoIntegradoraSM").val(),

        URLMercadoLivre: $("#txtMercadoLivreURL").val(),
        IDMercadoLivre: $("#txtMercadoLivreID").val(),
        SecretKeyMercadoLivre: $("#txtMercadoLivreSecretKey").val(),

        WsIntegracaoEnvioCTeEmbarcadorTMS: $("#txtWsIntegracaoEnvioCTeEmbarcadorTMS").val(),
        WsIntegracaoEnvioNFSeEmbarcadorTMS: $("#txtWsIntegracaoEnvioNFSeEmbarcadorTMS").val(),
        WsIntegracaoEnvioMDFeEmbarcadorTMS: $("#txtWsIntegracaoEnvioMDFeEmbarcadorTMS").val(),
        TokenIntegracaoEmbarcadorTMS: $("#txtTokenIntegracaoEmbarcadorTMS").val()

    }, function (r) {
        if (r.Sucesso) {
            BuscarDados();
            ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function GetUrlParam(name) {
    var url = window.location.search.replace("?", "");
    var itens = url.split("&");
    for (n in itens) {
        if (itens[n].match(name)) {
            return itens[n].replace(name + "=", "");
        }
    }
    return null;
}

function TrocarIntegradorEnotas(integrarEnotas) {
    switch (integrarEnotas) {
        case "0":
            $(".idENotas").hide();
            break;
        case "1":
            $(".idENotas").show();
            break;
        default:
            $(".idENotas").hide();
            break;
    }
}

function TrocarIntegradoraCIOT(codigoIntegradora) {

    $(".campoPgtoCIOT").show();
    $(".campoCNPJMatrizCIOT").show();

    switch (codigoIntegradora) {
        case "1":
            $(".campoSigaFacil").show();
            $(".campoTruckpad").hide();
            $(".campoEFrete").hide();
            break;
        case "2": case "5":
            $(".campoSigaFacil").hide();
            $(".campoTruckpad").hide();
            $(".campoEFrete").hide();
            break;
        case "3": case "4":
            $(".campoSigaFacil").hide();
            $(".campoTruckpad").hide();
            $(".campoEFrete").show();
            break;
        case "6":
            $(".campoSigaFacil").hide();
            $(".campoTruckpad").show();
            $(".campoEFrete").hide();
            $(".campoPgtoCIOT").hide();
            $(".campoCNPJMatrizCIOT").hide();
            break;
        default:
            $(".campoSigaFacil").hide();
            $(".campoTruckpad").hide();
            $(".campoEFrete").hide();
            break;
    }
}

function TrocarIntegradoraSM(codigoIntegradora) {
    switch (codigoIntegradora) {
        case "1":
            $(".campoTrafegus").show();
            $(".campoBuonny").hide();
            break;
        case "2": case "5":
            $(".campoTrafegus").hide();
            $(".campoBuonny").show();
            break;
        default:
            $(".campoTrafegus").hide();
            $(".campoBuonny").hide();
            break;
    }
}


function ValidarServicoPorCidade() {
    var servicosCidades = $("body").data("servicosCidades") == null ? new Array() : $("body").data("servicosCidades");
    var codigoCidade = $("#txtCidadeNFe").data("codigo");

    var valido = true;

    if ($("#txtCidadeNFe").val() == "") {
        CampoComErro("#txtCidadeNFe");
        jAlert("Necessário informar uma Cidade!", "Atenção!");
        valido = false;
    } else
        if ($("#txtNaturezaPorCidade").val() == "") {
            CampoComErro("#txtNaturezaPorCidade");
            jAlert("Necessário informar uma Natureza!", "Atenção!");
            valido = false;
        } else
            if ($("#txtServicoPorCidade").val() == "") {
                CampoComErro("#txtServicoPorCidade");
                jAlert("Necessário informar um Serviço!", "Atenção!");
                valido = false;
            } else {
                CampoSemErro("#txtCidadeNFe");
                CampoSemErro("#txtNaturezaPorCidade");
                CampoSemErro("#txtServicoPorCidade");

                for (var i = 0; i < servicosCidades.length; i++) {
                    if (servicosCidades[i].CodigoCidade == codigoCidade && servicosCidades[i].Excluir == false) {
                        CampoComErro("#txtCidadeNFe");
                        jAlert("Cidade já foi adicionada!", "Atenção!");
                        valido = false;
                        break;
                    } else {
                        CampoSemErro("#txtCidadeNFe");
                    }
                }
            }
    return valido;
}

function AdicionarServicoCidade() {

    var servicosCidades = $("body").data("servicosCidades") == null ? new Array() : $("body").data("servicosCidades");

    var codigoCidade = $("#txtCidadeNFe").data("codigo");
    var descricaoCidade = $("#txtCidadeNFe").val();

    var codigoNatureza = $("#txtNaturezaPorCidade").data("codigo");
    var descricaoNatureza = $("#txtNaturezaPorCidade").val();

    var codigoServico = $("#txtServicoPorCidade").data("codigo");
    var descricaoServico = $("#txtServicoPorCidade").val();

    var servicoCidade = {
        CodigoCidade: codigoCidade,
        DescricaoCidade: descricaoCidade,
        CodigoNatureza: codigoNatureza,
        DescricaoNatureza: descricaoNatureza,
        CodigoServico: codigoServico,
        DescricaoServico: descricaoServico,
        Excluir: false
    }

    servicosCidades.push(servicoCidade);

    $("body").data("servicosCidades", servicosCidades);

    LimparServicosCidades();
    RenderizarServicosCidades();
}

function LimparServicosCidades() {
    $("#txtCidadeNFe").data("codigo", 0);
    $("#txtCidadeNFe").val("");
    $("#txtNaturezaPorCidade").data("codigo", 0);
    $("#txtNaturezaPorCidade").val("");
    $("#txtServicoPorCidade").data("codigo", 0);
    $("#txtServicoPorCidade").val("");
}


function RenderizarServicosCidades() {
    var servicosCidades = $("body").data("servicosCidades") == null ? new Array() : $("body").data("servicosCidades");

    $("#tblServicoPorCidade tbody").html("");

    for (var i = 0; i < servicosCidades.length; i++) {
        if (!servicosCidades[i].Excluir)
            $("#tblServicoPorCidade tbody").append("<tr><td>" + servicosCidades[i].DescricaoCidade + "</td><td>" + servicosCidades[i].DescricaoNatureza + "</td><td>" + servicosCidades[i].DescricaoServico + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirServicosCidades(" + JSON.stringify(servicosCidades[i]) + ")'>Excluir</button></td></tr>");
    }

    if ($("#tblServicoPorCidade tbody").html() == "")
        $("#tblServicoPorCidade tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}


function ExcluirServicosCidades(cidade) {
    var servicosCidades = $("body").data("servicosCidades") == null ? new Array() : $("body").data("servicosCidades");

    for (var i = 0; i < servicosCidades.length; i++) {
        if (servicosCidades[i].CodigoCidade == cidade.CodigoCidade) {
            if (cidade.CodigoCidade <= 0)
                servicosCidades.splice(i, 1);
            else
                servicosCidades[i].Excluir = true;
            break;
        }
    }

    $("body").data("servicosCidades", servicosCidades);

    RenderizarServicosCidades();
}

function ValidarCodigoServico() {
    let codigosServicos = $("body").data("codigosServicos") == null ? new Array() : $("body").data("codigosServicos");
    let codigoTributacao = $("#txtCodigoTributacaoCodigosServicos").val();

    let valido = true;

    if ($("#txtCodigoTributacaoCodigosServicos").val() == "") {
        CampoComErro("#txtCodigoTributacaoCodigosServicos");
        jAlert("Necessário informar um Código de Tributação!", "Atenção!");
        valido = false;
    }
    else if ($("#txtCodigoTributacaoPrefeituraCodigosServicos").val() == "") {
        CampoComErro("#txtCodigoTributacaoPrefeituraCodigosServicos");
        jAlert("Necessário informar um Código de Tributação da Prefeitura!", "Atenção!");
        valido = false;
    }
    else if ($("#txtNumeroTributacaoPrefeituraCodigosServicos").val() == "") {
        CampoComErro("#txtNumeroTributacaoPrefeituraCodigosServicos");
        jAlert("Necessário informar um Número de Tributação da Prefeitura!", "Atenção!");
        valido = false;
    }
    else {
        CampoSemErro("#txtCodigoTributacaoCodigosServicos");
        CampoSemErro("#txtCodigoTributacaoPrefeituraCodigosServicos");
        CampoSemErro("#txtNumeroTributacaoPrefeituraCodigosServicos");

        for (let i = 0; i < codigosServicos.length; i++) {
            if (codigosServicos[i].CodigoTributacao == codigoTributacao && codigosServicos[i].Excluir == false) {
                CampoComErro("#txtCodigoTributacaoCodigosServicos");
                jAlert("Código de Tributação já foi adicionado!", "Atenção!");
                valido = false;
                break;
            } else {
                CampoSemErro("#txtCodigoTributacaoCodigosServicos");
            }
        }
    }

    return valido;
}

function AdicionarCodigoServico() {

    let codigosServicos = $("body").data("codigosServicos") == null ? new Array() : $("body").data("codigosServicos");

    let codigoTributacao = $("#txtCodigoTributacaoCodigosServicos").val();
    let codigoTributacaoPrefeitura = $("#txtCodigoTributacaoPrefeituraCodigosServicos").val();
    let numeroTributacaoPrefeitura = $("#txtNumeroTributacaoPrefeituraCodigosServicos").val();
    let CNAE = $("#txtCNAECodigosServicos").val();

    let codigoServico = {
        Codigo: guid(),
        CodigoTributacao: codigoTributacao,
        CodigoTributacaoPrefeitura: codigoTributacaoPrefeitura,
        NumeroTributacaoPrefeitura: numeroTributacaoPrefeitura,
        CNAE: CNAE,
        Excluir: false
    }

    codigosServicos.push(codigoServico);

    $("body").data("codigosServicos", codigosServicos);

    LimparCodigosServicos();
    RenderizarCodigosServicos();
}

function LimparCodigosServicos() {
    $("#txtCodigoTributacaoCodigosServicos").val("");
    $("#txtCodigoTributacaoPrefeituraCodigosServicos").val("");
    $("#txtNumeroTributacaoPrefeituraCodigosServicos").val("");
    $("#txtCNAECodigosServicos").val("");
}


function RenderizarCodigosServicos() {
    let codigosServicos = $("body").data("codigosServicos") == null ? new Array() : $("body").data("codigosServicos");

    $("#tblCodigosServicos tbody").html("");

    for (let i = 0; i < codigosServicos.length; i++) {
        if (!codigosServicos[i].Excluir)
            $("#tblCodigosServicos tbody").append("<tr><td>" + codigosServicos[i].CodigoTributacao + "</td><td>" + codigosServicos[i].CodigoTributacaoPrefeitura + "</td><td>" + codigosServicos[i].NumeroTributacaoPrefeitura + "</td><td>" + codigosServicos[i].CNAE + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirCodigosServicos(" + JSON.stringify(codigosServicos[i]) + ")'>Excluir</button></td></tr>");
    }

    if ($("#tblCodigosServicos tbody").html() == "")
        $("#tblCodigosServicos tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}


function ExcluirCodigosServicos(codigoServico) {
    let codigosServicos = $("body").data("codigosServicos") == null ? new Array() : $("body").data("codigosServicos");

    for (var i = 0; i < codigosServicos.length; i++) {
        if (codigosServicos[i].Codigo === codigoServico.Codigo) {
            if (isNaN(codigoServico.Codigo))
                codigosServicos.splice(i, 1);
            else
                codigosServicos[i].Excluir = true;
            break;
        }
    }

    $("body").data("codigosServicos", codigosServicos);

    RenderizarCodigosServicos();
}
