var idTextArea, idTextArea2, idTextArea3;
$(document).ready(function () {
    $(".taggedInput").focus(function () {
        idTextArea = this.id;
    });
    $(".taggedInput2").focus(function () {
        idTextArea2 = this.id;
    });
    $(".taggedInput3").focus(function () {
        idTextArea3 = this.id;
    });
    $("#lnkNumeroCTe").click(function () {
        InserirTag(idTextArea, "#NumeroCTe#");
    });
    $("#lnkChaveCTe").click(function () {
        InserirTag(idTextArea, "#ChaveCTe#");
    });
    $("#lnkDataEmissaoCTe").click(function () {
        InserirTag(idTextArea, "#DataEmissaoCTe#");
    });
    $("#lnkCPFCNPJProprietarioVeiculo").click(function () {
        InserirTag(idTextArea2, "#CPFCNPJProprietario#")
    });
    $("#lnkNomeProprietarioVeiculo").click(function () {
        InserirTag(idTextArea2, "#NomeProprietario#")
    });
    $("#lnkRNTRCProprietario").click(function () {
        InserirTag(idTextArea2, "#RNTRCProprietario#")
    });
    $("#lnkPlaca").click(function () {
        InserirTag(idTextArea2, "#PlacaVeiculo#")
    });
    $("#lnkRENAVAMVeiculo").click(function () {
        InserirTag(idTextArea2, "#RENAVAMVeiculo#")
    });
    $("#lnkUFVeiculo").click(function () {
        InserirTag(idTextArea2, "#UFVeiculo#")
    });
    $("#lnkMarcaVeiculo").click(function () {
        InserirTag(idTextArea2, "#MarcaVeiculo#")
    });
    $("#lnkPlacasVinculadas").click(function () {
        InserirTag(idTextArea2, "#PlacasVinculadas#")
    });
    $("#lnkNomeMotorista").click(function () {
        InserirTag(idTextArea2, "#NomeMotorista#");
    });
    $("#lnkCPFMotorista").click(function () {
        InserirTag(idTextArea2, "#CPFMotorista#");
    });
    $("#lnkPlacasRenavamVinculadas").click(function () {
        InserirTag(idTextArea2, "#PlacasRenavamVinculadas#");
    });
    $("#lnkQuantidadeNotas").click(function () {
        InserirTag(idTextArea2, "#QuantidadeNotas#");
    });

    $("#lnkNomePDFCNPJTransportador").click(function () {
        InserirTag(idTextArea3, "#CNPJTransportador#");
    });
    $("#lnkNomePDFNomeTransportador").click(function () {
        InserirTag(idTextArea3, "#NomeTransportador#");
    });
    $("#lnkNomePDFCTeNumeroCTe").click(function () {
        InserirTag(idTextArea3, "#NumeroCTe#");
    });
    $("#lnkNomePDFCTeSerieCTe").click(function () {
        InserirTag(idTextArea3, "#SerieCTe#");
    });
    $("#lnkNomePDFChaveCTe").click(function () {
        InserirTag(idTextArea3, "#ChaveCTe#");
    });
    $("#lnkNomePDFPlaca").click(function () {
        InserirTag(idTextArea3, "#PlacaVeiculo#");
    });
    $("#lnkNomePDFMotorista").click(function () {
        InserirTag(idTextArea3, "#NomeMotorista#");
    });
    $("#lnkNomePDFClienteRemetente").click(function () {
        InserirTag(idTextArea3, "#ClienteRemetente#");
    });
    $("#lnkNomePDFClienteDestinatario").click(function () {
        InserirTag(idTextArea3, "#ClienteDestinatario#");
    });
    $("#lnkNomePDFCidadeOrigem").click(function () {
        InserirTag(idTextArea3, "#CidadeOrigem#");
    });
    $("#lnkNomePDFUFOrigem").click(function () {
        InserirTag(idTextArea3, "#UFOrigem#");
    });
    $("#lnkNomePDFCidadeDestino").click(function () {
        InserirTag(idTextArea3, "#CidadeDestino#");
    });
    $("#lnkNomePDFUFDestino").click(function () {
        InserirTag(idTextArea3, "#UFDestino#");
    });

    $("#lnkNomeTamanhoTag").click(function () {
        if ($("#txtTamanhoTag").val() != "")
            InserirTag(idTextArea3, "#TamanhoTag#" + $("#txtTamanhoTag").val() + "#");
        else
            InserirTag(idTextArea3, "#TamanhoTag#5#");
    });


});
function InserirTag(id, text) {
    if (id != null && id.trim() != "") {
        var txtarea = document.getElementById(id);
        var scrollPos = txtarea.scrollTop;
        var strPos = 0;
        var br = ((txtarea.selectionStart || txtarea.selectionStart == '0') ? "ff" : (document.selection ? "ie" : false));
        if (br == "ie") {
            txtarea.focus();
            var range = document.selection.createRange();
            range.moveStart('character', -txtarea.value.length);
            strPos = range.text.length;
        } else if (br == "ff") {
            strPos = txtarea.selectionStart;
        }
        var front = (txtarea.value).substring(0, strPos);
        var back = (txtarea.value).substring(strPos, txtarea.value.length);
        txtarea.value = front + text + back;
        strPos = strPos + text.length;
        if (br == "ie") {
            txtarea.focus();
            var range = document.selection.createRange();
            range.moveStart('character', -txtarea.value.length);
            range.moveStart('character', strPos);
            range.moveEnd('character', 0);
            range.select();
        } else if (br == "ff") {
            txtarea.selectionStart = strPos;
            txtarea.selectionEnd = strPos;
            txtarea.focus();
        }
        txtarea.scrollTop = scrollPos;
    }
}