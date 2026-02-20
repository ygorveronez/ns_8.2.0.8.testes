$(document).ready(function () {
    CarregarConsultaDeObservacoesPorTipo("txtDescricaoObservacaoFisco", "btnBuscarObservacaoFisco", 1, SelecionarObservacaoFisco, true, false);
    $("#btnSalvarObservacaoFisco").click(function () {
        SalvarObservacaoFisco();
    });
    $("#btnExcluirObservacaoFisco").click(function () {
        ExcluirObservacaoFisco();
    });
    $("#btnCancelarObservacaoFisco").click(function () {
        LimparCamposObservacaoFisco();
    });
});

function SelecionarObservacaoFisco(observacao) {
    $("#txtDescricaoObservacaoFisco").val(observacao.Descricao);
}

function LimparCamposObservacaoFisco() {
    $("#txtDescricaoObservacaoFisco").val("");
    $("#txtIdentificadorObservacaoFisco").val("");
    $("#hddIdObservacaoFiscoEmEdicao").val("0");
    $("#btnExcluirObservacaoFisco").hide();
    $("#btnCancelarObservacaoFisco").hide();
}

function ValidarCamposObservacaoFisco() {
    var descricao = $("#txtDescricaoObservacaoFisco").val();
    var valido = true;
    if (descricao != "") {
        CampoSemErro("#txtDescricaoObservacaoFisco");
    } else {
        CampoComErro("#txtDescricaoObservacaoFisco");
        valido = false;
    }
    return valido;
}

function SalvarObservacaoFisco() {
    if (ValidarCamposObservacaoFisco()) {
        var observacao = {
            Codigo: Globalize.parseInt($("#hddIdObservacaoFiscoEmEdicao").val()),
            Descricao: $("#txtDescricaoObservacaoFisco").val(),
            Identificador: $("#txtIdentificadorObservacaoFisco").val(),
            Excluir: false
        };
        var observacoes = $("#hddObservacoesFisco").val() == "" ? new Array() : JSON.parse($("#hddObservacoesFisco").val());
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
        $("#hddObservacoesFisco").val(JSON.stringify(observacoes));
        RenderizarObservacoesFisco();
        LimparCamposObservacaoFisco();
    }
}

function EditarObservacaoFisco(observacao) {
    $("#hddIdObservacaoFiscoEmEdicao").val(observacao.Codigo);
    $("#txtDescricaoObservacaoFisco").val(observacao.Descricao);
    $("#txtIdentificadorObservacaoFisco").val(observacao.Identificador);
    $("#btnExcluirObservacaoFisco").show();
    $("#btnCancelarObservacaoFisco").show();
}

function ExcluirObservacaoFisco() {
    jConfirm("Deseja realmente excluir esta observação?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddIdObservacaoFiscoEmEdicao").val());
            var observacoes = $("#hddObservacoesFisco").val() == "" ? new Array() : JSON.parse($("#hddObservacoesFisco").val());
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
            $("#hddObservacoesFisco").val(JSON.stringify(observacoes));
            RenderizarObservacoesFisco();
            LimparCamposObservacaoFisco();
        }
    });
}

function RenderizarObservacoesFisco() {
    $("#tblObservacoesFisco tbody").html("");
    var observacoes = $("#hddObservacoesFisco").val() == "" ? new Array() : JSON.parse($("#hddObservacoesFisco").val());
    for (var i = 0; i < observacoes.length; i++) {
        if (!observacoes[i].Excluir) {
            $("#tblObservacoesFisco tbody").append("<tr class='linha'><td>" + observacoes[i].Identificador + "</td><td>" + observacoes[i].Descricao + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarObservacaoFisco(" + JSON.stringify(observacoes[i]) + ")'>Editar</button></td></tr>");
        }
    }
    if ($("#tblObservacoesFisco tbody").html() == "") {
        $("#tblObservacoesFisco tbody").html("<tr class='linha'><td colspan='3'>Nenhum registro encontrado!</td></tr>");
    }
}