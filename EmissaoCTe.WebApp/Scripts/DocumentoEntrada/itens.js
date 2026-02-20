$(document).ready(function () {
    CarregarConsultaDeProdutos("btnBuscarProduto", "btnBuscarProduto", "A", RetornoConsultaProduto, true, false);

    BuscarUnidadesDeMedida();
    BuscarCFOPs();

    $("#txtQuantidade").priceFormat({ centsLimit: 4 });
    $("#txtValorUnitario").priceFormat({ centsLimit: 4 });
    $("#txtDesconto").priceFormat();
    $("#txtValorTotalItem").priceFormat();
    $("#txtAliquotaICMS").priceFormat();
    $("#txtValorICMS").priceFormat();
    $("#txtAliquotaIPI").priceFormat();
    $("#txtValorIPI").priceFormat();
    $("#txtValorPIS").priceFormat();
    $("#txtValorICMSST").priceFormat();
    $("#txtValorCOFINS").priceFormat();
    $("#txtValorOutrasDespesas").priceFormat();
    $("#txtValorFrete").priceFormat();
    $("#txtBaseCalculoIPIItem").priceFormat();
    $("#txtBaseCalculoICMSSTItem").priceFormat();
    $("#txtBaseCalculoICMSItem").priceFormat();

    $("#txtProduto").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("body").data("produto", null);
            } else {
                e.preventDefault();
            }
        }
    });

    $("#txtQuantidade, #txtValorUnitario, #txtDesconto").focusout(function () {
        AtualizarValorTotalItem();
    });

    $("#txtAliquotaIPI, #txtBaseCalculoIPIItem").focusout(function () {
        AtualizarValorIPI();
    });

    $("#txtAliquotaICMS, #txtBaseCalculoICMSItem").focusout(function () {
        AtualizarValorICMS();
    });

    $("#btnSalvarItem").click(function () {
        SalvarItem();
    });

    $("#btnExcluirItem").click(function () {
        ExcluirItem();
    });

    $("#btnCancelarItem").click(function () {
        LimparCamposItem();
    });

    LimparCamposItem();
});

function LimparCamposItem() {
    $("body").data("item", null);

    $("#txtItem").val("Automático");
    $("#txtQuantidade").val('0,0000');
    $("#txtValorUnitario").val('0,0000');
    $("#txtDesconto").val('0,00');
    $("#txtBaseCalculoICMSItem").val("0,00");
    $("#txtAliquotaICMS").val('0,00');
    $("#txtValorICMS").val('0,00');
    $("#txtBaseCalculoIPIItem").val("0,00");
    $("#txtAliquotaIPI").val('0,00');
    $("#txtValorIPI").val('0,00');
    $("#txtValorPIS").val('0,00');
    $("#txtBaseCalculoICMSSTItem").val("0,00");
    $("#txtValorICMSST").val('0,00');
    $("#txtValorCOFINS").val('0,00');
    $("#txtValorOutrasDespesas").val('0,00');
    $("#txtValorFrete").val('0,00');
    $("#txtValorTotalItem").val("0,00");

    $("body").data("produto", 0);
    $("#txtProduto").val('');

    $("#selUnidadeMedida").val($("#selUnidadeMedida option:first").val()).change();

    $("#selCFOP").val($("#selCFOP option:first").val()).change();
    $("#selCSTICMS").val($("#selCSTICMS option:first").val());
    $("#selCSTIPI").val($("#selCSTIPI option:first").val());
    $("#selCSTPIS").val($("#selCSTPIS option:first").val());
    $("#selCSTCOFINS").val($("#selCSTCOFINS option:first").val());

    $("#btnExcluirItem").hide();
}

function RetornoConsultaProduto(produto) {
    executarRest("/Produto/ObterDetalhes?callback=?", { CodigoProduto: produto.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("produto", r.Objeto.Codigo);
            $("#txtProduto").val(r.Objeto.Descricao);
            $("#selUnidadeMedida").val(r.Objeto.CodigoUnidadeMedida);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function BuscarUnidadesDeMedida() {
    executarRest("/UnidadeMedidaGeral/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {

            var selUnidadeMedida = document.getElementById("selUnidadeMedida");

            selUnidadeMedida.length = 0;

            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '0';
            selUnidadeMedida.options.add(optn);

            for (var i = 0; i < r.Objeto.length; i++) {
                optn = document.createElement("option");
                optn.text = r.Objeto[i].Sigla + " - " + r.Objeto[i].Descricao;
                optn.value = r.Objeto[i].Codigo;
                selUnidadeMedida.options.add(optn);
            }

        } else {
            ExibirMensagem(r.Erro, "Atenção!");
        }
    });
}

function BuscarCFOPs() {
    executarRest("/CFOP/BuscarTodos?callback=?", { Tipo: 0 }, function (r) {
        if (r.Sucesso) {

            var selCFOP = document.getElementById("selCFOP");

            selCFOP.length = 0;

            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '0';
            selCFOP.options.add(optn);

            for (var i = 0; i < r.Objeto.length; i++) {
                optn = document.createElement("option");
                optn.text = r.Objeto[i].Numero + " - " + r.Objeto[i].Descricao;
                optn.value = r.Objeto[i].Codigo;
                selCFOP.options.add(optn);
            }

        } else {
            ExibirMensagem(r.Erro, "Atenção!");
        }
    });
}

function AtualizarValorTotalItem() {
    var quantidade = Globalize.parseFloat($("#txtQuantidade").val());
    var valorUnitario = Globalize.parseFloat($("#txtValorUnitario").val());
    var desconto = Globalize.parseFloat($("#txtDesconto").val());

    var valorTotal = (quantidade * valorUnitario); //- desconto

    $("#txtValorTotalItem").val(Globalize.format(valorTotal, "n2"));

    AtualizarValorICMS();
    AtualizarValorIPI();
    AtualizarValorPIS();
    AtualizarValorCOFINS();
}

function AtualizarValorPIS() {
    if (AliquotaPIS > 0) {
        var valorTotal = Globalize.parseFloat($("#txtValorTotalItem").val());
        var desconto = Globalize.parseFloat($("#txtDesconto").val());

        valorTotal = valorTotal - desconto;

        var valor = (AliquotaPIS / 100) * valorTotal;

        $("#txtValorPIS").val(Globalize.format(valor, "n2"));
    }
}

function AtualizarValorCOFINS() {
    if (AliquotaCOFINS > 0) {
        var valorTotal = Globalize.parseFloat($("#txtValorTotalItem").val());
        var desconto = Globalize.parseFloat($("#txtDesconto").val());
        valorTotal = valorTotal - desconto;

        var valor = (AliquotaCOFINS / 100) * valorTotal;

        $("#txtValorCOFINS").val(Globalize.format(valor, "n2"));
    }
}

function AtualizarValorICMS() {
    var aliquota = Globalize.parseFloat($("#txtAliquotaICMS").val());
    var valorTotal = Globalize.parseFloat($("#txtBaseCalculoICMSItem").val());

    var valor = (aliquota / 100) * valorTotal;

    $("#txtValorICMS").val(Globalize.format(valor, "n2"));
}

function AtualizarValorIPI() {
    var aliquota = Globalize.parseFloat($("#txtAliquotaIPI").val());
    var valorTotal = Globalize.parseFloat($("#txtBaseCalculoIPIItem").val());

    var valor = (aliquota / 100) * valorTotal;

    $("#txtValorIPI").val(Globalize.format(valor, "n2"));
}

function ValidarCamposItem() {
    var quantidade = Globalize.parseFloat($("#txtQuantidade").val());
    var codigoProduto = Globalize.parseInt($("body").data("produto").toString());
    var unidadeMedida = Globalize.parseInt($("#selUnidadeMedida").val());
    var cfop = Globalize.parseInt($("#selCFOP").val());
    var cst = Globalize.parseInt($("#selCSTICMS").val());
    var valorUnitario = Globalize.parseFloat($("#txtValorUnitario").val());
    var valido = true;

    if (isNaN(quantidade) || quantidade <= 0) {
        CampoComErro("#txtQuantidade");
        valido = false;
    } else {
        CampoSemErro("#txtQuantidade");
    }

    if (isNaN(codigoProduto) || codigoProduto <= 0) {
        CampoComErro("#txtProduto");
        valido = false;
    } else {
        CampoSemErro("#txtProduto");
    }

    if (isNaN(unidadeMedida) || unidadeMedida <= 0) {
        CampoComErro("#selUnidadeMedida");
        valido = false;
    } else {
        CampoSemErro("#selUnidadeMedida");
    }

    if (isNaN(cfop) || cfop <= 0) {
        CampoComErro("#selCFOP");
        valido = false;
    } else {
        CampoSemErro("#selCFOP");
    }

    if (isNaN(cst) || $("#selCSTICMS").val().length < 2) {
        CampoComErro("#selCSTICMS");
        valido = false;
    } else {
        CampoSemErro("#selCSTICMS");
    }

    if (isNaN(valorUnitario) || valorUnitario <= 0) {
        CampoComErro("#txtValorUnitario");
        valido = false;
    } else {
        CampoSemErro("#txtValorUnitario");
    }

    return valido;
}

function SalvarItem() {
    if (ValidarCamposItem()) {
        var item = {
            Codigo: $("body").data("item") != null ? $("body").data("item").Codigo : 0,
            CodigoProdutoFornecedor: $("body").data("item") != null ? $("body").data("item").CodigoProdutoFornecedor : "",
            Sequencial: $("body").data("item") != null ? $("body").data("item").Sequencial : 0,
            CodigoProduto: $("body").data("produto"),
            DescricaoProduto: $("#txtProduto").val(),
            CodigoUnidadeMedida: $("#selUnidadeMedida").val(),
            DescricaoUnidadeMedida: $("#selUnidadeMedida option:selected").text(),
            Quantidade: Globalize.parseFloat($("#txtQuantidade").val()),
            ValorUnitario: Globalize.parseFloat($("#txtValorUnitario").val()),
            Desconto: Globalize.parseFloat($("#txtDesconto").val()),
            ValorTotal: Globalize.parseFloat($("#txtValorTotalItem").val()),
            CST: $("#selCSTICMS").val(),
            CSTIPI: $("#selCSTIPI").val(),
            CSTPIS: $("#selCSTPIS").val(),
            CSTCOFINS: $("#selCSTCOFINS").val(),
            CodigoCFOP: $("#selCFOP").val(),
            DescricaoCFOP: $("#selCFOP option:selected").text().substring(0, 4),
            AliquotaICMS: Globalize.parseFloat($("#txtAliquotaICMS").val()),
            ValorICMS: Globalize.parseFloat($("#txtValorICMS").val()),
            AliquotaIPI: Globalize.parseFloat($("#txtAliquotaIPI").val()),
            ValorIPI: Globalize.parseFloat($("#txtValorIPI").val()),
            ValorICMSST: Globalize.parseFloat($("#txtValorICMSST").val()),
            ValorOutrasDespesas: Globalize.parseFloat($("#txtValorOutrasDespesas").val()),
            ValorFrete: Globalize.parseFloat($("#txtValorFrete").val()),
            ValorPIS: Globalize.parseFloat($("#txtValorPIS").val()),
            ValorCOFINS: Globalize.parseFloat($("#txtValorCOFINS").val()),
            BaseCalculoIPI: Globalize.parseFloat($("#txtBaseCalculoIPIItem").val()),
            BaseCalculoICMSST: Globalize.parseFloat($("#txtBaseCalculoICMSSTItem").val()),
            BaseCalculoICMS: Globalize.parseFloat($("#txtBaseCalculoICMSItem").val()),
            Excluir: false
        };

        var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");

        itens.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (item.Codigo == 0)
            item.Codigo = (itens.length > 0 ? (itens[0].Codigo > 0 ? -1 : (itens[0].Codigo - 1)) : -1);

        for (var i = 0; i < itens.length; i++) {
            if (itens[i].Codigo == item.Codigo) {
                itens.splice(i, 1);
                break;
            }
        }

        itens.push(item);

        itens.sort(function (a, b) { return a.Sequencial < b.Sequencial ? -1 : 1; });

        if (item.Sequencial <= 0)
            itens = ReorganizarSequenciaItens(itens);

        $("body").data("itens", itens);

        RenderizarItens();
        LimparCamposItem();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-mensagem-itens");
    }
}

function RenderizarItens() {
    var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");
    var valorTotalProdutos = 0,
        valorTotalICMSProdutos = 0,
        valorTotalPisProdutos = 0,
        valorTotalCofinsProdutos = 0,
        valorTotalIPIProdutos = 0,
        valorTotalICMSSTProdutos = 0,
        valorTotalDescontoProdutos = 0,
        valorTotalOutrasDespesasProdutos = 0,
        valorTotalFreteProdutos = 0,
        valorTotalBCICMSProdutos = 0,
        valorTotalBCICMSStProdutos = 0;

    $("#tblItens tbody").html("");
    var possuiItemComLitros = false;

    for (var i = 0; i < itens.length; i++) {
        if (!itens[i].Excluir) {
            if (/litro/i.test(itens[i].DescricaoUnidadeMedida))
                possuiItemComLitros = true;
            $("#tblItens tbody").append("<tr><td>" + itens[i].Sequencial + "</td><td>" + itens[i].DescricaoProduto + "</td><td>" + Globalize.format(itens[i].Quantidade, "n2") + "</td><td>" + Globalize.format(itens[i].ValorUnitario, "n2") + "</td><td>" + Globalize.format(itens[i].ValorTotal, "n2") + "</td><td>" + itens[i].DescricaoCFOP + "</td><td>" + itens[i].CST + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarItem(" + JSON.stringify(itens[i]) + ")'>Editar</button></td></tr>");
            valorTotalProdutos += itens[i].ValorTotal;
            valorTotalBCICMSProdutos += itens[i].BaseCalculoICMS;
            valorTotalICMSProdutos += itens[i].ValorICMS;
            valorTotalBCICMSStProdutos += itens[i].BaseCalculoICMSST;
            valorTotalICMSSTProdutos += itens[i].ValorICMSST;
            valorTotalPisProdutos += itens[i].ValorPIS;
            valorTotalCofinsProdutos += itens[i].ValorCOFINS;
            valorTotalIPIProdutos += itens[i].ValorIPI;
            valorTotalDescontoProdutos += itens[i].Desconto;
            valorTotalOutrasDespesasProdutos += itens[i].ValorOutrasDespesas;
            valorTotalFreteProdutos += itens[i].ValorFrete;
        }
    }

    $("#txtValorProdutos").val(Globalize.format(valorTotalProdutos, "n2"));
    $("#txtBaseCalculoICMS").val(Globalize.format(valorTotalBCICMSProdutos, "n2"));
    $("#txtValorTotalICMS").val(Globalize.format(valorTotalICMSProdutos, "n2"));
    $("#txtBaseCalculoICMSST").val(Globalize.format(valorTotalBCICMSStProdutos, "n2"));
    $("#txtValorTotalICMSST").val(Globalize.format(valorTotalICMSSTProdutos, "n2"));
    $("#txtValorTotalPIS").val(Globalize.format(valorTotalPisProdutos, "n2"));
    $("#txtValorTotalCOFINS").val(Globalize.format(valorTotalCofinsProdutos, "n2"));
    $("#txtValorTotalIPI").val(Globalize.format(valorTotalIPIProdutos, "n2"));
    $("#txtValorTotalDesconto").val(Globalize.format(valorTotalDescontoProdutos, "n2"));
    $("#txtValorTotalOutrasDespesas").val(Globalize.format(valorTotalOutrasDespesasProdutos, "n2"));
    $("#txtValorTotalFrete").val(Globalize.format(valorTotalFreteProdutos, "n2"));

    CopiarDadosAbastecimentos(possuiItemComLitros);

    if ($("#tblItens tbody").html() == "")
        $("#tblItens tbody").html("<tr><td colspan='8'>Nenhum registro encontrado.</td></tr>");
}

function EditarItem(item) {
    $("body").data("item", item);
    $("#txtItem").val(item.Sequencial);
    $("body").data("produto", item.CodigoProduto);
    $("#txtProduto").val(item.DescricaoProduto);
    $("#selUnidadeMedida").val(item.CodigoUnidadeMedida);
    $("#txtQuantidade").val(Globalize.format(item.Quantidade, "n4"));
    $("#txtValorUnitario").val(Globalize.format(item.ValorUnitario, "n4"));

    $("#txtDesconto").val(Globalize.format(item.Desconto, "n2"));
    $("#txtValorTotalItem").val(Globalize.format(item.ValorTotal, "n2"));
    $("#selCSTICMS").val(item.CST);
    $("#selCSTIPI").val(item.CSTIPI);
    $("#selCSTPIS").val(item.CSTPIS);
    $("#selCSTCOFINS").val(item.CSTCOFINS);
    $("#selCFOP").val(item.CodigoCFOP);

    $("#txtAliquotaICMS").val(Globalize.format(item.AliquotaICMS, "n2"));
    $("#txtValorICMS").val(Globalize.format(item.ValorICMS, "n2"));
    $("#txtAliquotaIPI").val(Globalize.format(item.AliquotaIPI, "n2"));
    $("#txtValorIPI").val(Globalize.format(item.ValorIPI, "n2"));
    $("#txtValorICMSST").val(Globalize.format(item.ValorICMSST, "n2"));
    $("#txtValorOutrasDespesas").val(Globalize.format(item.ValorOutrasDespesas, "n2"));
    $("#txtValorFrete").val(Globalize.format(item.ValorFrete, "n2"));
    $("#txtValorPIS").val(Globalize.format(item.ValorPIS, "n2"));
    $("#txtValorCOFINS").val(Globalize.format(item.ValorCOFINS, "n2"));

    $("#txtBaseCalculoIPIItem").val(Globalize.format(item.BaseCalculoIPI, "n2"));
    $("#txtBaseCalculoICMSSTItem").val(Globalize.format(item.BaseCalculoICMSST, "n2"));
    $("#txtBaseCalculoICMSItem").val(Globalize.format(item.BaseCalculoICMS, "n2"));

    if (item.ValorPIS == 0 && item.ValorCOFINS == 0) {
        AtualizarValorPIS();
        AtualizarValorCOFINS();
    }

    $("#btnExcluirItem").show();
}

function ExcluirItem() {
    var item = $("body").data("item");

    var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");

    for (var i = 0; i < itens.length; i++) {
        if (itens[i].Codigo == item.Codigo) {
            if (item.Codigo <= 0)
                itens.splice(i, 1);
            else
                itens[i].Excluir = true;
            break;
        }
    }

    $("body").data("itens", itens);

    itens = ReorganizarSequenciaItens(itens);

    RenderizarItens();
    LimparCamposItem();
}

function ReorganizarSequenciaItens(itens) {
    itens.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

    var sequencia = 1;

    for (var i = 0; i < itens.length; i++) {
        if (!itens[i].Excluir) {
            itens[i].Sequencial = sequencia;
            sequencia += 1;
        }
    }

    return itens;
}