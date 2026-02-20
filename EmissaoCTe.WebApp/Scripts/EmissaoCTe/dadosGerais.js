$(document).ready(function () {
    ObterAliquotasDoICMS();

    $("#ddlUFLocalEmissaoCTe").change(function () {
        BuscarLocalidades($(this).val(), "ddlMunicipioLocalEmissaoCTe", null);
    });
    $("#ddlUFInicioPrestacao").change(function () {
        BuscarLocalidades($(this).val(), "ddlMunicipioInicioPrestacao", null);
    });
    $("#ddlUFTerminoPrestacao").change(function () {
        BuscarLocalidades($(this).val(), "ddlMunicipioTerminoPrestacao", null);
    });
    $("#selPago_APagar").change(function () {
        TrocarTomador();
    });
    $("#selTomadorServico").change(function () {//.on('change', function () {
        SetarIndicadorTomador();
        $("#selTomadorServicoResumo").val($("#selTomadorServico").val());
    });
    $("#ddlModelo").on('change', function () {
        ControlarCamposCTeOS(true);
    });
});

function SetarInicioPrestacao() {
    if ($("#selCidadeExpedidor").val() != null) {
        if ($("#ddlUFInicioPrestacao").val() != $("#ddlEstadoExpedidor").val()) {
            $("#ddlUFInicioPrestacao").val($("#ddlEstadoExpedidor").val());

            BuscarLocalidades($("#ddlUFInicioPrestacao").val(), "ddlMunicipioInicioPrestacao", $("#selCidadeExpedidor").val());
        }

        $("#ddlMunicipioInicioPrestacao").val($("#selCidadeExpedidor").val());
    }
    else if ($("#selCidadeRemetente").val() != null) {
        if ($("#ddlUFInicioPrestacao").val() != $("#ddlEstadoRemetente").val()) {
            $("#ddlUFInicioPrestacao").val($("#ddlEstadoRemetente").val());

            BuscarLocalidades($("#ddlUFInicioPrestacao").val(), "ddlMunicipioInicioPrestacao", $("#selCidadeRemetente").val());
        }

        $("#ddlMunicipioInicioPrestacao").val($("#selCidadeRemetente").val());
    }
}

function SetarTerminoPrestacao() {
    if ($("#selCidadeRecebedor").val() != null) {
        if ($("#ddlUFTerminoPrestacao").val() != $("#ddlEstadoRecebedor").val()) {
            $("#ddlUFTerminoPrestacao").val($("#ddlEstadoRecebedor").val());

            BuscarLocalidades($("#ddlUFTerminoPrestacao").val(), "ddlMunicipioTerminoPrestacao", $("#selCidadeRecebedor").val());
        }

        $("#ddlMunicipioTerminoPrestacao").val($("#selCidadeRecebedor").val());
    }
    else if ($("#selCidadeDestinatario").val() != null) {
        if ($("#ddlUFTerminoPrestacao").val() != $("#ddlEstadoDestinatario").val()) {
            $("#ddlUFTerminoPrestacao").val($("#ddlEstadoDestinatario").val());

            BuscarLocalidades($("#ddlUFTerminoPrestacao").val(), "ddlMunicipioTerminoPrestacao", $("#selCidadeDestinatario").val());
        }

        $("#ddlMunicipioTerminoPrestacao").val($("#selCidadeDestinatario").val());
    }
}

function TrocarTomador() {
    switch ($("#selPago_APagar").val()) {
        case "0": //pago
            $("#selTomadorServico").val('0');
            break;
        case "1": //a pagar
            $("#selTomadorServico").val('3');
            break;
        case "2": //outros
            $("#selTomadorServico").val('4');
            break;
    }
    BloquearAbaTomador();
    SetarIndicadorTomador();
}

function SetarIndicadorTomador() {
    /**
     * Funcao dispara quando:
     * - Tomador modificado
     * - Atividade selecionada (Remetente/Expedidor/Recebedor/Destinatário/Outros)
     * - Inscricao Estadual modificada (Remetente/Expedidor/Recebedor/Destinatário/Outros)
     * - Remetente/Destinatario/Expedidor/Recebedor/Outros selecionado
     *
     * Regra de Negocio: 
     * - Se a atividade do tomador for 7 (Nao Contribuinte) indicacao deve ser => "Nao Contribuinte";
     * - Se tiver Inscricao Estadual indicacao deve ser => "Contribuinte ICMS" 
     * - Se for pessoa fisica e nao tiver inscricao  => "Nao Contribuinte" 
     * - Padrao => "Contribuinte ICMS"
     */
    var enumTomador = {
        Remetente: 0,
        Expedidor: 1,
        Recebedor: 2,
        Destinatario: 3,
        Outros: 4
    };

    var enumIndicadorTomador = {
        Contribuinte_ICMS: 1,
        Isento_Inscricao: 2,
        Nao_Contribuinte: 9
    };

    var tomador = parseInt($("#selTomadorServico").val());
    var selIndIEToma = $("#selIndIEToma");
    var codigoAtividade = 0;
    var ieTomador = "";
    var documentoPessoa = "";

    if (tomador == enumTomador.Remetente) {
        codigoAtividade = $("#hddAtividadeRemetente").val();
        ieTomador = $("#txtRGIERemetente").val();
        documentoPessoa = $("#txtCPFCNPJRemetente").val();

    } else if (tomador == enumTomador.Expedidor) {
        codigoAtividade = $("#hddAtividadeExpedidor").val();
        ieTomador = $("#txtRGIEExpedidor").val();
        documentoPessoa = $("#txtCPFCNPJExpedidor").val();

    } else if (tomador == enumTomador.Recebedor) {
        codigoAtividade = $("#hddAtividadeRecebedor").val();
        ieTomador = $("#txtRGIERecebedor").val();
        documentoPessoa = $("#txtCPFCNPJRecebedor").val();

    } else if (tomador == enumTomador.Destinatario) {
        codigoAtividade = $("#hddAtividadeDestinatario").val();
        ieTomador = $("#txtRGIEDestinatario").val();
        documentoPessoa = $("#txtCPFCNPJDestinatario").val();

    } else if (tomador == enumTomador.Outros) {
        codigoAtividade = $("#hddAtividadeTomador").val();
        ieTomador = $("#txtRGIETomador").val();
        documentoPessoa = $("#txtCPFCNPJTomador").val();

    } else {
        // Selecao invalida
        return;
    }

    codigoAtividade = parseInt(codigoAtividade);
    ieTomador = ieTomador.toLocaleLowerCase();
    documentoPessoa = documentoPessoa.replace(/[^0-9]/g, '');

    if (ieTomador == "isento") {
        selIndIEToma.val(enumIndicadorTomador.Isento_Inscricao);
    } else if (ieTomador != "") {
        selIndIEToma.val(enumIndicadorTomador.Contribuinte_ICMS);
    } else if (codigoAtividade == 7) {
        selIndIEToma.val(enumIndicadorTomador.Nao_Contribuinte);
    } else if (ieTomador == "") {
        selIndIEToma.val(enumIndicadorTomador.Nao_Contribuinte);
    } else {
        // Padrao
        selIndIEToma.val(enumIndicadorTomador.Contribuinte_ICMS);
    }
}

function SetarTomadorXML(formaPagamento) {
    if (formaPagamento == 0)
        $("#selPago_APagar").val('0');
    else if (formaPagamento == 1 || formaPagamento == 'A_Pagar')
        $("#selPago_APagar").val('1');
    else if (formaPagamento == 2)
        $("#selPago_APagar").val('2');
    else
        $("#selPago_APagar").val($("#selPago_APagar option:first").val());
    TrocarTomador();
}

function BuscarLocalidades(uf, idSelect, codigo) {
    executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
        if (r.Sucesso) {
            RenderizarLocalidades(r.Objeto, idSelect, codigo);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function RenderizarLocalidades(localidades, idSelect, codigo) {
    var locs = new Array();

    if (typeof idSelect == 'string')
        locs.push(idSelect);
    else
        locs = idSelect;

    for (var x = 0; x < locs.length; x++) {

        var selLocalidades = document.getElementById(locs[x]);
        selLocalidades.options.length = 0;

        for (var i = 0; i < localidades.length; i++) {

            var optn = document.createElement("option");
            optn.text = localidades[i].Descricao;
            optn.value = localidades[i].Codigo;

            selLocalidades.options.add(optn);
        }

        if (codigo != null)
            $(selLocalidades).val(codigo).change();
    }
}

function ValidarDados() {
    var naturezaDaOperacao = Globalize.parseInt($("#ddlNaturezaOperacao").val());
    var cfop = Globalize.parseInt($("#selCFOP").val());
    var dataEmissao = $("#txtDataEmissao").val();
    var horaEmissao = $("#txtHoraEmissao").val();
    var valido = true;
    if (dataEmissao == "") {
        valido = false;
        CampoComErro("#txtDataEmissao");
    } else {
        CampoSemErro("#txtDataEmissao");
    }
    if (horaEmissao == "") {
        valido = false;
        CampoComErro("#txtHoraEmissao");
    } else {
        CampoSemErro("#txtHoraEmissao");
    }
    if (naturezaDaOperacao <= 0) {
        valido = false;
        CampoComErro("#ddlNaturezaOperacao");
    } else {
        CampoSemErro("#ddlNaturezaOperacao");
    }
    if (cfop <= 0) {
        valido = false;
        CampoComErro("#selCFOP");
    } else {
        CampoSemErro("#selCFOP");
    }
    return valido;
}

function ObterAliquotasDoICMS() {
    executarRest("/AliquotaDeICMS/ObterAliquotasDaEmpresa?callback=?", {}, function (r) {
        if (r.Sucesso) {
            RenderizarAliquotasDoICMS(r.Objeto);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function RenderizarAliquotasDoICMS(aliquotas) {
    var selAliquotaICMS = document.getElementById("selAliquotaICMS");
    selAliquotaICMS.options.length = 0;

    var optn = document.createElement("option");
    optn.text = "Selecione";
    optn.value = "0";
    selAliquotaICMS.options.add(optn);

    for (var i = 0; i < aliquotas.length; i++) {
        optn = document.createElement("option");
        optn.text = Globalize.format(aliquotas[i].Aliquota, "n2") + " %";
        optn.value = Globalize.format(aliquotas[i].Aliquota, "n2");
        selAliquotaICMS.options.add(optn);
    }

    RenderizarAliquotasDoICMSResumo(aliquotas);
}

function exigePreenchimentoDoIbsCbs() {
    return $("#txtIBSCBS_CST").val() && $("#txtIBSCBS_CST").val() != "410";
}

function exibirRowIbsCbs(visible) {
    const row = document.getElementById('row-ibscbs');
    row.style.display = visible ? '' : 'none';

    if (visible) {
        $("#divBaseCalculoIbsCbs").show();
    } else {
        $("#divBaseCalculoIbsCbs").hide();
    }
}

function BuscarAliquotasPorClass() {
    if (!exigePreenchimentoDoIbsCbs()) return;

    var codigoMunicipioDestino = $("#ddlMunicipioTerminoPrestacao").val();
    var valorBaseCalculoICMS = 0;
    var naoReduzirPisCofins = false;

    var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());

    for (var i = 0; i < componentesDaPrestacao.length; i++) {

        var comp = componentesDaPrestacao[i];
        if (!comp || comp.Excluir === true) continue;

        var valor = comp.Valor;

        valorBaseCalculoICMS += valor;
    }

    var valorICMS = Globalize.parseFloat($("#txtValorICMS").val());
    var valorPIS = Globalize.parseFloat($("#txtValorPIS").val());
    var valorCOFINS = Globalize.parseFloat($("#txtValorCOFINS").val());
    var totalPISCOFINS = valorPIS + valorCOFINS;

    var totalAbater = 0;

    if (valorICMS > 0) {
        totalAbater += valorICMS;
    }

    if (totalPISCOFINS > 0) {
        totalAbater += totalPISCOFINS;
        naoReduzirPisCofins = true;
    }

    if (totalAbater > 0) {
        valorBaseCalculoICMS = Math.max(0, valorBaseCalculoICMS - totalAbater);
    }

    if (valorBaseCalculoICMS < 0) valorBaseCalculoICMS = 0;

    executarRest("/OutrasAliquotas/BuscarAliquotasPorClassificacao?callback=?", {
        codiMunicipioDestino: codigoMunicipioDestino,
        baseCalculo: valorBaseCalculoICMS,
        naoReduzirPisCofins: naoReduzirPisCofins
    }, function (r) {
        if (r.Sucesso) {
            CalcularImpostosDeIBSeCBS(r.Objeto);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function ControlarCamposCTeOS(emEdicao) {
    $("#txtNumero").prop("disabled", true);

    var cteReferencia = Globalize.parseFloat($("#hddCodigoCTEReferenciado").val());
    if (emEdicao && cteReferencia <= 0) {
        switch ($("#ddlModelo").val()) {
            case '67':
                $("#selPago_APagar").prop("disabled", true);
                $("#selPago_APagar").val("0")
                $("#selTomadorServico").prop("disabled", true);
                $("#selTomadorServico").val("0");
                $("#ddlModalTransporte").prop("disabled", true);
                $("#ddlModalTransporte").val("1");
                $("#selTipoServico").val("6");

                $("#txtValorTotalCarga").prop("disabled", true);
                $("#txtConteiner").prop("disabled", true);
                $("#txtDataPrevistaEntregaConteiner").prop("disabled", true);
                $("#txtNumeroLacre").prop("disabled", true);
                $("#txtProdutoPredominante").prop("disabled", true);
                $("#txtOutrasCaracteristicasCarga").prop("disabled", true);
                $("#txtCaracteristicaAdicionalTransporte").prop("disabled", true);
                $("#txtRNTRC").prop("disabled", true);
                $("#txtDataPrevistaEntregaCargaRecebedor").prop("disabled", true);
                $("#txtSerieCTRB").prop("disabled", true);
                $("#txtNumeroCTRB").prop("disabled", true);
                $("#txtCIOT").prop("disabled", true);
                $("#chkIndicadorLotacao").prop("disabled", true);
                $("#txtNomeMotorista").prop("disabled", true);
                $("#txtCPFMotorista").prop("disabled", true);

                $('#tabInformacaoCarga a[href="#tabProdutosPerigosos"]').hide();
                $('#tabInformacaoCarga a[href="#tabDadosCobranca"]').hide();
                $('#tabInformacaoCarga a[href="#tabPercurso"]').show();

                if ($("#selAliquotaPIS").val() == $("#selAliquotaPIS option:first").val())
                    $("#selAliquotaPIS").val("0,00");
                if ($("#selAliquotaCOFINS").val() == $("#selAliquotaCOFINS option:first").val())
                    $("#selAliquotaCOFINS").val("0,00");

                $('#tabServicosEImpostos a[href="#tabIR"]').show();
                $('#tabServicosEImpostos a[href="#tabINSS"]').show();
                $('#tabServicosEImpostos a[href="#tabCSLL"]').show();

                $("#selTipoDocumentoRemetente").prop("disabled", true);
                $("#selTipoDocumentoRemetente").val("2")
                $('#tabsRemetente a[href="#tabNFeRemetente"]').hide();
                $('#tabsRemetente a[href="#tabOutrosRemetente"]').hide();
                $('#tabsRemetente a[href="#tabNotasFiscaisRemetente"]').show();

                $("#txtSerieNotaFiscalRemetente").prop("disabled", false);
                $("#txtNumeroNotaFiscalRemetente").prop("disabled", false);
                $("#txtDataEmissaoNotaFiscalRemetente").prop("disabled", false);
                $("#txtValorNotaNotaFiscalRemetente").prop("disabled", false);

                $("#txtCFOPNotaFiscalRemetente").prop("disabled", true);
                $("#txtBaseCalculoICMSNotaFiscalRemetente").prop("disabled", true);
                $("#txtValorICMSNotaFiscalRemetente").prop("disabled", true);
                $("#txtBaseCalculoICMSSTNotaFiscalRemetente").prop("disabled", true);
                $("#txtValorICMSSTNotaFiscalRemetente").prop("disabled", true);
                $("#txtPINNotaFiscalRemetente").prop("disabled", true);
                $("#txtValorProdutosNotaFiscalRemetente").prop("disabled", true);

                $('#tabsEmissaoCTe a[href="#tabDestinatario"]').hide();
                $('#tabsEmissaoCTe a[href="#tabTomador"]').hide();
                $('#tabsEmissaoCTe a[href="#tabRecebedor"]').hide();
                $('#tabsEmissaoCTe a[href="#tabExpedidor"]').hide();
                //LimparCamposTomador();
                //LimparCamposRecebedor();
                //LimparCamposExpedidor();
                //LimparCamposDestinatario();

                $("#selTipoFretamento").prop("disabled", false);
                $("#txtDataHoraViagem").prop("disabled", false);

                $("#selUnidadeMedida").prop("disabled", true);
                $("#selUnidadeMedida").val("3");
                $("#txtTipoUnidadeMedida").val("UN");
                break;
            default:
                $("#selPago_APagar").prop("disabled", false);
                $("#selTomadorServico").prop("disabled", false);
                $("#ddlModalTransporte").prop("disabled", false);

                $("#txtValorTotalCarga").prop("disabled", false);
                $("#txtConteiner").prop("disabled", false);
                $("#txtDataPrevistaEntregaConteiner").prop("disabled", false);
                $("#txtNumeroLacre").prop("disabled", false);
                $("#txtProdutoPredominante").prop("disabled", false);
                $("#txtOutrasCaracteristicasCarga").prop("disabled", false);
                $("#txtCaracteristicaAdicionalTransporte").prop("disabled", false);
                $("#txtRNTRC").prop("disabled", false);
                $("#txtDataPrevistaEntregaCargaRecebedor").prop("disabled", false);
                $("#txtSerieCTRB").prop("disabled", false);
                $("#txtNumeroCTRB").prop("disabled", false);
                $("#txtCIOT").prop("disabled", false);
                $("#chkIndicadorLotacao").prop("disabled", false);
                $("#txtNomeMotorista").prop("disabled", false);
                $("#txtCPFMotorista").prop("disabled", false);

                $('#tabInformacaoCarga a[href="#tabProdutosPerigosos"]').show();
                $('#tabInformacaoCarga a[href="#tabDadosCobranca"]').show();
                $('#tabInformacaoCarga a[href="#tabPercurso"]').hide();

                if ($("#selAliquotaPIS").val() == $("#selAliquotaPIS option:first").val())
                    $("#selAliquotaPIS").val("0,65");
                if ($("#selAliquotaCOFINS").val() == $("#selAliquotaCOFINS option:first").val())
                    $("#selAliquotaCOFINS").val("3,00");

                $('#tabServicosEImpostos a[href="#tabIR"]').hide();
                $('#tabServicosEImpostos a[href="#tabINSS"]').hide();
                $('#tabServicosEImpostos a[href="#tabCSLL"]').hide();

                $("#selTipoDocumentoRemetente").prop("disabled", false);

                $("#txtSerieNotaFiscalRemetente").prop("disabled", false);
                $("#txtNumeroNotaFiscalRemetente").prop("disabled", false);
                $("#txtDataEmissaoNotaFiscalRemetente").prop("disabled", false);
                $("#txtValorNotaNotaFiscalRemetente").prop("disabled", false);

                $("#txtCFOPNotaFiscalRemetente").prop("disabled", false);
                $("#txtBaseCalculoICMSNotaFiscalRemetente").prop("disabled", false);
                $("#txtValorICMSNotaFiscalRemetente").prop("disabled", false);
                $("#txtBaseCalculoICMSSTNotaFiscalRemetente").prop("disabled", false);
                $("#txtValorICMSSTNotaFiscalRemetente").prop("disabled", false);
                $("#txtPINNotaFiscalRemetente").prop("disabled", false);
                $("#txtValorProdutosNotaFiscalRemetente").prop("disabled", false);

                $('#tabsEmissaoCTe a[href="#tabDestinatario"]').show();
                $('#tabsEmissaoCTe a[href="#tabRecebedor"]').show();
                $('#tabsEmissaoCTe a[href="#tabExpedidor"]').show();

                $("#selTipoFretamento").prop("disabled", true);
                $("#txtDataHoraViagem").prop("disabled", true);

                $("#selUnidadeMedida").prop("disabled", false);

                break;
        }
    }
}