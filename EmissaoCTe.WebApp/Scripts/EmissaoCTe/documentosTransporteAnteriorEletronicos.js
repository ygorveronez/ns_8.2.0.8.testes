$(document).ready(function () {

    $("#txtChaveDocTranspAntEletronico").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");

    CarregarConsultadeClientes("btnBuscarEmissorDocTranspAntEletronico", "btnBuscarEmissorDocTranspAntEletronico", RetornoConsultaClienteDocTranspAntEletronico, true, false);

    $("#btnSalvarDocTranspAntEletronico").click(function () {
        SalvarDocTranspAntEletronico();
    });

    $("#btnExcluirDocTranspAntEletronico").click(function () {
        ExcluirDocTranspAntEletronico();
    });

    $("#btnCancelarDocTranspAntEletronico").click(function () {
        LimparCamposDocTranspAntEletronico();
    });

    $("#txtEmissorDocTranspAntEletronico").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                LimparCamposEmissorDocumentoAnteriorEletronico();
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtCPFCNPJEmissorDocTranspAntEletronico").focusout(function () {
        BuscarEmissorDocumentoAnteriorEletronico();
    });
});

function BuscarEmissorDocumentoAnteriorEletronico() {
    if ($("#hddEmissorDocTranspAntEletronico").val() != $("#txtCPFCNPJEmissorDocTranspAntEletronico").val()) {
        var cpfCnpj = $("#txtCPFCNPJEmissorDocTranspAntEletronico").val().replace(/[^0-9]/g, '');
        if (cpfCnpj != "") {
            if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto != null) {
                            $("#hddEmissorDocTranspAntEletronico").val(r.Objeto.CPF_CNPJ);
                            $("#txtCPFCNPJEmissorDocTranspAntEletronico").val(r.Objeto.CPF_CNPJ);
                            $("#txtEmissorDocTranspAntEletronico").val(r.Objeto.Nome);
                        } else {
                            LimparCamposEmissorDocumentoAnteriorEletronico();
                            jAlert("Emissor não encontrado.", "Atenção");
                        }
                    } else {
                        LimparCamposEmissorDocumentoAnteriorEletronico();
                        jAlert(r.Erro, "Erro");
                    }
                });
            } else {
                LimparCamposEmissorDocumentoAnteriorEletronico();
                jAlert("O CPF/CNPJ digitado é inválido!", "Atenção");
            }
        } else {
            LimparCamposEmissorDocumentoAnteriorEletronico();
        }
    }
}

function LimparCamposEmissorDocumentoAnteriorEletronico() {
    $("#hddEmissorDocTranspAntEletronico").val("");
    $("#txtCPFCNPJEmissorDocTranspAntEletronico").val("");
    $("#txtEmissorDocTranspAntEletronico").val("");
}

function RetornoConsultaClienteDocTranspAntEletronico(cliente) {
    $("#txtCPFCNPJEmissorDocTranspAntEletronico").val(cliente.CPFCNPJ);
    $("#txtEmissorDocTranspAntEletronico").val(cliente.Nome);
    $("#hddEmissorDocTranspAntEletronico").val(cliente.CPFCNPJ);
}

function LimparCamposDocTranspAntEletronico() {
    $("#hddDocTranspAntEletronicoEmEdicao").val("0");
    $("#txtEmissorDocTranspAntEletronico").val("");
    $("#txtCPFCNPJEmissorDocTranspAntEletronico").val("");
    $("#hddEmissorDocTranspAntEletronico").val("");
    $("#txtChaveDocTranspAntEletronico").val("");
    $("#btnExcluirDocTranspAntEletronico").hide();
}

function ValidarCamposDocTranspAntEletronico() {
    var emissor = $("#hddEmissorDocTranspAntEletronico").val();
    var chave = $("#txtChaveDocTranspAntEletronico").val();
    var valido = true;
    if (emissor != "") {
        CampoSemErro("#txtEmissorDocTranspAntEletronico");
        CampoSemErro("#txtCPFCNPJEmissorDocTranspAntEletronico");
    } else {
        CampoComErro("#txtEmissorDocTranspAntEletronico");
        CampoComErro("#txtCPFCNPJEmissorDocTranspAntEletronico");
        valido = false;
    }
    if (chave != "") {
        CampoSemErro("#txtChaveDocTranspAntEletronico");
    } else {
        CampoComErro("#txtChaveDocTranspAntEletronico");
        valido = false;
    }
    return valido;
}

function SalvarDocTranspAntEletronico() {
    if (ValidarCamposDocTranspAntEletronico()) {
        var documentoAnterior = {
            Codigo: Globalize.parseInt($("#hddDocTranspAntEletronicoEmEdicao").val()),
            Emissor: $("#hddEmissorDocTranspAntEletronico").val(),
            NomeEmissor: $("#txtEmissorDocTranspAntEletronico").val(),
            Chave: $("#txtChaveDocTranspAntEletronico").val(),
            Excluir: false
        };
        var documentos = $("#hddDocsTranspAntEletronico").val() == "" ? new Array() : JSON.parse($("#hddDocsTranspAntEletronico").val());
        if (documentoAnterior.Codigo == 0)
            documentoAnterior.Codigo = -(documentos.length + 1);
        if (documentos.length > 0) {
            for (var i = 0; i < documentos.length; i++) {
                if (documentos[i].Codigo == documentoAnterior.Codigo) {
                    documentos.splice(i, 1);
                    break;
                }
            }
        }
        documentos.push(documentoAnterior);
        documentos.sort();
        $("#hddDocsTranspAntEletronico").val(JSON.stringify(documentos));
        RenderizarDocTranspAntEletronico();
        LimparCamposDocTranspAntEletronico();
    }
}

function EditarDocTranspAntEletronico(documento) {
    $("#hddDocTranspAntEletronicoEmEdicao").val(documento.Codigo);
    $("#txtCPFCNPJEmissorDocTranspAntEletronico").val(documento.Emissor);
    $("#hddEmissorDocTranspAntEletronico").val(documento.Emissor);
    $("#txtChaveDocTranspAntEletronico").val(documento.Chave);
    $("#txtEmissorDocTranspAntEletronico").val(documento.NomeEmissor);
    $("#btnExcluirDocTranspAntEletronico").show();
}

function ExcluirDocTranspAntEletronico() {
    //jConfirm("Deseja realmente excluir este documento de transporte anterior?", "Atenção", function (r) {
    //    if (r) {
            var codigo = Globalize.parseInt($("#hddDocTranspAntEletronicoEmEdicao").val());
            var documentos = $("#hddDocsTranspAntEletronico").val() == "" ? new Array() : JSON.parse($("#hddDocsTranspAntEletronico").val());
            for (var i = 0; i < documentos.length; i++) {
                if (documentos[i].Codigo == codigo) {
                    if (codigo > 0) {
                        documentos[i].Excluir = true;
                    } else {
                        documentos.splice(i, 1);
                    }
                    break;
                }
            }
            $("#hddDocsTranspAntEletronico").val(JSON.stringify(documentos));
            RenderizarDocTranspAntEletronico();
            LimparCamposDocTranspAntEletronico();
    //    }
    //});
}

function RenderizarDocTranspAntEletronico() {
    $("#tblDocsTranspAntEletronico tbody").html("");
    var documentos = $("#hddDocsTranspAntEletronico").val() == "" ? new Array() : JSON.parse($("#hddDocsTranspAntEletronico").val());
    for (var i = 0; i < documentos.length; i++) {
        if (!documentos[i].Excluir) {
            $("#tblDocsTranspAntEletronico tbody").append("<tr><td>" + documentos[i].Emissor + "</td><td>" + documentos[i].Chave + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarDocTranspAntEletronico(" + JSON.stringify(documentos[i]) + ")'>Editar</button></td></tr>");
        }
    }
    if ($("#tblDocsTranspAntEletronico tbody").html() == "") {
        $("#tblDocsTranspAntEletronico tbody").html("<tr><td colspan='3'>Nenhum registro encontrado!</td></tr>");
    }
}