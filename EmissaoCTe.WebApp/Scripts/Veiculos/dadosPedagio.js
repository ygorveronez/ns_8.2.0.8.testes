$(document).ready(function () {
    $("#txtValorValePedagio").priceFormat({ prefix: '' });
    //$("#txtDataInicioVigenciaTagValePedagio").mask("99/99/9999");
    //$("#txtDataInicioVigenciaTagValePedagio").datepicker();
    //$("#txtDataFimVigenciaTagValePedagio").mask("99/99/9999");
    //$("#txtDataFimVigenciaTagValePedagio").datepicker();

    $("#txtFornecedorValePedagio").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoFornecedorValePedagio").val("");
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtResponsavelValePedagio").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoResponsavelValePedagio").val("");
            } else {
                e.preventDefault();
            }
        }
    });

    CarregarConsultadeClientes("btnBuscarFornecedorValePedagio", "btnBuscarFornecedorValePedagio", RetornoConsultaFornecedorValePedagio, true, false);
    CarregarConsultadeClientes("btnBuscarResponsavelValePedagio", "btnBuscarResponsavelValePedagio", RetornoConsultaResponsavelValePedagio, true, false);
});

function RetornoConsultaFornecedorValePedagio(fornecedorValePedagio) {
    $("#txtFornecedorValePedagio").val(fornecedorValePedagio.CPFCNPJ + " - " + fornecedorValePedagio.Nome);
    $("#hddCodigoFornecedorValePedagio").val(fornecedorValePedagio.CPFCNPJ);
}

function RetornoConsultaResponsavelValePedagio(responsavelValePedagio) {
    $("#txtResponsavelValePedagio").val(responsavelValePedagio.CPFCNPJ + " - " + responsavelValePedagio.Nome);
    $("#hddCodigoResponsavelValePedagio").val(responsavelValePedagio.CPFCNPJ);
}