var StateComponente;
var IdComponenteEmEdicao = 0;

function arredondarPara2Casas(valor) {
    return Math.round((parseFloat(valor) + Number.EPSILON) * 100) / 100;
}

function formatarValorBR(valor) {
    return arredondarPara2Casas(valor).toFixed(2).replace('.', ',');
}

function parseValorBR(valor) {
    if (typeof valor === 'string') {
        var valorLimpo = valor.replace(/\./g, '').replace(',', '.');
        return parseFloat(valorLimpo) || 0;
    }
    return parseFloat(valor) || 0;
}

$(document).ready(function () {
    $("#btnSalvarComponente").click(function () {
        SalvarComponente();
    });

    $("#btnCancelarComponente").click(function () {
        LimparCamposComponente();
    });

    $("#btnExcluirComponente").click(function () {
        ExcluirComponente();
    });

    $("#divEmissaoMDFe").on("hide.bs.modal", function () {
        StateComponente.clear();
        LimparCamposComponente();
    });

    StateComponente = new State({
        name: "componente",
        id: "Id",
        render: RenderizarComponentesGrid
    });
});

function LimparCamposComponente() {
    IdComponenteEmEdicao = 0;
    $("#txtTipoInfPgto").val("");
    $("#txtValorDoPagamento").val("0,00");
    CampoSemErro($("#txtTipoInfPgto"));
    CampoSemErro($("#txtValorDoPagamento"));

    $("#btnExcluirComponente").hide();
}

function SalvarComponente() {
    var erros = ValidaComponente();
    if (erros.length == 0) {
        var _valorPagamento = $("#txtValorDoPagamento").val();

        var componente = {
            Id: IdComponenteEmEdicao,
            TipoComponente: $("#txtTipoInfPgto").val(),
            ValorComponente: _valorPagamento
        };

        InsereComponente(componente);
        LimparCamposComponente();
    } else {
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        $("#placeholder-validacao-componentes").html("");
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:", "placeholder-validacao-componentes");
    }
}

function CalcularSomaComponentes() {
    var itens = StateComponente.get();
    var soma = 0;

    itens.forEach(function (item) {
        if (!item.Excluir) 
            soma += parseValorBR(item.ValorComponente);
    });

    return arredondarPara2Casas(soma);
}

function ValidaComponente() {
    var valido = [];

    const tipoPagamentoNumerico = Number($("#txtTipoInfPgto").val());
    const valoresValidos = [1, 2, 3, 4, 5];

    if (!valoresValidos.includes(tipoPagamentoNumerico)) {
        valido.push("O Tipo é obrigatório.");
        CampoComErro($("#txtTipoInfPgto"));
    } else {
        CampoSemErro($("#txtTipoInfPgto"));

        var itens = StateComponente.get();
        var tipoJaExiste = false;

        itens.forEach(function (item) {
            if (!item.Excluir && Number(item.TipoComponente) === tipoPagamentoNumerico && item.Id !== IdComponenteEmEdicao) {
                tipoJaExiste = true;
            }
        });

        if (tipoJaExiste) {
            valido.push("Já existe um componente com o tipo selecionado. Não é permitido duplicar tipos.");
            CampoComErro($("#txtTipoInfPgto"));
        }
    }

    const valorPagamento = Globalize.parseFloat($("#txtValorDoPagamento").val());

    if (valorPagamento <= 0) {
        valido.push("Valor Total é obrigatório.");
        CampoComErro($("#txtValorDoPagamento"));
    } else {
        CampoSemErro($("#txtValorDoPagamento"));
    }

    return valido;
}

function InsereComponente(obj) {
    var obj = $.extend({
        Id: 0,
        TipoComponente: 0,
        ValorComponente: "",
        Excluir: false
    }, obj);

    obj.ValorComponente = formatarValorBR(parseValorBR(obj.ValorComponente));

    if (obj.Id != 0)
        StateComponente.update(obj);
    else {
        StateComponente.insert(obj);
        CampoSemErro($("#txtValorDaParcela"));
    }

    RecalcularValorDasParcelas();
}

function ExcluirComponente() {
    StateComponente.remove({ Id: IdComponenteEmEdicao });
    LimparCamposComponente();

    RecalcularValorDasParcelas();
}

function EditarComponente(info) {
    IdComponenteEmEdicao = info.Id;
    $("#txtTipoInfPgto").val(info.TipoComponente).trigger("blur");
    $("#txtValorDoPagamento").val(info.ValorComponente).trigger("blur");
    $("#btnExcluirComponente").show();
}

function RenderizarComponentesGrid() {
    var itens = StateComponente.get();
    var $tabela = $("#tblComponente");

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {

            var tipoComponente = "";

            switch (Number(info.TipoComponente)) {
                case 1:
                    tipoComponente = "Vale Pedágio"
                    break;
                case 2:
                    tipoComponente = "Impostos"
                    break;
                case 3:
                    tipoComponente = "Despesas"
                    break;
                case 4:
                    tipoComponente = "Frete"
                    break;
                case 5:
                    tipoComponente = "Outros"
                    break;
            }

            var $row = $("<tr>" +
                "<td>" + tipoComponente + "</td>" +
                "<td>" + info.ValorComponente + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Editar</button></td>" +
                "</tr>");

            $row.on("click", "button", function () {
                EditarComponente(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}