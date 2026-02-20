$(document).ready(function () {
    $("#txtCNPJCPF").focus(function () {
        $("#txtCNPJCPF").trigger("unmask");
        setTimeout(function () {
            $("#txtCNPJCPF").val(PegaDocumentoSemFormato());
        }, 15);
    });
    $("#txtCNPJCPF").blur(function () {
        var docLength = PegaDocumentoSemFormato().length;

        if (docLength == 11)
            $("#txtCNPJCPF").mask("999.999.999-99");
        else if (docLength == 14)
            $("#txtCNPJCPF").mask("99.999.999/9999-99");
        else
            $("#txtCNPJCPF").trigger("unmask");
    });
    $("#txtCNPJCPF").keyup(function (e) {
        if (e.keyCode == 13 || e.which == 13) {
            $("#txtCNPJCPF").blur();
            SalvarDocumento();
            $("#txtCNPJCPF").focus();
        }
    });
    $("#btnAdicionarCNPJCPF").click(function () {
        SalvarDocumento();
    });            
});

function PegaDocumentoSemFormato() {
    return $("#txtCNPJCPF").val().replace(/[^0-9]/g, '');
}

function ValidarDocumento() {
    var doc = PegaDocumentoSemFormato();
    return (doc.length == 11 && ValidarCPF(doc)) || (doc.length == 14 && ValidarCNPJ(doc));
}

function SalvarDocumento() {
    if (!ValidarDocumento()) {
        var docLength = PegaDocumentoSemFormato().length;
        return jAlert((docLength == 11 ? "CPF" : "CNPJ") + " informado é inválido.", "Atenção");
    }
    if (!ValidarDocumentoDuplicado()) {
        var docLength = PegaDocumentoSemFormato().length;
        return jAlert((docLength == 11 ? "CPF" : "CNPJ") + " informado já foi cadastrado.", "Atenção");
    }
            
    var informacaoDocumento = {
        Id: Globalize.parseInt($("#hddIdDocumentosXML").val()),
        CnpjCpf: PegaDocumentoSemFormato(),
        Excluir: false
    };
    var infomacoesDocumentos = $("#hddInformacoesDocumento").val() == "" ? new Array() : JSON.parse($("#hddInformacoesDocumento").val());
    if (informacaoDocumento.Id == 0) {
        informacaoDocumento.Id = -(infomacoesDocumentos.length + 1);
    }
    for (var i = 0; i < infomacoesDocumentos.length; i++) {
        if (infomacoesDocumentos[i].Id == informacaoDocumento.Id) {
            infomacoesDocumentos.splice(i, 1);
            break;
        }
    }
    infomacoesDocumentos.push(informacaoDocumento);
    $("#hddInformacoesDocumento").val(JSON.stringify(infomacoesDocumentos));
    RenderizarCNPJCPFAutenticados();
    LimparDocumento();
}

function RenderizaRichEditor() {
    $("#txtDicasEmissaoCTe").ckeditor();
    // $("#txtDicasEmissaoCTe").attr("rows", 40);
}

function RenderizarCNPJCPFAutenticados() {
    var infomacoesDocumentos = $("#hddInformacoesDocumento").val() == "" ? new Array() : JSON.parse($("#hddInformacoesDocumento").val());
    var formataDocumento = function (documento) {
        var colocaMascara = function (mascara) {
            var str = "", j = 0, doc = documento.toString(); // Converte o documento (number) para string
            for (var i in mascara) {
                if (mascara[i] == "#") {
                    str += doc[j];
                    j++;
                } else {
                    str += mascara[i];
                }
            }

            return str;
        };
        if (documento.length == 11)
            return colocaMascara("###.###.###-##");
        else if (documento.length == 14)
            return colocaMascara("##.###.###/####-##");
    }

    $("#tblDocumentosDownload tbody").html("");
    for (var i = 0; i < infomacoesDocumentos.length; i++) {
        var dadosDocumento = infomacoesDocumentos[i];
        if (!dadosDocumento.Excluir) {
            $("#tblDocumentosDownload tbody").append("<tr><td>" + formataDocumento(dadosDocumento.CnpjCpf) + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirDocumento(" + dadosDocumento.Id + ")'>Excluir</button></td></tr>");
        }
    }
    if ($("#tblDocumentosDownload tbody").html() == "")
        $("#tblDocumentosDownload tbody").html("<tr><td colspan='2'>Nenhum registro encontrado.</td></tr>");
}

function ValidarDocumentoDuplicado() {
    var infomacoesDocumentos = $("#hddInformacoesDocumento").val() == "" ? new Array() : JSON.parse($("#hddInformacoesDocumento").val());
    var documento = PegaDocumentoSemFormato();

    for (var i = 0; i < infomacoesDocumentos.length; i++)
        if (infomacoesDocumentos[i].CnpjCpf == documento && infomacoesDocumentos[i].Excluir == false)
            return false;
            
    return true;
}

function ExcluirDocumento(id) {
    var infomacoesDocumentos = $("#hddInformacoesDocumento").val() == "" ? new Array() : JSON.parse($("#hddInformacoesDocumento").val());
    for (var i = 0; i < infomacoesDocumentos.length; i++) {
        if (infomacoesDocumentos[i].Id == id) {
            if (id <= 0)
                infomacoesDocumentos.splice(i, 1);
            else
                infomacoesDocumentos[i].Excluir = true;
            break;
        }
    }
    $("#hddInformacoesDocumento").val(JSON.stringify(infomacoesDocumentos));
    RenderizarCNPJCPFAutenticados();
    LimparDocumento();
}

function LimparDocumento() {
    $("#hddIdDocumentosXML").val('0');
    $("#txtCNPJCPF").trigger("unmask").val("");
}