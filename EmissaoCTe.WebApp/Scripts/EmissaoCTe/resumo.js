$(document).ready(function () {
    CarregarConsultaDeVeiculos("btnBuscarVeiculoResumo", "btnBuscarVeiculoResumo", RetornoConsultaVeiculoResumo, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotoristaResumo", "btnBuscarMotoristaResumo", RetornoConsultaMotoristaResumo, true, false);

    $("#txtValorFreteContratadoResumo").priceFormat({ prefix: '' });
    $("#txtValorAReceberResumo").priceFormat({ prefix: '' });    
    $("#txtValorBaseCalculoICMSResumo").priceFormat({ prefix: '' });
    $("#txtValorICMSResumo").priceFormat({ prefix: '' });
    $("#txtValorComponentePrestacaoServicoResumo").priceFormat({ prefix: '' });
    $("#txtPlacaVeiculoResumo").mask("*******");

    $("#selCFOPResumo").change(function () {
        $("#selCFOP").val($("#selCFOPResumo").val());
    });

    $("#selCFOP").change(function () {
        $("#selCFOPResumo").val($("#selCFOP").val());
    });

    $("#selTomadorServicoResumo").change(function () {
        $("#selTomadorServico").val($("#selTomadorServicoResumo").val());
        $("#selTomadorServico").change();
    });

    $("#txtValorFreteContratadoResumo").focusout(function () {
        $("#txtValorFreteContratado").val($("#txtValorFreteContratadoResumo").val());
        $("#txtValorFreteContratado").focusout();

        BuscarImpostosPadrao();
    });

    $("#txtValorAReceberResumo").focusout(function () {
        $("#txtValorAReceber").val($("#txtValorAReceberResumo").val());
        $("#txtValorAReceber").focusout();
    });    

    $("#chkIncluirICMSNoFreteResumo").click(function () {
        if ($("#chkIncluirICMSNoFreteResumo")[0].checked != $("#chkIncluirICMSNoFrete")[0].checked)
            $("#chkIncluirICMSNoFrete").click();
    });


    $("#selAliquotaICMSResumo").change(function () {
        $("#selAliquotaICMS").val($("#selAliquotaICMSResumo").val());
        $("#selAliquotaICMS").change();
    });

    $("#selICMSResumo").change(function () {
        $("#selICMS").val($("#selICMSResumo").val());
        $("#selICMS").change();
    });

    $("#txtValorBaseCalculoICMSResumo").focusout(function () {
        $("#txtValorBaseCalculoICMS").val($("#txtValorBaseCalculoICMSResumo").val());
        $("#txtValorBaseCalculoICMS").focusout();
    });

    $("#txtValorICMSResumo").focusout(function () {
        $("#txtValorICMS").val($("#txtValorICMSResumo").val());
        $("#txtValorICMS").focusout();
    });


    $("#btnRecalcularFreteResumo").click(function () {
        $("#btnRecalcularFrete").click();
        BuscarImpostosPadrao();
    });

    $("#btnSalvarComponentePrestacaoServicoResumo").click(function () {
        SalvarComponenteDaPrestacaoResumo();
    });

    $("#btnExcluirComponentePrestacaoServicoResumo").click(function () {
        ExcluirComponenteDaPrestacaoResumo();
    });

    $("#btnCancelarComponentePrestacaoServicoResumo").click(function () {
        LimparCamposComponenteDaPrestacaoResumo();
    });

    $("#txtObservacaoGeralResumo").focusout(function () {
        $("#txtObservacaoGeral").val($("#txtObservacaoGeralResumo").val());
    });

    $("#txtObservacaoGeral").focusout(function () {
        $("#txtObservacaoGeralResumo").val($("#txtObservacaoGeral").val());
    });

    $("#btnAdicionarVeiculoResumo").click(function () {
        AdicionarVeiculoResumo();
    });

    $("#btnSalvarMotoristaResumo").click(function () {
        SalvarMotoristaResumo();
    });

    $("#btnExcluirMotoristaResumo").click(function () {
        ExcluirMotorista();
    });

    $("#btnCancelarMotoristaResumo").click(function () {
        LimparCamposMotoristaResumo();
    });

    $("#txtCPFMotoristaResumo").focusout(function () {
        ConsultarMotoristaResumo($(this).val());
    });


    $("#btnLiberarEdicaoCTe").click(function () {
        DesbloquearCamposCTe();
        $("#hddCodigoCTEReferenciado").val("-1");
    });
});

function CopiarValoresParaResumo() {
    $("#txtValorFreteContratadoResumo").val($("#txtValorFreteContratado").val());
    $("#txtValorAReceberResumo").val($("#txtValorAReceber").val());    

    $("#selAliquotaICMSResumo").val($("#selAliquotaICMS").val());
    $("#selICMSResumo").val($("#selICMS").val());
    $("#txtValorBaseCalculoICMSResumo").val($("#txtValorBaseCalculoICMS").val());
    $("#txtValorICMSResumo").val($("#txtValorICMS").val());

    $("#txtObservacaoGeralResumo").val($("#txtObservacaoGeral").val());

    RenderizarComponentesDaPrestacaoResumo();
}

function ControlarCamposResumo(permiteEditar) {
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    if (configuracaoEmpresa != null && configuracaoEmpresa.UtilizaResumoEmissaoCTe == true) {
        var cteReferencia = Globalize.parseFloat($("#hddCodigoCTEReferenciado").val());
        BloquearCamposCTe();
        if (permiteEditar) {
            if (cteReferencia <= 0) {
                $("#selCFOPResumo").prop("disabled", false);
                $("#selTomadorServicoResumo").prop("disabled", false);
            }
            else {
                if ($("#selTomadorServicoResumo").val() == "4") {
                    $("#txtCPFCNPJTomador").prop("disabled", false);
                    $("#btnBuscarTomador").prop("disabled", false);
                    $("#btnConsultarTomadorReceita").prop("disabled", false);
                    $("#txtRGIETomador").prop("disabled", false);
                    $("#txtRazaoSocialTomador").prop("disabled", false);
                    $("#txtNomeFantasiaTomador").prop("disabled", false);
                    $("#txtTelefone1Tomador").prop("disabled", false);
                    $("#txtTelefone2Tomador").prop("disabled", false);
                    $("#txtAtividadeTomador").prop("disabled", false);
                    $("#btnBuscarAtividadeTomador").prop("disabled", false);
                    $("#txtEnderecoTomador").prop("disabled", false);
                    $("#txtNumeroTomador").prop("disabled", false);
                    $("#txtBairroTomador").prop("disabled", false);
                    $("#txtComplementoTomador").prop("disabled", false);
                    $("#txtCEPTomador").prop("disabled", false);
                    $("#ddlPaisTomador").prop("disabled", false);
                    $("#ddlEstadoTomador").prop("disabled", false);
                    $("#selCidadeTomador").prop("disabled", false);
                    $("#txtCidadeTomadorExportacao").prop("disabled", false);
                    $("#txtEmailsTomador").prop("disabled", false);
                    $("#chkStatusEmailsTomador").prop("disabled", false);
                    $("#txtEmailsContatoTomador").prop("disabled", false);
                    $("#txtEmailsContadorTomador").prop("disabled", false);
                    $("#chkStatusEmailsContadorTomador").prop("disabled", false);
                    $("#txtEmailsTransportadorTomador").prop("disabled", false);
                    $("#chkStatusEmailsTransportadorTomador").prop("disabled", false);
                    $("#chkSalvarEnderecoTomador").prop("disabled", false);
                }
            }

            $("#btnRecalcularFreteResumo").prop("disabled", false);
            $("#txtValorFreteContratadoResumo").prop("disabled", false);
            if (cteReferencia <= 0) {
                $("#selICMSResumo").prop("disabled", false);
                $("#txtValorBaseCalculoICMSResumo").prop("disabled", false);
                $("#selAliquotaICMSResumo").prop("disabled", false);
                $("#txtValorICMSResumo").prop("disabled", false);
                $("#chkIncluirICMSNoFreteResumo").prop("disabled", false);
                $("#txtValorAReceberResumo").prop("disabled", false);
            }

            $("#txtDescricaoComponentePrestacaoServicoResumo").prop("disabled", false);
            $("#txtValorComponentePrestacaoServicoResumo").prop("disabled", false);
            $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMSResumo").prop("disabled", false);
            $("#chkIncluirValorComponentePrestacaoNoTotalAReceberResumo").prop("disabled", false);
            $("#btnSalvarComponentePrestacaoServicoResumo").prop("disabled", false);
            $("#btnExcluirComponentePrestacaoServicoResumo").prop("disabled", false);
            $("#btnCancelarComponentePrestacaoServicoResumo").prop("disabled", false);
            $("#tblComponentesPrestacaoServicoResumo").prop("disabled", false);

            $("#txtPlacaVeiculoResumo").prop("disabled", false);
            $("#btnBuscarVeiculoResumo").prop("disabled", false);
            $("#btnAdicionarVeiculoResumo").prop("disabled", false);
            $("#tblVeiculosResumo").prop("disabled", false);

            $("#txtNomeMotoristaResumo").prop("disabled", false);
            $("#txtCPFMotoristaResumo").prop("disabled", false);
            $("#btnBuscarMotoristaResumo").prop("disabled", false);
            $("#btnSalvarMotoristaResumo").prop("disabled", false);
            $("#btnExcluirMotoristaResumo").prop("disabled", false);
            $("#btnCancelarMotoristaResumo").prop("disabled", false);
            $("#tblMotoristasResumo").prop("disabled", false);

            $("#txtObservacaoGeralResumo").prop("disabled", false);

            $("#btnLiberarEdicaoCTe").prop("disabled", false);

            //$("#selPago_APagar").prop("disabled", true);
            //$("#selTomadorServico").prop("disabled", true);
            //$("#ddlModalTransporte").prop("disabled", true);
        }
    }
}

function HabilitarCamposImpostosResumo(status) {
    var cteReferencia = Globalize.parseFloat($("#hddCodigoCTEReferenciado").val());
    if (cteReferencia > 0) {
        status = true;
    }

    $("#txtValorBaseCalculoICMSResumo").prop("disabled", status);
    $("#selAliquotaICMSResumo").prop("disabled", status);
    $("#txtValorICMSResumo").prop("disabled", status);
}

function RenderizarCFOPsResumo(cfops, codigo) {
    var selCFOPs = document.getElementById("selCFOPResumo");
    selCFOPs.options.length = 0;
    var optn = document.createElement("option");
    optn.text = "Selecione";
    optn.value = 0;
    selCFOPs.options.add(optn);
    for (var i = 0; i < cfops.length; i++) {
        optn = document.createElement("option");
        optn.text = cfops[i].CodigoCFOP;
        optn.value = cfops[i].Codigo;
        if (codigo != null) {
            if (codigo == cfops[i].Codigo) {
                optn.setAttribute("selected", "selected");
            }
        }
        selCFOPs.options.add(optn);
    }
}

function RenderizarAliquotasDoICMSResumo(aliquotas) {
    var selAliquotaICMS = document.getElementById("selAliquotaICMSResumo");
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
}

function RenderizarComponentesDaPrestacaoResumo() {
    $("#tblComponentesPrestacaoServicoResumo tbody").html("");
    var componentesDaPrestacao = $("#hddComponentesDaPrestacao").val() == "" ? new Array() : JSON.parse($("#hddComponentesDaPrestacao").val());
    for (var i = 0; i < componentesDaPrestacao.length; i++) {
        if (!componentesDaPrestacao[i].Excluir) {
            $("#tblComponentesPrestacaoServicoResumo tbody").append("<tr>" +
                "<td class=\"text-uppercase\">" + componentesDaPrestacao[i].Descricao + "</td>" +
                "<td>" + Globalize.format(componentesDaPrestacao[i].Valor, "n2") + "</td>" +
                "<td>" + (componentesDaPrestacao[i].IncluiBaseCalculoICMS ? "Sim" : "Não") + "</td>" +
                "<td>" + (componentesDaPrestacao[i].IncluiValorAReceber ? "Sim" : "Não") + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarComponenteDaPrestacaoResumo(" + JSON.stringify(componentesDaPrestacao[i]) + ")'>Editar</button></td>" +
                "</tr>");
        }
    }
    if ($("#tblComponentesPrestacaoServicoResumo tbody").html() == "") {
        $("#tblComponentesPrestacaoServicoResumo tbody").html("<tr><td colspan='5'>Nenhum registro encontrado!</td></tr>");
    }
}

function SalvarComponenteDaPrestacaoResumo() {
    if (ValidarCamposComponenteDaPrestacaoResumo()) {
        $("#txtDescricaoComponentePrestacaoServico").val($("#txtDescricaoComponentePrestacaoServicoResumo").val());
        $("#txtValorComponentePrestacaoServico").val($("#txtValorComponentePrestacaoServicoResumo").val());
        $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMS").prop("checked", $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMSResumo")[0].checked);
        $("#chkIncluirValorComponentePrestacaoNoTotalAReceber").prop("checked", $("#chkIncluirValorComponentePrestacaoNoTotalAReceberResumo")[0].checked);

        SalvarComponenteDaPrestacao();
    }
}

function EditarComponenteDaPrestacaoResumo(componente) {
    $("#hddIdComponenteDaPrestacaoEmEdicao").val(componente.Id);
    $("#txtDescricaoComponentePrestacaoServicoResumo").val(componente.Descricao);
    $("#txtValorComponentePrestacaoServicoResumo").val(Globalize.format(componente.Valor, "n2"));
    $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMSResumo").prop("checked", componente.IncluiBaseCalculoICMS);
    $("#chkIncluirValorComponentePrestacaoNoTotalAReceberResumo").prop("checked", componente.IncluiValorAReceber);
    $("#btnExcluirComponentePrestacaoServicoResumo").show();
}

function ExcluirComponenteDaPrestacaoResumo() {
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
            LimparCamposComponenteDaPrestacaoResumo();
            AtualizarValoresGerais();
        }
    });
}

function LimparCamposComponenteDaPrestacaoResumo() {
    $("#hddIdComponenteDaPrestacaoEmEdicao").val('0');
    $("#txtDescricaoComponentePrestacaoServicoResumo").val('');
    $("#txtValorComponentePrestacaoServicoResumo").val('0,00');
    $("#chkIncluirValorComponentePrestacaoNaBaseDeCalculoDoICMSResumo").prop("checked", true);
    $("#chkIncluirValorComponentePrestacaoNoTotalAReceberResumo").prop("checked", true);
    $("#btnExcluirComponentePrestacaoServicoResumo").hide();
}

function ValidarCamposComponenteDaPrestacaoResumo() {
    var descricao = $("#txtDescricaoComponentePrestacaoServicoResumo").val();
    var valor = Globalize.parseFloat($("#txtValorComponentePrestacaoServicoResumo").val());
    var valido = true;

    if (descricao == "") {
        CampoComErro("#txtDescricaoComponentePrestacaoServicoResumo");
        valido = false;
    } else {
        CampoSemErro("#txtDescricaoComponentePrestacaoServicoResumo");
    }

    if (valor <= 0 || isNaN(valor)) {
        CampoComErro("#txtValorComponentePrestacaoServicoResumo");
        valido = false;
    } else {
        CampoSemErro("#txtValorComponentePrestacaoServicoResumo");
    }

    return valido;
}

function RetornoConsultaVeiculoResumo(veiculo) {
    $("#txtPlacaVeiculoResumo").val(veiculo.Placa);
}

function AdicionarVeiculoResumo() {
    if ($("#txtPlacaVeiculoResumo").val() != "") {
        executarRest("/Veiculo/BuscarPorPlaca?callback=?", { Placa: $("#txtPlacaVeiculoResumo").val() }, function (r) {
            if (r.Sucesso) {
                SalvarVeiculo(r.Objeto);
                var observacao, placas, renavans, placasVinculadas, renavansVinculados = "";

                if (r.Objeto.Proprietario != null && r.Objeto.Proprietario.ObservacaoCTe != null) {
                    placas = r.Objeto.Placa + ", ";
                    renavans = r.Objeto.Renavam + ", ";
                }

                if (r.Objeto.VeiculosVinculados != null) {

                    if (r.Objeto.VeiculosVinculados.length > 0) {
                        placasVinculadas = "";
                        for (var i = 0; i < r.Objeto.VeiculosVinculados.length; i++) {

                            SalvarVeiculo(r.Objeto.VeiculosVinculados[i]);

                            if (r.Objeto.Proprietario != null && r.Objeto.Proprietario.ObservacaoCTe != null) {
                                if (placasVinculadas != "") {
                                    placasVinculadas += r.Objeto.VeiculosVinculados[i].Placa += ", ";
                                    renavansVinculados += r.Objeto.VeiculosVinculados[i].Renavam += ", ";
                                } else {
                                    placasVinculadas = r.Objeto.VeiculosVinculados[i].Placa += ", ";
                                    renavansVinculados = r.Objeto.VeiculosVinculados[i].Renavam += ", ";
                                }
                            }
                        }
                    } else {
                        if (r.Objeto.VeiculosPai.length > 0) {
                            $("#spanPlacaFilho").text(r.Objeto.Placa);
                            $("#selSelecionarPlacaVinculada").html("");
                            var html = "";
                            for (var i = 0; i < r.Objeto.VeiculosPai.length; i++) {
                                html += "<option value='" + r.Objeto.VeiculosPai[i].Placa + "'>" + r.Objeto.VeiculosPai[i].Placa + "</option>";
                            }
                            $("#selSelecionarPlacaVinculada").html(html);
                            $("#divSelecionarPlacaVinculada").modal({ keyboard: false, backdrop: 'static' });

                            $("#btnConfirmarPlaca").click(function () {
                                for (var i = 0; i < r.Objeto.VeiculosPai.length; i++) {
                                    if (r.Objeto.VeiculosPai[i].Placa == $("#selSelecionarPlacaVinculada").val()) {
                                        SalvarVeiculo(r.Objeto.VeiculosPai[i]);
                                        if (r.Objeto.Proprietario != null && r.Objeto.Proprietario.ObservacaoCTe != null) {
                                            placas += r.Objeto.VeiculosPai[i].Placa += ", ";
                                            renavans += r.Objeto.VeiculosPai[i].Renavam += ", ";
                                        }
                                        break;
                                    }
                                }
                                $("#divSelecionarPlacaVinculada").modal('hide');
                            });

                        }
                    }


                }

                if (r.Objeto.Proprietario != null && r.Objeto.Proprietario.ObservacaoCTe != null) {
                    placas = placas.substr(0, placas.length - 2);
                    renavans = renavans.substr(0, renavans.length - 2);
                    if (placasVinculadas != null && placasVinculadas != "")
                        placasVinculadas = placasVinculadas.substr(0, placasVinculadas.length - 2);
                    if (renavansVinculados != null && renavansVinculados != "")
                        renavansVinculados = renavansVinculados.substr(0, renavansVinculados.length - 2);

                    observacao = r.Objeto.Proprietario.ObservacaoCTe
                        .replace(/#PlacaVeiculo#/g, placas)
                        .replace(/#RENAVAMVeiculo#/g, renavans)
                        .replace(/#NomeProprietarioVeiculo#/g, r.Objeto.Proprietario.NomeProprietario)
                        .replace(/#CPFCNPJProprietarioVeiculo#/g, r.Objeto.Proprietario.CPFCNPJProprietario)
                        .replace(/#RNTRCProprietario#/g, r.Objeto.Proprietario.RNTRC)
                        .replace(/#UFVeiculo#/g, r.Objeto.UF)
                        .replace(/#MarcaVeiculo#/g, r.Objeto.DescricaoMarca)
                        .replace(/#PlacasVinculadas#/g, placasVinculadas);

                    var txtObservacaoGeral = $("#txtObservacaoGeral");
                    txtObservacaoGeral.val(txtObservacaoGeral.val() + (txtObservacaoGeral.val().length > 0 ? "\n" : "") + observacao);
                }

                AdicionarMotorista(r.Objeto.NomeMotorista, r.Objeto.CPFMotorista);

                if (r.Objeto.Motoristas != null && r.Objeto.Motoristas.length > 0) {
                    for (var i = 0; i < r.Objeto.Motoristas.length; i++) {
                        AdicionarMotorista(r.Objeto.Motoristas[i].Nome, r.Objeto.Motoristas[i].CPF);
                    }
                }

            } else {
                jAlert(r.Erro, "Atenção");
            }
        });
    }
}

function LimparCamposVeiculoResumo() {
    $("#hddVeiculoEmEdicaoResumo").val('0');
    $("#txtPlacaVeiculoResumo").val('');
}

function RenderizarVeiculosResumo() {
    var veiculos = $("#hddVeiculos").val() == "" ? new Array() : JSON.parse($("#hddVeiculos").val());
    $("#tblVeiculosResumo tbody").html("");
    for (var i = 0; i < veiculos.length; i++) {
        if (!veiculos[i].Excluir)
            $("#tblVeiculosResumo tbody").append("<tr><td>" + veiculos[i].Placa + "</td><td>" + veiculos[i].UF + "</td><td>" + veiculos[i].Renavam + "</td><td>" + veiculos[i].DescricaoTipo + "</td><td>" + veiculos[i].DescricaoTipoRodado + "</td><td>" + veiculos[i].DescricaoTipoCarroceria + "</td><td>" + veiculos[i].TipoDoVeiculo + "</td><td>" + veiculos[i].CapacidadeKG + "</td><td>" + veiculos[i].CapacidadeM3 + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirVeiculo(" + JSON.stringify(veiculos[i]) + ")'>Excluir</button></td></tr>");
    }
    if ($("#tblVeiculosResumo tbody").html() == "")
        $("#tblVeiculosResumo tbody").html("<tr><td colspan='10'>Nenhum registro encontrado.</td></tr>");
}

function RenderizarMotoristasResumo(disabled) {
    $("#tblMotoristasResumo tbody").html("");
    var motoristas = $("#hddMotoristas").val() == "" ? new Array() : JSON.parse($("#hddMotoristas").val());
    for (var i = 0; i < motoristas.length; i++) {
        if (!motoristas[i].Excluir) {
            motoristas[i].Nome = RemoveAspas(motoristas[i].Nome);
            $("#tblMotoristasResumo tbody").append("<tr>" +
                "<td class=\"text-uppercase\">" + motoristas[i].Nome + "</td>" +
                "<td>" + motoristas[i].CPF + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarMotoristaResumo(" + JSON.stringify(motoristas[i]) + ")'>Editar</button></td>" +
                "</tr>");
        }
    }
    if ($("#tblMotoristasResumo tbody").html() == "") {
        $("#tblMotoristasResumo tbody").html("<tr><td colspan='3'>Nenhum registro encontrado!</td></tr>");
    }
}

function RetornoConsultaMotoristaResumo(motorista) {
    $("#txtCPFMotoristaResumo").val(motorista.CPFCNPJ);
    $("#txtNomeMotoristaResumo").val(motorista.Nome);
}

function SalvarMotoristaResumo() {
    if (ValidarCamposMotoristaResumo()) {
        var motorista = {
            Codigo: Globalize.parseInt($("#hddIdMotoristaEmEdicao").val()),
            Nome: $("#txtNomeMotoristaResumo").val(),
            CPF: $("#txtCPFMotoristaResumo").val(),
            Excluir: false
        };
        var motoristas = $("#hddMotoristas").val() == "" ? new Array() : JSON.parse($("#hddMotoristas").val());
        if (motorista.Codigo == 0)
            motorista.Codigo = -(motoristas.length + 1);
        if (motoristas.length > 0) {
            for (var i = 0; i < motoristas.length; i++) {
                if (motoristas[i].Codigo == motorista.Codigo) {
                    motoristas.splice(i, 1);
                    break;
                }
            }
        }
        motoristas.push(motorista);
        motoristas.sort();
        $("#hddMotoristas").val(JSON.stringify(motoristas));
        RenderizarMotoristas();
        LimparCamposMotoristaResumo();

    }
}

function ValidarCamposMotoristaResumo() {
    var nome = $("#txtNomeMotoristaResumo").val();
    var cpf = $("#txtCPFMotoristaResumo").val();
    var valido = true;
    if (nome != "") {
        CampoSemErro("#txtNomeMotoristaResumo");
    } else {
        CampoComErro("#txtNomeMotoristaResumo");
        valido = false;
    }
    if (cpf != "") {
        CampoSemErro("#txtCPFMotoristaResumo");
    } else {
        CampoComErro("#txtCPFMotoristaResumo");
        valido = false;
    }
    return valido;
}

function LimparCamposMotoristaResumo() {
    $("#txtNomeMotoristaResumo").val("");
    $("#txtCPFMotoristaResumo").val("");
    $("#hddIdMotoristaEmEdicao").val("0");
    $("#btnExcluirMotoristaResumo").hide();
}

function ConsultarMotoristaResumo(cpf) {
    if (cpf.replace(/[^a-zA-Z0-9]/g, "") != "") {
        executarRest("/Motorista/BuscarPorCPF?callback=?", { CPF: cpf }, function (r) {
            if (r.Sucesso) {
                $("#txtCPFMotoristaResumo").val(r.Objeto.CPF);
                $("#txtNomeMotoristaResumo").val(r.Objeto.Nome);
            }
            else {
                $("#txtNomeMotorista").val("");
                ExibirMensagemAlerta("Motorista não possui cadastro!", "Atenção!");
            }
        });
    }
}

function EditarMotoristaResumo(motorista) {
    $("#hddIdMotoristaEmEdicao").val(motorista.Codigo);
    $("#txtNomeMotoristaResumo").val(motorista.Nome).trigger("change");
    $("#txtCPFMotoristaResumo").val(motorista.CPF);
    $("#btnExcluirMotoristaResumo").show();
}

function BuscarImpostosPadrao() {
    var icms = $("#selICMS").val();
    if (icms == "0")
        BuscarDadosImpostos(false);
}

function VincularEventoCalculoFreteNaAbaServicosEImpostosResumo() {
    $('a[data-toggle="tab"][href="#tabsResumo"]').on('shown.bs.tab', function (e) {
        //var valorFrete = Globalize.parseFloat($("#txtValorFreteContratado").val());
        //if (valorFrete == 0) {
        //    BuscarTabelaFrete();
        //}
        //else {
            var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
            if (configuracaoEmpresa != null && configuracaoEmpresa.UtilizaTabelaDeFrete) {
                $('#divInformacaoServicosEImpostosResumo').find('span').remove();
                $("#divInformacaoServicosEImpostosResumo").prepend('<span class="label label-success" style="padding-top: 4px; padding-bottom: 4px;">Para recalcular clique em:</span>');
                $("#divInformacaoServicosEImpostosResumo").removeClass("hidden");
            }
       // }
    });
}

function RemoverEventoCalculoFreteNaAbaServicosEImpostosResumo() {
    $('a[data-toggle="tab"][href="#tabsResumo"]').off('shown.bs.tab');
}