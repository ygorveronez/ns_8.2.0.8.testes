$(document).ready(function () {
    CarregarConsultaDeSeriesPorTipo("btnBuscarSerieCTeAverbacao", "btnBuscarSerieCTeAverbacao", "A", "0", RetornoBuscarSerieCTeAverbacao, true, false);
});


function RetornoBuscarSerieCTeAverbacao(serie) {
    $("#txtSerieCTeAverbacao").data("CodigoSerie", serie.Codigo);
    $("#txtSerieCTeAverbacao").val(serie.Numero);
}


function ValidarAverbacaoSerie() {
    return $("#txtSerieCTeAverbacao").data("CodigoSerie") != null;
}

function SalvarAverbacaoSerie() {
    if (!ValidarAverbacaoSerie())
        return jAlert("Série é obrigatório.", "Atenção");

    var informacaoAverbacaoSerie = {
        Id: Globalize.parseInt($("#hddIdAverbacaoSerieEmEdicao").val()),
        CodigoSerie: $("#txtSerieCTeAverbacao").data("CodigoSerie"),
        NumeroSerie: $("#txtSerieCTeAverbacao").val(),
        Excluir: false
    };
    var infomacoesAverbacoesSerie = $("#hddInformacoesAverbacaoSerie").val() == "" ? new Array() : JSON.parse($("#hddInformacoesAverbacaoSerie").val());
    if (informacaoAverbacaoSerie.Id == 0) {
        informacaoAverbacaoSerie.Id = -(infomacoesAverbacoesSerie.length + 1);
    }
    for (var i = 0; i < infomacoesAverbacoesSerie.length; i++) {
        if (infomacoesAverbacoesSerie[i].Id == informacaoAverbacaoSerie.Id) {
            infomacoesAverbacoesSerie.splice(i, 1);
            break;
        }
    }
    infomacoesAverbacoesSerie.push(informacaoAverbacaoSerie);
    $("#hddInformacoesAverbacaoSerie").val(JSON.stringify(infomacoesAverbacoesSerie));
    RenderizarAverbacoesSerie();
    LimparAverbacaoSerie();
}

function RenderizarAverbacoesSerie() {
    var infomacoesAverbacoesSerie = $("#hddInformacoesAverbacaoSerie").val() == "" ? new Array() : JSON.parse($("#hddInformacoesAverbacaoSerie").val());

    $("#tblAverbacoesSerie tbody").html("");
    for (var i = 0; i < infomacoesAverbacoesSerie.length; i++) {
        var dadosDaAverbacaoSerie = infomacoesAverbacoesSerie[i];

        if (!dadosDaAverbacaoSerie.Excluir) {

            var $row = $("<tr><td>" + dadosDaAverbacaoSerie.NumeroSerie + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarAverbacaoSerie(" + JSON.stringify(infomacoesAverbacoesSerie[i]) + ")'>Editar</button></td></tr>");

            $("#tblAverbacoesSerie tbody").append($row);
        }
    }
    if ($("#tblAverbacoesSerie tbody").html() == "")
        $("#tblAverbacoesSerie tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

function ExcluirAverbacaoSerie() {
    var id = $("#hddIdAverbacaoSerieEmEdicao").val();
    var infomacoesAverbacoesSerie = $("#hddInformacoesAverbacaoSerie").val() == "" ? new Array() : JSON.parse($("#hddInformacoesAverbacaoSerie").val());
    for (var i = 0; i < infomacoesAverbacoesSerie.length; i++) {
        if (infomacoesAverbacoesSerie[i].Id == id) {
            if (id <= 0)
                infomacoesAverbacoesSerie.splice(i, 1);
            else
                infomacoesAverbacoesSerie[i].Excluir = true;
            break;
        }
    }
    $("#hddInformacoesAverbacaoSerie").val(JSON.stringify(infomacoesAverbacoesSerie));

    RenderizarAverbacoesSerie();
    LimparAverbacaoSerie();
}

function EditarAverbacaoSerie(averbacao) {
    LimparAverbacaoSerie();

    $("#hddIdAverbacaoSerieEmEdicao").val(averbacao.Id);
    $("#txtSerieCTeAverbacao").val("").data("CodigoSerie", averbacao.CodigoSerie);
    $("#txtSerieCTeAverbacao").val(averbacao.NumeroSerie)

    $("#btnExcluirAverbacaoSerie").show();
}

function LimparAverbacaoSerie() {
    $("#hddIdAverbacaoSerieEmEdicao").val('0');
    $("#txtSerieCTeAverbacao").val("").data("CodigoSerie", null);
    $("#btnExcluirAverbacaoSerie").hide();
}