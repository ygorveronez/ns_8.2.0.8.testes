$(document).ready(function () {
    $("#txtColetaPesoTotal").priceFormat();

    $("#btnSalvarColeta").click(function () {
        SalvarColeta();
    });

    StateColetas = new State({
        name: "coletas",
        id: "Id",
        render: RenderizarColetas
    });

    RenderizarColetas();
    LimparCamposColetas();
});

var StateColetas;

function LimparColetas() {
    StateColetas.clear();
    LimparCamposColetas();
}

function LimparCamposColetas() {
    $("#txtColetaDescricao").val("");
    $("#txtColetaPesoTotal").val("0,00");
}

function SalvarColeta() {
    var erros = ValidaColeta();
    if (erros.length == 0) {
        var coleta = ObjetoColeta();

        InsereColeta(coleta);
        LimparCamposColetas();
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        // Limpa quaisquer erros existentes
        $("#placeholder-validacao-coletas").html("");

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:", "placeholder-validacao-coletas");
    }
}

function ValidaColeta() {
    var valido = [];

    var chave = $("#txtChave").val().replace(/[^0-9]/g, '');

    if ($("#txtCliente").data('codigo') == 0) {
        valido.push("É preciso selecionar um cliente antes de informar coletas.");
    } 

    if ($("#txtColetaDescricao").val() == "") {
        valido.push("Descrição da coleta é obrigatório.");
        CampoComErro($("#txtColetaDescricao"));
    } else {
        CampoSemErro($("#txtColetaDescricao"));
    }

    if ($("#txtColetaPesoTotal").val() == "0,00") {
        valido.push("Peso não pode ser zerado.");
        CampoComErro($("#txtColetaPesoTotal"));
    } else {
        CampoSemErro($("#txtColetaPesoTotal"));
    }

    return valido;
}

function ObjetoColeta() {
    return {
        Coleta: $("#txtColetaDescricao").val(),
        Peso: Globalize.parseFloat($("#txtColetaPesoTotal").val())
    };
}

function InsereColeta(obj) {
    obj = $.extend({
        Id: 0,
        Coleta: "",
        Peso: 0,
        ValorEvento: 0,
        ValorFracao: 0,
        Excluir: false
    }, obj);

    if (CalculoRelacaoCTesEntregues != null) {
        obj.ValorEvento = CalculoRelacaoCTesEntregues.ColetaValorPorEvento;
        obj.ValorFracao = CalculaValorFracao(obj);
    }

    StateColetas.insert(obj);
    CalculaColetas();
}

function RecalcularTodasColetas() {
    if (CalculoRelacaoCTesEntregues == null)
        return;

    var coletas = StateColetas.get();

    var coletasRecalculadas = coletas.map(function (coleta) {
        coleta.ValorEvento = CalculoRelacaoCTesEntregues.ColetaValorPorEvento;
        coleta.ValorFracao = CalculaValorFracao(coleta);

        return coleta;
    });

    StateColetas.set(coletas);
    CalculaColetas();
}

function ExcluirColeta(info) {
    StateColetas.remove({ Id: info.Id });
    CalculaColetas();

    LimparCamposColetas();
}

function RenderizarColetas() {
    var itens = StateColetas.get();
    var $tabela = $("#tblColetas");

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {
            var $row = $("<tr>" +
                "<td>" + info.Coleta + "</td>" +
                "<td>" + Globalize.format(info.Peso, "n2") + "</td>" +
                "<td>" + Globalize.format(info.ValorEvento, "n2") + "</td>" +
                "<td>" + Globalize.format(info.ValorFracao, "n2") + "</td>" +
                ((STATUS_RELACAO_ABERTA == STATUS_RELACAO.Aberto || STATUS_RELACAO_ABERTA == STATUS_RELACAO.Todas) ? "<td><button type='button' class='btn btn-default btn-xs btn-block'>Excluir</button></td>" : "") +
                "</tr>");

            $row.on("click", "button", function () {
                ExcluirColeta(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}

function CalculaValorFracao(obj) {
    var quantidadesFracionadas = Math.floor(obj.Peso / CalculoRelacaoCTesEntregues.ColetaFracao);
    var totalPorFracao = quantidadesFracionadas * CalculoRelacaoCTesEntregues.ColetaValorPorFracao;

    return totalPorFracao;
}