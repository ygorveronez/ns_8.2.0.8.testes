$(document).ready(function () {
    FormatarCampoDate("txtData");

    $("#txtFiltroNumeroInicial, #txtFiltroNumeroFinal").mask("9?99999999");
    $("#txtFiltroCNPJEmissor").mask("9?9999999999999");
    $("#txtFiltroCNPJRemetente").mask("9?9999999999999");
    $("#txtFiltroCNPJTomador").mask("9?9999999999999");

    $("#btnConsultarCTesDestinados").click(function () {
        ConsultarCTesDestinados();
    });

    LimparCampos();
});

function LimparCampos() {
    CodigoArquivo = 0;
    $("#txtData").val(moment(new Date).format("DD/MM/YYYY"));
    $("#selStatus").val($("#selStatus option:first").val());
}

function ConsultarCTesDestinados() {
    executarRest("/DestinadosCTes/ConsultarCTesDestinados", null, function (r) {
        if (r.Sucesso) {
            AtualizarGridDocumentos();
        } else {
            jAlert(r.Erro, "Atenção");
            AtualizarGridDocumentos();
        }
    });
}


