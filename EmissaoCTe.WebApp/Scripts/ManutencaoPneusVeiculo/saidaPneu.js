$(function () {

    $("#txtDataSaida").mask("99/99/9999");
    $("#txtDataSaida").datepicker();

    $("#txtCalibragemSaidaPneu").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
    $("#txtKMSaida").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });

    $("#txtStatusSaidaPneu").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("codigoStatusSaidaPneu", null);
            }
        }
        e.preventDefault();
    });

    CarregarConsultaDeStatusDePneus("btnBuscarStatusSaidaPneu", "btnBuscarStatusSaidaPneu", "A", "E", RetornoConsultaStatusSaidaPneu, true, false);

    $("#btnSalvarSaidaPneu").click(function () {
        SalvarSaidaPneu();
    });

    $("#btnCancelarSaidaPneu").click(function () {
        CancelarSaidaPneu();
    });

    $("#btnFecharSaidaPneu").click(function () {
        CancelarSaidaPneu();
    });

    LimparCamposSaidaPneu();
    BuscarUltimoStatusSaidaPneu();
});

function CancelarSaidaPneu() {
    var elemento = $("body").data("pneuUI");

    if ($(elemento.draggable).parent("#divContainerEstoquePneu").length > 0)
        AtualizarEstoquePneus(0, false);
    else
        AtualizarManutencaoPneus(0, false);

    $(elemento.draggable).remove();

    FecharTelaSaidaPneu();
    LimparCamposSaidaPneu();
}

function RetornoConsultaStatusSaidaPneu(status) {
    $("#txtStatusSaidaPneu").val(status.Descricao);
    $("body").data("codigoStatusSaidaPneu", status.Codigo);
}

function AbrirTelaSaidaPneu() {
    $("#divSaidaPneu").modal({ keyboard: false, backdrop: 'static' });
}

function FecharTelaSaidaPneu() {
    $("#divSaidaPneu").modal("hide");
}

function SalvarSaidaPneu() {
    if (ValidarCamposSaidaPneu()) {
        var dados = {
            Codigo: 0,
            CodigoPneu: $("body").data("pneuDrop").Codigo,
            CodigoVeiculo: $("body").data("codigoVeiculo"),
            CodigoEixo: $("body").data("eixoDrop").Codigo,
            CodigoStatusPneu: $("body").data("codigoStatusSaidaPneu"),
            Tipo: "E",
            Data: $("#txtDataSaida").val(),
            KM: $("#txtKMSaida").val(),
            Calibragem: $("#txtCalibragemSaidaPneu").val(),
            Observacao: $("#txtObservacaoSaidaPneu").val()
        };
        executarRest("/ManutencaoPneusVeiculo/Salvar?callback=?", dados, function (r) {
            if (r.Sucesso) {
                VincularPneuAoEixo();
                AtualizarEstoquePneus(0, false);
                AtualizarManutencaoPneus(0, false);
                FecharTelaSaidaPneu();
                LimparCamposSaidaPneu();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção", "messages-placeholder-saida-pneu");
            }
        });
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!", "messages-placeholder-saida-pneu");
    }
}

function VincularPneuAoEixo() {
    var pneu = $("body").data("pneuDrop");
    var eixo = $("body").data("eixoDrop");

    $("#pneu_" + pneu.Codigo).remove();

    var divEixo = document.getElementById("eixo_" + eixo.Codigo);

    divEixo.innerHTML = "";

    divEixo.appendChild(ObterElementoPneu(pneu, "pneuEixo"));

    $("#pneu_" + pneu.Codigo).draggable(padraoDraggable);
    $("#pneu_" + pneu.Codigo).data("pneu", pneu);

    $("#eixo_" + eixo.Codigo).droppable("option", "disabled", true);
}

function LimparCamposSaidaPneu() {
    $("#txtDataSaida").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#txtCalibragemSaidaPneu").val("0");
    $("#txtObservacaoSaidaPneu").val("");
    $("body").data("pneuUI", null);
    $("body").data("pneuDrop", null);
    $("body").data("eixoDrop", null);
}

function BuscarUltimoStatusSaidaPneu() {
    executarRest("/StatusDePneu/ObterUltimoRegistroPorTipo?callback=?", { Tipo: "E" }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto != null) {
                $("body").data("codigoStatusSaidaPneu", r.Objeto.Codigo);
                $("#txtStatusSaidaPneu").val(r.Objeto.Descricao);
            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function ValidarCamposSaidaPneu() {
    var pneu = $("body").data("pneuDrop");
    var eixo = $("body").data("eixoDrop");

    var codigoPneu = pneu != null ? pneu.Codigo : 0;
    var codigoVeiculo = $("body").data("codigoVeiculo") || 0;
    var codigoEixo = eixo != null ? eixo.Codigo : 0;
    var codigoStatusPneu = $("body").data("codigoStatusSaidaPneu") || 0;
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
        CampoSemErro("#txtStatusSaidaPneu");
    } else {
        CampoComErro("#txtStatusSaidaPneu");
        valido = false;
    }

    return valido;
}