$(document).ready(function () {
    CarregarConsultaDeDocumentosDeEntrada("default-search", "default-search", "", RetornoConsultaDocumentoEntrada, true, false);
    CarregarConsultadeClientes("btnBuscarFornecedor", "btnBuscarFornecedor", RetornoConsultaFornecedor, true, false);
    CarregarConsultaDePlanosDeContas("btnBuscarPlanoConta", "btnBuscarPlanoConta", "A", "A", RetornoConsultaPlanoConta, true, false);
    CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);

    $("#txtDataEntrada").datepicker();
    $("#txtDataEmissao").datepicker();

    $("#txtDataEntrada").mask("99/99/9999");
    $("#txtDataEmissao").mask("99/99/9999");

    $("#txtValorProdutos").priceFormat();
    $("#txtValorTotal").priceFormat();
    $("#txtBaseCalculoICMS").priceFormat();
    $("#txtValorTotalICMS").priceFormat();
    $("#txtValorTotalPIS").priceFormat();
    $("#txtValorTotalCOFINS").priceFormat();
    $("#txtValorTotalDesconto").priceFormat();
    $("#txtValorTotalOutrasDespesas").priceFormat();
    $("#txtValorTotalFrete").priceFormat();
    $("#txtBaseCalculoICMSST").priceFormat();
    $("#txtValorTotalICMSST").priceFormat();
    $("#txtValorTotalIPI").priceFormat();

    RemoveConsulta($("#txtFornecedor"), function ($this) {
        $this.val("");
        $("body").data("fornecedor", null);
    });

    RemoveConsulta($("#txtPlanoConta"), function ($this) {
        $this.val("");
        $("body").data("planoConta", null);
    });

    RemoveConsulta($("#txtVeiculo"), function ($this) {
        $this.val("");
        $("body").data("veiculo", null);
    });

    $("#txtNumero, #txtSerie").on('blur', function () {
        if ($.trim($(this).val()).length > 0)
            VerificaDuplicidade();
    });

    $("#selModeloDocumento").on('change', function () {
        if ($(this).val() > 0)
            VerificaDuplicidade();
    });

    $("#selEspecieDocumento").on('change', function () {
        AlterarEspecieDocumentoFiscal();
    });

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnCancelar").click(function () {
        LimparCamposDocumentoEntrada();
    });

    BuscarEspeciesDeDocumentosFiscais();

    BuscarModelosDeDocumentosFiscais();

    LimparCamposDocumentoEntrada();

    BuscarAliquotaPisCofins();
});

var AliquotaPIS = 0;
var AliquotaCOFINS = 0;
var CarregandoDados = false;
var DocumentoDuplicado = false;
var AvisoDocumentoDuplicado = "Já existe um documento de entrada com o mesmo número, série e modelo deste fornecedor!";

function BuscarAliquotaPisCofins() {
    AliquotaPIS = 0;
    AliquotaCOFINS = 0;

    executarRest("/DocumentoEntrada/ObterAliquotaPisCofinsEmpresa?callback=?", {}, function (r) {
        if (r.Sucesso) {
            if (!isNaN(r.Objeto.AliquotaPIS) && r.Objeto.AliquotaPIS != null)
                AliquotaPIS = r.Objeto.AliquotaPIS;
            if (!isNaN(r.Objeto.AliquotaCOFINS) && r.Objeto.AliquotaCOFINS != null)
                AliquotaCOFINS = r.Objeto.AliquotaCOFINS;
        };
    });
}

function RetornoConsultaDocumentoEntrada(documento) {
    executarRest("/DocumentoEntrada/ObterDetalhes?callback=?", { CodigoDocumento: documento.Codigo }, function (r) {
        if (r.Sucesso) {
            CarregandoDados = true;
            RenderizarDocumentoEntrada(r.Objeto);   
            CarregandoDados = false;
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function RenderizarDocumentoEntrada(documento) {    
    LimparCamposDocumentoEntrada();

    $("body").data("documentoEntrada", documento);

    $("#txtChave").val(documento.Chave);
    $("#txtDataEntrada").val(documento.DataEntrada);
    $("#txtDataEmissao").val(documento.DataEmissao);
    $("#txtValorProdutos").val(documento.ValorProdutos);
    $("#txtValorTotal").val(documento.ValorTotal);
    $("#txtBaseCalculoICMS").val(documento.BaseCalculoICMS);
    $("#txtValorTotalICMS").val(documento.ValorTotalICMS);
    $("#txtValorTotalPIS").val(documento.ValorTotalPIS);
    $("#txtValorTotalCOFINS").val(documento.ValorTotalCOFINS);
    $("#txtNumeroLancamento").val(documento.NumeroLancamento);
    $("#txtNumero").val(documento.Numero);
    $("#selEspecieDocumento").val(documento.SiglaEspecie).change();
    $("#selModeloDocumento").val(documento.CodigoModelo).change();
    $("#txtSerie").val(documento.Serie);
    $("#txtFornecedor").val(documento.CPFCNPJFornecedor + " - " + documento.NomeFornecedor);
    $("body").data("fornecedor", documento.CPFCNPJFornecedor);
    $("#selStatus").val(documento.Status);
    $("#txtPlanoConta").val(documento.DescricaoPlanoConta);
    $("body").data("planoConta", documento.CodigoPlanoConta);
    $("#selIndicadorPagamento").val(documento.IndicadorPagamento);
    $("#txtValorTotalDesconto").val(documento.ValorTotalDesconto);
    $("#txtValorTotalOutrasDespesas").val(documento.ValorTotalOutrasDespesas);
    $("#txtValorTotalFrete").val(documento.ValorTotalFrete);
    $("#txtBaseCalculoICMSST").val(documento.BaseCalculoICMSST);
    $("#txtValorTotalICMSST").val(documento.ValorTotalICMSST);
    $("#txtValorTotalIPI").val(documento.ValorTotalIPI);
    $("#txtVeiculo").val(documento.PlacaVeiculo);
    $("body").data("veiculo", documento.CodigoVeiculo);

    $("body").data("itens", documento.Itens);
    RenderizarItens();

    $("body").data("cobrancas", documento.Cobrancas);
    RenderizarCobrancas();

    $("body").data("abastecimentos", documento.Abastecimentos);
    RenderizarAbastecimento();

    if (documento.ParcelasPagas == true) {
        ControlarCampos(true);
        ExibirMensagemAlerta("Não é possível fazer alterações, duplicata deste documento de entrada possui parcelas pagas.", "Atenção!", "messages-placeholder");
    }
}

function AlterarEspecieDocumentoFiscal() {
    switch ($("#selEspecieDocumento").val()) {
        case "NFE":
            $(".nfe").removeClass("hidden");
            break;
        default:
            $(".nfe").addClass("hidden");
            break;
    }
}

function LimparCamposDocumentoEntrada() {
    $("body").data("documentoEntrada", null);

    $("#txtDataEntrada").val(Globalize.format(new Date(), "dd/MM/yyyy"));

    $("#txtValorProdutos").val('0,00');
    $("#txtValorTotal").val('0,00');
    $("#txtBaseCalculoICMS").val('0,00');
    $("#txtValorTotalICMS").val('0,00');
    $("#txtValorTotalPIS").val('0,00');
    $("#txtValorTotalCOFINS").val('0,00');
    $("#txtValorTotalDesconto").val('0,00');
    $("#txtValorTotalOutrasDespesas").val('0,00');
    $("#txtValorTotalFrete").val('0,00');
    $("#txtBaseCalculoICMSST").val('0,00');
    $("#txtValorTotalICMSST").val('0,00');
    $("#txtValorTotalIPI").val('0,00');
    $("#txtNumeroLancamento").val('Automático');
    $("#selEspecieDocumento").val($("#selEspecieDocumento option:first").val()).change();
    $("#selModeloDocumento").val($("#selModeloDocumento option:first").val()).change();
    $("#txtChave").val('');
    $("#txtDataEmissao").val('');
    $("#txtNumero").val('');
    $("#txtSerie").val('');
    $("#txtFornecedor").val('');
    $("body").data("fornecedor", null);
    $("#selStatus").val($("#selStatus option:first").val());
    $("#txtPlanoConta").val('');
    $("body").data("planoConta", null);
    $("#txtChave").val('');
    $("#selIndicadorPagamento").val($("#selIndicadorPagamento option:first").val());
    $("body").data("veiculo", null);
    $("#txtVeiculo").val("");

    LimparCamposItem();
    $("body").data("itens", null);
    RenderizarItens();

    LimparCamposCobranca();
    $("body").data("cobrancas", null);
    RenderizarCobrancas();

    LimparAbastecimentos();

    ControlarCampos(false);
    DocumentoDuplicado = false;
}

function BuscarEspeciesDeDocumentosFiscais() {
    executarRest("/EspecieDocumentoFiscal/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {

            var selEspecie = document.getElementById("selEspecieDocumento");

            selEspecie.length = 0;

            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '';
            selEspecie.options.add(optn);

            for (var i = 0; i < r.Objeto.length; i++) {
                optn = document.createElement("option");
                optn.text = r.Objeto[i].Sigla + " - " + r.Objeto[i].Descricao;
                optn.value = r.Objeto[i].Sigla;
                selEspecie.options.add(optn);
            }
        } else {
            ExibirMensagem(r.Erro, "Atenção!");
        }
    });
}

function BuscarModelosDeDocumentosFiscais() {
    executarRest("/ModeloDocumentoFiscal/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {

            var selModelo = document.getElementById("selModeloDocumento");

            selModelo.length = 0;

            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '0';
            selModelo.options.add(optn);

            for (var i = 0; i < r.Objeto.length; i++) {
                optn = document.createElement("option");
                optn.text = r.Objeto[i].Numero + " - " + r.Objeto[i].Descricao;
                optn.value = r.Objeto[i].Codigo;
                selModelo.options.add(optn);
            }
        } else {
            ExibirMensagem(r.Erro, "Atenção!");
        }
    });
}

function VerificaDuplicidade() {
    if (CarregandoDados) return;

    var dados = {
        Codigo: $("body").data("documentoEntrada") != null ? $("body").data("documentoEntrada").Codigo : 0,
        Numero: $("#txtNumero").val(),
        Serie: $("#txtSerie").val(),
        Modelo: $("#selModeloDocumento").val(),
        Fornecedor: $("body").data("fornecedor")
    };

    executarRest("/DocumentoEntrada/VerificaDuplicidade?callback=?", dados, function (r) {
        if (r.Sucesso && r.Objeto.DocumentoExiste) {
            ExibirMensagemAlerta(AvisoDocumentoDuplicado, "Duplicidade!");
            DocumentoDuplicado = true;
        } else {
            DocumentoDuplicado = false;
        }
    });
}

function RetornoConsultaFornecedor(cliente) {
    $("body").data("fornecedor", cliente.CPFCNPJ);
    $("#txtFornecedor").val(cliente.CPFCNPJ + " - " + cliente.Nome);

    VerificaDuplicidade();
}

function RetornoConsultaPlanoConta(plano) {
    $("#txtPlanoConta").val(plano.Conta + " - " + plano.Descricao);
    $("body").data("planoConta", plano.Codigo);
}

function RetornoConsultaVeiculos(veiculo) {
    $("body").data("veiculo", veiculo.Codigo);
    $("#txtVeiculo").val(veiculo.Placa);
}

function ValidarCampos() {
    var dataEntrada = $("#txtDataEntrada").val();
    var dataEmissao = $("#txtDataEmissao").val();
    var numero = Globalize.parseInt($("#txtNumero").val());
    var serie = $("#txtSerie").val();
    var especie = $("#selEspecieDocumento").val();
    var modelo = Globalize.parseInt($("#selModeloDocumento").val());
    var chave = $("#txtChave").val();
    var fornecedor = $("body").data("fornecedor");
    var valorTotal = Globalize.parseFloat($("#txtValorTotal").val());
    var planoConta = $("body").data("planoConta");
    var valido = true;

    if (dataEntrada == null || dataEntrada == "") {
        CampoComErro("#txtDataEntrada");
        valido = false;
    } else {
        CampoSemErro("#txtDataEntrada");
    }

    if (dataEmissao == null || dataEmissao == "") {
        CampoComErro("#txtDataEmissao");
        valido = false;
    } else {
        CampoSemErro("#txtDataEmissao");
    }

    if (dataEmissao == null || dataEmissao == "") {
        CampoComErro("#txtDataEmissao");
        valido = false;
    } else {
        CampoSemErro("#txtDataEmissao");
    }

    if (isNaN(numero) || numero <= 0) {
        CampoComErro("#txtNumero");
        valido = false;
    } else {
        CampoSemErro("#txtNumero");
    }

    if (especie == null || especie == "") {
        CampoComErro("#selEspecieDocumento");
        valido = false;
    } else {
        CampoSemErro("#selEspecieDocumento");
    }

    if (isNaN(modelo) || modelo <= 0) {
        CampoComErro("#selModeloDocumento");
        valido = false;
    } else {
        CampoSemErro("#selModeloDocumento");
    }

    if ($("#selEspecieDocumento").val() == "NFE") {
        if (chave == null || chave == "" || chave.length != 44) {
            CampoComErro("#txtChave");
            valido = false;
        } else {
            CampoSemErro("#txtChave");
        }
    }

    if (fornecedor == null || fornecedor == "") {
        CampoComErro("#txtFornecedor");
        valido = false;
    } else {
        CampoSemErro("#txtFornecedor");
    }

    if (isNaN(valorTotal) || valorTotal <= 0) {
        CampoComErro("#txtValorTotal");
        valido = false;
    } else {
        CampoSemErro("#txtValorTotal");
    }

    if (isNaN(planoConta) || planoConta <= 0) {
        CampoComErro("#txtPlanoConta");
        valido = false;
    } else {
        CampoSemErro("#txtPlanoConta");
    }

    if (!valido)
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!");

    return valido;
}

function ValidarTotalCobranca() {
    return true;
    //Retirado validação do JS, será mantido apenas no controller.    
}

function ValidarTotaisItens() {
    var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");

    var quantidadeItens = 0,
        totalItens = 0,
        totalICMSItens = 0,
        totalPISItens = 0,
        totalCOFINSItens = 0,
        totalDescontoItens = 0,
        totalOutrasDespesasItens = 0,
        totalFreteItens = 0,
        totalICMSSTItens = 0,
        totalIPIItens = 0,
        baseCalculoICMSItens = 0,
        baseCalculoICMSSTItens = 0,
        totalProdutos = Globalize.parseFloat($("#txtValorProdutos").val()),
        totalICMSDocumento = Globalize.parseFloat($("#txtValorTotalICMS").val()),
        totalPISDocumento = Globalize.parseFloat($("#txtValorTotalPIS").val()),
        totalCOFINSDocumento = Globalize.parseFloat($("#txtValorTotalCOFINS").val()),
        totalDescontoDocumento = Globalize.parseFloat($("#txtValorTotalDesconto").val()),
        totalOutrasDespesasDocumento = Globalize.parseFloat($("#txtValorTotalOutrasDespesas").val()),
        totalFreteDocumento = Globalize.parseFloat($("#txtValorTotalFrete").val()),
        totalICMSSTDocumento = Globalize.parseFloat($("#txtValorTotalICMSST").val()),
        totalIPIDocumento = Globalize.parseFloat($("#txtValorTotalIPI").val()),
        baseCalculoICMSSTDocumento = Globalize.parseFloat($("#txtBaseCalculoICMSST").val()),
        baseCalculoICMSDocumento = Globalize.parseFloat($("#txtBaseCalculoICMS").val());

    for (var i = 0; i < itens.length; i++) {
        if (!itens[i].Excluir) {
            totalItens += itens[i].ValorTotal;
            totalICMSItens += itens[i].ValorICMS;
            totalPISItens += itens[i].ValorPIS;
            totalCOFINSItens += itens[i].ValorCOFINS;
            totalDescontoItens += itens[i].Desconto;
            totalOutrasDespesasItens += itens[i].ValorOutrasDespesas;
            totalFreteItens += itens[i].ValorFrete;
            totalICMSSTItens += itens[i].ValorICMSST;
            totalIPIItens += itens[i].ValorIPI;
            baseCalculoICMSItens += itens[i].BaseCalculoICMS;
            baseCalculoICMSSTItens += itens[i].BaseCalculoICMSST;
            quantidadeItens = quantidadeItens + 1;
        }
    }

    var mensagem = "";

    if (quantidadeItens == 0)
        mensagem += "Não foi informado nenhum Item. <br/>";
    //if (totalItens.toFixed(2) != totalProdutos.toFixed(2))
    //    mensagem += "O valor total dos itens (" + Globalize.format(totalItens, "n2") + ") não é igual ao valor total dos produtos (" + Globalize.format(totalProdutos, "n2") + "). <br/>";

    if (totalICMSItens.toFixed(2) != totalICMSDocumento.toFixed(2))
        mensagem += "O valor total de ICMS dos itens (" + Globalize.format(totalICMSItens, "n2") + ") não é igual ao valor total de ICMS do documento (" + Globalize.format(totalICMSDocumento, "n2") + "). <br/>";

    if (totalPISItens.toFixed(2) != totalPISDocumento.toFixed(2))
        mensagem += "O valor total de PIS dos itens (" + Globalize.format(totalPISItens, "n2") + ") não é igual ao valor total de PIS do documento (" + Globalize.format(totalPISDocumento, "n2") + "). <br/>";

    if (totalCOFINSItens.toFixed(2) != totalCOFINSDocumento.toFixed(2))
        mensagem += "O valor total de COFINS dos itens (" + Globalize.format(totalCOFINSItens, "n2") + ") não é igual ao valor total de COFINS do documento (" + Globalize.format(totalCOFINSDocumento, "n2") + "). <br/>";

    if (totalICMSSTItens.toFixed(2) != totalICMSSTDocumento.toFixed(2))
        mensagem += "O valor total de ICMS ST dos itens (" + Globalize.format(totalICMSSTItens, "n2") + ") não é igual ao valor total de ICMS ST do documento (" + Globalize.format(totalICMSSTDocumento, "n2") + "). <br/>";

    if (totalIPIItens.toFixed(2) != totalIPIDocumento.toFixed(2))
        mensagem += "O valor total de IPI dos itens (" + Globalize.format(totalIPIItens, "n2") + ") não é igual ao valor total de IPI do documento (" + Globalize.format(totalIPIDocumento, "n2") + "). <br/>";

    if (totalDescontoItens.toFixed(2) != totalDescontoDocumento.toFixed(2))
        mensagem += "O valor total de Desconto dos itens (" + Globalize.format(totalDescontoItens, "n2") + ") não é igual ao valor total de Desconto do documento (" + Globalize.format(totalDescontoDocumento, "n2") + "). <br/>";

    if (totalOutrasDespesasItens.toFixed(2) != totalOutrasDespesasDocumento.toFixed(2))
        mensagem += "O valor total de Outras Despesas dos itens (" + Globalize.format(totalOutrasDespesasItens, "n2") + ") não é igual ao valor total de Outras Despesas do documento (" + Globalize.format(totalOutrasDespesasDocumento, "n2") + "). <br/>";

    if (totalFreteItens.toFixed(2) != totalFreteDocumento.toFixed(2))
        mensagem += "O valor total de Frete dos itens (" + Globalize.format(totalFreteItens, "n2") + ") não é igual ao valor total de Frete do documento (" + Globalize.format(totalFreteDocumento, "n2") + "). <br/>";

    if (baseCalculoICMSSTItens.toFixed(2) != baseCalculoICMSSTDocumento.toFixed(2))
        mensagem += "O valor total de B. C. do ICMS ST dos itens (" + Globalize.format(baseCalculoICMSSTItens, "n2") + ") não é igual ao valor total de B. C. do ICMS ST do documento (" + Globalize.format(baseCalculoICMSSTDocumento, "n2") + "). <br/>";

    if (baseCalculoICMSItens.toFixed(2) != baseCalculoICMSDocumento.toFixed(2))
        mensagem += "O valor total de B. C. do ICMS dos itens (" + Globalize.format(baseCalculoICMSItens, "n2") + ") não é igual ao valor total de B. C. do ICMS do documento (" + Globalize.format(baseCalculoICMSDocumento, "n2") + "). <br/>";

    if (mensagem != "") {
        ExibirMensagemAlerta("<br/>" + mensagem, "Atenção!");
        return false;
    } else {
        return true;
    }
}

function ValidarDuplicidade() {
    if (DocumentoDuplicado)
        ExibirMensagemAlerta(AvisoDocumentoDuplicado, "Duplicidade!");

    return !DocumentoDuplicado;
}

function ValidarItens() {
    var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");

    var valido = true;
    var mensagem = "<br/>";

    for (var i = 0; i < itens.length; i++) {
        if (!itens[i].Excluir) {
            if (itens[i].CodigoProduto <= 0) {
                mensagem += "O produto do item (sequencial: " + itens[i].Sequencial.toString() + ") não pode ser nulo.<br/>";
                valido = false;
            }
            if (itens[i].CodigoCFOP <= 0) {
                mensagem += "A CFOP do item (sequencial: " + itens[i].Sequencial.toString() + ") não pode ser nula.<br/>";
                valido = false;
            }
        }
    }

    if (!valido) {
        ExibirMensagemAlerta(mensagem, "Atenção!");
        return false;
    } else {
        return true;
    }
}

function Salvar() {
    if (!ValidarCampos() || !ValidarTotalCobranca() || !ValidarTotaisItens() || !ValidarItens() || !ValidarDuplicidade())
        return;

    var documento = {
        Codigo: $("body").data("documentoEntrada") != null ? $("body").data("documentoEntrada").Codigo : 0,
        DataEntrada: $("#txtDataEntrada").val(),
        DataEmissao: $("#txtDataEmissao").val(),
        Numero: $("#txtNumero").val(),
        Serie: $("#txtSerie").val(),
        Especie: $("#selEspecieDocumento").val(),
        Modelo: $("#selModeloDocumento").val(),
        Chave: $("#txtChave").val(),
        Fornecedor: $("body").data("fornecedor"),
        ValorProdutos: $("#txtValorProdutos").val(),
        ValorTotal: $("#txtValorTotal").val(),
        BaseCalculoICMS: $("#txtBaseCalculoICMS").val(),
        ValorTotalICMS: $("#txtValorTotalICMS").val(),
        ValorTotalPIS: $("#txtValorTotalPIS").val(),
        ValorTotalCOFINS: $("#txtValorTotalCOFINS").val(),
        PlanoConta: $("body").data("planoConta"),
        ValorTotalDesconto: $("#txtValorTotalDesconto").val(),
        ValorTotalOutrasDespesas: $("#txtValorTotalOutrasDespesas").val(),
        ValorTotalFrete: $("#txtValorTotalFrete").val(),
        BaseCalculoICMSST: $("#txtBaseCalculoICMSST").val(),
        ValorTotalICMSST: $("#txtValorTotalICMSST").val(),
        ValorTotalIPI: $("#txtValorTotalIPI").val(),
        IndicadorPagamento: $("#selIndicadorPagamento").val(),
        Status: $("#selStatus").val(),
        Veiculo: $("body").data("veiculo"),
        Itens: JSON.stringify($("body").data("itens")),
        Cobrancas: JSON.stringify($("body").data("cobrancas")),
        Abastecimentos: JSON.stringify($("body").data("abastecimentos"))
    };

    executarRest("/DocumentoEntrada/Salvar?callback=?", documento, function (r) {
        if (r.Sucesso) {
            if (r.Objeto) {
                ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
                LimparCamposDocumentoEntrada();
            }
            else
                ExibirMensagemAlerta(r.Erro, "Atenção!");
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function ControlarCampos(desabilitar) {
    document.getElementById("txtDataEntrada").disabled = desabilitar;
    document.getElementById("txtFornecedor").disabled = desabilitar;
    document.getElementById("txtValorProdutos").disabled = desabilitar;
    document.getElementById("txtValorTotal").disabled = desabilitar;
    document.getElementById("txtBaseCalculoICMS").disabled = desabilitar;
    document.getElementById("txtValorTotalICMS").disabled = desabilitar;
    document.getElementById("txtValorTotalPIS").disabled = desabilitar;
    document.getElementById("txtValorTotalCOFINS").disabled = desabilitar;
    document.getElementById("txtValorTotalDesconto").disabled = desabilitar;
    document.getElementById("txtValorTotalOutrasDespesas").disabled = desabilitar;
    document.getElementById("txtValorTotalFrete").disabled = desabilitar;
    document.getElementById("txtBaseCalculoICMSST").disabled = desabilitar;
    document.getElementById("txtValorTotalICMSST").disabled = desabilitar;
    document.getElementById("txtValorTotalIPI").disabled = desabilitar;
    document.getElementById("selEspecieDocumento").disabled = desabilitar;
    document.getElementById("selModeloDocumento").disabled = desabilitar;
    document.getElementById("txtChave").disabled = desabilitar;
    document.getElementById("txtDataEmissao").disabled = desabilitar;
    document.getElementById("txtNumero").disabled = desabilitar;
    document.getElementById("txtSerie").disabled = desabilitar;
    document.getElementById("selStatus").disabled = desabilitar;
    
    document.getElementById("txtPlanoConta").disabled = desabilitar;
    document.getElementById("selIndicadorPagamento").disabled = desabilitar;
    document.getElementById("txtVeiculo").disabled = desabilitar;

    document.getElementById("btnSalvar").disabled = desabilitar;
    document.getElementById("btnImportarNotaFiscal").disabled = desabilitar;

    document.getElementById("btnSalvarItem").disabled = desabilitar;
    document.getElementById("btnExcluirItem").disabled = desabilitar;

    document.getElementById("btnGerarDuplicatasAutomaticamente").disabled = desabilitar;
    document.getElementById("btnSalvarDuplicata").disabled = desabilitar;
    document.getElementById("btnExcluirDuplicata").disabled = desabilitar; 
}