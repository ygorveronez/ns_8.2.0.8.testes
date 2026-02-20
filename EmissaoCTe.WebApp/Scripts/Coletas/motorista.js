$(document).ready(function () {
    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);

    $("#txtCPFMotorista").mask("99999999999");

    $("#btnSalvarMotorista").click(function () {
        SalvarMotorista();
    });

    $("#btnExcluirMotorista").click(function () {
        ExcluirMotorista();
    });

    $("#btnCancelarMotorista").click(function () {
        LimparCamposMotorista();
    });

    $("#txtCPFMotorista").focusout(function () {
        ConsultarMotorista($(this).val());
    });

    $("#txtNomeMotorista").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("codigoMotorista", null);
                $("#txtCPFMotorista").val("");
            }
            e.preventDefault();
        }
    });
});

function RetornoConsultaMotorista(motorista) {
    $("body").data("codigoMotorista", motorista.Codigo);
    $("#txtCPFMotorista").val(motorista.CPFCNPJ);
    $("#txtNomeMotorista").val(motorista.Nome);
}

function ValidarCamposMotorista() {
    var codigoMotorista = $("body").data("codigoMotorista");

    var valido = true;

    if (codigoMotorista != null && codigoMotorista > 0) {
        CampoSemErro("#txtCPFMotorista");
    } else {
        CampoComErro("#txtCPFMotorista");
        valido = false;
    }

    return valido;
}

function ConsultarMotorista(cpf) {
    if (cpf.replace(/[^a-zA-Z0-9]/g, "") != "") {
        executarRest("/Motorista/BuscarPorCPF?callback=?", { CPF: cpf }, function (r) {
            if (r.Sucesso) {
                $("body").data("codigoMotorista", r.Objeto.Codigo);
                $("#txtCPFMotorista").val(r.Objeto.CPF);
                $("#txtNomeMotorista").val(r.Objeto.Nome);
            }
        });
    }
}

function SalvarMotorista() {
    if (ValidarCamposMotorista()) {
        var motorista = {
            Codigo: $("body").data("codigoMotorista") != null ? $("body").data("codigoMotorista") : 0,
            CPF: $("#txtCPFMotorista").val(),
            Nome: $("#txtNomeMotorista").val()
        };

        var motoristas = $("body").data("motoristas") == null ? new Array() : $("body").data("motoristas");

        for (var i = 0; i < motoristas.length; i++) {
            if (motoristas[i].Codigo == motorista.Codigo) {
                motoristas.splice(i, 1);
                break;
            }
        }

        motoristas.push(motorista);

        motoristas.sort(function (a, b) { return a.Nome < b.Nome ? -1 : 1; });

        $("body").data("motoristas", motoristas);

        RenderizarMotoristas();
        LimparCamposMotorista();

    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoMDFe");
    }
}

function ExcluirMotorista(motorista) {
    jConfirm("Deseja realmente excluir este motorista?", "Atenção!", function (r) {
        if (r) {
            var motoristas = $("body").data("motoristas") == null ? new Array() : $("body").data("motoristas");

            for (var i = 0; i < motoristas.length; i++) {
                if (motoristas[i].Codigo == motorista.Codigo) {
                    motoristas.splice(i, 1);
                    break;
                }
            }

            $("body").data("motoristas", motoristas);

            RenderizarMotoristas();
            LimparCamposMotorista();
        }
    });
}

function RenderizarMotoristas(disabled) {
    var motoristas = $("body").data("motoristas") == null ? new Array() : $("body").data("motoristas");

    $("#tblMotoristas tbody").html("");

    for (var i = 0; i < motoristas.length; i++) {
        if (!motoristas[i].Excluir)
            $("#tblMotoristas tbody").append("<tr><td>" + motoristas[i].CPF + "</td><td>" + motoristas[i].Nome + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' " + (disabled ? "disabled" : "") + " onclick='ExcluirMotorista(" + JSON.stringify(motoristas[i]) + ")'>Excluir</button></td></tr>");
    }

    if ($("#tblMotoristas tbody").html() == "")
        $("#tblMotoristas tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposMotorista() {
    $("body").data("codigoMotorista", null);
    $("#txtCPFMotorista").val('');
    $("#txtNomeMotorista").val('');
}