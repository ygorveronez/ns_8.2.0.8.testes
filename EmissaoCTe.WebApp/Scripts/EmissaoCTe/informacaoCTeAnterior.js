$(document).ready(function () {
    $("#ddlUFInicioPrestacao").change(function () {
        ObterDadosSemelhantesPorEstado();
    });
    $("#ddlUFTerminoPrestacao").change(function () {
        ObterDadosSemelhantesPorEstado();
    });


    $("#btnImportarInfosSemelhantesDoCTe").click(function () {
        PreencherDadosSemelhantesCTe();
    });
    $("#btnNaoImportarInfosSemelhantesDoCTe").click(function () {
        $("#divDadosSimilaresCTe").modal("hide");
        BuscarImpostosPadrao();
    });
    $("#divDadosSimilaresCTe").on("shown.bs.modal", function () {
        $("#btnImportarInfosSemelhantesDoCTe").focus();
    });
});
function ObterDadosSemelhantesPorEstado() {
    // Nao importa dados quando cte for do tipo complementar
    if ($("body").data("ctecomplementar") == "complementar")
        return false;

    if (!$("#divDadosSimilaresCTe").hasClass("in")) {
        var valorFrete = Globalize.parseFloat($("#txtValorFreteContratado").val());
        var cpfCnpjRemetente = $("#hddRemetente").val();
        var cpfCnpjDestinatario = $("#hddDestinatario").val();

        if (cpfCnpjDestinatario != "" && cpfCnpjRemetente != "") {

            executarRest("/ConhecimentoDeTransporteEletronico/ObterInformacoesReferencia?callback=?", { EstadoInicioPrestacao: $("#ddlUFInicioPrestacao").val(), EstadoFimPrestacao: $("#ddlUFTerminoPrestacao").val(), CPFCNPJRemetente: $("#hddRemetente").val(), CPFCNPJDestinatario: $("#hddDestinatario").val(), CPFCNPJExpedidor: $("#hddExpedidor").val(), CPFCNPJRecebedor: $("#hddRecebedor").val(), LocalidadeInicioPrestacao: $("#ddlMunicipioInicioPrestacao").val(), LocalidadeTerminoPrestacao: $("#ddlMunicipioTerminoPrestacao").val() }, function (r) {
                if (r.Sucesso) {
                    $("#divDadosSimilaresCTe").data("dadosCTeSemelhante", r.Objeto);
                    PreencherDadosSemelhantesCTe();
                }
                else
                {
                    executarRest("/ConhecimentoDeTransporteEletronico/ObterInformacoesSemelhantes?callback=?", { EstadoInicioPrestacao: $("#ddlUFInicioPrestacao").val(), EstadoFimPrestacao: $("#ddlUFTerminoPrestacao").val(), CPFCNPJRemetente: $("#hddRemetente").val(), CPFCNPJDestinatario: $("#hddDestinatario").val() }, function (r) {
                        if (r.Sucesso) {
                            $("#divDadosSimilaresCTe").data("dadosCTeSemelhante", r.Objeto);
                            var msg = "Existe um CT-e semelhante a este já emitido no sistema: <br/><br/>Número: <b>" + r.Objeto.NumeroCTe + "</b><br/>Valor do Frete: <b>" + Globalize.format(r.Objeto.ValorFrete, "n2") + "</b><br/>CST do ICMS: <b>" + $("#selICMS option[value='" + r.Objeto.CSTICMS + "']").text() + "</b><br/>Alíquota do ICMS: <b>" + Globalize.format(r.Objeto.AliquotaICMS, "n2") + "</b><br/>Remetente: <b>" + r.Objeto.Remetente + "</b><br/>Destinatário: <b>" + r.Objeto.Destinatario + "</b><br/><br/>Deseja importar as informações principais deste CT-e?";

                            $("#infoCTeSemelhante").html(msg);
                            $("#divDadosSimilaresCTe").modal("show");
                        }
                    });
                }
            });
        }
    }
}

function PreencherDadosSemelhantesCTe() {
    var dados = $("#divDadosSimilaresCTe").data("dadosCTeSemelhante");

    if (dados != null) {

        var valorFrete = Globalize.parseFloat($("#txtValorFreteContratado").val());

        if (dados.CodigoNatureza > 0)
            $("#ddlNaturezaOperacao").val(dados.CodigoNatureza);
        if (dados.CodigoCFOP > 0)
            BuscarCFOPs(dados.CodigoCFOP);

        if (dados.IncluirICMS == 1) {
            $("#chkIncluirICMSNoFrete").prop("checked", true);
            $("#chkIncluirICMSNoFreteResumo").prop("checked", true);
            $("#divPercentualICMSRecolhido").show();
        } else {
            $("#chkIncluirICMSNoFrete").prop("checked", false);
            $("#chkIncluirICMSNoFreteResumo").prop("checked", false);
            $("#divPercentualICMSRecolhido").hide();
        }

        if (dados.Serie != null && dados.Serie > 0)
            $("#ddlSerie").val(dados.Serie);

        if ($("#ddlSerie").val() == null || $("#ddlSerie").val() == "")
            $("#ddlSerie").val($("#ddlSerie option:first").val());

        if (dados.CSTICMS > 0) {
            $("#selICMS").val(dados.CSTICMS);
            $("#selICMSResumo").val(dados.CSTICMS);
            TrocarICMS(dados.CSTICMS.toString(), false);
        }

        if (dados.AliquotaICMS > 0) {
            $("#selAliquotaICMS").val(Globalize.format(dados.AliquotaICMS, "n2"));
            $("#selAliquotaICMSResumo").val(Globalize.format(dados.AliquotaICMS, "n2"));
        }

        $("#chkExibirICMSNaDACTE").prop("checked", dados.ExibeICMSNaDACTE);

        if (dados.ClientesIguais) {

            $("#selPago_APagar").val(dados.TipoPagamento);
            $("#selTomadorServico").val(dados.TipoTomador);
            $("#selTomadorServicoResumo").val(dados.TipoTomador);
            $("#selTipoServico").val(dados.TipoServico);

            BloquearAbaTomador();

            var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());
            //Exclui componentes já inseridos quando tiver componentes do CTe anterior
            if (dados.ComponentesPrestacao.length > 0) {
                for (var i = 0; i < componentesDaPrestacao.length; i++) {
                    if (componentesDaPrestacao[i].Descricao != "FRETE VALOR" && componentesDaPrestacao[i].Descricao != "VALOR FRETE" && componentesDaPrestacao[i].Descricao != "IMPOSTOS") {
                        if (componentesDaPrestacao[i].Id > 0) {
                            componentesDaPrestacao[i].Excluir = true;
                        } else {
                            componentesDaPrestacao.splice(i, 1);
                        }
                    }
                }
            }

            // Remove componentes repetidos do CTe anterior
            var componentesInseridos = [];
            var componentesParaInserir = dados.ComponentesPrestacao.filter(function (componentes) {
                if ($.inArray(componentes.Descricao.toLowerCase(), componentesInseridos) < 0) {
                    componentesInseridos.push(componentes.Descricao.toLowerCase());
                    return true;
                }
                return false;
            });

            for (var i = 0; i < componentesParaInserir.length; i++) {
                if (componentesParaInserir[i].Descricao != "FRETE VALOR" && componentesParaInserir[i].Descricao != "VALOR FRETE" && componentesParaInserir[i].Descricao != "IMPOSTOS")
                    componentesDaPrestacao.push(componentesParaInserir[i]);
            }
            
            $("#hddComponentesDaPrestacao").val(JSON.stringify(componentesDaPrestacao));
            RenderizarComponentesDaPrestacao();

            $("#hddObservacoesFisco").val(JSON.stringify(dados.ObservacoesFisco));
            RenderizarObservacoesFisco();

            $("#hddObservacoesContribuinte").val(JSON.stringify(dados.ObservacoesContribuinte));
            RenderizarObservacoesContribuinte();

            $("#hddInformacoesSeguro").val(JSON.stringify(dados.InformacoesSeguro))
            RenderizarInformacaoSeguro();

            if (valorFrete == 0) {
                valorFrete = dados.ValorFrete;
                $("#txtValorFreteContratado").val(Globalize.format(dados.ValorFrete, "n2"));
            }
            $("#txtPercentualICMSRecolhido").val(Globalize.format(dados.PercentualIncluirICMS, "n2"));

            if ($("#chkImportarObservacaoGeral")[0].checked)
                $("#txtObservacaoGeral").val(dados.ObservacoesGerais);

            //$("#txtProdutoPredominante").val(dados.ProdutoPredominante);

            if (valorFrete > 0) {
                SalvarFreteContratado();
                SetarCreditoPresumido();
            }

            AtualizarValoresGerais();

            BuscarTabelaFrete();
        }

        $("#hddCodigoCTEReferenciado").val(dados.CodigoCTEReferenciado);
        if (dados.CodigoCTEReferenciado > 0) {            
            ControlarCamposResumo(true);
        }

    }

    $("#divDadosSimilaresCTe").modal("hide");

    $("#divDadosSimilaresCTe").data("dadosCTeSemelhante", null);
}