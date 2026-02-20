$(document).ready(function () {
    $("#txtPlacaVeiculo").mask("*******");
    $("#txtTaraVeiculo").priceFormat({ limit: 6, centsLimit: 0, centsSeparator: '' });
    $("#txtCapacidadeKGVeiculo").priceFormat({ limit: 6, centsLimit: 0, centsSeparator: '' });
    $("#txtCapacidadeM3Veiculo").priceFormat({ limit: 3, centsLimit: 0, centsSeparator: '' });
    $("#txtRNTRCVeiculo").mask("99999999");
    $("#txtCPFCNPJProprietarioVeiculo").mask("99999999999?999");

    CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false, "0");

    $("#txtPlacaVeiculo").focusout(function () {
        ConsultarVeiculo($("#txtPlacaVeiculo").val());
    });

    $("#txtIEProprietarioVeiculo").blur(ConfereIEProprietarioVeiculo);
    $("#txtIEProprietarioReboque").blur(ConfereIEProprietarioReboque);
});

function ConfereIEProprietarioVeiculo() {
    ConfereIEProprietario($("#txtIEProprietarioVeiculo"), "Veículo");
}

function ConfereIEProprietarioReboque() {
    ConfereIEProprietario($("#txtIEProprietarioReboque"), "Reboque");
}

function ConfereIEProprietario($txt, tipoVeiculo) {
    if (/(isento|isent|isen|ise|ISENTO)/.test($txt.val())) {
        $txt.val("ISENTO");
        return true;
    } else if ($txt.val() != "") { //&& !((/^[0-9]$/).test($txt.val()))
        var inscricao = $txt.val().replace(/[^0-9]/g, '');
        if (inscricao == "") {
            $txt.val("");
            ExibirMensagemAlerta("IE do Proprietário do " + tipoVeiculo + " deve ser apenas número ou ISENTO.", "Atenção!", "placeholder-msgEmissaoMDFe");
        }
        else
            $txt.val(inscricao);
    }
}

function LimparCamposVeiculo() {
    $("#txtPlacaVeiculo").val("");
    $("#txtRENAVAMVeiculo").val("");
    $("#txtTaraVeiculo").val("");
    $("#txtCapacidadeKGVeiculo").val("");
    $("#txtCapacidadeM3Veiculo").val("");
    $("#txtRNTRCVeiculo").val("");
    $("#selRodadoVeiculo").val("");
    $("#selCarroceriaVeiculo").val("");
    $("#selUFVeiculo").val("");
    $("#txtCPFCNPJProprietarioVeiculo").val("");
    $("#txtIEProprietarioVeiculo").val("");
    $("#txtNomeProprietarioVeiculo").val("");
    $("#selUFProprietarioVeiculo").val("");
    $("#selTipoProprietarioVeiculo").val("");
}

function RetornoConsultaVeiculo(veiculo) {
    ConsultarVeiculo(veiculo.Placa);
}

function ConsultarVeiculo(placa) {
    if (placa.replace(/[^a-zA-Z0-9]/g, "") != "") {
        executarRest("/Veiculo/BuscarPorPlacaETipoVeiculo?callback=?", { Placa: placa, Tipo: "0" }, function (r) {
            if (r.Sucesso) {

                $("#txtPlacaVeiculo").val(r.Objeto.Veiculo.Placa.toUpperCase());
                $("#txtRENAVAMVeiculo").val(r.Objeto.Veiculo.RENAVAM);
                $("#txtTaraVeiculo").val(Globalize.format(r.Objeto.Veiculo.Tara, "n0"));
                $("#txtCapacidadeKGVeiculo").val(Globalize.format(r.Objeto.Veiculo.CapacidadeKG, "n0"));
                $("#txtCapacidadeM3Veiculo").val(Globalize.format(r.Objeto.Veiculo.CapacidadeM3, "n0"));
                $("#selRodadoVeiculo").val(r.Objeto.Veiculo.TipoRodado);
                $("#selCarroceriaVeiculo").val(r.Objeto.Veiculo.TipoCarroceria);
                $("#selUFVeiculo").val(r.Objeto.Veiculo.UF);
                $("#txtCPFCNPJProprietarioVeiculo").val(r.Objeto.Veiculo.CPFCNPJ);
                $("#txtIEProprietarioVeiculo").val(r.Objeto.Veiculo.IE);
                $("#txtNomeProprietarioVeiculo").val(r.Objeto.Veiculo.Nome);
                $("#selUFProprietarioVeiculo").val(r.Objeto.Veiculo.UFProprietario);
                $("#selTipoProprietarioVeiculo").val(r.Objeto.Veiculo.TipoProprietario);
                $("#txtRNTRCVeiculo").val(r.Objeto.Veiculo.RNTRC);

                if (r.Objeto.Reboques != null) {

                    var reboques = $("body").data("reboques") == null ? new Array() : $("body").data("reboques");

                    if (reboques.length <= 0) {

                        for (var i = 0; i < r.Objeto.Reboques.length; i++) {

                            var reboque = {
                                Codigo: -(i + 1),
                                Placa: r.Objeto.Reboques[i].Placa.toUpperCase(),
                                RENAVAM: r.Objeto.Reboques[i].RENAVAM,
                                Tara: r.Objeto.Reboques[i].Tara,
                                CapacidadeKG: r.Objeto.Reboques[i].CapacidadeKG,
                                CapacidadeM3: r.Objeto.Reboques[i].CapacidadeM3,
                                TipoCarroceria: r.Objeto.Reboques[i].TipoCarroceria,
                                UF: r.Objeto.Reboques[i].UF,
                                CPFCNPJ: r.Objeto.Reboques[i].CPFCNPJ,
                                IE: r.Objeto.Reboques[i].IE,
                                Nome: r.Objeto.Reboques[i].Nome,
                                RNTRC: r.Objeto.Reboques[i].RNTRC,
                                UFProprietario: r.Objeto.Reboques[i].UFProprietario,
                                TipoProprietario: r.Objeto.Reboques[i].TipoProprietario,
                                Excluir: false
                            };

                            reboques.push(reboque);
                        }

                        $("body").data("reboques", reboques);

                        RenderizarReboques();
                    }

                }

                if (r.Objeto.NomeMotorista != "" && r.Objeto.CPFMotorista != "") {
                    var motoristas = $("body").data("motoristas") == null ? new Array() : $("body").data("motoristas");

                    if (motoristas.length <= 0) {

                        var motorista = {
                            Codigo: -1,
                            CPF: r.Objeto.CPFMotorista,
                            Nome: r.Objeto.NomeMotorista,
                            Excluir: false
                        }

                        motoristas.push(motorista);

                        $("body").data("motoristas", motoristas);

                        RenderizarMotoristas();
                    }
                }

                if (r.Objeto.Motoristas != null && r.Objeto.Motoristas.length > 0) {
                    var motoristas = $("body").data("motoristas") == null ? new Array() : $("body").data("motoristas");

                    if (motoristas.length <= 1) {
                        for (var i = 0; i < r.Objeto.Motoristas.length; i++) {
                            var motorista = {
                                Codigo: -1,
                                CPF: r.Objeto.Motoristas[i].CPF,
                                Nome: r.Objeto.Motoristas[i].Nome,
                                Excluir: false
                            }

                            motoristas.push(motorista);

                            $("body").data("motoristas", motoristas);

                            RenderizarMotoristas();
                        }
                    }
                }

            }
        });
    }
}

function ObterVeiculo() {
    var veiculo = {
        Placa: $("#txtPlacaVeiculo").val().toUpperCase(),
        RENAVAM: $("#txtRENAVAMVeiculo").val(),
        Tara: Globalize.parseInt($("#txtTaraVeiculo").val()),
        CapacidadeKG: Globalize.parseInt($("#txtCapacidadeKGVeiculo").val()),
        CapacidadeM3: Globalize.parseInt($("#txtCapacidadeM3Veiculo").val()),
        RNTRC: $("#txtRNTRCVeiculo").val(),
        TipoRodado: $("#selRodadoVeiculo").val(),
        TipoCarroceria: $("#selCarroceriaVeiculo").val(),
        UF: $("#selUFVeiculo").val(),
        CPFCNPJ: $("#txtCPFCNPJProprietarioVeiculo").val(),
        IE: $("#txtIEProprietarioVeiculo").val(),
        Nome: $("#txtNomeProprietarioVeiculo").val(),
        UFProprietario: $("#selUFProprietarioVeiculo").val(),
        TipoProprietario: $("#selTipoProprietarioVeiculo").val()
    };

    return veiculo;
}