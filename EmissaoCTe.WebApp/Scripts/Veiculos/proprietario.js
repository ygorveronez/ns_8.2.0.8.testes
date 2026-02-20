$(document).ready(function () {
    $("#txtRNTRCProprietarioVeiculo").mask("99999999");
    $(".portlet-header").hover(function () {
        $(this).addClass("ui-portlet-hover");
    },
    function () {
        $(this).removeClass("ui-portlet-hover");
    });
    $(".portlet-header .ui-icon").click(function () {
        $(this).toggleClass("ui-icon-circle-arrow-n ui-icon-circle-arrow-s");
        $(this).parents(".portlet:first").find(".portlet-content").toggle();
    });
    $("#lnkPlacas").click(function () {
        InserirTag("txtObservacaoProprietario", "#PlacaVeiculo#");
    });
    $("#lnkRenavans").click(function () {
        InserirTag("txtObservacaoProprietario", "#RENAVAMVeiculo#");
    });
    $("#lnkUFVeiculo").click(function () {
        InserirTag("txtObservacaoProprietario", "#UFVeiculo#");
    });
    $("#lnkMarcaVeiculo").click(function () {
        InserirTag("txtObservacaoProprietario", "#MarcaVeiculo#");
    });
    $("#lnkNomeProprietario").click(function () {
        InserirTag("txtObservacaoProprietario", "#NomeProprietarioVeiculo#");
    });
    $("#lnkCPFCNPJProprietario").click(function () {
        InserirTag("txtObservacaoProprietario", "#CPFCNPJProprietarioVeiculo#");
    });
    $("#lnkRNTRCProprietario").click(function () {
        InserirTag("txtObservacaoProprietario", "#RNTRCProprietario#");
    });
    $("#lnkPlacasVinculadas").click(function () {
        InserirTag("txtObservacaoProprietario", "#PlacasVinculadas#");
    });
    $("#selTipoPropriedade").change(function () {
        AlterarTipoPropriedade($(this).val());
    });
    $("#txtProprietarioVeiculo").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoProprietario").val("");
            } else {
                e.preventDefault();
            }
        }
    });
    CarregarConsultadeClientes("btnBuscarProprietarioVeiculo", "btnBuscarProprietarioVeiculo", RetornoConsultaProprietario, true, false);
});
function RetornoConsultaProprietario(proprietario) {
    $("#txtProprietarioVeiculo").val(proprietario.CPFCNPJ + " - " + proprietario.Nome);
    $("#hddCodigoProprietario").val(proprietario.CPFCNPJ);
}
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
function AlterarTipoPropriedade(tipo) {
    if (tipo == "T") {
        $("#divDadosProprietarioVeiculo").show();
    } else {
        LimparCamposDadosProprietarioVeiculo();
        $("#divDadosProprietarioVeiculo").hide();
    }
}

function LimparCamposDadosProprietarioVeiculo() {
    $("#txtProprietarioVeiculo").val('');
    $("#txtTAF").val('');
    $("#txtNroRegEstadual").val('');
    $("#txtRNTRCProprietarioVeiculo").val('');
    $("#selTipoProprietarioVeiculo").val(0);
    $("#txtCIOTProprietarioVeiculo").val('')
    $("#txtObservacaoProprietario").val('');
    $("#txtProprietarioVeiculo").val('');

}