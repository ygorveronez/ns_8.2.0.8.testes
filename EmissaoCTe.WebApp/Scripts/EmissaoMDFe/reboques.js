$(document).ready(function () {
    $("#txtPlacaReboque").mask("*******");
    $("#txtTaraKGReboque").priceFormat({ limit: 6, centsLimit: 0, centsSeparator: '' });
    $("#txtCapacidadeKGReboque").priceFormat({ limit: 6, centsLimit: 0, centsSeparator: '' });
    $("#txtCapacidadeM3Reboque").priceFormat({ limit: 3, centsLimit: 0, centsSeparator: '' });
    $("#txtRNTRCReboque").mask("99999999");
    $("#txtCPFCNPJProprietarioReboque").mask("99999999999?999");

    CarregarConsultaDeVeiculos("btnBuscarReboque", "btnBuscarReboque", RetornoConsultaReboque, true, false, "1");

    $("#btnSalvarReboque").click(function () {
        SalvarReboque();
    });

    $("#btnExcluirReboque").click(function () {
        ExcluirReboque();
    });

    $("#btnCancelarReboque").click(function () {
        LimparCamposReboque();
    });

    $("#txtPlacaReboque").focusout(function () {
        ConsultarReboque($("#txtPlacaReboque").val());
    });
});

function RetornoConsultaReboque(reboque) {
    ConsultarReboque(reboque.Placa);
}

function ConsultarReboque(placa) {
    if (placa.replace(/[^a-zA-Z0-9]/g, "") != "") {
        executarRest("/Veiculo/BuscarPorPlacaETipoVeiculo?callback=?", { Placa: placa, Tipo: "1" }, function (r) {
            if (r.Sucesso) {

                $("#txtPlacaReboque").val(r.Objeto.Veiculo.Placa.toUpperCase());
                $("#txtRENAVAMReboque").val(r.Objeto.Veiculo.RENAVAM);
                $("#txtTaraKGReboque").val(Globalize.format(r.Objeto.Veiculo.Tara, "n0"));
                $("#txtCapacidadeKGReboque").val(Globalize.format(r.Objeto.Veiculo.CapacidadeKG, "n0"));
                $("#txtCapacidadeM3Reboque").val(Globalize.format(r.Objeto.Veiculo.CapacidadeM3, "n0"));
                $("#txtRNTRCReboque").val(r.Objeto.Veiculo.RNTRC);
                $("#selCarroceriaReboque").val(r.Objeto.Veiculo.TipoCarroceria);
                $("#selUFReboque").val(r.Objeto.Veiculo.UF);
                $("#txtCPFCNPJProprietarioReboque").val(r.Objeto.Veiculo.CPFCNPJ);
                $("#txtIEProprietarioReboque").val(r.Objeto.Veiculo.IE);
                $("#txtNomeProprietarioReboque").val(r.Objeto.Veiculo.Nome);
                $("#selUFProprietarioReboque").val(r.Objeto.Veiculo.UFProprietario);
                $("#selTipoProprietarioReboque").val(r.Objeto.Veiculo.TipoProprietario);

            }
        });
    }
}

function SalvarReboque() {
    if (ValidarCamposReboque()) {

        var reboque = {
            Codigo: $("body").data("reboque") != null ? $("body").data("reboque").Codigo : 0,
            Placa: $("#txtPlacaReboque").val().toUpperCase(),
            RENAVAM: $("#txtRENAVAMReboque").val(),
            Tara: Globalize.parseInt($("#txtTaraKGReboque").val()),
            CapacidadeKG: Globalize.parseInt($("#txtCapacidadeKGReboque").val()),
            CapacidadeM3: Globalize.parseInt($("#txtCapacidadeM3Reboque").val()),
            RNTRC: $("#txtRNTRCReboque").val(),
            TipoCarroceria: $("#selCarroceriaReboque").val(),
            UF: $("#selUFReboque").val(),
            CPFCNPJ: $("#txtCPFCNPJProprietarioReboque").val(),
            IE: $("#txtIEProprietarioReboque").val(),
            Nome: $("#txtNomeProprietarioReboque").val(),
            UFProprietario: $("#selUFProprietarioReboque").val(),
            TipoProprietario: $("#selTipoProprietarioReboque").val(),
            Excluir: false
        };

        var reboques = $("body").data("reboques") == null ? new Array() : $("body").data("reboques");

        reboques.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (reboque.Codigo == 0)
            reboque.Codigo = (reboques.length > 0 ? (reboques[0].Codigo > 0 ? -1 : (reboques[0].Codigo - 1)) : -1);

        for (var i = 0; i < reboques.length; i++) {
            if (reboques[i].Codigo == reboque.Codigo) {
                reboques.splice(i, 1);
                break;
            }
        }

        reboques.push(reboque);

        if (reboques.length > 3) {

            var countReboques = 0;

            for (var i = 0; i < reboques.length; i++) {
                if (!reboques[i].Excluir)
                    countReboques++;
            }

            if (countReboques > 3) {
                ExibirMensagemAlerta("Número máximo de reboques (3) já foi preenchido.", "Atenção!", "placeholder-msgEmissaoMDFe");
                return;
            }
        }

        reboques.sort(function (a, b) { return a.Placa < b.Placa ? -1 : 1; });

        $("body").data("reboques", reboques);

        RenderizarReboques();
        LimparCamposReboque();

    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoMDFe");
    }
}

function EditarReboque(reboque) {
    $("body").data("reboque", reboque);
    $("#txtPlacaReboque").val(reboque.Placa.toUpperCase());
    $("#txtRENAVAMReboque").val(reboque.RENAVAM);
    $("#txtTaraKGReboque").val(Globalize.format(reboque.Tara, "n0"));
    $("#txtCapacidadeKGReboque").val(Globalize.format(reboque.CapacidadeKG, "n0"));
    $("#txtCapacidadeM3Reboque").val(Globalize.format(reboque.CapacidadeM3, "n0"));
    $("#txtRNTRCReboque").val(reboque.RNTRC);
    $("#selCarroceriaReboque").val(reboque.TipoCarroceria);
    $("#selUFReboque").val(reboque.UF);
    $("#txtCPFCNPJProprietarioReboque").val(reboque.CPFCNPJ);
    $("#txtIEProprietarioReboque").val(reboque.IE);
    $("#txtNomeProprietarioReboque").val(reboque.Nome);
    $("#selUFProprietarioReboque").val(reboque.UFProprietario);
    $("#selTipoProprietarioReboque").val(reboque.TipoProprietario);
    $("#btnExcluirReboque").show();
}

function ExcluirReboque() {
    var reboque = $("body").data("reboque");

    var reboques = $("body").data("reboques") == null ? new Array() : $("body").data("reboques");

    for (var i = 0; i < reboques.length; i++) {
        if (reboques[i].Codigo == reboque.Codigo) {
            if (reboque.Codigo <= 0)
                reboques.splice(i, 1);
            else
                reboques[i].Excluir = true;
            break;
        }
    }

    $("body").data("reboques", reboques);

    RenderizarReboques();
    LimparCamposReboque();
}

function ValidarCamposReboque() {
    var placa = $("#txtPlacaReboque").val();
    var tara = Globalize.parseInt($("#txtTaraKGReboque").val());
    var rntrc = $("#txtRNTRCReboque").val();
    var tipoCarroceria = $("#selCarroceriaReboque").val();
    var uf = $("#selUFReboque").val();
    var cpfCnpj = $("#txtCPFCNPJProprietarioReboque").val();
    var nome = $("#txtNomeProprietarioReboque").val();
    var ufProprietario = $("#selUFProprietarioReboque").val();
    var tipoProprietario = $("#selTipoProprietarioReboque").val();

    var valido = true;

    if (placa != "") {
        CampoSemErro("#txtPlacaReboque");
    } else {
        CampoComErro("#txtPlacaReboque");
        valido = false;
    }

    if (isNaN(tara) || tara <= 0) {
        CampoComErro("#txtTaraKGReboque");
        valido = false;
    } else {
        CampoSemErro("#txtTaraKGReboque");
    }

    if (tipoCarroceria == null || tipoCarroceria == "") {
        CampoComErro("#selCarroceriaReboque");
        valido = false;
    } else {
        CampoSemErro("#selCarroceriaReboque");
    }

    if (uf == null || uf == "") {
        CampoComErro("#selUFReboque");
        valido = false;
    } else {
        CampoSemErro("#selUFReboque");
    }

    if (cpfCnpj != "") {
        if (cpfCnpj.length != 11 && cpfCnpj.length != 14) {
            CampoComErro("#txtCPFCNPJProprietarioReboque");
            valido = false;
        } else {
            CampoSemErro("#txtCPFCNPJProprietarioReboque");
        }

        if (rntrc == "" || rntrc.length != 8) {
            CampoComErro("#txtRNTRCReboque");
            valido = false;
        } else {
            CampoSemErro("#txtRNTRCReboque");
        }

        if (nome == "") {
            CampoComErro("#txtNomeProprietarioReboque");
            valido = false;
        } else {
            CampoSemErro("#txtNomeProprietarioReboque");
        }

        if (ufProprietario == null || ufProprietario == "") {
            CampoComErro("#selUFProprietarioReboque");
            valido = false;
        } else {
            CampoSemErro("#selUFProprietarioReboque");
        }

        if (tipoProprietario == null || tipoProprietario == "") {
            CampoComErro("#selTipoProprietarioReboque");
            valido = false;
        } else {
            CampoSemErro("#selTipoProprietarioReboque");
        }
    }

    return valido;
}

function LimparCamposReboque() {
    $("body").data("reboque", null);
    $("#txtPlacaReboque").val('');
    $("#txtRENAVAMReboque").val('');
    $("#txtTaraKGReboque").val('0');
    $("#txtCapacidadeKGReboque").val('0');
    $("#txtCapacidadeM3Reboque").val('0');
    $("#txtRNTRCReboque").val('');
    $("#selCarroceriaReboque").val("");
    $("#selUFReboque").val("");
    $("#txtCPFCNPJProprietarioReboque").val("");
    $("#txtIEProprietarioReboque").val("");
    $("#txtNomeProprietarioReboque").val("");
    $("#selUFProprietarioReboque").val("");
    $("#selTipoProprietarioReboque").val("");
    $("#btnExcluirReboque").hide();
}

function RenderizarReboques(disabled) {
    var reboques = $("body").data("reboques") == null ? new Array() : $("body").data("reboques");

    $("#tblReboques tbody").html("");

    for (var i = 0; i < reboques.length; i++) {
        if (!reboques[i].Excluir)
            $("#tblReboques tbody").append("<tr><td>" + reboques[i].Placa + "</td><td>"+ reboques[i].RENAVAM +"</td><td>" + Globalize.format(reboques[i].Tara, "n0") + "</td><td>" + (reboques[i].CapacidadeKG > 0 ? Globalize.format(reboques[i].CapacidadeKG, "n0") : "") + "</td><td>" + (reboques[i].CapacidadeM3 > 0 ? Globalize.format(reboques[i].CapacidadeM3, "n0") : "") + "</td><td>" + reboques[i].RNTRC + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' " + (disabled ? "disabled" : "") + " onclick='EditarReboque(" + JSON.stringify(reboques[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblReboques tbody").html() == "")
        $("#tblReboques tbody").html("<tr><td colspan='7'>Nenhum registro encontrado.</td></tr>");
}