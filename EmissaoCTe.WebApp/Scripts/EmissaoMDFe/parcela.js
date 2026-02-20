var _permitirMultiplasParcelas = false;
let verificarUpdateRemove = true;
var StateParcela;
var IdParcelaEmEdicao = 0;

$(document).ready(function () {
    $("#btnRecalcularParcela").click(function () {
        RecalcularValorDasParcelas();
        $("#btnRecalcularParcela").hide();
    });

    $("#btnSalvarParcela").click(function () {
        SalvarParcela();
    });

    $("#btnCancelarParcela").click(function () {
        LimparCamposParcela();
    });

    $("#btnExcluirParcela").click(function () {
        ExcluirParcela();
    });

    $("#divEmissaoMDFe").on("hide.bs.modal", function () {
        StateParcela.clear();
        LimparCamposParcela();
    });

    StateParcela = new State({
        name: "parcela",
        id: "Id",
        render: RenderizarParcelasGrid
    });
});

function ParcelaUnica() {
    if (!_permitirMultiplasParcelas) {
        var dataParcela = StateParcela.get()[0]?.DataVencimento ? new Date(formatarDataEN(StateParcela.get()[0].DataVencimento)) :  new Date();
        StateParcela.clear();
        CalcularValorProximaParcela(dataParcela);
        return true;
    }
    return false
}

function LimparCamposParcela() {
    IdParcelaEmEdicao = 0;

    $("#txtNumeroDaParcela").val(StateParcela.get().length + 1);
    $("#txtVencimentoDaParcela").val(DataVencimentoProximaParcela);
    CalcularValorProximaParcela()

    CampoSemErro($("#txtNumeroDaParcela"));
    CampoSemErro($("#txtVencimentoDaParcela"));
    CampoSemErro($("#txtValorDaParcela"));

    $("#btnExcluirParcela").hide();
}

function DataVencimentoProximaParcela() {
    var parcelas = StateParcela.get();

    if (!parcelas || parcelas.length === 0) {
        var dataAtual = new Date();
        dataAtual.setMonth(dataAtual.getMonth() + 1);
        return formatarDataBR(dataAtual);
    }

    var ultimaParcela = parcelas[parcelas.length - 1];
    var dataUltimoComponente = new Date(formatarDataEN(ultimaParcela.DataVencimento));
    dataUltimoComponente.setMonth(dataUltimoComponente.getMonth() + 1);

    return formatarDataBR(dataUltimoComponente);
}

function formatarDataBR(dataEN) {
    var dia = String(dataEN.getDate()).padStart(2, '0');
    var mes = String(dataEN.getMonth() + 1).padStart(2, '0');
    var ano = dataEN.getFullYear();
    return dia + '/' + mes + '/' + ano;
}

function formatarDataEN(dataBR) {
    var partes = dataBR.split('/');
    return new Date(partes[2], partes[1] - 1, partes[0]);
}

function SalvarParcela() {
    var erros = ValidaParcela();
    if (erros.length == 0) {
        var _valorParcela = $("#txtValorDaParcela").val();

        var parcela = {
            Id: IdParcelaEmEdicao,
            NumeroParcela: $("#txtNumeroDaParcela").val(),
            DataVencimento: $("#txtVencimentoDaParcela").val(),
            ValorParcela: _valorParcela
        };

        InsereParcela(parcela);
        LimparCamposParcela();
    } else {
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        $("#placeholder-validacao-inf-pagamento-parcelas").html("");
        ExibirMensagemAlerta(listaErros, "Os seguintes erros foram encontrados:", "placeholder-validacao-inf-pagamento-parcelas");
    }
}

function ValidaParcela(excluindo = false) {
    var valido = [];

    if ($("#selFormaPagamento").val() == "0") {
        valido.push("Só é permitido gerar parcela para pagamentos À Prazo");
        return valido;
    }

    if (excluindo) {
        if ($("#selFormaPagamento").val() == "1" && StateParcela.get().length == 1)
            valido.push("É obrigatório no mínimo uma parcela para pagamentos À Prazo");

        return valido;
    }

    if (ParcelaUnica() && $("#btnExcluirParcela").is(":hidden")) {
        valido.push("Para o emissor de documento Integrador só está liberado uma parcela.");
        return valido;
    }

    var dataVencimentoString = $("#txtVencimentoDaParcela").val()
    var partes = dataVencimentoString.split('/');
    var dataVencimento = new Date(partes[2], partes[1] - 1, partes[0]);

    var hoje = new Date();
    hoje.setHours(0, 0, 0, 0);

    if (dataVencimento <= hoje) {
        valido.push("A Data de Vencimento não pode ser menor ou igual a data atual.");
        CampoComErro($("#txtVencimentoDaParcela"));
    } else {
        var parcelas = StateParcela.get();
        if (parcelas && parcelas.length > 0) {
            var ultimaParcela = parcelas[parcelas.length - 1];
            var partesUltimaData = ultimaParcela.DataVencimento.split('/');
            var ultimaData = new Date(partesUltimaData[2], partesUltimaData[1] - 1, partesUltimaData[0]);

            if (dataVencimento.getFullYear() === ultimaData.getFullYear() && dataVencimento.getMonth() === ultimaData.getMonth() && dataVencimento.getDate() === ultimaData.getDate()) {
                valido.push("Não é permitido ter duas parcelas com a mesma data de vencimento.");
                CampoComErro($("#txtVencimentoDaParcela"));
            } else
                CampoSemErro($("#txtVencimentoDaParcela"));
        } else
            CampoSemErro($("#txtVencimentoDaParcela"));
    }

    const valorParcela = Globalize.parseFloat($("#txtValorDaParcela").val());

    if (valorParcela <= 0) {
        valido.push("Para adicionar parcelas crie componentes.");
        CampoComErro($("#txtValorDaParcela"));
    } else {
        CampoSemErro($("#txtValorDaParcela"));
    }

    return valido;
}

function CalcularSomaParcelas() {
    var itens = StateParcela.get();
    var soma = 0;

    itens.forEach(function (item) {
        if (!item.Excluir) 
            soma += parseValorBR(item.ValorParcela);
    });

    return arredondarPara2Casas(soma);
}

function CalcularValorProximaParcela(dataParcela = new Date()) {
    if (telaSomenteLeitura() || $("#selFormaPagamento").val() == "0") {
        return;
    }

    var valorPagamento = CalcularSomaComponentes();
    var valorAdiantamento = parseValorBR($("#txtValorDoAdiantamento").val());
    var resultado = arredondarPara2Casas(valorPagamento - valorAdiantamento);

    var parcelas = StateParcela.get();
    var quantidadeParcelas = parcelas.length + 1;
    var novoValorParcelas = arredondarPara2Casas(resultado / quantidadeParcelas);
    $("#txtValorDaParcela").val(formatarValorBR(novoValorParcelas));

    if (!parcelas || parcelas.length == 0) {
        var dataVencimento = new Date(dataParcela);
        if (dataVencimento == new Date())
            dataVencimento.setDate(dataParcela.getDate() + 30);

        var parcelaUnica = {
            Id: 0,
            NumeroParcela: 1,
            DataVencimento: Globalize.format(dataVencimento, "dd/MM/yyyy"),
            ValorParcela: formatarValorBR(resultado),
            Excluir: false
        };

        InsereParcela(parcelaUnica);
        $("#txtNumeroDaParcela").val(2);
        $("#txtVencimentoDaParcela").val(DataVencimentoProximaParcela());
    }
}

function RecalcularValorDasParcelas() {
    if ($("#selFormaPagamento").val() == "0") {
        return;
    }

    var parcelas = StateParcela.get();

    if (parcelas && parcelas.length > 0) {
        var valorPagamento = CalcularSomaComponentes();
        var valorAdiantamento = parseValorBR($("#txtValorDoAdiantamento").val());
        var resultado = arredondarPara2Casas(valorPagamento - valorAdiantamento);
        var valorPorParcela = arredondarPara2Casas(resultado / parcelas.length);
        var totalDistribuido = valorPorParcela * parcelas.length;
        var diferenca = arredondarPara2Casas(resultado - totalDistribuido);

        parcelas.forEach(function (parcela, index) {
            var valorFinal = valorPorParcela;
            if (index === parcelas.length - 1 && diferenca !== 0) {
                valorFinal = arredondarPara2Casas(valorPorParcela + diferenca);
            }

            parcela.ValorParcela = formatarValorBR(valorFinal);
        });

        StateParcela.set(parcelas);
        RenderizarParcelasGrid();
    }

    CalcularValorProximaParcela()
}

function InsereParcela(obj) {
    var obj = $.extend({
        Id: 0,
        NumeroParcela: 0,
        DataVencimento: "",
        ValorParcela: "",
        Excluir: false
    }, obj);

    obj.ValorParcela = formatarValorBR(parseValorBR(obj.ValorParcela));

    if (obj.Id != 0)
        StateParcela.update(obj);
    else
        StateParcela.insert(obj);

    RecalcularValorDasParcelas();
}

function ExcluirParcela() {
    var erros = ValidaParcela(true);
    if (erros.length == 0) {
        StateParcela.remove({ Id: IdParcelaEmEdicao });
        LimparCamposParcela();

        RecalcularValorDasParcelas();
    } else {
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        $("#placeholder-validacao-inf-pagamento-parcelas").html("");
        ExibirMensagemAlerta(listaErros, "Os seguintes erros foram encontrados:", "placeholder-validacao-inf-pagamento-parcelas");

        LimparCamposParcela();
    }
}

function EditarParcela(info) {
    IdParcelaEmEdicao = info.Id;
    $("#txtNumeroDaParcela").val(info.NumeroParcela).trigger("blur");
    $("#txtVencimentoDaParcela").val(info.DataVencimento).trigger("blur");
    $("#txtValorDaParcela").val(info.ValorParcela).trigger("blur");
    $("#btnExcluirParcela").show();
}

function RenderizarParcelasGrid() {
    var itens = StateParcela.get();
    var $tabela = $("#tblParcela");

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {
            var $row = $("<tr>" +
                "<td>" + info.NumeroParcela + "</td>" +
                "<td>" + info.DataVencimento + "</td>" +
                "<td>" + info.ValorParcela + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Editar</button></td>" +
                "</tr>");

            $row.on("click", "button", function () {
                EditarParcela(info);
                ParcelaUnica(true)
            });

            $tabela.find("tbody").append($row);
        }
    });

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}