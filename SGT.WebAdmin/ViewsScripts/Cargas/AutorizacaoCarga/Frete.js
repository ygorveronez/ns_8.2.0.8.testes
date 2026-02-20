/// <reference path="../../Cargas/Carga/Frete/ComposicaoFrete.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CodigoCargaDetalhamentoFrete;

/*
 * Declaração das Funções de Inicialização
 */

function loadFrete() {
    loadComposicaoFrete();
}

/*
 * Declaração das Funções Públicas
 */

function verificarFrete(codigoCarga) {
    executarReST("CargaFrete/VerificarFrete", { Codigo: codigoCarga }, function (retorno) {
        if (retorno.Success) {
            _CodigoCargaDetalhamentoFrete = codigoCarga;

            resetarTabsDetalheFrete();
            preencherDetalhesFrete(retorno.Data);
                        
            Global.abrirModal('divModalDetalheValorFrete');
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Privadas
 */

function isPreencherComposicaoFrete(retornoFrete) {
    return (
        (retornoFrete.ComposicaoFreteCarga != null && retornoFrete.ComposicaoFreteCarga.length > 0) ||
        (retornoFrete.ComposicaoFretePedido != null && retornoFrete.ComposicaoFretePedido.length > 0) ||
        (
            (retornoFrete.ComposicaoFreteDocumento != null) && (
                (retornoFrete.ComposicaoFreteDocumento.NotasFiscais != null && retornoFrete.ComposicaoFreteDocumento.NotasFiscais.length > 0) ||
                (retornoFrete.ComposicaoFreteDocumento.CTesParaSubcontratacao != null && retornoFrete.ComposicaoFreteDocumento.CTesParaSubcontratacao.length > 0)
            )
        )
    );
}

function preencherDetalhesFrete(retornoFrete) {
    var header = [
        { data: "Descricao", title: "Descrição", width: "75%" },
        { data: "Valor", title: "Valor", width: "25%", className: "text-align-right" }
    ];
    var valorTotalPrestacao = retornoFrete.valorFreteAPagar;
    var dataFrete = new Array();
    var frete = { "Descricao": "Valor do Frete", "Valor": Globalize.format(retornoFrete.valorFrete, "n2") };

    dataFrete.push(frete);

    if (retornoFrete.componentesFrete != null) {
        $.each(retornoFrete.componentesFrete, function (i, componente) {
            dataFrete.push({ "Descricao": componente.Descricao, "Valor": Globalize.format(componente.Valor, "n2") });
        });
    }
    $("#spanDetalheFreteTotalPrestacao").text(Globalize.format(valorTotalPrestacao, "n2"));
    $("#contentDetalheValorFrete").html("<table width='100%' class='table table-bordered table-hover' cellspacing='0'></table>");
    var gridDetalhesValor = new BasicDataTable("contentDetalheValorFrete table", header, null);
    gridDetalhesValor.CarregarGrid(dataFrete);

    if (isPreencherComposicaoFrete(retornoFrete))
        PreencherComposicaoFrete(retornoFrete, false);
    else
        $("#liComposicaoFrete").hide();

    if (retornoFrete.DadosFreteFilialEmissora != null)
        preencherDetalhesFreteFilialEmissora(retornoFrete.DadosFreteFilialEmissora);
    else {
        $("#liDetalhesFreteFilialEmissora").hide();
        $("#liComposicaoFreteFilialEmissora").hide();
    }
}

function preencherDetalhesFreteFilialEmissora(retornoFrete) {
    var header = [
        { data: "Descricao", title: "Descrição", width: "75%" },
        { data: "Valor", title: "Valor", width: "25%", className: "text-align-right" }
    ];
    var valorTotalPrestacao = retornoFrete.valorFreteAPagar;
    var dataFrete = new Array();
    var frete = { "Descricao": "Valor do Frete", "Valor": Globalize.format(retornoFrete.valorFrete, "n2") };

    dataFrete.push(frete);

    if (retornoFrete.componentesFrete != null) {
        $.each(retornoFrete.componentesFrete, function (i, componente) {
            dataFrete.push({ "Descricao": componente.Descricao, "Valor": Globalize.format(componente.Valor, "n2") });
        });
    }

    $("#spanDetalheFreteTotalPrestacaoFilialEmissora").text(Globalize.format(valorTotalPrestacao, "n2"));
    $("#contentDetalheValorFreteFilialEmissora").html("<table width='100%' class='table table-bordered table-hover' cellspacing='0'></table>");
    var gridDetalhesValor = new BasicDataTable("contentDetalheValorFreteFilialEmissora table", header, null);
    gridDetalhesValor.CarregarGrid(dataFrete);

    $("#liDetalhesFreteFilialEmissora").show();

    if (isPreencherComposicaoFrete(retornoFrete))
        PreencherComposicaoFrete(retornoFrete, true);
    else
        $("#liComposicaoFreteFilialEmissora").hide();
}

function resetarTabsDetalheFrete() {
    $("#liDetalhesFreteFilialEmissora").hide();
    $("#liComposicaoFreteFilialEmissora").hide();

    $("#liDetalhesFreteFilialEmissora").removeClass("active");
    $("#liComposicaoFreteFilialEmissora").removeClass("active");
    $("#liComposicaoFrete").removeClass("active");

    $("#divDetalhesFreteFilialEmissora").attr("class", "tab-pane fade");
    $("#divComposicaoFrete").attr("class", "tab-pane fade");
    $("#divComposicaoFreteFilialEmissora").attr("class", "tab-pane fade");

    $("#liDetalheFrete").attr("class", "active");
    $("#divDetalheFrete").attr("class", "tab-pane active in");
}