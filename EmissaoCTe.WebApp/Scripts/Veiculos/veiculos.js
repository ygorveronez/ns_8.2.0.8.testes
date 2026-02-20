$(document).ready(function () {
    HeaderAuditoria("Veiculo");

    $("#txtKilometragemAtual").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
    $("#txtCPFMotorista").mask("999.999.999-99");
    $("#txtCPFMotoristaAdicional").mask("999.999.999-99");
    $("#txtPlaca").mask("*******");
    $("#txtCapacidadeM3").mask("9?99");
    $("#txtCapacidadeKG").mask("9?99999");
    $("#txtTaraKG").mask("9?99999");
    $("#txtNumeroFrota").mask("9?99999999999999999999");

    CarregarConsultaDeVeiculos("default-search", "default-search", RetornoConsultaVeiculos, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotoristaAdicional", "btnBuscarMotoristaAdicional", RetornoConsultaMotoristaAdicional, true, false);

    BuscarUFs("selUF");

    $("#txtNomeMotorista").change(LimparNome);
    $("#txtNomeMotoristaAdicional").change(LimparNome);

    $("#btnSalvar").click(function () {
        Salvar();
    });
    $("#btnCancelar").click(function () {
        LimparCampos();
    });

    $("#btnSalvarMotoristaAdicional").click(function () {
        SalvarMotoristaAdicional();
    });

    $("#btnExcluirMotoristaAdicional").click(function () {
        ExcluirMotoristaAdicional();
    });

    $("#btnCancelarMotoristaAdicional").click(function () {
        LimparCamposMotoristaAdicional();
    });

    $("#txtCPFMotorista").focusout(function () {
        ConsultarMotorista($(this).val());
    });

    $("#txtCPFMotoristaAdicional").focusout(function () {
        ConsultarMotoristaAdicional($(this).val());
    });

    LimparCampos();
});

function RetornoConsultaMotorista(motorista) {
    $("#txtCPFMotorista").val(motorista.CPFCNPJ);
    $("#txtNomeMotorista").val(motorista.Nome);
}

function RetornoConsultaMotoristaAdicional(motorista) {
    $("#txtCPFMotoristaAdicional").val(motorista.CPFCNPJ);
    $("#txtNomeMotoristaAdicional").val(motorista.Nome);
}

function LimparNome() {
    var nome = $("#txtNomeMotorista").val();

    nome = nome.replace(/"/gm, '');
    nome = nome.replace(/'/gm, '');

    $("#txtNomeMotorista").val(nome);
}

function BuscarUFs(idSelect) {
    executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {
            RenderizarUFs(r.Objeto, idSelect);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}
function RenderizarUFs(ufs, idSelect) {
    var selUFs = document.getElementById(idSelect);
    selUFs.options.length = 0;
    for (var i = 0; i < ufs.length; i++) {
        var optn = document.createElement("option");
        optn.text = ufs[i].Sigla + " - " + ufs[i].Nome;
        optn.value = ufs[i].Sigla;
        selUFs.options.add(optn);
    }
}
function RetornoConsultaVeiculos(veiculo) {
    executarRest("/Veiculo/ObterDetalhes?callback=?", { Codigo: veiculo.Codigo }, function (r) {
        if (r.Sucesso) {
            HeaderAuditoriaCodigo(r.Objeto.Codigo);

            $("#txtPlaca").prop("disabled", true);

            $("#hddCodigo").val(r.Objeto.Codigo);
            $("#txtPlaca").val(r.Objeto.Placa);
            $("#txtRenavam").val(r.Objeto.Renavam);
            $("#txtTaraKG").val(r.Objeto.TaraKG);
            $("#txtCapacidadeKG").val(r.Objeto.CapacidadeKG);
            $("#txtCapacidadeM3").val(r.Objeto.CapacidadeM3);
            $("#selTipoPropriedade").val(r.Objeto.Tipo);
            $("#selTipoVeiculo").val(r.Objeto.TipoVeiculo);
            $("#selTipoRodado").val(r.Objeto.TipoRodado);
            $("#selStatus").val(r.Objeto.Status);
            $("#selCarroceria").val(r.Objeto.Carroceria);
            $("#selUF").val(r.Objeto.SiglaUF);
            $("#txtNomeMotorista").val(r.Objeto.NomeMotorista).trigger("change");
            $("#txtCPFMotorista").val(r.Objeto.CPFMotorista);
            $("#txtKilometragemAtual").val(Globalize.format(r.Objeto.KilometragemAtual, "n0"));
            $("#txtTipoVeiculo").val(r.Objeto.DescricaoTipo);
            $("#hddCodigoTipoVeiculo").val(r.Objeto.CodigoTipo);
            $("#txtMarcaVeiculo").val(r.Objeto.DescricaoMarca);
            $("#hddCodigoMarcaVeiculo").val(r.Objeto.CodigoMarca);
            $("#txtModeloVeiculo").val(r.Objeto.DescricaoModelo);
            $("#hddCodigoModeloVeiculo").val(r.Objeto.CodigoModelo);
            $("#txtChassi").val(r.Objeto.Chassi);
            $("#txtDataCompra").val(r.Objeto.DataCompra);
            $("#txtValorAquisicao").val(Globalize.format(r.Objeto.ValorAquisicao, "n2"));
            $("#txtCapacidadeTanque").val(Globalize.format(r.Objeto.CapacidadeTanque, "n0"));
            $("#txtDataLicenca").val(r.Objeto.DataLicenca);
            $("#txtAnoFabricacao").val(r.Objeto.AnoFabricacao);
            $("#txtAnoModelo").val(r.Objeto.AnoModelo);
            $("#txtNumeroMotor").val(r.Objeto.NumeroMotor);
            $("#txtMediaPadrao").val(Globalize.format(r.Objeto.MediaPadrao, "n2"));
            $("#txtDataVencimentoGarantiaPlena").val(r.Objeto.DataVencimentoGarantiaPlena);
            $("#txtDataVencimentoGarantiaEscalonada").val(r.Objeto.DataVencimentoGarantiaEscalonada);
            $("#txtContrato").val(r.Objeto.Contrato);
            $("#txtNumeroFrota").val(r.Objeto.NumeroFrota);
            $("#txtObservacao").val(r.Objeto.Observacao);
            $("#hddVeiculosVinculados").val(r.Objeto.VeiculosVinculados != null ? JSON.stringify(r.Objeto.VeiculosVinculados) : "");
            AlterarTipoPropriedade($("#selTipoPropriedade").val());
            $("#hddCodigoProprietario").val(r.Objeto.CodigoProprietario);
            $("#txtProprietarioVeiculo").val(r.Objeto.DescricaoProprietario);
            $("#selTipoProprietarioVeiculo").val(r.Objeto.TipoProprietario);
            $("#txtRNTRCProprietarioVeiculo").val(r.Objeto.RNTRC);
            $("#txtCIOTProprietarioVeiculo").val(r.Objeto.CIOT);
            $("#txtObservacaoProprietario").val(r.Objeto.ObservacaoCTe);
            $("body").data("codigoModeloVeicularCarga", r.Objeto.CodigoModeloVeicularCarga);
            $("#txtModeloVeicularCarga").val(r.Objeto.DescricaoModeloVeicularCarga);

            $("#hddCodigoFornecedorValePedagio").val(r.Objeto.CodigoFornecedorValePedagio);
            $("#txtFornecedorValePedagio").val(r.Objeto.DescricaoFornecedorValePedagio);
            $("#hddCodigoResponsavelValePedagio").val(r.Objeto.CodigoResponsavelValePedagio);
            $("#txtResponsavelValePedagio").val(r.Objeto.DescricaoResponsavelValePedagio);
            $("#txtNumeroCompraValePedagio").val(r.Objeto.NumeroCompraValePedagio);
            $("#txtValorValePedagio").val(Globalize.format(r.Objeto.ValorValePedagio, "n2"));

            $("#chkPossuiTagValePedagio").prop("checked", r.Objeto.PossuiTagValePedagio);
            $("#selTipoTag").val(r.Objeto.TipoTag);
            //$("#txtDataInicioVigenciaTagValePedagio").val(r.Objeto.DataInicioVigenciaTagValePedagio);
            //$("#txtDataFimVigenciaTagValePedagio").val(r.Objeto.DataFimVigenciaTagValePedagio);

            LoadRastreadorVeiculo(r.Objeto);

            $("#txtTAF").val(r.Objeto.TAF);
            $("#txtNroRegEstadual").val(r.Objeto.NroRegEstadual);

            $("#txtXCampo").val(r.Objeto.XCampo);
            $("#txtXTexto").val(r.Objeto.XTexto);

            RenderizarVeiculosVinculados();

            $("body").data("motoristas", r.Objeto.Motoristas);
            RenderizarMotoristasAdicional();

            LimparNome();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}
function ValidarStatus() {
    var valido = true;

    // Validar status (nao e possivel salvar inativo se houver vínculo)
    var veiculos = $("#hddVeiculosVinculados").val() == "" ? new Array() : JSON.parse($("#hddVeiculosVinculados").val());
    if ($("#selStatus").val() == "I" && veiculos.length > 0) {
        CampoComErro("#selStatus");
        valido = false;
    } else {
        CampoSemErro("#selStatus");
        valido = true;
    }

    return valido;
}
function ValidarCampos() {
    var valido = true;
    var placa = $("#txtPlaca").val().trim();
    var renavam = $("#txtRenavam").val().trim();
    var tara = Globalize.parseFloat($("#txtTaraKG").val());

    if (placa != "") {
        CampoSemErro("#txtPlaca");
    } else {
        CampoComErro("#txtPlaca");
        valido = false;
    }
    if (renavam != "") {
        CampoSemErro("#txtRenavam");
    } else {
        CampoComErro("#txtRenavam");
        valido = false;
    }
    
    if (isNaN(tara) || tara <= 0) {
        CampoComErro("#txtTaraKG");
        valido = false;
    } else {
        CampoSemErro("#txtTaraKG");
    }

    return valido;
}
function ValidarProprietario() {
    var valido = true;

    var ciot = $("#txtCIOTProprietarioVeiculo").val().trim();
    var taf = $("#txtTAF").val().trim();
    var nroRegEstadual = $("#txtNroRegEstadual").val().trim();

    if (ciot.length != 0 && ciot.length != 12) {
        CampoComErro("#txtCIOTProprietarioVeiculo");
        ExibirMensagemAlerta("CIOT quando informado deve possuir 12 digitos.", "Atenção");
        valido = false;
    }
    else {
        CampoSemErro("#txtCIOTProprietarioVeiculo");
    }

    if (taf != "" && taf.length != 12) {
        CampoComErro("#txtTAF");
        valido = false;
        ExibirMensagemAlerta("Campo TAF deve conter 12 caracteres.", "Atenção!");
    } else {
        CampoSemErro("#txtTAF");
    }
    if (nroRegEstadual != "" && nroRegEstadual.length != 25) {
        CampoComErro("#txtNroRegEstadual");
        valido = false;
        ExibirMensagemAlerta("Campo Nº Reg. Estadual deve conter 25 caracteres.", "Atenção!");
    } else {
        CampoSemErro("#txtNroRegEstadual");
    }


    return valido;
}
function LimparCampos() {
    HeaderAuditoriaCodigo();

    $("#txtPlaca").prop("disabled", false);

    $("#hddCodigo").val('0');
    $("#txtKilometragemAtual").val('0');
    $("#txtPlaca").val("");
    $("#txtRenavam").val("");
    $("#txtTaraKG").val("");
    $("#txtCapacidadeKG").val("");
    $("#txtCapacidadeM3").val("");
    $("#selTipoPropriedade").val($("#selTipoPropriedade option:first").val());
    $("#selTipoVeiculo").val($("#selTipoVeiculo option:first").val());
    $("#selTipoRodado").val($("#selTipoRodado option:first").val());
    $("#selCarroceria").val($("#selCarroceria option:first").val());
    $("#selStatus").val($("#selStatus option:first").val());
    $("#selUF").val($("#selUF option:first").val());
    $("#txtNomeMotorista").val("");
    $("#txtCPFMotorista").val("");
    $("#hddVeiculosVinculados").val('');
    $("#txtTipoVeiculo").val('');
    $("#hddCodigoTipoVeiculo").val('0');
    $("#txtMarcaVeiculo").val('');
    $("#hddCodigoMarcaVeiculo").val('0');
    $("#txtModeloVeiculo").val('');
    $("#hddCodigoModeloVeiculo").val('0');
    $("#txtChassi").val('');
    $("#txtDataCompra").val('');
    $("#txtValorAquisicao").val('0,00');
    $("#txtCapacidadeTanque").val('');
    $("#txtDataLicenca").val('');
    $("#txtAnoFabricacao").val('');
    $("#txtAnoModelo").val('');
    $("#txtNumeroMotor").val('');
    $("#txtMediaPadrao").val('');
    $("#txtDataVencimentoGarantiaPlena").val('');
    $("#txtDataVencimentoGarantiaEscalonada").val('');
    $("#txtContrato").val('');
    $("#txtNumeroFrota").val('');
    $("#txtObservacao").val('');
    RenderizarVeiculosVinculados();
    AlterarTipoPropriedade($("#selTipoPropriedade").val());
    $("#hddCodigoProprietario").val("");
    $("#selTipoProprietarioVeiculo").val($("#selTipoProprietarioVeiculo option:first").val());
    $("#txtRNTRCProprietarioVeiculo").val("");
    $("#txtCIOTProprietarioVeiculo").val("");
    $("#txtObservacaoProprietario").val("");
    $("#txtProprietarioVeiculo").val("");
    $("body").data("codigoModeloVeicularCarga", null);
    $("#txtModeloVeicularCarga").val("");

    $("#hddCodigoFornecedorValePedagio").val("");
    $("#txtFornecedorValePedagio").val("");
    $("#hddCodigoResponsavelValePedagio").val("");
    $("#txtResponsavelValePedagio").val("");
    $("#txtNumeroCompraValePedagio").val("");
    $("#txtValorValePedagio").val('0,00');

    $("#chkPossuiTagValePedagio").attr("checked", false);
    $("#selTipoTag").val($("#selTipoTag option:first").val());   
    //$("#txtDataInicioVigenciaTagValePedagio").val('');
    //$("#txtDataFimVigenciaTagValePedagio").val('');
 

    LimparRastreadorVeiculo();

    $("#txtXCampo").val("");
    $("#txtXTexto").val("");

    $("#txtTAF").val("");
    $("#txtNroRegEstadual").val("");

    LimparCamposMotoristaAdicional();
    $("body").data("motoristas", null);
    RenderizarMotoristasAdicional();
}

function Salvar() {
    if (!ValidarCampos())
        return ExibirMensagemAlerta("Os campos com asterísco (*) ou em vermelho são obrigatórios.", "Atenção");

    if (!ValidarStatus())
        return ExibirMensagemAlerta("Não é possível inativar quando há veículos vinculados.", "Atenção");

    if (!ValidarRastreadorVeiculo())
        return ExibirMensagemAlerta("Ao indicar a existência de rastreador, deve ser informada a tecnologia, comunicação e o número do equipamento.", "Atenção");

    if (ValidarProprietario()) {
        var dados = {
            Codigo: $("#hddCodigo").val(),
            Placa: $("#txtPlaca").val(),
            Renavam: $("#txtRenavam").val(),
            TaraKG: $("#txtTaraKG").val(),
            CapacidadeKG: $("#txtCapacidadeKG").val(),
            CapacidadeM3: $("#txtCapacidadeM3").val(),
            Tipo: $("#selTipoPropriedade").val(),
            TipoVeiculo: $("#selTipoVeiculo").val(),
            TipoRodado: $("#selTipoRodado").val(),
            Carroceria: $("#selCarroceria").val(),
            SiglaUF: $("#selUF").val(),
            NomeMotorista: $("#txtNomeMotorista").val(),
            CPFMotorista: $("#txtCPFMotorista").val(),
            KilometragemAtual: $("#txtKilometragemAtual").val(),
            Status: $("#selStatus").val(),
            CodigoTipoVeiculo: Globalize.parseInt($("#hddCodigoTipoVeiculo").val()),
            CodigoMarcaVeiculo: Globalize.parseInt($("#hddCodigoMarcaVeiculo").val()),
            CodigoModeloVeiculo: Globalize.parseInt($("#hddCodigoModeloVeiculo").val()),
            CodigoModeloVeicularCarga: $("body").data("codigoModeloVeicularCarga"),
            Chassi: $("#txtChassi").val(),
            DataCompra: $("#txtDataCompra").val(),
            ValorAquisicao: Globalize.parseFloat($("#txtValorAquisicao").val()),
            CapacidadeTanque: Globalize.parseInt($("#txtCapacidadeTanque").val()),
            DataLicenca: $("#txtDataLicenca").val(),
            AnoFabricacao: Globalize.parseInt($("#txtAnoFabricacao").val()),
            AnoModelo: Globalize.parseInt($("#txtAnoModelo").val()),
            NumeroMotor: $("#txtNumeroMotor").val(),
            MediaPadrao: Globalize.parseFloat($("#txtMediaPadrao").val()),
            DataVencimentoGarantiaPlena: $("#txtDataVencimentoGarantiaPlena").val(),
            DataVencimentoGarantiaEscalonada: $("#txtDataVencimentoGarantiaEscalonada").val(),
            Contrato: $("#txtContrato").val(),
            NumeroFrota: $("#txtNumeroFrota").val(),
            Observacao: $("#txtObservacao").val(),
            CodigoProprietario: $("#hddCodigoProprietario").val(),
            TipoProprietario: $("#selTipoProprietarioVeiculo").val(),
            RNTRCProprietario: $("#txtRNTRCProprietarioVeiculo").val(),
            CIOTProprietario: $("#txtCIOTProprietarioVeiculo").val(),
            ObservacaoProprietario: $("#txtObservacaoProprietario").val(),
            CodigoFornecedorValePedagio: $("#hddCodigoFornecedorValePedagio").val(),
            CodigoResponsavelValePedagio: $("#hddCodigoResponsavelValePedagio").val(),
            NumeroCompraValePedagio: $("#txtNumeroCompraValePedagio").val(),
            ValorValePedagio: Globalize.parseFloat($("#txtValorValePedagio").val()),
            PossuiTagValePedagio: $("#chkPossuiTagValePedagio")[0].checked,
            TipoTag: $("#selTipoTag").val(),
            //DataInicioVigenciaTagValePedagio: $("#txtDataInicioVigenciaTagValePedagio").val(),
            //DataFimVigenciaTagValePedagio: $("#txtDataFimVigenciaTagValePedagio").val(),
            TAF: $("#txtTAF").val(),
            NroRegEstadual: $("#txtNroRegEstadual").val(),
            XCampo: $("#txtXCampo").val(),
            XTexto: $("#txtXTexto").val(),
            PossuiRastreador: $("#chkPossuiRastreador").is(":checked"),
            CodigoTecnologiaRastreador: $("#hddCodigoTecnologiaRastreador").val(),
            CodigoTipoComunicacaoRastreador: $("#hddCodigoTipoComunicacaoRastreador").val(),
            NumeroEquipamentoRastreador: $("#txtNumeroEquipamentoRastreador").val(),
            Motoristas: JSON.stringify($("body").data("motoristas"))
        };
        executarRest("/Veiculo/Salvar?callback=?", dados, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                LimparCampos();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção");
            }
        });
    }
}

function SalvarMotoristaAdicional() {
    if (ValidarCamposMotoristaAdicional()) {
        var motorista = {
            Codigo: $("body").data("motorista") != null ? $("body").data("motorista").Codigo : 0,
            CPF: $("#txtCPFMotoristaAdicional").val(),
            Nome: $("#txtNomeMotoristaAdicional").val(),
            Excluir: false
        };

        var motoristas = $("body").data("motoristas") == null ? new Array() : $("body").data("motoristas");

        motoristas.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (motorista.Codigo == 0)
            motorista.Codigo = (motoristas.length > 0 ? (motoristas[0].Codigo > 0 ? -1 : (motoristas[0].Codigo - 1)) : -1);

        for (var i = 0; i < motoristas.length; i++) {
            if (motoristas[i].Codigo == motorista.Codigo) {
                motoristas.splice(i, 1);
                break;
            }
        }

        motoristas.push(motorista);

        motoristas.sort(function (a, b) { return a.Nome < b.Nome ? -1 : 1; });

        $("body").data("motoristas", motoristas);

        RenderizarMotoristasAdicional();
        LimparCamposMotoristaAdicional();

    } else {
        ExibirMensagemAlerta("Os campos em vermelho são de preenchimento obrigatório!", "Atenção!");
    }
}

function RenderizarMotoristasAdicional(disabled) {
    var motoristas = $("body").data("motoristas") == null ? new Array() : $("body").data("motoristas");

    $("#tblMotoristasAdicionais tbody").html("");

    for (var i = 0; i < motoristas.length; i++) {
        if (!motoristas[i].Excluir)
            $("#tblMotoristasAdicionais tbody").append("<tr><td>" + motoristas[i].CPF + "</td><td>" + motoristas[i].Nome + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' " + (disabled ? "disabled" : "") + " onclick='EditarMotoristaAdicional(" + JSON.stringify(motoristas[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblMotoristasAdicionais tbody").html() == "")
        $("#tblMotoristasAdicionais tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposMotoristaAdicional() {
    $("body").data("motorista", null);
    $("#txtCPFMotoristaAdicional").val('');
    $("#txtNomeMotoristaAdicional").val('');
    $("#btnExcluirMotoristaAdicional").hide();
}

function ConsultarMotorista(cpf) {
    if (cpf.replace(/[^a-zA-Z0-9]/g, "") != "") {
        executarRest("/Motorista/BuscarPorCPF?callback=?", { CPF: cpf }, function (r) {
            if (r.Sucesso) {
                $("#txtCPFMotorista").val(r.Objeto.CPF);
                $("#txtNomeMotorista").val(r.Objeto.Nome);
            }
            else {
                $("#txtNomeMotorista").val("");
                ExibirMensagemAlerta("Motorista não possui cadastro!", "Atenção!");
            }
        });
    }
}

function ConsultarMotoristaAdicional(cpf) {
    if (cpf.replace(/[^a-zA-Z0-9]/g, "") != "") {
        executarRest("/Motorista/BuscarPorCPF?callback=?", { CPF: cpf }, function (r) {
            if (r.Sucesso) {
                $("#txtCPFMotoristaAdicional").val(r.Objeto.CPF);
                $("#txtNomeMotoristaAdicional").val(r.Objeto.Nome);
            }
            else {
                $("#txtNomeMotoristaAdicional").val("");
                ExibirMensagemAlerta("Motorista não possui cadastro!", "Atenção!");
            }
        });
    }
}

function EditarMotoristaAdicional(motorista) {
    $("body").data("motorista", motorista);
    $("#txtCPFMotoristaAdicional").val(motorista.CPF);
    $("#txtNomeMotoristaAdicional").val(motorista.Nome);
    $("#btnExcluirMotoristaAdicional").show();
}

function ExcluirMotoristaAdicional() {
    var motorista = $("body").data("motorista");

    var motoristas = $("body").data("motoristas") == null ? new Array() : $("body").data("motoristas");

    for (var i = 0; i < motoristas.length; i++) {
        if (motoristas[i].Codigo == motorista.Codigo) {
            if (motorista.Codigo <= 0)
                motoristas.splice(i, 1);
            else
                motoristas[i].Excluir = true;
            break;
        }
    }

    $("body").data("motoristas", motoristas);

    RenderizarMotoristasAdicional();
    LimparCamposMotoristaAdicional();
}

function ValidarCamposMotoristaAdicional() {
    var cpf = $("#txtCPFMotoristaAdicional").val();
    var nome = $("#txtNomeMotoristaAdicional").val();

    var valido = true;

    if (cpf != "") {
        CampoSemErro("#txtCPFMotoristaAdicional");
    } else {
        CampoComErro("#txtCPFMotoristaAdicional");
        valido = false;
    }

    if (nome != "") {
        CampoSemErro("#txtNomeMotoristaAdicional");
    } else {
        CampoComErro("#txtNomeMotoristaAdicional");
        valido = false;
    }

    return valido;
}