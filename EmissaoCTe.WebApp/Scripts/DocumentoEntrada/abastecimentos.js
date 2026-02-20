var CodigoAbastecimento = 0;
var KMMaior = 0;

$(document).ready(function () {
    $("#txtKMInicialAbastecimento").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
    $("#txtKMFinalAbastecimento").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
    $("#txtLitrosAbastecimento").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtValorUnitarioAbastecimento").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtDataAbastecimento").mask("99/99/9999");
    $("#txtDataAbastecimento").datepicker();
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

    $("#btnCopiarDadosItens").click(function () {
        CopiarDadosItens();
    });
    SetarDadosPadraoAbastecimento();

    LimparCamposAbastecimento();
    $("body").data("abastecimentos", []);
});
function SetarDadosPadraoAbastecimento() {
    $("#txtDataAbastecimento").val(Globalize.format(new Date(), "dd/MM/yyyy"));
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

function LimparAbastecimentos() {
    KMMaior = 0;
    $("body").data("abastecimentos", []);
    RenderizarAbastecimento();
    LimparCamposAbastecimento();
}

function LimparCamposAbastecimento() {
    CodigoAbastecimento = 0;
    $("#txtDataAbastecimento").val("");
    $("#txtKMInicialAbastecimento").val(Globalize.format(KMMaior, "n0"));
    $("#txtKMFinalAbastecimento").val("0");
    $("#txtLitrosAbastecimento").val("0,0000");
    $("#txtValorUnitarioAbastecimento").val("0,0000");
    $("#txtValorTotalAbastecimento").val("0,00");
    $("#txtMediaAbastecimento").val("0,00");
    $("#btnExcluirAbastecimento").hide();
    $("#btnCancelarAbastecimento").hide();
    $("#chkAbastecimentoPago").prop("checked", false);
    $("#btnCopiarDadosItens").prop('disabled', true);
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
            Codigo: CodigoAbastecimento,
            KMInicial: Globalize.parseInt($("#txtKMInicialAbastecimento").val()),
            KMFinal: Globalize.parseInt($("#txtKMFinalAbastecimento").val()),
            Litros: $("#txtLitrosAbastecimento").val(),
            ValorUnitario: $("#txtValorUnitarioAbastecimento").val(),
            Media: $("#txtMediaAbastecimento").val(),
            Data: $("#txtDataAbastecimento").val(),
            Pago: $("#chkAbastecimentoPago")[0].checked,
            Excluir: false
        };

        var abastecimentos = $("body").data("abastecimentos");

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

        $("body").data("abastecimentos", abastecimentos);

        RenderizarAbastecimento();
        LimparCamposAbastecimento();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!", "mensagensAbastecimentos-placeholder");
    }
}
function EditarAbastecimento(abastecimento) {
    LimparCamposAbastecimento();
    $("#txtDataAbastecimento").val('');

    CodigoAbastecimento = abastecimento.Codigo;

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
            var abastecimentos = $("body").data("abastecimentos");
            for (var i = 0; i < abastecimentos.length; i++) {
                if (abastecimentos[i].Codigo == CodigoAbastecimento) {
                    if (CodigoAbastecimento > 0) {
                        abastecimentos[i].Excluir = true;
                    } else {
                        abastecimentos.splice(i, 1);
                    }
                    break;
                }
            }
            $("body").data("abastecimentos", abastecimentos);
            RenderizarAbastecimento();
            LimparCamposAbastecimento();
            AtualizarValores();
        }
    });
}

function CopiarDadosAbastecimentos(possuiItemComLitros){
    if (possuiItemComLitros)
        $("#btnCopiarDadosItens").prop('disabled', false);
    else
        $("#btnCopiarDadosItens").prop('disabled', true);
}

function CopiarDadosItens() {
    var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");
    for (var i = 0; i < itens.length; i++) {
        if (/litro/i.test(itens[i].DescricaoUnidadeMedida)) {
            $("#txtLitrosAbastecimento").val(Globalize.format(itens[i].Quantidade, "n4"));
            $("#txtValorUnitarioAbastecimento").val(Globalize.format(itens[i].ValorUnitario, "n4"));
            break;
        }
    }

    CalcularMedia();
    CalcularValorTotalAbastecimento();
}

function RenderizarAbastecimento() {
    var itens = $("body").data("abastecimentos");
    var $tabela = $("#tblAbastecimentos");

    var kmMaior = 0;

    $tabela.find("tbody").html("");

    itens.sort(function (a, b) { return a.KMInicial < b.KMInicial ? -1 : 1; });
    itens.forEach(function (info) {
        if (!info.Excluir) {
            if (kmMaior < info.KMFinal)
                kmMaior = info.KMFinal;

            var $row = $("<tr>" +
                "<td>" + info.Data + "</td>" +
                "<td>" + Globalize.format(info.KMInicial, "n0") + "</td>" +
                "<td>" + Globalize.format(info.KMFinal, "n0")+ "</td>" +
                "<td>" + info.Litros + "</td>" +
                "<td>" + info.ValorUnitario + "</td>" +
                "<td>" + (info.Pago ? "Sim" : "Não") + "</td>" +
                "<td>" +
                    "<button type='button' class='btn btn-default btn-xs editar'>Editar</button> " +
                "</td>" +
            "</tr>");

            $row.on("click", ".editar", function () {
                EditarAbastecimento(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if (kmMaior > 0)
        KMMaior = kmMaior;

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}