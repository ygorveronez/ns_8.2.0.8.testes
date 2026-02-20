$(function () {

    $("#txtDataEntrada").mask("99/99/9999");
    $("#txtDataEntrada").datepicker();

    $("#txtCalibragemEntradaPneu").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
    $("#txtKMEntrada").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });

    $("#txtStatusEntradaPneu").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("codigoStatusEntradaPneu", null);
            }
        }
        e.preventDefault();
    });

    $("#btnSalvarEntradaPneu").click(function () {
        SalvarEntradaPneu();
    });

    $("#btnCancelarEntradaPneu").click(function () {
        CancelarEntradaPneu();
    });

    $("#btnFecharEntradaPneu").click(function () {
        CancelarEntradaPneu();
    });

    LimparCamposEntradaPneu();
});

function CancelarEntradaPneu() {

    if ($("body").data("entradaDrop") == "divContainerEstoquePneu")
        VincularPneuAoEixo();
    else
        AtualizarEstoquePneus(0, false);

    FecharTelaEntradaPneu();
    LimparCamposEntradaPneu();
}

function RetornoConsultaStatusEntradaPneu(status) {
    $("#txtStatusEntradaPneu").val(status.Descricao);
    $("body").data("codigoStatusEntradaPneu", status.Codigo);
}

function AbrirTelaEntradaPneu() {

    $("#btnBuscarStatusEntradaPneu").off();

    if ($("body").data("entradaDrop") == "divContainerEstoquePneu") {
        BuscarUltimoStatusEntradaPneu("A");
        CarregarConsultaDeStatusDePneus("btnBuscarStatusEntradaPneu", "btnBuscarStatusEntradaPneu", "A", "A", RetornoConsultaStatusEntradaPneu, true, false);
    } else if ($("body").data("entradaDrop") == "divDescartePneu") {
        BuscarUltimoStatusEntradaPneu("F");
        CarregarConsultaDeStatusDePneus("btnBuscarStatusEntradaPneu", "btnBuscarStatusEntradaPneu", "A", "F", RetornoConsultaStatusEntradaPneu, true, false);
    } else if ($("body").data("entradaDrop") == "divContainerManutencaoPneu") {
        BuscarUltimoStatusEntradaPneu("S");
        CarregarConsultaDeStatusDePneus("btnBuscarStatusEntradaPneu", "btnBuscarStatusEntradaPneu", "A", "S", RetornoConsultaStatusEntradaPneu, true, false);
    }

    $("#divEntradaPneu").modal({ keyboard: false, backdrop: 'static' });
}

function FecharTelaEntradaPneu() {
    $("#divEntradaPneu").modal("hide");
}

function SalvarEntradaPneu() {
    if (ValidarCamposEntradaPneu()) {
        var dados = {
            Codigo: 0,
            CodigoPneu: $("body").data("pneuDrop").Codigo,
            CodigoVeiculo: $("body").data("codigoVeiculo") || 0,
            CodigoEixo: $("body").data("eixoDrop") != null ? $("body").data("eixoDrop").Codigo : 0,
            CodigoStatusPneu: $("body").data("codigoStatusEntradaPneu"),
            Tipo: "S",//$("body").data("entradaDrop") == "divContainerEstoquePneu" ? "S" : "R",
            Data: $("#txtDataEntrada").val(),
            KM: $("#txtKMEntrada").val(),
            Calibragem: $("#txtCalibragemEntradaPneu").val(),
            Observacao: $("#txtObservacaoEntradaPneu").val()
        };
        executarRest("/ManutencaoPneusVeiculo/Salvar?callback=?", dados, function (r) {
            if (r.Sucesso) {

                $("#pneu_" + $("body").data("pneuDrop").Codigo).remove();
                $("#eixo_" + $("body").data("eixoDrop").Codigo).text("Arraste um pneu para vincular ao eixo.");
                $("#eixo_" + $("body").data("eixoDrop").Codigo).droppable("option", "disabled", false);

                if ($("body").data("entradaDrop") == "divContainerEstoquePneu")
                    AtualizarEstoquePneus(0, false);
                else if ($("body").data("entradaDrop") == "divContainerManutencaoPneu")
                    AtualizarManutencaoPneus(0, false);

                FecharTelaEntradaPneu();
                LimparCamposEntradaPneu();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção", "messages-placeholder-entrada-pneu");
            }
        });
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!", "messages-placeholder-entrada-pneu");
    }
}

function LimparCamposEntradaPneu() {
    $("#txtDataEntrada").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#txtCalibragemEntradaPneu").val("0");
    $("#txtObservacaoEntradaPneu").val("");
    $("body").data("pneuUI", null);
    $("body").data("pneuDrop", null);
    $("body").data("eixoDrop", null);
    $("body").data("entradaDrop", null);
}

function BuscarUltimoStatusEntradaPneu(tipo) {
    executarRest("/StatusDePneu/ObterUltimoRegistroPorTipo?callback=?", { Tipo: tipo }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto != null) {
                $("body").data("codigoStatusEntradaPneu", r.Objeto.Codigo);
                $("#txtStatusEntradaPneu").val(r.Objeto.Descricao);
            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function ValidarCamposEntradaPneu() {
    var pneu = $("body").data("pneuDrop");
    var eixo = $("body").data("eixoDrop");

    var codigoPneu = pneu != null ? pneu.Codigo : 0;
    var codigoVeiculo = $("body").data("codigoVeiculo") || 0;
    var codigoEixo = eixo != null ? eixo.Codigo : 0;
    var codigoStatusPneu = $("body").data("codigoStatusEntradaPneu") || 0;
    var valido = true;

    if (isNaN(codigoPneu)) codigoPneu = 0;
    if (isNaN(codigoVeiculo)) codigoVeiculo = 0;
    if (isNaN(codigoEixo)) codigoEixo = 0;
    if (isNaN(codigoStatusPneu)) codigoStatusPneu = 0;

    if (codigoEixo <= 0)
        valido = false;

    if (codigoVeiculo <= 0)
        valido = false;

    if (codigoPneu <= 0)
        valido = false;

    if (codigoStatusPneu > 0) {
        CampoSemErro("#txtStatusEntradaPneu");
    } else {
        CampoComErro("#txtStatusEntradaPneu");
        valido = false;
    }

    return valido;
}