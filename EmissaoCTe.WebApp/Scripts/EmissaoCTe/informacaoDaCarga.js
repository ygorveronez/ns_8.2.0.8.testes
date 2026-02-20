$(document).ready(function () {
    $("#txtPlacaVeiculo").mask("*******");
    $("#txtValorTotalCarga").priceFormat({ prefix: '' });
    $("#txtValorCargaAverbacao").priceFormat({ prefix: '' });
    $("#txtQuantidade").priceFormat({ prefix: '', centsLimit: 4 });

    FormatarCampoDate("txtDataPrevistaEntregaConteiner");
    FormatarCampoDate("txtDataPrevistaEntregaCargaRecebedor");

    $("#selUnidadeMedida").change(function () {
        TrocarUnidadeDeMedida();
    });
    $("#btnSalvarInformacaoQuantidadeCarga").click(function () {
        SalvarInformacaoQuantidadeCarga();
    });
    $("#btnCancelarInformacaoQuantidadeCarga").click(function () {
        LimparCamposInformacaoQuantidadeCarga();
    });
    $("#btnExcluirInformacaoQuantidadeCarga").click(function () {
        ExcluirInformacaoQuantidadeCarga();
    });
    $("#btnAdicionarVeiculo").click(function () {
        AdicionarVeiculo();
    });
    $("#txtValorTotalCarga").on('change', function () {
        $("#txtValorCargaAverbacao").val($("#txtValorTotalCarga").val());
    });

    CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false);
});
function RetornoConsultaVeiculo(veiculo) {
    $("#txtPlacaVeiculo").val(veiculo.Placa);
}
function TrocarUnidadeDeMedida() {
    switch ($("#selUnidadeMedida").val()) {
        case "0":
            $("#txtTipoUnidadeMedida").val("M3");//$("#txtTipoUnidadeMedida").val("Metro Cúbico");
            break;
        case "1":
            $("#txtTipoUnidadeMedida").val("KG"); //$("#txtTipoUnidadeMedida").val("Kilograma");
            break;
        case "2":
            $("#txtTipoUnidadeMedida").val("TON"); //$("#txtTipoUnidadeMedida").val("Tonelada");
            break;
        case "3":
            $("#txtTipoUnidadeMedida").val("UN");// $("#txtTipoUnidadeMedida").val("Unidade");
            break;
        case "4":
            $("#txtTipoUnidadeMedida").val("LT"); //$("#txtTipoUnidadeMedida").val("Litro");
            break;
        case "5":
            $("#txtTipoUnidadeMedida").val("MMBTU"); //$("#txtTipoUnidadeMedida").val("Milhão de Unidades Térmicas Britânicas");
            break;
        case "99":
            $("#txtTipoUnidadeMedida").val("ST");
            break
        default:
            $("#txtTipoUnidadeMedida").val("");
            break;
    }
}

//Informacao Quantidade Carga
function ValidarCamposInformacaoQuantidadeCarga() {
    var unidadeMedida = Globalize.parseInt($("#selUnidadeMedida").val());
    var tipoUnidade = $("#txtTipoUnidadeMedida").val();
    var quantidade = Globalize.parseFloat($("#txtQuantidade").val());
    var valido = true;
    if (unidadeMedida < 0) {
        CampoComErro("#txtQuantidade");
        valido = false;
    } else {
        CampoSemErro("#txtQuantidade");
    }
    if (tipoUnidade == "") {
        CampoComErro("#txtTipoUnidadeMedida");
        valido = false;
    } else {
        CampoSemErro("#txtTipoUnidadeMedida");
    }
    if (quantidade <= 0) {
        CampoComErro("#txtQuantidade");
        valido = false;
    } else {
        CampoSemErro("#txtQuantidade");
    }
    return valido;
}
function SalvarInformacaoQuantidadeCarga() {
    if (ValidarCamposInformacaoQuantidadeCarga()) {
        var informacaoQuantidade = {
            Id: Globalize.parseInt($("#hddIdInformacaoQuantidadeCargaEmEdicao").val()),
            UnidadeMedida: Globalize.parseInt($("#selUnidadeMedida").val()),
            DescricaoUnidadeMedida: $("#selUnidadeMedida :selected").text(),
            TipoUnidade: $("#txtTipoUnidadeMedida").val(),
            Quantidade: Globalize.parseFloat($("#txtQuantidade").val()),
            Excluir: false
        };
        var infomacoesQuantidadeCarga = $("#hddInformacoesQuantidadeCarga").val() == "" ? new Array() : JSON.parse($("#hddInformacoesQuantidadeCarga").val());
        if (informacaoQuantidade.Id == 0) {
            informacaoQuantidade.Id = -(infomacoesQuantidadeCarga.length + 1);
        }
        for (var i = 0; i < infomacoesQuantidadeCarga.length; i++) {
            if (infomacoesQuantidadeCarga[i].Id == informacaoQuantidade.Id) {
                infomacoesQuantidadeCarga.splice(i, 1);
                break;
            }
        }
        infomacoesQuantidadeCarga.push(informacaoQuantidade);
        $("#hddInformacoesQuantidadeCarga").val(JSON.stringify(infomacoesQuantidadeCarga));
        RenderizarInformacaoQuantidadeCarga();
        LimparCamposInformacaoQuantidadeCarga();
        //BuscarFrete();
    }
}
function EditarInformacaoQuantidadeCarga(informacao) {
    $("#hddIdInformacaoQuantidadeCargaEmEdicao").val(informacao.Id);
    $("#selUnidadeMedida").val(informacao.UnidadeMedida);
    $("#txtTipoUnidadeMedida").val(informacao.TipoUnidade);
    $("#txtQuantidade").val(Globalize.format(informacao.Quantidade, "n4"));
    $("#btnExcluirInformacaoQuantidadeCarga").show();
}
function ExcluirInformacaoQuantidadeCarga() {
    var id = Globalize.parseInt($("#hddIdInformacaoQuantidadeCargaEmEdicao").val());
    var infomacoesQuantidadeCarga = $("#hddInformacoesQuantidadeCarga").val() == "" ? new Array() : JSON.parse($("#hddInformacoesQuantidadeCarga").val());
    for (var i = 0; i < infomacoesQuantidadeCarga.length; i++) {
        if (infomacoesQuantidadeCarga[i].Id == id) {
            if (id <= 0)
                infomacoesQuantidadeCarga.splice(i, 1);
            else
                infomacoesQuantidadeCarga[i].Excluir = true;
            break;
        }
    }
    $("#hddInformacoesQuantidadeCarga").val(JSON.stringify(infomacoesQuantidadeCarga));
    RenderizarInformacaoQuantidadeCarga();
    LimparCamposInformacaoQuantidadeCarga();
    //BuscarFrete();
}
function LimparCamposInformacaoQuantidadeCarga() {
    $("#hddIdInformacaoQuantidadeCargaEmEdicao").val('0');
    $("#selUnidadeMedida").val('-1');
    $("#txtTipoUnidadeMedida").val('');
    $("#txtQuantidade").val('0,0000');
    $("#btnExcluirInformacaoQuantidadeCarga").hide();
}
function RenderizarInformacaoQuantidadeCarga() {
    var infomacoesQuantidadeCarga = $("#hddInformacoesQuantidadeCarga").val() == "" ? new Array() : JSON.parse($("#hddInformacoesQuantidadeCarga").val());
    $("#tblInformacaoQuantidadeCarga tbody").html("");
    for (var i = 0; i < infomacoesQuantidadeCarga.length; i++) {
        if (!infomacoesQuantidadeCarga[i].Excluir)
            $("#tblInformacaoQuantidadeCarga tbody").append("<tr><td>" + infomacoesQuantidadeCarga[i].DescricaoUnidadeMedida + "</td><td>" + infomacoesQuantidadeCarga[i].TipoUnidade + "</td><td>" + Globalize.format(infomacoesQuantidadeCarga[i].Quantidade, "n4") + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarInformacaoQuantidadeCarga(" + JSON.stringify(infomacoesQuantidadeCarga[i]) + ")'>Editar</button></td></tr>");
    }
    if ($("#tblInformacaoQuantidadeCarga tbody").html() == "")
        $("#tblInformacaoQuantidadeCarga tbody").html("<tr><td colspan='4'>Nenhum registro encontrado.</td></tr>");
}

//Informacao Veiculos
function AdicionarVeiculo() {
    if ($("#txtPlacaVeiculo").val() != "") {
        executarRest("/Veiculo/BuscarPorPlaca?callback=?", { Placa: $("#txtPlacaVeiculo").val() }, function (r) {
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

                if (r.Objeto.Motoristas != null && r.Objeto.Motoristas.length > 0)
                {
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
function AdicionarMotorista(nome, cpf) {
    if ((nome != null && nome != "") && (cpf != null && cpf != "")) {
        $("#txtNomeMotorista").val(nome);
        $("#txtCPFMotorista").val(cpf);
        SalvarMotorista();
    }
}
function SalvarVeiculo(veiculo) {
    veiculo.Id = 0;
    veiculo.Excluir = false;
    var veiculos = $("#hddVeiculos").val() == "" ? new Array() : JSON.parse($("#hddVeiculos").val());
    if (veiculos.length == 0)
        $("#chkIndicadorLotacao").prop("checked", true);
    if (veiculo.Id == 0) {
        veiculo.Id = -(veiculos.length + 1);
    }
    for (var i = 0; i < veiculos.length; i++) {
        if (veiculos[i].Placa == veiculo.Placa) {
            veiculos.splice(i, 1);
            break;
        }
    }
    veiculos.push(veiculo);
    $("#hddVeiculos").val(JSON.stringify(veiculos));
    
    if (veiculo.NumeroCIOT != "")
        $("#txtCIOT").val(veiculo.NumeroCIOT)

    RenderizarVeiculos();
    LimparCamposVeiculo();
    LimparCamposVeiculoResumo();
}
function ExcluirVeiculo(veiculo) {
    jConfirm("Deseja realmente excluir este veículo?", "Atenção", function (r) {
        if (r) {
            var veiculos = $("#hddVeiculos").val() == "" ? new Array() : JSON.parse($("#hddVeiculos").val());
            for (var i = 0; i < veiculos.length; i++) {
                if (veiculos[i].Id == veiculo.Id) {
                    if (veiculo.Id <= 0)
                        veiculos.splice(i, 1);
                    else
                        veiculos[i].Excluir = true;
                    break;
                }
            }
            $("#hddVeiculos").val(JSON.stringify(veiculos));
            RenderizarVeiculos();
            LimparCamposVeiculo();
        }
    });
}
function LimparCamposVeiculo() {
    $("#hddVeiculoEmEdicao").val('0');
    $("#txtPlacaVeiculo").val('');
}
function RenderizarVeiculos() {
    var veiculos = $("#hddVeiculos").val() == "" ? new Array() : JSON.parse($("#hddVeiculos").val());
    $("#tblVeiculos tbody").html("");
    for (var i = 0; i < veiculos.length; i++) {
        if (!veiculos[i].Excluir)
            $("#tblVeiculos tbody").append("<tr><td>" + veiculos[i].Placa + "</td><td>" + veiculos[i].UF + "</td><td>" + veiculos[i].Renavam + "</td><td>" + veiculos[i].DescricaoTipo + "</td><td>" + veiculos[i].DescricaoTipoRodado + "</td><td>" + veiculos[i].DescricaoTipoCarroceria + "</td><td>" + veiculos[i].TipoDoVeiculo + "</td><td>" + veiculos[i].CapacidadeKG + "</td><td>" + veiculos[i].CapacidadeM3 + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirVeiculo(" + JSON.stringify(veiculos[i]) + ")'>Excluir</button></td></tr>");
    }
    if ($("#tblVeiculos tbody").html() == "")
        $("#tblVeiculos tbody").html("<tr><td colspan='10'>Nenhum registro encontrado.</td></tr>");

    RenderizarVeiculosResumo();    
}