$(document).ready(function () {
    $("#txtDataCompra").mask("99/99/9999");
    $("#txtDataCompra").datepicker();
    $("#txtDataLicenca").mask("99/99/9999");
    $("#txtDataLicenca").datepicker();
    $("#txtValorAquisicao").priceFormat({ prefix: '' });
    $("#txtCapacidadeTanque").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
    $("#txtAnoFabricacao").mask("9999");
    $("#txtAnoModelo").mask("9999");
    $("#txtMediaPadrao").priceFormat({ prefix: '' });
    $("#txtDataVencimentoGarantiaPlena").mask("99/99/9999");
    $("#txtDataVencimentoGarantiaPlena").datepicker();
    $("#txtDataVencimentoGarantiaEscalonada").mask("99/99/9999");
    $("#txtDataVencimentoGarantiaEscalonada").datepicker();
    $("#txtTipoVeiculo").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoTipoVeiculo").val("0");
            } else {
                e.preventDefault();
            }
        }
    });
    $("#txtMarcaVeiculo").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoMarcaVeiculo").val("0");
            } else {
                e.preventDefault();
            }
        }
    });
    $("#txtModeloVeiculo").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoModeloVeiculo").val("0");
            } else {
                e.preventDefault();
            }
        }
    });
    CarregarConsultaDeTiposDeVeiculos("btnBuscarTipoVeiculo", "btnBuscarTipoVeiculo", "A", RetornoConsultaTipoVeiculo, true, false);
    CarregarConsultaDeMarcasDeVeiculos("btnBuscarMarcaVeiculo", "btnBuscarMarcaVeiculo", "A", RetornoConsultaMarcaVeiculo, true, false);
    CarregarConsultaDeModelosDeVeiculos("btnBuscarModeloVeiculo", "btnBuscarModeloVeiculo", "A", RetornoConsultaModeloVeiculo, true, false);
    CarregarConsultaModeloVeicularCarga("btnBuscarModeloVeicularCarga", "btnBuscarModeloVeicularCarga", RetornoConsultaModeloVeicularCarga, true, false);
});
function RetornoConsultaTipoVeiculo(tipo) {
    $("#txtTipoVeiculo").val(tipo.Descricao);
    $("#hddCodigoTipoVeiculo").val(tipo.Codigo);
}
function RetornoConsultaMarcaVeiculo(marca) {
    $("#txtMarcaVeiculo").val(marca.Descricao);
    $("#hddCodigoMarcaVeiculo").val(marca.Codigo);
}
function RetornoConsultaModeloVeiculo(modelo) {
    $("#txtModeloVeiculo").val(modelo.Descricao);
    $("#hddCodigoModeloVeiculo").val(modelo.Codigo);
}
function RetornoConsultaModeloVeicularCarga(modelo) {
    $("#txtModeloVeicularCarga").val(modelo.Descricao);
    $("body").data("codigoModeloVeicularCarga", modelo.Codigo);
}