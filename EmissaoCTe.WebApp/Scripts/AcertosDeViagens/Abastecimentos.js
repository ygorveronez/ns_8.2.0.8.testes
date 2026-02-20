$(document).ready(function () {
    $("#txtKMInicialAbastecimento").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
    $("#txtKMFinalAbastecimento").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
    $("#txtLitrosAbastecimento").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtValorUnitarioAbastecimento").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtDataAbastecimento").mask("99/99/9999");
    $("#txtDataAbastecimento").datepicker();
    $("#txtPostoAbastecimento").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoPosto").val("");
                $("#hddDescricaoPosto").val("");
                $("#txtDescricaoPostoNaoCadastrado").attr("disabled", false);
            } else {
                e.preventDefault();
            }
        }
    });
    $("#txtDescricaoPostoNaoCadastrado").blur(function () {
        if ($(this).val().trim() != "") {
            $("#btnBuscarPostoAbastecimento").attr("disabled", true);
        } else {
            $("#btnBuscarPostoAbastecimento").attr("disabled", false);
        }
    });
    $("#txtKMInicialAbastecimento").blur(function () {
        CalcularMedia();
    });
    $("#txtKMFinalAbastecimento").blur(function () {
        CalcularMedia();
    });
    $("#txtLitrosAbastecimento").blur(function () {
        CalcularMedia();
        CalcularValorTotalAbastecimento();
    });
    $("#txtValorUnitarioAbastecimento").blur(function () {
        CalcularValorTotalAbastecimento();
    });
    $("#btnSalvarAbastecimento").click(function () {
        SalvarAbastecimento();
    });
    $("#btnExcluirAbastecimento").click(function () {
        ExcluirAbastecimento();
    });
    $("#btnCancelarAbastecimento").click(function () {
        LimparCamposAbastecimento();
    });
    CarregarConsultadeClientes("btnBuscarPostoAbastecimento", "btnBuscarPostoAbastecimento", RetornoConsultaPosto, true, false);
    SetarDadosPadraoAbastecimento();
});
function SetarDadosPadraoAbastecimento() {
    $("#txtDataAbastecimento").val(Globalize.format(new Date(), "dd/MM/yyyy"));
}
function RetornoConsultaPosto(posto) {
    $("#hddCodigoPosto").val(posto.CPFCNPJ);
    $("#txtPostoAbastecimento").val(posto.CPFCNPJ + " - " + posto.Nome);
    $("#hddDescricaoPosto").val(posto.CPFCNPJ + " - " + posto.Nome);
    $("#txtDescricaoPostoNaoCadastrado").attr("disabled", true);
}
function CalcularMedia() {
    var kmInicial = Globalize.parseInt($("#txtKMInicialAbastecimento").val());
    var kmFinal = Globalize.parseInt($("#txtKMFinalAbastecimento").val());
    var litros = Globalize.parseFloat($("#txtLitrosAbastecimento").val());
    var media = (kmFinal - kmInicial) / litros;
    if (isNaN(media) || !isFinite(media))
        media = 0;
    $("#txtMediaAbastecimento").val(Globalize.format(media, "n2"));
}
function CalcularValorTotalAbastecimento() {
    var valor = Globalize.parseFloat($("#txtValorUnitarioAbastecimento").val());
    var litros = Globalize.parseFloat($("#txtLitrosAbastecimento").val());
    if (!isNaN(valor) && !isNaN(litros))
        $("#txtValorTotalAbastecimento").val(Globalize.format(valor * litros, "n2"));
}

function LimparCamposAbastecimento() {
    $("#hddCodigoAbastecimento").val("0");
    $("#hddDescricaoPosto").val("");
    $("#hddCodigoPosto").val("");
    $("#txtPostoAbastecimento").val("");
    $("#txtDataAbastecimento").val("");
    $("#txtKMInicialAbastecimento").val($("#hddKmVeiculo").val());
    $("#txtKMFinalAbastecimento").val("0");
    $("#txtLitrosAbastecimento").val("0,0000");
    $("#txtValorUnitarioAbastecimento").val("0,0000");
    $("#txtValorTotalAbastecimento").val("0,00");
    $("#txtMediaAbastecimento").val("0,00");
    $("#btnExcluirAbastecimento").hide();
    $("#btnCancelarAbastecimento").hide();
    $("#txtDescricaoPostoNaoCadastrado").val('');
    $("#txtDescricaoPostoNaoCadastrado").attr("disabled", false);
    $("#btnBuscarPostoAbastecimento").attr("disabled", false);
    $("#chkAbastecimentoPago").prop("checked", false);
    SetarDadosPadraoAbastecimento();
}
function ValidarCamposAbastecimento() {
    var kmInicial = Globalize.parseInt($("#txtKMInicialAbastecimento").val());
    var kmFinal = Globalize.parseInt($("#txtKMFinalAbastecimento").val());
    var valido = true;
    if ((kmFinal != 0 && kmInicial != 0) && kmFinal <= kmInicial) {
        CampoComErro("#txtKMInicialAbastecimento");
        CampoComErro("#txtKMFinalAbastecimento");
        valido = false;
    } else {
        CampoSemErro("#txtKMInicialAbastecimento");
        CampoSemErro("#txtKMFinalAbastecimento");
    }
    return valido;
}
function SalvarAbastecimento() {
    if (ValidarCamposAbastecimento()) {
        var abastecimento = {
            Codigo: Globalize.parseInt($("#hddCodigoAbastecimento").val()),
            DescricaoPosto: $("#hddCodigoPosto").val() != "" ? $("#hddDescricaoPosto").val() : $("#txtDescricaoPostoNaoCadastrado").val(),
            CodigoPosto: $("#hddCodigoPosto").val(),
            KMInicial: Globalize.parseInt($("#txtKMInicialAbastecimento").val()),
            KMFinal: Globalize.parseInt($("#txtKMFinalAbastecimento").val()),
            Litros: $("#txtLitrosAbastecimento").val(),
            ValorUnitario: $("#txtValorUnitarioAbastecimento").val(),
            Media: $("#txtMediaAbastecimento").val(),
            Data: $("#txtDataAbastecimento").val(),
            Pago: $("#chkAbastecimentoPago")[0].checked,
            Excluir: false
        };

        var abastecimentos = $("#hddAbastecimentos").val() == "" ? new Array() : JSON.parse($("#hddAbastecimentos").val());

        abastecimentos.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (abastecimento.Codigo == 0)
            abastecimento.Codigo = (abastecimentos.length > 0 ? (abastecimentos[0].Codigo > 0 ? -1 : (abastecimentos[0].Codigo - 1)) : -1);

        if (abastecimentos.length > 0) {
            for (var i = 0; i < abastecimentos.length; i++) {
                if (abastecimentos[i].Codigo == abastecimento.Codigo) {
                    abastecimentos.splice(i, 1);
                    break;
                }
            }
        }

        abastecimentos.push(abastecimento);

        $("#hddAbastecimentos").val(JSON.stringify(abastecimentos));

        RenderizarAbastecimento();
        LimparCamposAbastecimento();
        AtualizarValores();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!", "mensagensAbastecimentos-placeholder");
    }
}
function EditarAbastecimento(abastecimento) {
    LimparCamposAbastecimento();
    $("#txtDataAbastecimento").val('');

    $("#hddCodigoAbastecimento").val(abastecimento.Codigo);
    $("#hddCodigoPosto").val(abastecimento.CodigoPosto);

    if (abastecimento.CodigoPosto == "") {
        $("#txtDescricaoPostoNaoCadastrado").val(abastecimento.DescricaoPosto);

        if (abastecimento.DescricaoPosto)
            $("#btnBuscarPostoAbastecimento").attr("disabled", true);
    } else {
        $("#txtDescricaoPostoNaoCadastrado").attr("disabled", true);
        $("#txtPostoAbastecimento").val(abastecimento.DescricaoPosto);
        $("#hddDescricaoPosto").val(abastecimento.DescricaoPosto);
    }

    $("#txtDataAbastecimento").val(abastecimento.Data);
    $("#txtKMInicialAbastecimento").val(Globalize.format(abastecimento.KMInicial, "n0"));
    $("#txtKMFinalAbastecimento").val(Globalize.format(abastecimento.KMFinal, "n0"));
    $("#txtLitrosAbastecimento").val(abastecimento.Litros);
    $("#txtValorUnitarioAbastecimento").val(abastecimento.ValorUnitario);
    $("#txtMediaAbastecimento").val(abastecimento.Media);
    $("#chkAbastecimentoPago").prop("checked", abastecimento.Pago);
    $("#btnExcluirAbastecimento").show();
    $("#btnCancelarAbastecimento").show();
    CalcularValorTotalAbastecimento();
}
function ExcluirAbastecimento() {
    jConfirm("Deseja realmente excluir este abastecimento da viagem?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddCodigoAbastecimento").val());
            var abastecimentos = $("#hddAbastecimentos").val() == "" ? new Array() : JSON.parse($("#hddAbastecimentos").val());
            for (var i = 0; i < abastecimentos.length; i++) {
                if (abastecimentos[i].Codigo == codigo) {
                    if (codigo > 0) {
                        abastecimentos[i].Excluir = true;
                    } else {
                        abastecimentos.splice(i, 1);
                    }
                    break;
                }
            }
            $("#hddAbastecimentos").val(JSON.stringify(abastecimentos));
            RenderizarAbastecimento();
            LimparCamposAbastecimento();
            AtualizarValores();
        }
    });
}
function RenderizarAbastecimento() {
    $("#tblAbastecimentos tbody").html("");
    var kmMaior = 0;
    var abastecimentos = $("#hddAbastecimentos").val() == "" ? new Array() : JSON.parse($("#hddAbastecimentos").val());

    abastecimentos.sort(function (a, b) { return a.KMInicial < b.KMInicial ? -1 : 1; });

    for (var i = 0; i < abastecimentos.length; i++) {
        if (!abastecimentos[i].Excluir) {
            $("#tblAbastecimentos tbody").append("<tr><td>" + abastecimentos[i].DescricaoPosto + "</td><td>" + abastecimentos[i].Data + "</td><td>" + Globalize.format(abastecimentos[i].KMInicial, "n0") + "</td><td>" + Globalize.format(abastecimentos[i].KMFinal, "n0") + "</td><td>" + abastecimentos[i].Litros + "</td><td>" + abastecimentos[i].ValorUnitario + "</td><td>" + (abastecimentos[i].Pago ? "Sim" : "Não") + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarAbastecimento(" + JSON.stringify(abastecimentos[i]) + ")'>Editar</button></td></tr>");
            if (kmMaior < abastecimentos[i].KMFinal)
                kmMaior = abastecimentos[i].KMFinal;
        }
    }
    if ($("#tblAbastecimentos tbody").html() == "") {
        $("#tblAbastecimentos tbody").html("<tr><td colspan='8'>Nenhum registro encontrado!</td></tr>");
    }
    if (kmMaior > 0)
        $("#hddKmVeiculo").val(Globalize.format(kmMaior, "n0"));

}
function BuscarAbastecimentos(acertoDeViagem) {
    executarRest("/Abastecimento/BuscarPorAcertoDeViagem?callback=?", { CodigoAcertoViagem: acertoDeViagem.Codigo }, function (r) {
        if (r.Sucesso) {
            $("#hddAbastecimentos").val(JSON.stringify(r.Objeto));
            RenderizarAbastecimento();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "mensagensAbastecimentos-placeholder");
        }
    });
}
