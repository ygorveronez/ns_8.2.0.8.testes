$(document).ready(function () {
    CarregarConsultaDeObservacoesPorTipo("txtDescricaoObservacaoContribuinte", "btnBuscarObservacaoContribuinte", 0, SelecionarObservacaoContribuinte, true, false);
    $("#btnSalvarObservacaoContribuinte").click(function () {
        SalvarObservacaoContribuinte();
    });
    $("#btnExcluirObservacaoContribuinte").click(function () {
        ExcluirObservacaoContribuinte();
    });
    $("#btnCancelarObservacaoContribuinte").click(function () {
        LimparCamposObservacaoContribuinte();
    });
});

function SelecionarObservacaoContribuinte(observacao) {
    $("#txtDescricaoObservacaoContribuinte").val(observacao.Descricao);
}

function LimparCamposObservacaoContribuinte() {
    $("#txtDescricaoObservacaoContribuinte").val("");
    $("#txtIdentificadorObservacaoContribuinte").val("");
    $("#hddIdObservacaoContribuinteEmEdicao").val("0");
    $("#btnExcluirObservacaoContribuinte").hide();
    $("#btnCancelarObservacaoContribuinte").hide();
}

function ValidarCamposObservacaoContribuinte() {
    var descricao = $("#txtDescricaoObservacaoContribuinte").val();
    var valido = true;
    if (descricao != "") {
        CampoSemErro("#txtDescricaoObservacaoContribuinte");
    } else {
        CampoComErro("#txtDescricaoObservacaoContribuinte");
        valido = false;
    }
    return valido;
}

function SalvarObservacaoContribuinte() {
    if (ValidarCamposObservacaoContribuinte()) {
        var observacao = {
            Codigo: Globalize.parseInt($("#hddIdObservacaoContribuinteEmEdicao").val()),
            Descricao: $("#txtDescricaoObservacaoContribuinte").val(),
            Identificador: $("#txtIdentificadorObservacaoContribuinte").val(),
            Excluir: false
        };
        var observacoes = $("#hddObservacoesContribuinte").val() == "" ? new Array() : JSON.parse($("#hddObservacoesContribuinte").val());
        if (observacao.Codigo == 0)
            observacao.Codigo = -(observacoes.length + 1);
        if (observacoes.length > 0) {
            for (var i = 0; i < observacoes.length; i++) {
                if (observacoes[i].Codigo == observacoes.Codigo) {
                    observacoes.splice(i, 1);
                    break;
                }
            }
        }
        observacoes.push(observacao);
        observacoes.sort();
        $("#hddObservacoesContribuinte").val(JSON.stringify(observacoes));
        RenderizarObservacoesContribuinte();
        LimparCamposObservacaoContribuinte();
    }
}

function EditarObservacaoContribuinte(observacao) {
    $("#hddIdObservacaoContribuinteEmEdicao").val(observacao.Codigo);
    $("#txtDescricaoObservacaoContribuinte").val(observacao.Descricao);
    $("#txtIdentificadorObservacaoContribuinte").val(observacao.Identificador);
    $("#btnExcluirObservacaoContribuinte").show();
    $("#btnCancelarObservacaoContribuinte").show();
}

function ExcluirObservacaoContribuinte() {
    jConfirm("Deseja realmente excluir esta observação?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddIdObservacaoContribuinteEmEdicao").val());
            var observacoes = $("#hddObservacoesContribuinte").val() == "" ? new Array() : JSON.parse($("#hddObservacoesContribuinte").val());
            for (var i = 0; i < observacoes.length; i++) {
                if (observacoes[i].Codigo == codigo) {
                    if (codigo > 0) {
                        observacoes[i].Excluir = true;
                    } else {
                        observacoes.splice(i, 1);
                    }
                    break;
                }
            }
            $("#hddObservacoesContribuinte").val(JSON.stringify(observacoes));
            RenderizarObservacoesContribuinte();
            LimparCamposObservacaoContribuinte();
        }
    });
}

function RenderizarObservacoesContribuinte() {
    $("#tblObservacoesContribuinte tbody").html("");
    var observacoes = $("#hddObservacoesContribuinte").val() == "" ? new Array() : JSON.parse($("#hddObservacoesContribuinte").val());
    for (var i = 0; i < observacoes.length; i++) {
        if (!observacoes[i].Excluir) {
            $("#tblObservacoesContribuinte tbody").append("<tr class='linha'><td>" + observacoes[i].Identificador + "</td><td>" + observacoes[i].Descricao + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarObservacaoContribuinte(" + JSON.stringify(observacoes[i]) + ")'>Editar</button></td></tr>");
        }
    }
    if ($("#tblObservacoesContribuinte tbody").html() == "") {
        $("#tblObservacoesContribuinte tbody").html("<tr class='linha'><td colspan='3'>Nenhum registro encontrado!</td></tr>");
    }
}