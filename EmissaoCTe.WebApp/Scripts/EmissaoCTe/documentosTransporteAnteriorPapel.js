$(document).ready(function () {
    $("#txtDataEmissaoDocTranspAntPapel").mask("99/99/9999");
    
    FormatarCampoDate("txtDataEmissaoDocTranspAntPapel");

    CarregarConsultadeClientes("btnBuscarEmissorDocTranspAntPapel", "btnBuscarEmissorDocTranspAntPapel", RetornoConsultaClienteDocTranspAntPapel, true, false);

    $("#btnSalvarDocTranspAntPapel").click(function () {
        SalvarDocTranspAntPapel();
    });

    $("#btnExcluirDocTranspAntPapel").click(function () {
        ExcluirDocTranspAntPapel();
    });

    $("#btnCancelarDocTranspAntPapel").click(function () {
        LimparCamposDocTranspAntPapel();
    });

    $("#txtEmissorDocTranspAntPapel").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                LimparCamposEmissorDocumentoAnteriorPapel();
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtCPFCNPJEmissorDocTranspAntPapel").focusout(function () {
        BuscarEmissorDocumentoAnteriorPapel();
    });
});

function BuscarEmissorDocumentoAnteriorPapel() {
    if ($("#hddEmissorDocTranspAntPapel").val() != $("#txtCPFCNPJEmissorDocTranspAntPapel").val()) {
        var cpfCnpj = $("#txtCPFCNPJEmissorDocTranspAntPapel").val().replace(/[^0-9]/g, '');
        if (cpfCnpj != "") {
            if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto != null) {
                            $("#hddEmissorDocTranspAntPapel").val(r.Objeto.CPF_CNPJ);
                            $("#txtCPFCNPJEmissorDocTranspAntPapel").val(r.Objeto.CPF_CNPJ);
                            $("#txtEmissorDocTranspAntPapel").val(r.Objeto.Nome);
                        } else {
                            LimparCamposEmissorDocumentoAnteriorPapel();
                            jAlert("Emissor não encontrado.", "Atenção");
                        }
                    } else {
                        LimparCamposEmissorDocumentoAnteriorPapel();
                        jAlert(r.Erro, "Erro");
                    }
                });
            } else {
                LimparCamposEmissorDocumentoAnteriorPapel();
                jAlert("O CPF/CNPJ digitado é inválido!", "Atenção");
            }
        } else {
            LimparCamposEmissorDocumentoAnteriorPapel();
        }
    }
}

function LimparCamposEmissorDocumentoAnteriorPapel() {
    $("#hddEmissorDocTranspAntPapel").val("");
    $("#txtCPFCNPJEmissorDocTranspAntPapel").val("");
    $("#txtEmissorDocTranspAntPapel").val("");
}

function RetornoConsultaClienteDocTranspAntPapel(cliente) {
    $("#txtEmissorDocTranspAntPapel").val(cliente.Nome);
    $("#txtCPFCNPJEmissorDocTranspAntPapel").val(cliente.CPFCNPJ);
    $("#hddEmissorDocTranspAntPapel").val(cliente.CPFCNPJ);
}

function LimparCamposDocTranspAntPapel() {
    $("#hddDocTranspAntPapelEmEdicao").val("0");
    $("#selTipoDocTranspAntPapel").val("11");
    $("#txtEmissorDocTranspAntPapel").val("");
    $("#txtCPFCNPJEmissorDocTranspAntPapel").val("");
    $("#hddEmissorDocTranspAntPapel").val("");
    $("#txtNumeroDocTranspAntPapel").val("");
    $("#txtSerieDocTranspAntPapel").val("");
    $("#txtDataEmissaoDocTranspAntPapel").val("");
    $("#btnExcluirDocTranspAntPapel").hide();
}

function ValidarCamposDocTranspAntPapel() {
    var emissor = $("#hddEmissorDocTranspAntPapel").val();
    var numero = $("#txtNumeroDocTranspAntPapel").val();
    var serie = $("#txtSerieDocTranspAntPapel").val();
    var data = $("#txtDataEmissaoDocTranspAntPapel").val();
    var valido = true;
    if (emissor != "") {
        CampoSemErro("#txtEmissorDocTranspAntPapel");
    } else {
        CampoComErro("#txtEmissorDocTranspAntPapel");
        valido = false;
    }
    if (numero != "") {
        CampoSemErro("#txtNumeroDocTranspAntPapel");
    } else {
        CampoComErro("#txtNumeroDocTranspAntPapel");
        valido = false;
    }
    if (serie != "") {
        CampoSemErro("#txtSerieDocTranspAntPapel");
    } else {
        CampoComErro("#txtSerieDocTranspAntPapel");
        valido = false;
    }
    if (data != "") {
        CampoSemErro("#txtDataEmissaoDocTranspAntPapel");
    } else {
        CampoComErro("#txtDataEmissaoDocTranspAntPapel");
        valido = false;
    }
    return valido;
}

function SalvarDocTranspAntPapel() {
    if (ValidarCamposDocTranspAntPapel()) {
        var documentoAnterior = {
            Codigo: Globalize.parseInt($("#hddDocTranspAntPapelEmEdicao").val()),
            Emissor: $("#hddEmissorDocTranspAntPapel").val(),
            NomeEmissor: $("#txtEmissorDocTranspAntPapel").val(),
            Numero: $("#txtNumeroDocTranspAntPapel").val(),
            Serie: $("#txtSerieDocTranspAntPapel").val(),
            DataEmissao: $("#txtDataEmissaoDocTranspAntPapel").val(),
            Tipo: $("#selTipoDocTranspAntPapel").val(),
            DescricaoTipo: $("#selTipoDocTranspAntPapel :selected").text(),
            Excluir: false
        };
        var documentos = $("#hddDocsTranspAntPapel").val() == "" ? new Array() : JSON.parse($("#hddDocsTranspAntPapel").val());
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
        $("#hddDocsTranspAntPapel").val(JSON.stringify(documentos));
        RenderizarDocTranspAntPapel();
        LimparCamposDocTranspAntPapel();
    }
}

function EditarDocTranspAntPapel(documento) {
    $("#hddDocTranspAntPapelEmEdicao").val(documento.Codigo);
    $("#selTipoDocTranspAntPapel").val(documento.Tipo);
    $("#hddEmissorDocTranspAntPapel").val(documento.Emissor);
    $("#txtEmissorDocTranspAntPapel").val(documento.NomeEmissor);
    $("#txtCPFCNPJEmissorDocTranspAntPapel").val(documento.Emissor);
    $("#txtNumeroDocTranspAntPapel").val(documento.Numero);
    $("#txtSerieDocTranspAntPapel").val(documento.Serie);
    $("#txtDataEmissaoDocTranspAntPapel").val(documento.DataEmissao);
    $("#btnExcluirDocTranspAntPapel").show();
}

function ExcluirDocTranspAntPapel() {
    jConfirm("Deseja realmente excluir este documento de transporte anterior?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddDocTranspAntPapelEmEdicao").val());
            var documentos = $("#hddDocsTranspAntPapel").val() == "" ? new Array() : JSON.parse($("#hddDocsTranspAntPapel").val());
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
            $("#hddDocsTranspAntPapel").val(JSON.stringify(documentos));
            RenderizarDocTranspAntPapel();
            LimparCamposDocTranspAntPapel();
        }
    });
}

function RenderizarDocTranspAntPapel() {
    $("#tblDocsTranspAntPapel tbody").html("");
    var documentos = $("#hddDocsTranspAntPapel").val() == "" ? new Array() : JSON.parse($("#hddDocsTranspAntPapel").val());
    for (var i = 0; i < documentos.length; i++) {
        if (!documentos[i].Excluir) {
            $("#tblDocsTranspAntPapel tbody").append("<tr><td>" + documentos[i].Emissor + "</td><td>" + documentos[i].DescricaoTipo + "</td><td>" + documentos[i].Numero + "</td><td>" + documentos[i].Serie + "</td><td>" + documentos[i].DataEmissao + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarDocTranspAntPapel(" + JSON.stringify(documentos[i]) + ")'>Editar</button></td></tr>");
        }
    }
    if ($("#tblDocsTranspAntPapel tbody").html() == "") {
        $("#tblDocsTranspAntPapel tbody").html("<tr><td colspan='6'>Nenhum registro encontrado!</td></tr>");
    }
}