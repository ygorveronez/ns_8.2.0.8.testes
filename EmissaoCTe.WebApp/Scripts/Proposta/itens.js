$(document).ready(function () {
    $("#txtValorItem").priceFormat({ prefix: '' });

    StateItens = new State({
        name: "itens",
        id: "Id",
        render: RenderizarItens
    });


    $("#btnSalvarItem").click(function () {
        SalvarItem();
    });

    $("#btnCancelarItem").click(function () {
        LimparCamposItem()
    });

    $("#btnExcluirItem").click(function () {
        ExcluirItem();
    });
});

var StateItens;
var IdItemEmEdicao = 0;


function ItensJson(){
    return StateItens.toJson();
}

function CarregaItens(itens) {
    StateItens.set(itens);
}

function SalvarItem() {
    var erros = ValidaItem();
    if (erros.length == 0) {
        var item = {
            Id: IdItemEmEdicao,
            Descricao: $("#txtDescricaoItem").val(),
            Valor: Globalize.parseFloat($("#txtValorItem").val()),
            Tipo: $("#selTipoItem").val()
        };

        InsereItem(item);
        LimparCamposItem();
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>";

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:");
    }
}

function ValidaItem() {
    var valido = [];

    if ($("#txtDescricaoItem").val() == "") {
        valido.push("Descrição é obrigatório.");
        CampoComErro($("#txtDescricaoItem"));
    } else {
        CampoSemErro($("#txtDescricaoItem"));
    }

    return valido;
}

function InsereItem(obj) {
    var obj = $.extend({
        Id: 0,
        Descricao: "",
        Valor: 0,
        Tipo: 0,
        Excluir: false
    }, obj);

    if (obj.Id != 0)
        StateItens.update(obj);
    else
        StateItens.insert(obj);
}

function EditarItem(info) {
    IdItemEmEdicao = info.Id;
    $("#txtDescricaoItem").val(info.Descricao);    
    $("#txtValorItem").val(Globalize.format(info.Valor, "n2")).trigger("blur");
    $("#selTipoItem").val(info.Tipo);

    $("#btnExcluirItem").show();
}

function ExcluirItem() {
    StateItens.remove({ Id: IdItemEmEdicao });
    LimparCamposItem();
}

function LimparTodosItens() {
    StateItens.clear();
}

function LimparCamposItem() {
    IdItemEmEdicao = 0;
    $("#txtDescricaoItem").val("");
    $("#txtValorItem").val("0,00");
    $("#selTipoItem").val($("#selTipoItem option:first").val());

    CampoSemErro($("#txtDescricaoItem"));

    $("#btnExcluirItem").hide();
}

function RenderizarItens() {
    var itens = StateItens.get();
    var $tabela = $("#tblItensProposta");
    var valorTotal = 0;

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {
            // Adiciona ao sumarizador
            if (!isNaN(info.Valor) && info.Tipo == 0)
                valorTotal += info.Valor;

            var valor = info.Tipo == "0" ? "R$ " + Globalize.format(info.Valor, "n2") : Globalize.format(info.Valor, "n2")+" %";

            var $row = $("<tr>" +
                "<td>" + info.Descricao + "</td>" +
                "<td>" + (!isNaN(info.Valor) ? valor : "") + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Editar</button></td>" +
            "</tr>");

            $row.on("click", "button", function () {
                EditarItem(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if (itens.length > 0) {
        // Sumarizador
        var $row = $("<tr>" +
            "<td><strong>Total:</strong></td>" +
            "<td colspan='2'><strong>R$ " + Globalize.format(valorTotal, "n2") + "</strong></td>" +
        "</tr>");
    }

    $tabela.find("tbody").append($row);

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}