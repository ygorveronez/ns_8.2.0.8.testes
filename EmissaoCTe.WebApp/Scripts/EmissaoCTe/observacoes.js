$(document).ready(function () {
    $("#btnCopiarObservacaoGeral").click(function () {
        CopiarObservacaoGeral();
        jAlert("Observação copiada com sucesso.", "Sucesso");
    });
});

function CopiarObservacaoGeral() {
    $("#txtInformacaoAdicionalFisco").val($("#txtObservacaoGeral").val());
}