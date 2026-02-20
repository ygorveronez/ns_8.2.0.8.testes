$(document).ready(function () {
    $("#txtCPFMotorista").mask("99999999999");

    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);

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
});

function RetornoConsultaMotorista(motorista) {
    $("#txtCPFMotorista").val(motorista.CPFCNPJ);
    $("#txtNomeMotorista").val(motorista.Nome);
}

function ValidarCamposMotorista() {
    var cpf = $("#txtCPFMotorista").val();
    var nome = $("#txtNomeMotorista").val();
    var valido = true;

    if (cpf != "") {
        CampoSemErro("#txtCPFMotorista");
    } else {
        CampoComErro("#txtCPFMotorista");
        valido = false;
    }

    if (nome != "") {
        CampoSemErro("#txtNomeMotorista");
    } else {
        CampoComErro("#txtNomeMotorista");
        valido = false;
    }

    return valido;
}

function ConsultarMotorista(cpf) {
    if (cpf.replace(/[^a-zA-Z0-9]/g, "") != "") {
        executarRest("/Motorista/BuscarPorCPF?callback=?", { CPF: cpf }, function (r) {
            if (r.Sucesso) {
                $("#txtCPFMotorista").val(r.Objeto.CPF);
                $("#txtNomeMotorista").val(r.Objeto.Nome);
            }
        });
    }
}

function SalvarMotorista() {
    if (ValidarCamposMotorista()) {
        var motorista = {
            Codigo: $("body").data("motorista") != null ? $("body").data("motorista").Codigo : 0,
            CPF: $("#txtCPFMotorista").val(),
            Nome: $("#txtNomeMotorista").val(),
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

        RenderizarMotoristas();
        LimparCamposMotorista();

    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoMDFe");
    }
}

function EditarMotorista(motorista) {
    $("body").data("motorista", motorista);
    $("#txtCPFMotorista").val(motorista.CPF);
    $("#txtNomeMotorista").val(motorista.Nome);
    $("#btnExcluirMotorista").show();
}

function ExcluirMotorista() {
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

    RenderizarMotoristas();
    LimparCamposMotorista();
}

function RenderizarMotoristas(disabled) {
    var motoristas = $("body").data("motoristas") == null ? new Array() : $("body").data("motoristas");

    $("#tblMotoristas tbody").html("");

    for (var i = 0; i < motoristas.length; i++) {
        if (!motoristas[i].Excluir)
            $("#tblMotoristas tbody").append("<tr><td>" + motoristas[i].CPF + "</td><td>" + motoristas[i].Nome + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' " + (disabled ? "disabled" : "") + " onclick='EditarMotorista(" + JSON.stringify(motoristas[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblMotoristas tbody").html() == "")
        $("#tblMotoristas tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposMotorista() {
    $("body").data("motorista", null);
    $("#txtCPFMotorista").val('');
    $("#txtNomeMotorista").val('');
    $("#btnExcluirMotorista").hide();
}