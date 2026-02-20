$(document).ready(function () {
    //CarregarConsultaDeCTesSigaFacil("btnBuscarCTe", "btnBuscarCTe", RetornoConsultaCTe, true, false);

    $("#txtCTe").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("codigoCTe", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtQuantidade").priceFormat({ centsLimit: 0, centsSeparator: '' });
    $("#txtPesoBruto").priceFormat();
    $("#txtPesoLotacao").priceFormat();
    $("#txtValorMercadoriaKG").priceFormat();
    $("#txtValorTotalMercadoria").priceFormat();
    $("#txtValorTarifaFrete").priceFormat();
    $("#txtValorFrete").priceFormat();
    $("#txtTolerancia").priceFormat();
    $("#txtToleranciaSuperior").priceFormat();
    $("#txtValorAdiantamento").priceFormat();
    $("#txtValorSeguro").priceFormat();
    $("#txtValorTarifaEmissaoCartao").priceFormat();
    $("#txtValorPedagio").priceFormat();
    $("#txtValorAbastecimento").priceFormat();
    $("#txtValorCartaoPedagio").priceFormat();
    $("#txtValorIRRF").priceFormat();
    $("#txtValorINSS").priceFormat();
    $("#txtValorSENAT").priceFormat();
    $("#txtValorSEST").priceFormat();
    $("#txtValorOutrosDescontos").priceFormat();

    $("#txtNumeroInicialCTeConsulta").mask("9?99999999999");
    $("#txtNumeroFinalCTeConsulta").mask("9?99999999999");
    $("#txtNumeroCIOTConsulta").mask("9?99999999999999");
    $("#txtPlacaConsulta").mask("*******");

    $("#txtDataInicialCTeConsulta").mask("99/99/9999").datepicker();
    $("#txtDataFinalCTeConsulta").mask("99/99/9999").datepicker();

    $("#btnCancelarConsultaAvancada").click(function () {
        FecharTelaConsultaCTes();
    });

    $("#btnBuscarCTe, #btnBuscarMultiCTe").click(function () {
        AbrirTelaConsultaCTes();
    });

    $("#btnSalvarDocumento").click(function () {
        SalvarDocumento();
    });

    $("#btnCancelarDocumento").click(function () {
        LimparCamposDocumento();
    });

    $("#btnExcluirDocumento").click(function () {
        ExcluirDocumento();
    });

    $("#txtValorFrete").focusout(function () {
        CalcularImpostos();
    });

    $("#btnSelecionarTodosOsCTes").click(function () {
        SelecionarTodosOsCTes();
    });
});


function SelecionarTodosOsCTes() {
    var dados = {
        NumeroInicial: $("#txtNumeroInicialCTeConsulta").val(),
        NumeroFinal: $("#txtNumeroFinalCTeConsulta").val(),
        DataInicial: $("#txtDataInicialCTeConsulta").val(),
        DataFinal: $("#txtDataFinalCTeConsulta").val(),
        Status: $("#selStatusCTeConsulta").val(),
        CIOT: $("#txtNumeroCIOTConsulta").val(),
        Placa: $("#txtPlacaConsulta").val(),
        SemCIOT: $("#chkSemCIOT").is(":checked")
    };

    executarRest("/IntegracaoSigaFacil/SelecionarTodosOsCTes?callback=?", dados, function (r) {
        if (r.Sucesso) {
            for (var i in r.Objeto) {
                MultiSelecaoCTes.push(r.Objeto[i].Codigo);
            }
            documentosModificado(true);
            FinalizarSelecaoCTes()
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function FinalizarSelecaoCTes() {
    documentosModificado(true);
    var documentos = $("body").data("documentos") == null ? new Array() : $("body").data("documentos");

    var dados = {
        Codigo: $("body").data("codigo"),
        Documentos: JSON.stringify(MultiSelecaoCTes),
    };

    executarRest("/IntegracaoSigaFacil/ObterDetalhesCTesSelecionados?callback=?", dados, function (r) {
        if (r.Sucesso) {
            for (i in r.Objeto) {
                var duplicado = false;
                var cte = r.Objeto[i];
                var documento = {
                    Codigo: 0,
                    CodigoCTe: cte.CodigoCTe,
                    NumeroCTe: cte.NumeroCTe,
                    SerieCTe: cte.SerieCTe,
                    QuantidadeMercadoria: Globalize.parseInt(cte.PesoArredondado),
                    EspecieMercadoria: cte.Especie,
                    ValorFrete: Globalize.parseFloat(cte.ValorFrete),
                    ValorTotalMercadoria: Globalize.parseFloat(cte.ValorTotalMercadoria),
                    ValorMercadoriaKG: Globalize.parseFloat(cte.ValorMercadoriaKG),
                    PesoBruto: Globalize.parseFloat(cte.Peso),
                    Especie: cte.Especie,
                    Excluir: false,
                };

                documento.Codigo = -(++codigoDocumento);

                for (var i = 0; i < documentos.length; i++) {
                    if (documentos[i].CodigoCTe == documento.CodigoCTe && !documentos[i].Excluir) {
                        //documentos.splice(i, 1);
                        duplicado = true;
                        break;
                    }
                }

                if (!duplicado)
                    documentos.push(documento);
            }

            documentos.sort(function (a, b) { return a.Codigo > b.Codigo ? -1 : 1; });

            $("body").data("documentos", documentos);

            FecharTelaConsultaCTes();
            RenderizarDocumentos();
        } else {
            jAlert(r.Erro, "Atenção");
        }

        MultiSelecaoCTes = [];

        AtualizarGridCIOT();
    });
}

function AbrirTelaConsultaCTes(callback) {
    var dataInicial = new Date();
    var dataFinal = new Date();

    dataInicial.setDate(dataInicial.getDate() - 7);

    $("#txtDataEmissaoInicialCTeConsulta").val(Globalize.format(dataInicial, "dd/MM/yyyy"));
    $("#txtDataEmissaoFinalCTeConsulta").val(Globalize.format(dataFinal, "dd/MM/yyyy"));
    $("#selStatusCTeConsulta").val((OpcaoAbertura) ? "A" : "N");

    if (OpcaoAbertura) {
        $("#txtNumeroCIOTConsulta").val($("#txtNumeroCIOT").val());
        $("#txtPlacaConsulta").val($("#txtVeiculo").val());
        $("#btnSelecionarTodosOsCTes").show();
    } else {
        $("#btnSelecionarTodosOsCTes").hide();
    }

    $("body").data("ctesSelecionadosConsultaAvancada", null);

    $("#containerCTesSelecionados").html("");

    ConsultarCTes(callback);

    $("#divConsultaCTes").modal({ keyboard: false, backdrop: 'static' });

    $("#btnBuscarCTesConsulta").off();

    $("#btnBuscarCTesConsulta").on("click", function () { ConsultarCTes(callback) });
}

function FecharTelaConsultaCTes() {
    $("#divConsultaCTes").modal('hide');
    LimparCamposConsultaCTe();
}

function LimparCamposConsultaCTe() {
    $("#txtNumeroInicialCTeConsulta").val('');
    $("#txtNumeroFinalCTeConsulta").val('');
    $("#txtDataInicialCTeConsulta").val('');
    $("#txtDataFinalCTeConsulta").val('');
    $("#selStatusCTeConsulta").val($("#selStatusCTeConsulta option:last").val());
    $("#txtPlacaConsulta").val('');
    $("#chkSemCIOT").attr("checked", false);
    MultiSelecaoCTes = [];
}

function ConsultarCTes(callback, ignorarOrigem) {
    var dados = {
        NumeroInicial: $("#txtNumeroInicialCTeConsulta").val(),
        NumeroFinal: $("#txtNumeroFinalCTeConsulta").val(),
        DataInicial: $("#txtDataInicialCTeConsulta").val(),
        DataFinal: $("#txtDataFinalCTeConsulta").val(),
        Status: $("#selStatusCTeConsulta").val(),
        CIOT: $("#txtNumeroCIOTConsulta").val(),
        Placa: $("#txtPlacaConsulta").val(),
        SemCIOT: $("#chkSemCIOT").is(":checked"),
        inicioRegistros: 0
    };

    CriarGridView("/IntegracaoSigaFacil/ConsultarCTes?callback=?", dados, "tbl_ctes_consulta_table", "tbl_ctes_consulta", "tbl_ctes_consulta_paginacao", [{ Descricao: "Selecionar", Evento: OpcaoAbertura ? CTeSelecionado : RetornoConsultaCTe }], [0], null);
}

function CTeSelecionado(cte) {
    var $btn = $(cte.target);
    var cte = cte.data;

    if ($.inArray(cte.Codigo, MultiSelecaoCTes) < 0) {
        MultiSelecaoCTes.push(cte.Codigo);
    } else {
        $('#cteSelecionado_' + cte.Codigo).remove();
    }

    var $li = $([
        '<li class="tag-item tag-item-delete-experience" data-codigo="' + cte.Codigo + '" id="cteSelecionado_' + cte.Codigo + '">' +
        '<span class="tag-container tag-container-delete-experience"><span class="tag-box tag-box-delete-experience">' +
        ((cte.ValorFrete != null) ?
            '<b>' + cte.Numero + '</b> | ' + cte.ValorFrete :
            '<b>' + cte.Numero + '</b> | ' + cte.Valor
        ) +
        '</span>' +
        '<span class="tag-delete tag-box tag-box-delete-experience">&nbsp;</span></span>' +
        '</li>'
    ].join(""));

    $btn.attr("disabled", "disabled");
    $li.on("click", ".tag-delete", function () {
        MultiSelecaoCTes.splice($.inArray($li.data("codigo"), MultiSelecaoCTes), 1);

        $btn.removeAttr("disabled");
        $li.remove();
    });
    $("#containerCTesSelecionados").append($li);
}

function RetornoConsultaCTe(cte) {
    documentosModificado(true);
    $("body").data("cte", cte.data);
    $("#txtCTe").val(cte.data.Numero + " - " + cte.data.Serie);

    executarRest("/IntegracaoSigaFacil/ObterDetalhesCTe?callback=?", { CodigoCTe: cte.data.Codigo }, function (r) {
        if (r.Sucesso) {
            $("#txtQuantidade").val(r.Objeto.PesoArredondado);
            $("#txtEspecie").val(r.Objeto.Especie);
            $("#txtPesoBruto").val(r.Objeto.Peso);
            $("#txtValorMercadoriaKG").val(r.Objeto.ValorMercadoriaKG);
            $("#txtValorTotalMercadoria").val(r.Objeto.ValorTotalMercadoria);
            $("#txtValorFrete").val(r.Objeto.ValorFrete);
            $("#txtValorTarifaFrete").val(r.Objeto.ValorFreteKG);

            CalcularImpostos();
            FecharTelaConsultaCTes();
        } else {
            ExibirMensagemAlerta(r.Erro, "Atenção!", "messages-placeholder-emissaoCIOT");
            FecharTelaConsultaCTes();
        }
    });
}

function CalcularImpostos() {
    var idInput = OpcaoAbertura ? "Encerramento" : "";

    var valorFrete = Globalize.parseFloat($("#txt" + idInput + "ValorFrete").val());
    var imposto = $("body").data("impostosContratoFrete");

    if (imposto == null)
        return;

    if (isNaN(valorFrete))
        valorFrete = 0;

    var valorSEST = (valorFrete * (imposto.PercentualBCINSS / 100)) * (imposto.AliquotaSEST / 100);
    var valorSENAT = (valorFrete * (imposto.PercentualBCINSS / 100)) * (imposto.AliquotaSENAT / 100);

    var baseCalculoINSS = valorFrete * (imposto.PercentualBCINSS / 100);
    var valorINSS = 0;

    for (var i = 0; i < imposto.INSS.length; i++) {
        if (imposto.INSS[i].ValorInicial <= baseCalculoINSS && imposto.INSS[i].ValorFinal >= baseCalculoINSS) {
            valorINSS = baseCalculoINSS * (imposto.INSS[i].PercentualAplicar / 100);

            if (valorINSS > imposto.ValorTetoRetencaoINSS)
                valorINSS = imposto.ValorTetoRetencaoINSS;

            break;
        }
    }

    var baseCalculoIR = valorFrete * (imposto.PercentualBCIR / 100);
    if (baseCalculoIR > 0 && valorINSS > 0)
        baseCalculoIR = baseCalculoIR - valorINSS;
    if (baseCalculoIR > 0 && valorSEST > 0)
        baseCalculoIR = baseCalculoIR - valorSEST;
    if (baseCalculoIR > 0 && valorSENAT > 0)
        baseCalculoIR = baseCalculoIR - valorSENAT;
    var valorIR = 0;

    for (var i = 0; i < imposto.IR.length; i++) {
        if (imposto.IR[i].ValorInicial <= baseCalculoIR && imposto.IR[i].ValorFinal >= baseCalculoIR) {
            valorIR = (baseCalculoIR * (imposto.IR[i].PercentualAplicar / 100)) - imposto.IR[i].ValorDeduzir;

            if (valorIR < 0)
                valorIR = 0;

            break;
        }
    }

    $("#txt" + idInput + "ValorINSS").val(Globalize.format(valorINSS, "n2"));
    $("#txt" + idInput + "ValorSENAT").val(Globalize.format(valorSENAT, "n2"));
    $("#txt" + idInput + "ValorSEST").val(Globalize.format(valorSEST, "n2"));
    $("#txt" + idInput + "ValorIRRF").val(Globalize.format(valorIR, "n2"));
}

function ValidarDadosDocumento() {
    var cte = $("body").data("cte");
    var quantidade = Globalize.parseInt($("#txtQuantidade").val());
    var especie = $("#txtEspecie").val();
    var tipoPeso = $("#selTipoPeso").val();
    var pesoBruto = Globalize.parseFloat($("#txtPesoBruto").val());
    var pesoLotacao = Globalize.parseFloat($("#txtPesoLotacao").val());

    var valido = true;
    var mensagem = "";

    if (cte != null && cte.Codigo > 0) {
        CampoSemErro("#txtCTe");
    } else {
        CampoComErro("#txtCTe");
        valido = false;
    }

    if (!isNaN(quantidade) && quantidade > 0) {
        CampoSemErro("#txtQuantidade");
    } else {
        CampoComErro("#txtQuantidade");
        valido = false;
    }

    if (especie != null && especie.length > 0) {
        CampoSemErro("#txtEspecie");
    } else {
        CampoComErro("#txtEspecie");
        valido = false;
    }

    if (!isNaN(pesoBruto) && pesoBruto > 0) {
        CampoSemErro("#txtPesoBruto");
    } else {
        CampoComErro("#txtPesoBruto");
        valido = false;
    }

    if (tipoPeso == "1") {
        if (!isNaN(pesoLotacao) && pesoLotacao > 0) {
            CampoSemErro("#txtPesoLotacao");
        } else {
            mensagem += "O peso de lotação deve ser maior que zero quando o tipo de peso for peso lotação.<br/>";
            CampoComErro("#txtPesoLotacao");
        }
    } else {
        if (!isNaN(pesoLotacao) && pesoLotacao > 0) {
            mensagem += "O peso de lotação deve ser zero quando o tipo de peso for peso carregado.<br/>";
            CampoComErro("#txtPesoLotacao");
        } else {
            CampoSemErro("#txtPesoLotacao");
        }
    }

    if (!valido)
        mensagem += "Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.";
    return mensagem;
}

function SalvarDocumento() {
    var erros = ValidarDadosDocumento();

    if (erros.length > 0) {
        ExibirMensagemAlerta(erros, "Atenção!", "messages-placeholder-emissaoCIOT");
        return;
    }

    var documento = {
        Codigo: $("body").data("documento") != null ? $("body").data("documento").Codigo : 0,
        CodigoCTe: $("body").data("cte").Codigo,
        NumeroCTe: $("body").data("cte").Numero,
        SerieCTe: $("body").data("cte").Serie,
        QuantidadeMercadoria: Globalize.parseInt($("#txtQuantidade").val()),
        EspecieMercadoria: $("#txtEspecie").val(),
        TipoPeso: $("#selTipoPeso").val(),
        PesoBruto: Globalize.parseFloat($("#txtPesoBruto").val()),
        PesoLotacao: Globalize.parseFloat($("#txtPesoLotacao").val()),
        ValorMercadoriaKG: Globalize.parseFloat($("#txtValorMercadoriaKG").val()),
        ValorTotalMercadoria: Globalize.parseFloat($("#txtValorTotalMercadoria").val()),
        ValorTarifaFrete: Globalize.parseFloat($("#txtValorTarifaFrete").val()),
        ValorFrete: Globalize.parseFloat($("#txtValorFrete").val()),
        RecalculoFrete: $("#selCobraDiferencaFrete").val(),
        ExigePesoChegada: $("#selExigePesoChegada").val(),
        TipoQuebra: $("#selTipoQuebra").val(),
        TipoTolerancia: $("#selTipoTolerancia").val(),
        Tolerancia: Globalize.parseFloat($("#txtTolerancia").val()),
        ToleranciaSuperior: Globalize.parseFloat($("#txtToleranciaSuperior").val()),
        ValorAdiantamento: Globalize.parseFloat($("#txtValorAdiantamento").val()),
        ValorSeguro: Globalize.parseFloat($("#txtValorSeguro").val()),
        ValorTarifaEmissaoCartao: Globalize.parseFloat($("#txtValorTarifaEmissaoCartao").val()),
        ValorPedagio: Globalize.parseFloat($("#txtValorPedagio").val()),
        ValorAbastecimento: Globalize.parseFloat($("#txtValorAbastecimento").val()),
        ValorCartaoPedagio: Globalize.parseFloat($("#txtValorCartaoPedagio").val()),
        ValorIRRF: Globalize.parseFloat($("#txtValorIRRF").val()),
        ValorINSS: Globalize.parseFloat($("#txtValorINSS").val()),
        ValorSENAT: Globalize.parseFloat($("#txtValorSENAT").val()),
        ValorSEST: Globalize.parseFloat($("#txtValorSEST").val()),
        ValorOutrosDescontos: Globalize.parseFloat($("#txtValorOutrosDescontos").val()),
        Excluir: false
    };

    var documentos = $("body").data("documentos") == null ? new Array() : $("body").data("documentos");

    documentos.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

    if (documento.Codigo == 0)
        documento.Codigo = (documentos.length > 0 ? (documentos[0].Codigo > 0 ? -1 : (documentos[0].Codigo - 1)) : -1);

    for (var i = 0; i < documentos.length; i++) {
        if (documentos[i].Codigo == documento.Codigo) {
            documentos.splice(i, 1);
            break;
        }
    }

    documentos.push(documento);

    documentos.sort(function (a, b) { return a.Codigo > b.Codigo ? -1 : 1; });

    $("body").data("documentos", documentos);

    RenderizarDocumentos();
    LimparCamposDocumento();
    LimparCamposCidadesPedagio();
    LimparCamposEncerramento();
}

function EditarDocumento(documento) {
    $("body").data("documento", documento);
    $("body").data("cte", { Codigo: documento.CodigoCTe, Numero: documento.NumeroCTe, Serie: documento.SerieCTe });
    $("#txtCTe").val(documento.NumeroCTe + " - " + documento.SerieCTe);
    $("#txtQuantidade").val(Globalize.format(documento.QuantidadeMercadoria, "n0"));
    $("#txtEspecie").val(documento.EspecieMercadoria);
    $("#selTipoPeso").val(documento.TipoPeso);
    $("#txtPesoBruto").val(Globalize.format(documento.PesoBruto, "n2"));
    $("#txtPesoLotacao").val(Globalize.format(documento.PesoLotacao, "n2"));
    $("#txtValorMercadoriaKG").val(Globalize.format(documento.ValorMercadoriaKG, "n2"));
    $("#txtValorTotalMercadoria").val(Globalize.format(documento.ValorTotalMercadoria, "n2"));
    $("#txtValorTarifaFrete").val(Globalize.format(documento.ValorTarifaFrete, "n2"));
    $("#txtValorFrete").val(Globalize.format(documento.ValorFrete, "n2"));
    $("#selCobraDiferencaFrete").val(documento.RecalculoFrete);
    $("#selExigePesoChegada").val(documento.ExigePesoChegada);
    $("#selTipoQuebra").val(documento.TipoQuebra);
    $("#selTipoTolerancia").val(documento.TipoTolerancia);
    $("#txtTolerancia").val(Globalize.format(documento.Tolerancia, "n2"));
    $("#txtToleranciaSuperior").val(Globalize.format(documento.ToleranciaSuperior, "n2"));
    $("#txtValorAdiantamento").val(Globalize.format(documento.ValorAdiantamento, "n2"));
    $("#txtValorSeguro").val(Globalize.format(documento.ValorSeguro, "n2"));
    $("#txtValorTarifaEmissaoCartao").val(Globalize.format(documento.ValorTarifaEmissaoCartao, "n2"));
    $("#txtValorPedagio").val(Globalize.format(documento.ValorPedagio, "n2"));
    $("#txtValorAbastecimento").val(Globalize.format(documento.ValorAbastecimento, "n2"));
    $("#txtValorCartaoPedagio").val(Globalize.format(documento.ValorCartaoPedagio, "n2"));
    $("#txtValorIRRF").val(Globalize.format(documento.ValorIRRF, "n2"));
    $("#txtValorINSS").val(Globalize.format(documento.ValorINSS, "n2"));
    $("#txtValorSENAT").val(Globalize.format(documento.ValorSENAT, "n2"));
    $("#txtValorSEST").val(Globalize.format(documento.ValorSEST, "n2"));
    $("#txtValorOutrosDescontos").val(Globalize.format(documento.ValorOutrosDescontos, "n2"));

    $("#btnExcluirDocumento").show();
}

function ExcluirDocumento(documento) {
    documentosModificado(true);
    if (!documento)
        documento = $("body").data("documento");

    var documentos = $("body").data("documentos") == null ? new Array() : $("body").data("documentos");

    for (var i = 0; i < documentos.length; i++) {
        if (documentos[i].Codigo == documento.Codigo) {
            if (documento.Codigo <= 0)
                documentos.splice(i, 1);
            else
                documentos[i].Excluir = true;
            break;
        }
    }

    $("body").data("documentos", documentos);

    RenderizarDocumentos();
    LimparCamposDocumento();
}

function RenderizarDocumentos(disabled) {
    var documentos = $("body").data("documentos") == null ? new Array() : $("body").data("documentos");
    var documentosVinculados = "";
    var numeroDeDocumentosVinculados = 0;

    $("#tblDocumentos tbody tr").remove();

    for (var i = 0; i < documentos.length; i++) {
        if (!documentos[i].Excluir) {
            var onclick = "onclick = '" + (OpcaoAbertura ? "Excluir" : "Editar") + "Documento(" + JSON.stringify(documentos[i]) + ")' ";
            var $tr = [
                "<tr>",
                "<td>" + documentos[i].NumeroCTe + " - " + documentos[i].SerieCTe + "</td>",
                "<td>" + Globalize.format(documentos[i].QuantidadeMercadoria, "n0") + " " + documentos[i].EspecieMercadoria + "</td>",
                "<td>" + Globalize.format(documentos[i].PesoBruto, "n2") + "</td>",
                "<td>" + Globalize.format(documentos[i].ValorFrete, "n2") + "</td>",
                "<td><button type='button' class='btn btn-default btn-xs btn-block' " + onclick + (disabled ? "disabled" : "") + ">" + (OpcaoAbertura ? "Excluir" : "Editar") + "</button></td>",
                "</tr>"
            ].join("");

            $("#tblDocumentos tbody").append($tr);
        }
    }

    numeroDeDocumentosVinculados = $("#tblDocumentos tbody tr").length;

    if (numeroDeDocumentosVinculados == 0)
        $("#tblDocumentos tbody").html("<tr><td colspan='5'>Nenhum registro encontrado.</td></tr>");

    if (numeroDeDocumentosVinculados == 0)
        documentosVinculados = "Nenhum documento vinculado";
    else if (numeroDeDocumentosVinculados == 1)
        documentosVinculados = "1 Documento vinculado";
    else
        documentosVinculados = numeroDeDocumentosVinculados + " Documentos vinculados";

    $("#divDocumentosVinculados").text(documentosVinculados);
}

function LimparCamposDocumento() {
    $("body").data("documento", null);
    $("body").data("cte", null);
    $("#txtCTe").val('');
    $("#txtQuantidade").val('0');
    $("#txtEspecie").val('');
    $("#selTipoPeso").val($("#selTipoPeso option:first").val());
    $("#txtPesoBruto").val('0,00');
    $("#txtPesoLotacao").val('0,00');
    $("#txtValorMercadoriaKG").val('0,00');
    $("#txtValorTotalMercadoria").val('0,00');
    $("#txtValorTarifaFrete").val('0,00');
    $("#txtValorFrete").val('0,00');
    $("#selCobraDiferencaFrete").val($("#selCobraDiferencaFrete option:first").val());
    $("#selExigePesoChegada").val($("#selExigePesoChegada option:first").val());
    $("#selTipoQuebra").val($("#selTipoQuebra option:first").val());
    $("#selTipoTolerancia").val($("#selTipoTolerancia option:first").val());
    $("#txtTolerancia").val('0,00');
    $("#txtToleranciaSuperior").val('0,00');
    $("#txtValorAdiantamento").val('0,00');
    $("#txtValorSeguro").val('0,00');
    $("#txtValorTarifaEmissaoCartao").val('0,00');
    $("#txtValorPedagio").val('0,00');
    $("#txtValorAbastecimento").val('0,00');
    $("#txtValorCartaoPedagio").val('0,00');
    $("#txtValorIRRF").val('0,00');
    $("#txtValorINSS").val('0,00');
    $("#txtValorSENAT").val('0,00');
    $("#txtValorSEST").val('0,00');
    $("#txtValorOutrosDescontos").val('0,00');
    $("#btnExcluirDocumento").hide();
}
