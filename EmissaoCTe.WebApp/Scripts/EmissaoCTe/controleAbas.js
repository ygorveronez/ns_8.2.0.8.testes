$(document).ready(function () {

    $('#divEmissaoCTe').on('shown.bs.modal', function () {
        var listaTabs = $('#tabsEmissaoCTe a[data-toggle="tab"]');

        $("#btnTelaEmissaoAnterior").prop("disabled", true);
        $("#btnTelaEmissaoProximo").prop("disabled", false);

        $("#btnTelaEmissaoProximo").on('click', function () {
            $('#tabsEmissaoCTe a[href="' + listaTabs[1].hash + '"]').tab("show");
        });

        if ($("#ddlModelo").val() == "67")
            $("#divEmissaoCTe .modal-title").text("Emissão de CT-e OS (" + listaTabs[0].innerText + ")");
        else
            $("#divEmissaoCTe .modal-title").text("Emissão de CT-e (" + listaTabs[0].innerText + ")");

        $('#tabsEmissaoCTe a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            $("#btnTelaEmissaoAnterior").off();
            $("#btnTelaEmissaoProximo").off();

            listaTabs = $('#tabsEmissaoCTe a[data-toggle="tab"]');

            var indice = 0;
            for (var i = 0; i < listaTabs.length; i++) {
                if (listaTabs[i].hash == e.target.hash) {
                    if ($("#ddlModelo").val() == "67")
                        $("#divEmissaoCTe .modal-title").text("Emissão de CT-e OS (" + listaTabs[i].innerText + ")");
                        else
                           $("#divEmissaoCTe .modal-title").text("Emissão de CT-e (" + listaTabs[i].innerText + ")");
                    indice = i;
                    break;
                }
            }

            var proximaTela = null;
            for (var i = indice + 1; i < listaTabs.length; i++)
                if (listaTabs[i].style.display != 'none') {
                    proximaTela = listaTabs[i];
                    break;
                }

            var telaAnterior = null;
            for (var i = indice - 1; i >= 0; i--)
                if (listaTabs[i].style.display != 'none') {
                    telaAnterior = listaTabs[i];
                    break;
                }

            if (telaAnterior) {
                $("#btnTelaEmissaoAnterior").prop('disabled', false);
                $("#btnTelaEmissaoAnterior").on('click', function () {
                    $("#tabsEmissaoCTe a[href='" + telaAnterior.hash + "']").tab("show");
                });
            } else {
                $("#btnTelaEmissaoAnterior").prop('disabled', true);
            }

            if (proximaTela) {
                $("#btnTelaEmissaoProximo").prop('disabled', false);
                $("#btnTelaEmissaoProximo").on('click', function () {
                    $("#tabsEmissaoCTe a[href='" + proximaTela.hash + "']").tab("show");
                });
            } else {
                $("#btnTelaEmissaoProximo").prop('disabled', true);
            }
        });

    });

    $('#divEmissaoCTe').on('hidden.bs.modal', function () {
        //$('a[data-toggle="tab"]').off('shown.bs.tab');
        //$("#btnTelaEmissaoAnterior").off();
        //$("#btnTelaEmissaoProximo").off();
    });

});