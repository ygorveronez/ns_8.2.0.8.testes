$(document).ready(function () {
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
    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);

    $("#txtNomeMotorista").change(function () {
        this.value = RemoveAspas(this.value);
    });
});


function RemoveAspas(val) {
    return val.replace(/"/gm, '').replace(/'/gm, '');
}
function RetornoConsultaMotorista(motorista) {
    LimparCamposMotorista();
    $("#txtNomeMotorista").val(motorista.Nome).trigger("change");
    $("#txtCPFMotorista").val(motorista.CPFCNPJ);
}

function LimparCamposMotorista() {
    $("#txtNomeMotorista").val("");
    $("#txtCPFMotorista").val("");
    $("#hddIdMotoristaEmEdicao").val("0");
    $("#btnExcluirMotorista").hide();
}

function ValidarCamposMotorista() {
    var nome = $("#txtNomeMotorista").val();
    var cpf = $("#txtCPFMotorista").val();
    var valido = true;
    if (nome != "") {
        CampoSemErro("#txtNomeMotorista");
    } else {
        CampoComErro("#txtNomeMotorista");
        valido = false;
    }
    if (cpf != "") {
        CampoSemErro("#txtCPFMotorista");
    } else {
        CampoComErro("#txtCPFMotorista");
        valido = false;
    }
    return valido;
}

function SalvarMotorista() {
    if (ValidarCamposMotorista()) {
        var motorista = {
            Codigo: Globalize.parseInt($("#hddIdMotoristaEmEdicao").val()),
            Nome: $("#txtNomeMotorista").val(),
            CPF: $("#txtCPFMotorista").val(),
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
        LimparCamposMotorista();
    }
}

function EditarMotorista(motorista) {
    $("#hddIdMotoristaEmEdicao").val(motorista.Codigo);
    $("#txtNomeMotorista").val(motorista.Nome).trigger("change");
    $("#txtCPFMotorista").val(motorista.CPF);
    $("#btnExcluirMotorista").show();
}

function ExcluirMotorista() {
    jConfirm("Deseja realmente excluir este motorista?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddIdMotoristaEmEdicao").val());
            var motoristas = $("#hddMotoristas").val() == "" ? new Array() : JSON.parse($("#hddMotoristas").val());
            for (var i = 0; i < motoristas.length; i++) {
                if (motoristas[i].Codigo == codigo) {
                    if (codigo > 0) {
                        motoristas[i].Excluir = true;
                    } else {
                        motoristas.splice(i, 1);
                    }
                    break;
                }
            }
            $("#hddMotoristas").val(JSON.stringify(motoristas));
            RenderizarMotoristas();
            LimparCamposMotorista();
            LimparCamposMotoristaResumo();
        }
    });
}

function RenderizarMotoristas() {
    $("#tblMotoristas tbody").html("");
    var motoristas = $("#hddMotoristas").val() == "" ? new Array() : JSON.parse($("#hddMotoristas").val());
    for (var i = 0; i < motoristas.length; i++) {
        if (!motoristas[i].Excluir) {
            motoristas[i].Nome = RemoveAspas(motoristas[i].Nome);
            $("#tblMotoristas tbody").append("<tr>" +
                "<td class=\"text-uppercase\">" + motoristas[i].Nome + "</td>" +
                "<td>" + motoristas[i].CPF + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarMotorista(" + JSON.stringify(motoristas[i]) + ")'>Editar</button></td>" +
            "</tr>");
        }
    }
    if ($("#tblMotoristas tbody").html() == "") {
        $("#tblMotoristas tbody").html("<tr><td colspan='3'>Nenhum registro encontrado!</td></tr>");
    }

    RenderizarMotoristasResumo();
}