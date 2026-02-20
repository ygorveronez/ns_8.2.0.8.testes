$(document).ready(function () {
    $("#txtReducaoBaseCalculoICMS").priceFormat({ prefix: '' });
    $("#txtValorBaseCalculoICMS").priceFormat({ prefix: '' });
    $("#txtValorICMS").priceFormat({ prefix: '' });
    $("#txtValorCreditoICMS").priceFormat({ prefix: '' });
    $("#txtValorFreteContratado").priceFormat({ prefix: '' });
    $("#txtValorBaseCalculoCOFINS").priceFormat({ prefix: '' });
    $("#txtValorCOFINS").priceFormat({ prefix: '' });
    $("#txtValorBaseCalculoPIS").priceFormat({ prefix: '' });
    $("#txtValorPIS").priceFormat({ prefix: '' });
    $("#txtValorTotalPrestacaoServico").priceFormat({ prefix: '' });
    $("#txtValorAReceber").priceFormat({ prefix: '' });
    $("#txtValorComponentePrestacaoServico").priceFormat({ prefix: '' });
    $("#txtPercentualICMSRecolhido").priceFormat({ prefix: '' });
    $("#txtValorINSS").priceFormat({ prefix: '' });
    $("#txtValorIR").priceFormat({ prefix: '' });
    $("#txtValorCSLL").priceFormat({ prefix: '' });
    $("#txtValorICMSDesoneracao").priceFormat({ prefix: '' });

    $("#txtValorBaseCalculoIR").priceFormat({ prefix: '' });
    $("#txtAliquotaIR").priceFormat({ prefix: '' });
    $("#txtValorBaseCalculoINSS").priceFormat({ prefix: '' });
    $("#txtAliquotaINSS").priceFormat({ prefix: '' });
    $("#txtValorBaseCalculoCSLL").priceFormat({ prefix: '' });
    $("#txtAliquotaCSLL").priceFormat({ prefix: '' });

    $("#txtIBSCBSBaseCalculo").priceFormat({ prefix: '' });

    $("#txtIBSEstadualAliquota").priceFormat({ prefix: '' });
    $("#txtIBSEstadualReducao").priceFormat({ prefix: '' });
    $("#txtIBSEstadualEfetiva").priceFormat({ prefix: '' });
    $("#txtIBSEstadualValor").priceFormat({ prefix: '' });

    $("#txtIBSMunAliquota").priceFormat({ prefix: '' });
    $("#txtIBSMunReducao").priceFormat({ prefix: '' });
    $("#txtIBSMunEfetiva").priceFormat({ prefix: '' });
    $("#txtIBSMunValor").priceFormat({ prefix: '' });

    $("#txtCBSAliquota").priceFormat({ prefix: '' });
    $("#txtCBSReducao").priceFormat({ prefix: '' });
    $("#txtCBSEfetiva").priceFormat({ prefix: '' });
    $("#txtCBSValor").priceFormat({ prefix: '' });

    $("#btnRecalcularImpostos").click(function () {
        BuscarDadosImpostos(true);
    });

    $("#txtValorFreteContratado").focusout(function () {
        $("#txtValorFreteContratadoResumo").val($("#txtValorFreteContratado").val());

        SalvarFreteContratado();
        SetarCreditoPresumido();
    });

    $("#txtValorAReceber").focusout(function () {
        $("#txtValorAReceberResumo").val($("#txtValorAReceber").val());
    });

    $("#btnCancelarComponentePrestacaoServico").click(function () {
        LimparCamposComponenteDaPrestacao();
    });

    $("#btnSalvarComponentePrestacaoServico").click(function () {
        SalvarComponenteDaPrestacao();
    });

    $("#btnExcluirComponentePrestacaoServico").click(function () {
        ExcluirComponenteDaPrestacao();
    });

    $("#txtPercentualICMSRecolhido").focusout(function () {
        CalcularAlteracaoPercentualRecolhido();
    });

    $("#selAliquotaICMS").change(function () {
        CalcularICMS();
        CalcularAlteracaoPercentualRecolhido();
        SetarCreditoPresumido();
    });

    $("#selICMS").change(function () {
        ZerarICMS();
        TrocarICMS($(this).val(), true);
        CalcularICMS();
        SetarCreditoPresumido();
        $('#divBuscarImpostos').find('span').remove();
    });

    $("#txtValorBaseCalculoICMS").focusout(function () {
        CalcularICMS();
        SetarCreditoPresumido();
    });

    $("#txtReducaoBaseCalculoICMS").focusout(function () {
        AtualizarValoresGerais();
    });

    $("#txtValorCreditoICMS").focusout(function () {
        AtualizarValoresGerais();
    });

    $("#chkIncluirICMSNoFrete").click(function () {
        $("#chkIncluirICMSNoFreteResumo").prop('checked', this.checked);
        if (this.checked) {
            $("#divPercentualICMSRecolhido").show();
            CalcularInclusaoDoICMSNoFrete();
        } else {
            $("#divPercentualICMSRecolhido").hide();
            ResetarCalculoPercentualICMSRecolhido();
        }
        SetarCreditoPresumido();
    });

    $("#selAliquotaPIS").change(function () {
        CalcularPIS();
    });

    $("#txtValorBaseCalculoPIS").focusout(function () {
        CalcularPIS();
    });

    $("#selAliquotaCOFINS").change(function () {
        CalcularCOFINS();
    });

    $("#txtValorBaseCalculoCOFINS").focusout(function () {
        CalcularCOFINS();
    });

    $("#txtValorBaseCalculoIR").focusout(function () {
        CalcularIR();
    });
    $("#txtAliquotaIR").focusout(function () {
        CalcularIR();
    });

    $("#txtValorBaseCalculoINSS").focusout(function () {
        CalcularINSS();
    });
    $("#txtAliquotaINSS").focusout(function () {
        CalcularINSS();
    });

    $("#txtValorBaseCalculoCSLL").focusout(function () {
        CalcularCSLL();
    });
    $("#txtAliquotaCSLL").focusout(function () {
        CalcularCSLL();
    });
});
function SetarCreditoPresumido() {
    var dadosEmpresa = JSON.parse($("#hddDadosEmpresa").val());
    var percentualCredito = Globalize.parseFloat(dadosEmpresa.PercentualCredito);

    if (dadosEmpresa.RegimeEspecial == 8 && percentualCredito > 0) {
        var icms = $("#selICMS").val();
        var baseIcms = Globalize.parseFloat($("#txtValorICMS").val());
        if (icms == "9") { // || icms == "10"
            //$("#txtReducaoBaseCalculoICMS").val(dadosEmpresa.PercentualCredito);
            $("#txtValorCreditoICMS").val(Globalize.format((baseIcms * (percentualCredito / 100)), "n2"));
            AtualizarValoresGerais();
        } else if (icms == "6") {
            $("#txtValorCreditoICMS").val(Globalize.format((baseIcms * (percentualCredito / 100)), "n2"));
            AtualizarValoresGerais();
        }
    }
}
function CalcularPIS() {
    var base = Globalize.parseFloat($("#txtValorBaseCalculoPIS").val());
    var aliquota = Globalize.parseFloat($("#selAliquotaPIS").val() || "0");
    var valor = base * (aliquota / 100);
    $("#txtValorPIS").val(Globalize.format(valor, "n2"));
}
function CalcularCOFINS() {
    var base = Globalize.parseFloat($("#txtValorBaseCalculoCOFINS").val());
    var aliquota = Globalize.parseFloat($("#selAliquotaCOFINS").val() || "0");
    var valor = base * (aliquota / 100);
    $("#txtValorCOFINS").val(Globalize.format(valor, "n2"));
}
function CalcularIR() {
    var base = Globalize.parseFloat($("#txtValorBaseCalculoIR").val());
    var aliquota = Globalize.parseFloat($("#txtAliquotaIR").val() || "0");
    var valor = base * (aliquota / 100);
    $("#txtValorIR").val(Globalize.format(valor, "n2"));
}
function CalcularINSS() {
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    var percentualBase = 0;
    if (configuracaoEmpresa != null && configuracaoEmpresa.PercentualBaseINSS != "" && configuracaoEmpresa.PercentualBaseINSS != "0,00")
        percentualBase = Globalize.parseFloat(configuracaoEmpresa.PercentualBaseINSS);
    var base = Globalize.parseFloat($("#txtValorBaseCalculoINSS").val());
    if (percentualBase > 0) {
        base = base * (percentualBase / 100);
        $("#txtValorBaseCalculoINSS").val(Globalize.format(base, "n2"));
    }
    var aliquota = Globalize.parseFloat($("#txtAliquotaINSS").val() || "0");
    var valor = base * (aliquota / 100);
    $("#txtValorINSS").val(Globalize.format(valor, "n2"));

    if ($("#hddDescontarINSSValorReceber").val() == "1" && valor > 0) {
        var valorAReceber = Globalize.parseFloat($("#txtValorAReceber").val());
        if (valorAReceber > 0)
            $("#txtValorAReceber").val(Globalize.format((valorAReceber - valor), "n2"));
    }
}
function CalcularCSLL() {
    var base = Globalize.parseFloat($("#txtValorBaseCalculoCSLL").val());
    var aliquota = Globalize.parseFloat($("#txtAliquotaCSLL").val() || "0");
    var valor = base * (aliquota / 100);
    $("#txtValorCSLL").val(Globalize.format(valor, "n2"));
}

function ZerarICMS() {
    $("#txtReducaoBaseCalculoICMS").val("0,00");
    $("#txtValorBaseCalculoICMS").val("0,00");
    $("#txtValorICMS").val("0,00");
    $("#txtValorCreditoICMS").val("0,00");
    $("#txtValorICMSDesoneracao").val("0,00");
    $("#txtCodigoBeneficio").val("");
    $("#selAliquotaICMS").val($("#selAliquotaICMS option:first").val());
}

function CalcularICMS() {
    var icms = $("#selICMS").val();
    if (icms != "0" && icms != "3" && icms != "4" && icms != "5" && icms != "11") {
        var valor = Globalize.parseFloat($("#txtValorBaseCalculoICMS").val() != null ? $("#txtValorBaseCalculoICMS").val() : "0");
        var aliquota = Globalize.parseFloat($("#selAliquotaICMS").val() != null ? $("#selAliquotaICMS").val() : "0");
        var valorICMS = ArredondarNumero(valor * (aliquota / 100), 2, 1);

        if (isNaN(valorICMS))
            valorICMS = 0;

        $("#txtValorICMS").val(Globalize.format(valorICMS, "n2"));
    } else {
        ZerarICMS();
    }

    SalvarComponenteImpostos();
    CopiarValoresParaResumo();
}

function CalcularInclusaoDoICMSNoFrete() {
    if ($("#chkIncluirICMSNoFrete")[0].checked) {

        var dadosEmpresa = JSON.parse($("#hddDadosEmpresa").val());

        var icms = $("#selICMS").val();

        var valorFreteContratado = 0;
        var valorAReceber = 0;
        var valorBaseCalculoICMS = 0;
        var valorCreditoICMS = Globalize.parseFloat($("#txtValorCreditoICMS").val());
        if (dadosEmpresa.NaoSmarCreditoICMSNoValorDaPrestacao)
            valorCreditoICMS = 0;

        var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());

        for (var i = 0; i < componentesDaPrestacao.length; i++) {

            if (!componentesDaPrestacao[i].Excluir) {

                if (componentesDaPrestacao[i].Descricao == "FRETE VALOR" || componentesDaPrestacao[i].Descricao == "VALOR FRETE") {
                    valorFreteContratado += componentesDaPrestacao[i].Valor;
                    valorBaseCalculoICMS += componentesDaPrestacao[i].Valor;
                }

                if (componentesDaPrestacao[i].IncluiBaseCalculoICMS)
                    valorBaseCalculoICMS += componentesDaPrestacao[i].Valor;

                if (componentesDaPrestacao[i].IncluiValorAReceber)
                    valorAReceber += componentesDaPrestacao[i].Valor;

            }
        }

        var percentualReducaoBaseCalculoICMS = Globalize.parseFloat($("#txtReducaoBaseCalculoICMS").val());

        valorBaseCalculoICMS -= valorBaseCalculoICMS * (percentualReducaoBaseCalculoICMS / 100);

        var valorAliquotaICMS = $("#selAliquotaICMS").val();
        var aliquota = (icms != "0" && icms != "3" && icms != "4" && icms != "5" && icms != "11") ? valorAliquotaICMS != null ? Globalize.parseFloat(valorAliquotaICMS) : 0 : 0;

        var percentualICMSRecolhido = Globalize.parseFloat($("#txtPercentualICMSRecolhido").val());

        valorBaseCalculoICMS += (aliquota > 0 ? ((valorBaseCalculoICMS / ((100 - aliquota) / 100)) - valorBaseCalculoICMS) : 0);
        valorBaseCalculoICMS = ArredondarNumero(valorBaseCalculoICMS, 2, 1);

        var valorICMS = valorBaseCalculoICMS * (aliquota / 100);
        valorICMS = ArredondarNumero(valorICMS, 2, 1);

        valorRecolhido = valorICMS * (percentualICMSRecolhido / 100);
        valorRecolhido = ArredondarNumero(valorRecolhido, 2, 1);

        if ($("#selICMS").val() == "6") {
            $("#txtValorAReceber").val(Globalize.format((valorAReceber + valorCreditoICMS), "n2"));
            $("#txtValorTotalPrestacaoServico").val(Globalize.format((valorAReceber + valorRecolhido), "n2"));
        } else {
            $("#txtValorAReceber").val(Globalize.format((valorAReceber + valorRecolhido + valorCreditoICMS), "n2"));
            $("#txtValorTotalPrestacaoServico").val($("#txtValorAReceber").val());
        }

        $("#txtValorBaseCalculoICMS").val(Globalize.format(valorBaseCalculoICMS, "n2"));
        CalcularICMS();

        var valorICMS = Globalize.parseFloat($("#txtValorICMS").val());

        $("#txtValorBaseCalculoPIS").val(Globalize.format((valorFreteContratado + valorRecolhido - valorICMS), "n2"));
        CalcularPIS();

        $("#txtValorBaseCalculoCOFINS").val(Globalize.format((valorFreteContratado + valorRecolhido - valorICMS), "n2"));
        CalcularCOFINS();

        $("#txtValorBaseCalculoIR").val(Globalize.format((valorFreteContratado + valorRecolhido), "n2"));
        CalcularIR();

        $("#txtValorBaseCalculoINSS").val(Globalize.format((valorFreteContratado + valorRecolhido), "n2"));
        CalcularINSS();

        $("#txtValorBaseCalculoCSLL").val(Globalize.format((valorFreteContratado + valorRecolhido), "n2"));
        CalcularCSLL();

        BuscarAliquotasPorClass();
    }
}

function ArredondarNumero(numero, casasDecimais, ceilOrFloor) {
    //return numero;

    var arredondado = numero.toFixed(2);
    return parseFloat(arredondado);

    //var arredondado = numero;
    //arredondado *= (Math.pow(10, casasDecimais));
    //if (ceilOrFloor == 0) {
    //    arredondado = Math.ceil(arredondado);
    //} else {
    //    arredondado = Math.floor(arredondado);
    //}
    //arredondado /= (Math.pow(10, casasDecimais));
    //return arredondado;

}

function SalvarComponenteImpostos() {
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());

    var percentualImpostoSimplesNacional = 0;

    if (configuracaoEmpresa != null && configuracaoEmpresa.OptanteSimplesNacional) {
        percentualImpostoSimplesNacional = Globalize.parseFloat(configuracaoEmpresa.PercentualImpostoSimplesNacional);

        if (isNaN(percentualImpostoSimplesNacional))
            percentualImpostoSimplesNacional = 0;
    }

    var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());
    componentesDaPrestacao.sort(function (a, b) { return a.Id < b.Id ? -1 : 1; });

    var componente = null;
    var indice = -1;
    var indiceFreteValor = -1;
    var valorAReceber = 0;

    for (var i = 0; i < componentesDaPrestacao.length; i++) {
        if (componentesDaPrestacao[i].Descricao == "IMPOSTOS") {
            indice = i;
            valorAReceber += componentesDaPrestacao[i].Valor;
        }

        if (componentesDaPrestacao[i].Descricao == "FRETE VALOR") {
            indiceFreteValor = i;
            valorAReceber += componentesDaPrestacao[i].Valor;
        }
    }

    var valorImpostos = 0;
    var incluiValorAReceber = false;

    if ($("#chkIncluirICMSNoFrete")[0].checked) {

        valorImpostos = Globalize.parseFloat($("#txtValorICMS").val());

    } else if (configuracaoEmpresa != null && configuracaoEmpresa.OptanteSimplesNacional && percentualImpostoSimplesNacional > 0) {

        valorImpostos = (valorAReceber * (percentualImpostoSimplesNacional / 100));

        var valorFrete = valorAReceber - valorImpostos;

        $("#txtValorFreteContratado").val(Globalize.format(valorFrete, "n2"));

        incluiValorAReceber = true;

        if (indiceFreteValor > -1)
            componentesDaPrestacao[indiceFreteValor].Valor = valorFrete;
    }

    if (componente == null) {
        componente = {
            Valor: valorImpostos,
            Descricao: "IMPOSTOS",
            Excluir: false,
            IncluiBaseCalculoICMS: false,
            IncluiValorAReceber: incluiValorAReceber,
            Id: indice < 0 ? 0 : componentesDaPrestacao[indice].Id
        };
    }

    if (componente.Id == 0)
        componente.Id = BuscaMenorProximoCodigo(componentesDaPrestacao);

    if ($("#chkIncluirICMSNoFrete")[0].checked || (configuracaoEmpresa != null && configuracaoEmpresa.OptanteSimplesNacional && percentualImpostoSimplesNacional > 0)) {
        if (indice < 0)
            componentesDaPrestacao.push(componente);
        else
            componentesDaPrestacao[indice] = componente;
    } else {
        if (indice > -1) {
            if (componentesDaPrestacao[indice].Id < 0)
                componentesDaPrestacao.splice(indice, 1);
            else
                componentesDaPrestacao[indice].Excluir = true;
        }
    }

    $("#hddComponentesDaPrestacao").val(JSON.stringify(componentesDaPrestacao));

    RenderizarComponentesDaPrestacao();
}

function CalcularAlteracaoPercentualRecolhido() {
    CalcularInclusaoDoICMSNoFrete();
}

function ResetarCalculoPercentualICMSRecolhido() {
    var valorFreteContratado = Globalize.parseFloat($("#txtValorFreteContratado").val());
    $("#txtPercentualICMSRecolhido").val("100,00");
    AtualizarValoresGerais();
}

function SalvarFreteContratado() {
    if ($("#selTipoCTE").val() == 3) { // Substituição
        var valorOriginal = Globalize.parseFloat($("#hddValorFreteContratadoOriginal").val());
        var valorFrete = Globalize.parseFloat($("#txtValorFreteContratado").val());

        //if (valorFrete > valorOriginal) {
        //    jAlert("O valor do frete contratado não pode ser maior do que <b>" + Globalize.format(valorOriginal, "n2") + "</b>, pois este CT-e é de Substituição.", "Atenção");
        //    $("#txtValorFreteContratado").val(Globalize.format(valorOriginal, "n2"));
        //}
    }

    var componente = {
        Id: 0,
        Descricao: "FRETE VALOR",
        Valor: Globalize.parseFloat($("#txtValorFreteContratado").val()),
        IncluiBaseCalculoICMS: false,
        IncluiValorAReceber: true,
        Excluir: false
    };

    var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());

    for (var i = 0; i < componentesDaPrestacao.length; i++) {
        if (componentesDaPrestacao[i].Descricao == "VALOR FRETE" || componentesDaPrestacao[i].Descricao == "FRETE VALOR") {
            componente.Id = componentesDaPrestacao[i].Id;
            componentesDaPrestacao.splice(i, 1);
            break;
        }
    }

    componentesDaPrestacao.push(componente);

    $("#hddComponentesDaPrestacao").val(JSON.stringify(componentesDaPrestacao));

    RenderizarComponentesDaPrestacao();
    AtualizarValoresGerais();
}

function ValidarCamposComponenteDaPrestacao() {
    var descricao = $("#txtDescricaoComponentePrestacaoServico").val();
    var valor = Globalize.parseFloat($("#txtValorComponentePrestacaoServico").val());
    var valido = true;

    if (descricao == "") {
        CampoComErro("#txtDescricaoComponentePrestacaoServico");
        valido = false;
    } else {
        CampoSemErro("#txtDescricaoComponentePrestacaoServico");
    }

    if (valor <= 0 || isNaN(valor)) {
        CampoComErro("#txtValorComponentePrestacaoServico");
        valido = false;
    } else {
        CampoSemErro("#txtValorComponentePrestacaoServico");
    }

    return valido;
}

function SalvarComponenteDaPrestacao() {
    if (ValidarCamposComponenteDaPrestacao()) {
        var componente = {
            Id: Globalize.parseInt($("#hddIdComponenteDaPrestacaoEmEdicao").val()),
            Descricao: $("#txtDescricaoComponentePrestacaoServico").val(),
            Valor: Globalize.parseFloat($("#txtValorComponentePrestacaoServico").val()),
            IncluiBaseCalculoICMS: $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS")[0].checked,
            IncluiValorAReceber: $("#chkIncluirValorComponentePrestacaoNoTotalAReceber")[0].checked,
            Excluir: false
        };

        if (componente.Descricao.toUpperCase() == "VALOR FRETE" || componente.Descricao.toUpperCase() == "FRETE VALOR" || componente.Descricao.toUpperCase() == "IMPOSTOS") {
            jAlert("Não é possível salvar um componente com a descrição " + componente.Descricao.toUpperCase() + "!", "Atenção");
            return;
        }

        var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());

        if (componente.Id == 0) {
            componente.Id = BuscaMenorProximoCodigo(componentesDaPrestacao);
        }

        for (var i = 0; i < componentesDaPrestacao.length; i++) {
            if (componentesDaPrestacao[i].Id == componente.Id) {
                componentesDaPrestacao.splice(i, 1);
                break;
            }
        }

        componentesDaPrestacao.push(componente);

        $("#hddComponentesDaPrestacao").val(JSON.stringify(componentesDaPrestacao));

        RenderizarComponentesDaPrestacao();
        LimparCamposComponenteDaPrestacao();
        AtualizarValoresGerais();
    }
}

function AtualizarValoresGerais() {
    var dadosEmpresa = JSON.parse($("#hddDadosEmpresa").val());

    var valorFreteContratado = 0;
    var valorAReceber = 0;
    var valorBaseCalculoICMS = 0;
    var valorCreditoICMS = Globalize.parseFloat($("#txtValorCreditoICMS").val());
    if (dadosEmpresa.NaoSmarCreditoICMSNoValorDaPrestacao)
        valorCreditoICMS = 0;

    var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());

    for (var i = 0; i < componentesDaPrestacao.length; i++) {

        if (componentesDaPrestacao[i].Excluir == false) {

            if (componentesDaPrestacao[i].Descricao == "VALOR FRETE" || componentesDaPrestacao[i].Descricao == "FRETE VALOR") {
                valorFreteContratado += componentesDaPrestacao[i].Valor;
                valorBaseCalculoICMS += componentesDaPrestacao[i].Valor;
            }

            if (componentesDaPrestacao[i].IncluiBaseCalculoICMS)
                valorBaseCalculoICMS += componentesDaPrestacao[i].Valor;

            if (componentesDaPrestacao[i].IncluiValorAReceber)
                valorAReceber += componentesDaPrestacao[i].Valor;
        }
    }

    var percentualReducaoBaseCalculoICMS = Globalize.parseFloat($("#txtReducaoBaseCalculoICMS").val());

    valorBaseCalculoICMS -= valorBaseCalculoICMS * (percentualReducaoBaseCalculoICMS / 100);

    $("#txtValorBaseCalculoICMS").val(Globalize.format(valorBaseCalculoICMS, "n2"));
    $("#txtValorAReceber").val(Globalize.format((valorAReceber + valorCreditoICMS), "n2"));
    $("#txtValorTotalPrestacaoServico").val($("#txtValorAReceber").val());
    $("#txtValorBaseCalculoIR").val(Globalize.format(valorAReceber, "n2"));
    $("#txtValorBaseCalculoINSS").val(Globalize.format(valorAReceber, "n2"));
    $("#txtValorBaseCalculoCSLL").val(Globalize.format(valorAReceber, "n2"));

    CalcularIR();
    CalcularINSS();
    CalcularCSLL();

    if ($("#chkIncluirICMSNoFrete")[0].checked)
        CalcularInclusaoDoICMSNoFrete();
    else {
        CalcularICMS();

        var valorICMS = Globalize.parseFloat($("#txtValorICMS").val());

        $("#txtValorBaseCalculoPIS").val(Globalize.format(valorAReceber - valorICMS, "n2"));
        $("#txtValorBaseCalculoCOFINS").val(Globalize.format(valorAReceber - valorICMS, "n2"));
        CalcularPIS();
        CalcularCOFINS();
    }

}

function ExcluirComponenteDaPrestacao() {
    jConfirm("Deseja realmente excluir este componente da prestação?", "Atenção", function (r) {
        if (r) {
            var id = Globalize.parseInt($("#hddIdComponenteDaPrestacaoEmEdicao").val());

            var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());

            for (var i = 0; i < componentesDaPrestacao.length; i++) {
                if (componentesDaPrestacao[i].Id == id) {
                    if (componentesDaPrestacao[i].Descricao.toUpperCase() == "VALOR FRETE" || componentesDaPrestacao[i].Descricao.toUpperCase() == "FRETE VALOR" || componentesDaPrestacao[i].Descricao.toUpperCase() == "IMPOSTOS") {
                        jAlert("Não é possível excluir este componente.", "Atenção");
                        return;
                    }

                    if (id > 0) {
                        componentesDaPrestacao[i].Excluir = true;
                    } else {
                        componentesDaPrestacao.splice(i, 1);
                    }

                    break;
                }
            }

            $("#hddComponentesDaPrestacao").val(JSON.stringify(componentesDaPrestacao));

            RenderizarComponentesDaPrestacao();
            LimparCamposComponenteDaPrestacao();
            AtualizarValoresGerais();
        }
    });
}

function EditarComponenteDaPrestacao(componente) {
    $("#hddIdComponenteDaPrestacaoEmEdicao").val(componente.Id);
    $("#txtDescricaoComponentePrestacaoServico").val(componente.Descricao);
    $("#txtValorComponentePrestacaoServico").val(Globalize.format(componente.Valor, "n2"));
    $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", componente.IncluiBaseCalculoICMS);
    $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", componente.IncluiValorAReceber);
    $("#btnExcluirComponentePrestacaoServico").show();
}

function LimparCamposComponenteDaPrestacao() {
    $("#hddIdComponenteDaPrestacaoEmEdicao").val('0');
    $("#txtDescricaoComponentePrestacaoServico").val('');
    $("#txtValorComponentePrestacaoServico").val('0,00');
    $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", true);
    $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", true);
    $("#btnExcluirComponentePrestacaoServico").hide();
}

function RenderizarComponentesDaPrestacao() {
    $("#tblComponentesPrestacaoServico tbody").html("");
    var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());
    for (var i = 0; i < componentesDaPrestacao.length; i++) {
        if (!componentesDaPrestacao[i].Excluir) {
            $("#tblComponentesPrestacaoServico tbody").append("<tr>" +
                "<td class=\"text-uppercase\">" + componentesDaPrestacao[i].Descricao + "</td>" +
                "<td>" + Globalize.format(componentesDaPrestacao[i].Valor, "n2") + "</td>" +
                "<td>" + (componentesDaPrestacao[i].IncluiBaseCalculoICMS ? "Sim" : "Não") + "</td>" +
                "<td>" + (componentesDaPrestacao[i].IncluiValorAReceber ? "Sim" : "Não") + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarComponenteDaPrestacao(" + JSON.stringify(componentesDaPrestacao[i]) + ")'>Editar</button></td>" +
                "</tr>");
        }
    }
    if ($("#tblComponentesPrestacaoServico tbody").html() == "") {
        $("#tblComponentesPrestacaoServico tbody").html("<tr><td colspan='5'>Nenhum registro encontrado!</td></tr>");
    }

    RenderizarComponentesDaPrestacaoResumo();
}

function TrocarICMS(icms, atualizaValoresGerais) {
    switch (icms) {
        case "0":
            EsconderICMS(atualizaValoresGerais);
            HabilitarCamposImpostosResumo(true);

            break;
        case "1":
            ICMSNormal(atualizaValoresGerais);
            HabilitarCamposImpostosResumo(false);

            break;
        case "2":
            ICMSReducao(atualizaValoresGerais);
            HabilitarCamposImpostosResumo(false);

            break;
        case "3":
            ICMSIsencao(atualizaValoresGerais);
            HabilitarCamposImpostosResumo(true);

            break;
        case "4":
            ICMSNaoTributado(atualizaValoresGerais);
            HabilitarCamposImpostosResumo(true);

            break;
        case "5":
            ICMSDiferido(atualizaValoresGerais);
            HabilitarCamposImpostosResumo(true);

            break;
        case "6":
            ICMSPagamentoAtribuidoTomador(atualizaValoresGerais);
            HabilitarCamposImpostosResumo(false);

            break;
        case "9":
            ICMSOutrasSituacoes(atualizaValoresGerais);
            HabilitarCamposImpostosResumo(false);

            break;
        case "10":
            ICMSDevidoAUFOrigem(atualizaValoresGerais);
            HabilitarCamposImpostosResumo(false);

            break;
        case "11":
            ICMSSimplesNacional(atualizaValoresGerais);
            HabilitarCamposImpostosResumo(true);

            break;
    }
}

function EsconderICMS(atualizaValoresGerais) {
    $("#divDadosICMS").hide();
    $("#divValorCreditoICMS").hide();
    $("#divReducaoBaseCalculoICMS").hide();
    $("#divValorBaseCalculoICMS").hide();
    $("#divAliquotaICMS").hide();
    $("#divValorICMS").hide();
    $("#divExibirICMSNaDACTE").hide();
    $("#divValorICMSDesoneracao").hide();
    $("#divCodigoBeneficio").hide();

    if (atualizaValoresGerais)
        AtualizarValoresGerais();
}

function ExibirICMS(atualizaValoresGerais) {
    $("#divDadosICMS").show();
    if (atualizaValoresGerais)
        AtualizarValoresGerais();
}

function ICMSNormal(atualizaValoresGerais) {
    $("#divReducaoBaseCalculoICMS").hide();
    $("#divValorCreditoICMS").hide();
    $("#divValorBaseCalculoICMS").show();
    $("#divAliquotaICMS").show();
    $("#divValorICMS").show();
    $("#divExibirICMSNaDACTE").hide();
    $("#divValorICMSDesoneracao").hide();
    $("#divCodigoBeneficio").hide();
    ExibirICMS(atualizaValoresGerais);
}

function ICMSReducao(atualizaValoresGerais) {
    $("#divValorCreditoICMS").hide();
    $("#divReducaoBaseCalculoICMS").show();
    $("#divValorBaseCalculoICMS").show();
    $("#divAliquotaICMS").show();
    $("#divValorICMS").show();
    $("#divExibirICMSNaDACTE").hide();
    $("#divValorICMSDesoneracao").show();
    $("#divCodigoBeneficio").show();
    ExibirICMS(atualizaValoresGerais);
}

function ICMSIsencao(atualizaValoresGerais) {
    CSTGrupoICMS45(atualizaValoresGerais);
}

function ICMSNaoTributado(atualizaValoresGerais) {
    CSTGrupoICMS45(atualizaValoresGerais);
}

function ICMSDiferido(atualizaValoresGerais) {
    CSTGrupoICMS45(atualizaValoresGerais);
}

function ICMSPagamentoAtribuidoTomador(atualizaValoresGerais) {
    $("#divValorCreditoICMS").show();
    $("#divReducaoBaseCalculoICMS").show();
    $("#divValorBaseCalculoICMS").show();
    $("#divAliquotaICMS").show();
    $("#divValorICMS").show();
    $("#divExibirICMSNaDACTE").hide();
    $("#divValorICMSDesoneracao").show();
    $("#divCodigoBeneficio").show();
    ExibirICMS(atualizaValoresGerais);
}

function ICMSOutrasSituacoes(atualizaValoresGerais) {
    $("#divValorCreditoICMS").show();
    $("#divReducaoBaseCalculoICMS").show();
    $("#divValorBaseCalculoICMS").show();
    $("#divAliquotaICMS").show();
    $("#divValorICMS").show();
    $("#divExibirICMSNaDACTE").show();
    $("#divValorICMSDesoneracao").show();
    $("#divCodigoBeneficio").show();
    ExibirICMS(atualizaValoresGerais);
}

function ICMSDevidoAUFOrigem(atualizaValoresGerais) {
    $("#divValorCreditoICMS").hide();
    $("#divReducaoBaseCalculoICMS").show();
    $("#divValorBaseCalculoICMS").show();
    $("#divAliquotaICMS").show();
    $("#divValorICMS").show();
    $("#divExibirICMSNaDACTE").show();
    $("#divValorICMSDesoneracao").show();
    $("#divCodigoBeneficio").show();
    ExibirICMS(atualizaValoresGerais);
}

function ICMSSimplesNacional(atualizaValoresGerais) {
    EsconderICMS(atualizaValoresGerais);
}

function BuscarDadosImpostos(recalculadoManual) {
    $('#divBuscarImpostos').find('span').remove();
    var dados = {
        TipoServico: $("#selTipoServico").val(),
        TomadorServico: $("#selTomadorServico").val(),
        Tomador: JSON.stringify(ObterTomador()),
        Remetente: JSON.stringify(ObterRemetente()),
        Expedidor: JSON.stringify(ObterExpedidor()),
        Recebedor: JSON.stringify(ObterRecebedor()),
        Destinatario: JSON.stringify(ObterDestinatario()),
        MunicipioLocalEmissaoCTe: $("#ddlMunicipioLocalEmissaoCTe").val(),
        MunicipioInicioPrestacao: $("#ddlMunicipioInicioPrestacao").val(),
        MunicipioTerminoPrestacao: $("#ddlMunicipioTerminoPrestacao").val(),
        ICMS: $("#selICMS").val()
    };

    executarRest("/ConhecimentoDeTransporteEletronico/BuscarDadosImpostos?callback=?", dados, function (r) {
        if (r.Sucesso) {
            var dadosEmpresa = $("#hddDadosEmpresa").val() == "" ? null : JSON.parse($("#hddDadosEmpresa").val());

            if (recalculadoManual || $("#selCFOP").val() == "" || $("#selCFOP").val() == "0") {
                if (r.Objeto.CodigoNatureza > 0)
                    $("#ddlNaturezaOperacao").val(r.Objeto.CodigoNatureza);
                if (r.Objeto.CodigoCFOP > 0)
                    BuscarCFOPs(r.Objeto.CodigoCFOP);
            }

            if (r.Objeto.CSTICMS > 0) {
                $("#selICMS").val(r.Objeto.CSTICMS);
                TrocarICMS(r.Objeto.CSTICMS.toString(), false);
            }

            if (r.Objeto.AliquotaICMS > 0)
                $("#selAliquotaICMS").val(Globalize.format(r.Objeto.AliquotaICMS, "n2"));
            else if (recalculadoManual)
                $("#selAliquotaICMS").val(Globalize.format(r.Objeto.AliquotaICMS, "n2"));

            if (r.Objeto.ZerarBaseICMS) {
                $("#selAliquotaICMS").val("0,00");
                $("#txtValorBaseCalculoICMS").val("0,00");
            }

            if (r.Objeto.PercentualReducaoBC > 0)
                $("#txtReducaoBaseCalculoICMS").val(Globalize.format(r.Objeto.PercentualReducaoBC, "n2"));
            else if (recalculadoManual)
                $("#txtReducaoBaseCalculoICMS").val(Globalize.format(r.Objeto.PercentualReducaoBC, "n2"));

            AtualizarValoresGerais();

            if (r.Objeto.ZerarBaseICMS) {
                $("#selAliquotaICMS").val("0,00");
                $("#txtValorBaseCalculoICMS").val("0,00");
            }

            if ((dadosEmpresa != null && dadosEmpresa.RegimeTributarioCTe != 3) || !$("#txtIBSCBS_Class").val())
                LimparCamposIbsCbs();
            else
                BuscarAliquotasPorClass();
        }
        else {
            $('#divBuscarImpostos').find('span').remove();
            $("#divBuscarImpostos").prepend('<span class="label label-info" style="padding-top: 4px; padding-bottom: 4px;">' + r.Erro + '</span>');
        }
    });
}


function VincularEventoBuscarDadosImpostosNaAbaServicosEImpostos() {
    $('a[data-toggle="tab"][href="#tabServicosEImpostos"]').on('shown.bs.tab', function (e) {
        var icms = $("#selICMS").val();
        if (icms == "0")
            BuscarDadosImpostos(false);
        else
            $('#divBuscarImpostos').find('span').remove();
    });
}

function CSTGrupoICMS45(atualizaValoresGerais) {
    $("#divDadosICMS").show();
    $("#divValorCreditoICMS").hide();
    $("#divReducaoBaseCalculoICMS").hide();
    $("#divValorBaseCalculoICMS").hide();
    $("#divAliquotaICMS").hide();
    $("#divValorICMS").hide();
    $("#divExibirICMSNaDACTE").hide();
    $("#divValorICMSDesoneracao").show();
    $("#divCodigoBeneficio").show();

    if (atualizaValoresGerais)
        AtualizarValoresGerais();
}

function CalcularImpostosDeIBSeCBS(impostos) {
    $("#txtIBSCBSBaseCalculo").val(Globalize.format(impostos.baseCalculo, "n2"));

    $("#txtIBSEstadualAliquota").val(Globalize.format(impostos.ibs.estadual.Aliquota, "n4"));
    $("#txtIBSEstadualReducao").val(Globalize.format(impostos.ibs.estadual.Reducao, "n4"));
    $("#txtIBSEstadualEfetiva").val(Globalize.format(impostos.ibs.estadual.Efetiva, "n4"));
    $("#txtIBSEstadualValor").val(Globalize.format(impostos.ibs.estadual.Valor, "n2"));

    $("#txtIBSMunAliquota").val(Globalize.format(impostos.ibs.municipal.Aliquota, "n4"));
    $("#txtIBSMunReducao").val(Globalize.format(impostos.ibs.municipal.Reducao, "n4"));
    $("#txtIBSMunEfetiva").val(Globalize.format(impostos.ibs.municipal.Efetiva, "n4"));
    $("#txtIBSMunValor").val(Globalize.format(impostos.ibs.municipal.Valor, "n2"));

    $("#txtCBSAliquota").val(Globalize.format(impostos.cbs.Aliquota, "n4"));
    $("#txtCBSReducao").val(Globalize.format(impostos.cbs.Reducao, "n4"));
    $("#txtCBSEfetiva").val(Globalize.format(impostos.cbs.Efetiva, "n4"));
    $("#txtCBSValor").val(Globalize.format(impostos.cbs.Valor, "n2"));
}

function LimparCamposIbsCbs(somenteValores = false) {
    if (!somenteValores) {
        $("#txtIBSCBS_CST").val("");
        $("#txtIBSCBS_Class").val("");
    }

    $("#txtIBSCBSBaseCalculo").val("0,00");

    $("#txtIBSEstadualAliquota").val("0,00");
    $("#txtIBSEstadualReducao").val("0,00");
    $("#txtIBSEstadualEfetiva").val("0,00");
    $("#txtIBSEstadualValor").val("0,00");

    $("#txtIBSMunAliquota").val("0,00");
    $("#txtIBSMunReducao").val("0,00");
    $("#txtIBSMunEfetiva").val("0,00");
    $("#txtIBSMunValor").val("0,00");

    $("#txtCBSAliquota").val("0,00");
    $("#txtCBSReducao").val("0,00");
    $("#txtCBSEfetiva").val("0,00");
    $("#txtCBSValor").val("0,00");
}
