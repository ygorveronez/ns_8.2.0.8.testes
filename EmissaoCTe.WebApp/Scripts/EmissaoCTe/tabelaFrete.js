$(document).ready(function () {

    $("#btnRecalcularFrete").on('click', function () {
        BuscarTabelaFrete();
    });

});

function BuscarTabelaFrete() {
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    var tipoCTe = $("#selTipoCTE").val();

    if (tipoCTe == "0" && configuracaoEmpresa != null && configuracaoEmpresa.UtilizaTabelaDeFrete) {

        var dados = ObterDadosParaFrete();

        if (dados.CodigoLocalidadeDestino != undefined && dados.CodigoClienteOrigem != undefined) {

            executarRest("/Frete/ObterFretePorTabelas?callback=?", dados, function (r) {

                $('#divInformacaoServicosEImpostos').find('span').remove();
                $('#divInformacaoServicosEImpostosResumo').find('span').remove();

                if (r.Sucesso) {

                    var infoCalculoFrete = "";
                    switch (r.Objeto.Tabela) {
                        case 1: //TABELA POR PESO
                            infoCalculoFrete = SetarFretePorPeso(r.Objeto);
                            break;
                        case 2: //TABELA POR VALOR
                            infoCalculoFrete = SetarFretePorValor(r.Objeto);
                            break;
                        case 3: //TABELA POR TIPO DE VEÍCULO
                            infoCalculoFrete = SetarFretePorTipoDeVeiculo(r.Objeto);
                            break;
                        case 4: //TABELA FRACIONADA POR UNIDADE
                            infoCalculoFrete = SetarFreteFracionadoPorUnidade(r.Objeto);
                            break;
                    }

                    if (infoCalculoFrete != null && infoCalculoFrete.length > 10) {
                        $("#divInformacaoServicosEImpostos").prepend('<span class="label label-success" style="padding-top: 4px; padding-bottom: 4px;">' + infoCalculoFrete + '</span>');
                        $("#divInformacaoServicosEImpostos").removeClass("hidden");

                        $("#divInformacaoServicosEImpostosResumo").prepend('<span class="label label-success" style="padding-top: 4px; padding-bottom: 4px;">' + infoCalculoFrete + '</span>');
                        $("#divInformacaoServicosEImpostosResumo").removeClass("hidden");
                    } else {
                        $("#divInformacaoServicosEImpostos").addClass("hidden");

                        $("#divInformacaoServicosEImpostosResumo").addClass("hidden");
                    }

                } else {
                    $("#divInformacaoServicosEImpostos").prepend('<span class="label label-info" style="padding-top: 4px; padding-bottom: 4px;">Dados do frete não encontrados! O valor do frete deve ser informado manualmente.</span>');
                    $("#divInformacaoServicosEImpostos").removeClass("hidden");

                    $("#divInformacaoServicosEImpostosResumo").prepend('<span class="label label-info" style="padding-top: 4px; padding-bottom: 4px;">Dados do frete não encontrados! O valor do frete deve ser informado manualmente.</span>');
                    $("#divInformacaoServicosEImpostosResumo").removeClass("hidden");
                }

            });

        }
    }
}

function SetarFretePorPeso(frete) {

    var listaInfoQuantidade = $("#hddInformacoesQuantidadeCarga").val() == "" ? new Array() : JSON.parse($("#hddInformacoesQuantidadeCarga").val());

    if (listaInfoQuantidade.length > 0) {
        var quantidadeTotal = 0;

        for (var i = 0; i < listaInfoQuantidade.length; i++)
            if (listaInfoQuantidade[i].UnidadeMedida == frete.UnidadeMedida)
                quantidadeTotal += listaInfoQuantidade[i].Quantidade;

        var valorTotalFrete = (frete.ValorFrete * (quantidadeTotal > frete.QuantidadeExcedente && frete.QuantidadeExcedente > 0 && frete.ValorExcedente > 0 ? frete.QuantidadeExcedente : quantidadeTotal))// frete.ValorPedagio + frete.ValorSeguro + frete.OutrosValores;

        var valorPedagioPorUnidade = frete.ValorPedagioPerc > 0 ? frete.ValorPedagioPerc * quantidadeTotal : 0;
        var valorPedagioTotal = frete.ValorPedagio + valorPedagioPorUnidade;

        if (frete.ValorExcedente > 0 && frete.QuantidadeExcedente > 0 && frete.QuantidadeExcedente < quantidadeTotal)
            valorTotalFrete += ((quantidadeTotal - frete.QuantidadeExcedente) * frete.ValorExcedente);

        var incluirICMS = frete.IncluirICMS;
        var utilizouMinimoFrete = false;
        if (frete.Tipo == 1 && valorTotalFrete < frete.ValorMinimoFrete) //Minimo Garantido
        {
            valorTotalFrete = frete.ValorMinimoFrete;
            utilizouMinimoFrete = false
        }
        else if (frete.Tipo == 0) {
            if (SimularTotalFrete(frete, valorTotalFrete, valorPedagioTotal) < frete.ValorMinimoFrete) {
                valorTotalFrete = frete.ValorMinimoFrete;
                utilizouMinimoFrete = true;
                incluirICMS = false;
            }
        }

        $("#txtValorFreteContratado").val(Globalize.format(valorTotalFrete, "n2"));

        var id = Globalize.parseInt($("#hddIdComponenteDaPrestacaoEmEdicao").val());

        var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());

        for (var i = 0; i < componentesDaPrestacao.length; i++) {
            if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'PEDAGIO' && componentesDaPrestacao[i].Excluir == false) {
                if (componentesDaPrestacao[i].Id < 0) {
                    componentesDaPrestacao.splice(i, 1);
                    i = i - 1;
                } else
                    componentesDaPrestacao[i].Excluir = true;
            } else if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'DESCARGA' && componentesDaPrestacao[i].Excluir == false) {
                if (componentesDaPrestacao[i].Id < 0) {
                    componentesDaPrestacao.splice(i, 1);
                    i = i - 1;
                } else
                    componentesDaPrestacao[i].Excluir = true;
            } else if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'GRIS' && componentesDaPrestacao[i].Excluir == false) {
                if (componentesDaPrestacao[i].Id < 0) {
                    componentesDaPrestacao.splice(i, 1);
                    i = i - 1;
                } else
                    componentesDaPrestacao[i].Excluir = true;
            } else if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'AD VALOREM' && componentesDaPrestacao[i].Excluir == false) {
                if (componentesDaPrestacao[i].Id < 0) {
                    componentesDaPrestacao.splice(i, 1);
                    i = i - 1;
                } else
                    componentesDaPrestacao[i].Excluir = true;
            } else if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'SEGURO' && componentesDaPrestacao[i].Excluir == false) {
                if (componentesDaPrestacao[i].Id < 0) {
                    componentesDaPrestacao.splice(i, 1);
                    i = i - 1;
                } else
                    componentesDaPrestacao[i].Excluir = true;
            } else if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'OUTROS' && componentesDaPrestacao[i].Excluir == false) {
                if (componentesDaPrestacao[i].Id < 0) {
                    componentesDaPrestacao.splice(i, 1);
                    i = i - 1;
                } else
                    componentesDaPrestacao[i].Excluir = true;
            }
        }

        $("#hddComponentesDaPrestacao").val(JSON.stringify(componentesDaPrestacao));

        RenderizarComponentesDaPrestacao();

        if (valorPedagioTotal > 0) {
            $("#txtDescricaoComponentePrestacaoServico").val("PEDAGIO");
            $("#txtValorComponentePrestacaoServico").val(Globalize.format(valorPedagioTotal, "n2"))
            $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.AdicionarPedagioBcICMS == 1 && !utilizouMinimoFrete ? true : false);
            $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", !utilizouMinimoFrete ? true : false);

            SalvarComponenteDaPrestacao();
        }

        if (frete.ValorDescarga > 0) {
            $("#txtDescricaoComponentePrestacaoServico").val("DESCARGA");
            $("#txtValorComponentePrestacaoServico").val(Globalize.format(frete.ValorDescarga, "n2"))
            $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.AdicionarDescargaBcICMS == 1 && !utilizouMinimoFrete ? true : false);
            $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", !utilizouMinimoFrete ? true : false);

            SalvarComponenteDaPrestacao();
        }

        if (frete.ValorGris > 0) {
            $("#txtDescricaoComponentePrestacaoServico").val("GRIS");
            $("#txtValorComponentePrestacaoServico").val(Globalize.format(frete.ValorGris, "n2"))
            $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.AdicionarGrisBcICMS == 1 && !utilizouMinimoFrete ? true : false);
            $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", !utilizouMinimoFrete ? true : false);

            SalvarComponenteDaPrestacao();
        }

        if (frete.ValorAdValorem > 0) {
            $("#txtDescricaoComponentePrestacaoServico").val("AD VALOREM");
            $("#txtValorComponentePrestacaoServico").val(Globalize.format(frete.ValorAdValorem, "n2"))
            $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.AdicionarAdValoremBcICMS == 1 && !utilizouMinimoFrete ? true : false);
            $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", !utilizouMinimoFrete ? true : false);

            SalvarComponenteDaPrestacao();
        }

        if (frete.ValorSeguro > 0) {
            $("#txtDescricaoComponentePrestacaoServico").val("SEGURO");
            $("#txtValorComponentePrestacaoServico").val(Globalize.format(frete.ValorSeguro, "n2"))
            $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.AdicionarSeguroBcICMS == 1 && !utilizouMinimoFrete ? true : false);
            $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", !utilizouMinimoFrete ? true : false);

            SalvarComponenteDaPrestacao();
        }

        if (frete.OutrosValores > 0) {
            $("#txtDescricaoComponentePrestacaoServico").val("OUTROS");
            $("#txtValorComponentePrestacaoServico").val(Globalize.format(frete.OutrosValores, "n2"))
            $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.AdicionarOutrosBcICMS == 1 && !utilizouMinimoFrete ? true : false);
            $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", !utilizouMinimoFrete ? true : false);

            SalvarComponenteDaPrestacao();
        }

        $("#chkIncluirICMSNoFrete").prop("checked", incluirICMS);

        SalvarFreteContratado();
        AtualizarValoresGerais();

        return ObterRetornoFretePorPeso(frete, valorTotalFrete);
    } else {
        return "";
    }
}

function SimularTotalFrete(frete, valorFrete, valorPedagioTotal) {
    var valorTotal = valorFrete;

    if (valorPedagioTotal > 0 && frete.AdicionarPedagioBcICMS)
        valorTotal += valorPedagioTotal;

    if (frete.ValorDescarga > 0 && frete.AdicionarDescargaBcICMS)
        valorTotal += frete.ValorDescarga;

    if (frete.ValorGris > 0 && frete.AdicionarGrisBcICMS)
        valorTotal += frete.ValorGris;

    if (frete.ValorAdValorem > 0 && frete.AdicionarAdValoremBcICMS)
        valorTotal += frete.ValorAdValorem;

    if (frete.ValorSeguro > 0 && frete.AdicionarSeguroBcICMS)
        valorTotal += frete.ValorSeguro;

    if (frete.OutrosValores > 0 && frete.AdicionarOutrosBcICMS)
        valorTotal += frete.OutrosValores;

    if (frete.IncluirICMS && frete.FreteMinimoComICMS) {
        var icms = $("#selICMS").val();
        var valorAliquotaICMS = $("#selAliquotaICMS").val();
        var aliquota = (icms != "0" && icms != "3" && icms != "4" && icms != "5" && icms != "11") ? valorAliquotaICMS != null ? Globalize.parseFloat(valorAliquotaICMS) : 0 : 0;
        valorTotal += (aliquota > 0 ? ((valorTotal / ((100 - aliquota) / 100)) - valorTotal) : 0);
    }

    return valorTotal;
}

function ObterRetornoFretePorPeso(frete, valorFrete) {
    var msgRetorno = "Frete calculado por peso (R$ " + Globalize.format(frete.ValorFrete, "n4") + " por " + frete.DescricaoUnidadeMedida;

    if (frete.ValorMinimoFrete > 0)
        msgRetorno += " , com  valor mínimo de R$ " + Globalize.format(frete.ValorMinimoFrete, "n2") + ");";

    //if (frete.ValorExcedente > 0)
    //    msgRetorno += " (R$ " + Globalize.format(frete.ValorExcedente, "n4") + " após " + Globalize.format(frete.QuantidadeExcedente, "n2") + " " + frete.DescricaoUnidadeMedida + ")";

    //if (frete.ValorPedagio > 0)
    //    msgRetorno += ", pedágio de R$ " + Globalize.format(frete.ValorPedagio, "n2");

    //if (frete.ValorSeguro > 0)
    //    msgRetorno += ", seguro de R$ " + Globalize.format(frete.ValorSeguro, "n2");

    //if (frete.OutrosValores > 0)
    //    msgRetorno += ", outros valores de R$ " + Globalize.format(frete.OutrosValores, "n2");

    //msgRetorno += "). Total: R$ " + Globalize.format(valorFrete, "n2") + ".";

    return msgRetorno;
}

function SetarFretePorValor(frete) {
    var listaNotas = null;
    var tipoDocumento = $("#selTipoDocumentoRemetente").val();

    if (tipoDocumento == "1")  //NF-e
        listaNotas = $("#hddNotasFiscaisEletronicasRemetente").val() != "" ? JSON.parse($("#hddNotasFiscaisEletronicasRemetente").val()) : new Array();
    else if (tipoDocumento == "2") //NF
        listaNotas = $("#hddNotasFiscaisRemetente").val() != "" ? JSON.parse($("#hddNotasFiscaisRemetente").val()) : new Array();
    else if (tipoDocumento == "3") //Outros
        listaNotas = $("#hddOutrosDocumentosRemetente").val() != "" ? JSON.parse($("#hddOutrosDocumentosRemetente").val()) : new Array();

    if (listaNotas.length > 0) {

        var valorTotal = 0;

        for (var i = 0; i < listaNotas.length; i++)
            valorTotal += listaNotas[i].Valor == undefined ? listaNotas[i].ValorTotal : listaNotas[i].Valor;

        var valorTotalFrete = 0;
        var valorSobrePercentual = (valorTotal * (frete.PercentualSobreNF / 100));

        if (frete.Tipo == 0) { //Minimo + Percentual
            valorTotalFrete = valorSobrePercentual + frete.ValorMinimoFrete;
        } else if (frete.Tipo == 1) { //Minimo Garantido
            valorTotalFrete = frete.ValorMinimoFrete > valorSobrePercentual ? frete.ValorMinimoFrete : valorSobrePercentual;
        } else if (frete.Tipo == 2) { //Percentual
            valorTotalFrete = valorSobrePercentual;
        }

        var valorPedagio = 0;
        valorPedagio = frete.ValorPedagio;

        $("#txtValorFreteContratado").val(Globalize.format(valorTotalFrete, "n2"));

        if (valorPedagio > 0) {

            var id = Globalize.parseInt($("#hddIdComponenteDaPrestacaoEmEdicao").val());

            var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());

            for (var i = 0; i < componentesDaPrestacao.length; i++) {
                if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'PEDAGIO' && componentesDaPrestacao[i].Excluir == false) {
                    componentesDaPrestacao[i].Excluir = true;
                }
            }

            $("#hddComponentesDaPrestacao").val(JSON.stringify(componentesDaPrestacao));

            RenderizarComponentesDaPrestacao();

            $("#txtDescricaoComponentePrestacaoServico").val("PEDAGIO");
            $("#txtValorComponentePrestacaoServico").val(Globalize.format(valorPedagio, "n2"))
            $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.IncluirPedagioBC == 1);
            $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", true);

            SalvarComponenteDaPrestacao();
        }

        $("#chkIncluirICMSNoFrete").prop("checked", frete.IncluirICMS);

        SalvarFreteContratado();
        AtualizarValoresGerais();

        return ObterRetornoFretePorValor(frete, valorTotalFrete, valorSobrePercentual);
    } else {
        return "";
    }
}

function ObterRetornoFretePorValor(frete, valorFrete, valorSobrePercentual) {
    var msgRetorno = "Frete calculado por valor ";

    if (frete.Tipo == 0) { //Minimo + Percentual
        msgRetorno += "mínimo + percentual (R$ " + Globalize.format(frete.ValorMinimoFrete, "n2") + " mais " + Globalize.format(frete.PercentualSobreNF, "n2") + "%"
    } else if (frete.Tipo == 1) { //Minimo Garantido
        msgRetorno += "mínimo garantido ";

        if (frete.ValorMinimoFrete > valorSobrePercentual)
            msgRetorno += "(R$ " + Globalize.format(frete.ValorMinimoFrete, "n2");
        else
            msgRetorno += "(" + Globalize.format(frete.PercentualSobreNF, "n2") + "% sobre as notas fiscais";

    } else if (frete.Tipo == 2) { //Percentual
        msgRetorno += "valor (" + Globalize.format(frete.PercentualSobreNF, "n2") + "% sobre as notas fiscais";
    }

    msgRetorno += "). Total: R$ " + Globalize.format(valorFrete, "n2") + ".";

    return msgRetorno;
}

function SetarFretePorTipoDeVeiculo(frete) {
    var valorFrete = frete.ValorFrete;
    var valorPedagio = frete.ValorPedagio;
    var valorDescarga = frete.ValorDescarga;
    var valorGris = frete.ValorGris;
    var valorAdValorem = frete.ValorAdValorem;

    $("#txtValorFreteContratado").val(Globalize.format(valorFrete, "n2"));

    var id = Globalize.parseInt($("#hddIdComponenteDaPrestacaoEmEdicao").val());

    var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());

    for (var i = 0; i < componentesDaPrestacao.length; i++) {
        if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'PEDAGIO' && componentesDaPrestacao[i].Excluir == false) {
            if (componentesDaPrestacao[i].Id < 0) {
                componentesDaPrestacao.splice(i, 1);
                i = i - 1;
            } else
                componentesDaPrestacao[i].Excluir = true;
        } else if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'DESCARGA' && componentesDaPrestacao[i].Excluir == false) {
            if (componentesDaPrestacao[i].Id < 0) {
                componentesDaPrestacao.splice(i, 1);
                i = i - 1;
            } else
                componentesDaPrestacao[i].Excluir = true;
        } else if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'SEGURO' && componentesDaPrestacao[i].Excluir == false) {
            if (componentesDaPrestacao[i].Id < 0) {
                componentesDaPrestacao.splice(i, 1);
                i = i - 1;
            } else
                componentesDaPrestacao[i].Excluir = true;
        } else if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'GRIS' && componentesDaPrestacao[i].Excluir == false) {
            if (componentesDaPrestacao[i].Id < 0) {
                componentesDaPrestacao.splice(i, 1);
                i = i - 1;
            } else
                componentesDaPrestacao[i].Excluir = true;
        } else if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'AD VALOREM' && componentesDaPrestacao[i].Excluir == false) {
            if (componentesDaPrestacao[i].Id < 0) {
                componentesDaPrestacao.splice(i, 1);
                i = i - 1;
            } else
                componentesDaPrestacao[i].Excluir = true;
        }
    }

    $("#hddComponentesDaPrestacao").val(JSON.stringify(componentesDaPrestacao));

    RenderizarComponentesDaPrestacao();

    if (valorPedagio > 0) {
        $("#txtDescricaoComponentePrestacaoServico").val("PEDAGIO");
        $("#txtValorComponentePrestacaoServico").val(Globalize.format(valorPedagio, "n2"))
        $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.AdicionarPedagioBcICMS == 1 ? true : false);
        $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", true);

        SalvarComponenteDaPrestacao();
    }

    if (valorDescarga > 0) {
        $("#txtDescricaoComponentePrestacaoServico").val("DESCARGA");
        $("#txtValorComponentePrestacaoServico").val(Globalize.format(valorDescarga, "n2"))
        $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.AdicionarDescargaBcICMS == 1 ? true : false);
        $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", true);

        SalvarComponenteDaPrestacao();
    }

    if (valorGris > 0) {
        $("#txtDescricaoComponentePrestacaoServico").val("SEGURO");
        $("#txtValorComponentePrestacaoServico").val(Globalize.format(valorGris, "n2"))
        $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.AdicionarGrisBcICMS == 1 ? true : false);
        $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", true);

        SalvarComponenteDaPrestacao();
    }

    if (valorAdValorem > 0) {
        $("#txtDescricaoComponentePrestacaoServico").val("AD VALOREM");
        $("#txtValorComponentePrestacaoServico").val(Globalize.format(valorAdValorem, "n2"))
        $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", frete.AdicionarAdValoremBcICMS == 1 ? true : false);
        $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", true);

        SalvarComponenteDaPrestacao();
    }

    $("#chkIncluirICMSNoFrete").prop("checked", frete.IncluirICMS);

    SalvarFreteContratado();
    AtualizarValoresGerais();

    return ObterRetornoFretePorTipoDeVeiculo(frete, valorFrete);

    //var valorTotalFrete = (frete.ValorFrete + frete.ValorPedagio);

    //$("#txtValorFreteContratado").val(Globalize.format(valorTotalFrete, "n2"));

    //SalvarFreteContratado();
    //AtualizarValoresGerais();

    //return ObterRetornoFretePorTipoDeVeiculo(frete, valorTotalFrete);

}

function ObterRetornoFretePorTipoDeVeiculo(frete, valorFrete) {
    //var msgRetorno = "Frete calculado por tipo de veículo (R$ " + Globalize.format(frete.ValorFrete, "n2") +
    //                 (frete.ValorPedagio > 0 ? " mais pedágio de R$ " + Globalize.format(frete.ValorPedagio, "n2") : "") +
    //                 "). Total: R$ " + Globalize.format(valorFrete, "n2") + ".";

    var msgRetorno = "Frete calculado por tipo de veículo.";

    return msgRetorno;
}


function SetarFreteFracionadoPorUnidade(frete) {

    $("#txtValorFreteContratado").val(Globalize.format(frete.ValorFrete + frete.ValorExcedentePeso, "n2"));

    var id = Globalize.parseInt($("#hddIdComponenteDaPrestacaoEmEdicao").val());

    var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());

    for (var i = 0; i < componentesDaPrestacao.length; i++) {
        if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'PEDAGIO' && componentesDaPrestacao[i].Excluir == false) {
            componentesDaPrestacao[i].Excluir = true;
        }
        if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'AD VALOREM' && componentesDaPrestacao[i].Excluir == false) {
            componentesDaPrestacao[i].Excluir = true;
        }
        if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'GRIS' && componentesDaPrestacao[i].Excluir == false) {
            componentesDaPrestacao[i].Excluir = true;
        }
        if (componentesDaPrestacao[i].Descricao.toUpperCase() == 'TAS' && componentesDaPrestacao[i].Excluir == false) {
            componentesDaPrestacao[i].Excluir = true;
        }
    }

    $("#hddComponentesDaPrestacao").val(JSON.stringify(componentesDaPrestacao));

    RenderizarComponentesDaPrestacao();

    if (frete.ValorPedagio > 0) {
        $("#txtDescricaoComponentePrestacaoServico").val("PEDAGIO");
        $("#txtValorComponentePrestacaoServico").val(Globalize.format(frete.ValorPedagio, "n2"))
        $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", true);
        $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", true);

        SalvarComponenteDaPrestacao();
    }

    if (frete.ValorAdValorem > 0) {
        $("#txtDescricaoComponentePrestacaoServico").val("AD VALOREM");
        $("#txtValorComponentePrestacaoServico").val(Globalize.format(frete.ValorAdValorem, "n2"))
        $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", true);
        $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", true);

        SalvarComponenteDaPrestacao();
    }

    if (frete.ValorGris > 0) {
        $("#txtDescricaoComponentePrestacaoServico").val("GRIS");
        $("#txtValorComponentePrestacaoServico").val(Globalize.format(frete.ValorGris, "n2"))
        $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", true);
        $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", true);

        SalvarComponenteDaPrestacao();
    }

    if (frete.ValorTAS > 0) {
        $("#txtDescricaoComponentePrestacaoServico").val("TAS");
        $("#txtValorComponentePrestacaoServico").val(Globalize.format(frete.ValorTAS, "n2"))
        $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", true);
        $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", true);

        SalvarComponenteDaPrestacao();
    }

    RenderizarComponentesDaPrestacao();

    $("#chkIncluirICMSNoFrete").prop("checked", frete.IncluirICMS);

    SalvarFreteContratado();
    AtualizarValoresGerais();

    return ObterRetornoFreteFracionadoPorUnidade();
}

function ObterRetornoFreteFracionadoPorUnidade() {
    var msgRetorno = "Calculado por Frete Fracionado por Unidade ";

    return msgRetorno;
}

function ObterDadosParaFrete() {
    var dados = {};

    dados.CodigoLocalidadeDestino = $("#ddlMunicipioTerminoPrestacao").val();//$("#selCidadeDestinatario").val();
    dados.CodigoLocalidadeOrigem = $("#ddlMunicipioInicioPrestacao").val();//$("#selCidadeRemetente").val();
    dados.CodigoClienteOrigem = $("#hddRemetente").val();
    dados.CodigoClienteDestino = $("#hddDestinatario").val();
    dados.TipoPagamento = $("#selPago_APagar").val();
    dados.CodigoClienteExpedidor = $("#hddExpedidor").val();
    dados.CodigoClienteRecebedor = $("#hddRecebedor").val();
    dados.CodigoClienteOutros = $("#hddTomador").val();
    dados.Tomador = $("#selTomadorServico").val();
    dados.ValorMercadoria = $("#txtValorTotalCarga").val();

    dados.Veiculos = $("#hddVeiculos").val();

    dados.Notas = $("#hddNotasFiscaisEletronicasRemetente").val() != "" ? $("#hddNotasFiscaisEletronicasRemetente").val() : $("#hddNotasFiscaisRemetente").val() != "" ? $("#hddNotasFiscaisRemetente").val() : $("#hddOutrosDocumentosRemetente").val();
    dados.Quantidades = $("#hddInformacoesQuantidadeCarga").val();

    return dados;
}

function VincularEventoCalculoFreteNaAbaServicosEImpostos() {
    $('a[data-toggle="tab"][href="#tabServicosEImpostos"]').on('shown.bs.tab', function (e) {
        var valorFrete = Globalize.parseFloat($("#txtValorFreteContratado").val());
        if (valorFrete == 0)
            BuscarTabelaFrete();
        else {
            var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
            if (configuracaoEmpresa != null && configuracaoEmpresa.UtilizaTabelaDeFrete) {
                $('#divInformacaoServicosEImpostos').find('span').remove();
                $("#divInformacaoServicosEImpostos").prepend('<span class="label label-success" style="padding-top: 4px; padding-bottom: 4px;">Valor frete já informado, para recalcular clique em:</span>');
                $("#divInformacaoServicosEImpostos").removeClass("hidden");
            }
        }
    });
}

function RemoverEventoCalculoFreteNaAbaServicosEImpostos() {
    $('a[data-toggle="tab"][href="#tabServicosEImpostos"]').off('shown.bs.tab');
}