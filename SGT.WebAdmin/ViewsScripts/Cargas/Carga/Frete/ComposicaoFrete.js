/*
 * Declaração de Objetos Globais do Arquivo
 */

var _detalheComposicaoFreteCargaGeral
var _detalheComposicaoFreteCargaGeralFilialEmissora;

/*
 * Declaração das Classes
 */

var DetalheComposicaoFreteCargaGeral = function () {
    this.ComposicoesCarga = ko.observableArray([]);
    this.ComposicoesStages = ko.observableArray([]);
    this.ComposicoesPedidos = ko.observableArray([]);
    this.ComposicoesNotasFiscais = ko.observableArray([]);
    this.ComposicoesCTesSubcontratacao = ko.observableArray([]);
    this.ComposicoesFreteCargaSubTrecho = ko.observableArray([]);
};

var DetalheComposicaoFreteModel = function (composicao) {
    this.Codigo = PropertyEntity({ val: guid() });
    this.CodigoCargaEmbarcador = PropertyEntity({ val: composicao.CodigoCargaEmbarcador, text: Localization.Resources.Cargas.Carga.NumeroCarga.getFieldDescription() });
    this.Formula = PropertyEntity({ val: composicao.Formula, text: Localization.Resources.Cargas.Carga.Formula.getFieldDescription() });
    this.ValoresFormula = PropertyEntity({ val: composicao.ValoresFormula, text: Localization.Resources.Cargas.Carga.AplicacaoDaFormula.getFieldDescription() });
    this.Valor = PropertyEntity({ val: composicao.Valor, text: Localization.Resources.Cargas.Carga.TaxaDoElemento.getFieldDescription() });
    this.Descricao = PropertyEntity({ val: composicao.Descricao, text: Localization.Resources.Cargas.Carga.ElementoDeCusto.getFieldDescription() });
    this.DescricaoTipoCampoValor = PropertyEntity({ val: composicao.DescricaoTipoCampoValor });
    this.ValorCalculado = PropertyEntity({ val: composicao.ValorCalculado, text: Localization.Resources.Cargas.Carga.ValorCalculado.getFieldDescription() });

    this.CodigoTabela = PropertyEntity({ val: composicao.CodigoTabela, text: Localization.Resources.Cargas.Carga.CodigoTabela.getFieldDescription() });
    this.Origem = PropertyEntity({ val: composicao.Origem, text: Localization.Resources.Cargas.Carga.Origem.getFieldDescription() });
    this.Destino = PropertyEntity({ val: composicao.Destino, text: Localization.Resources.Cargas.Carga.Destino.getFieldDescription() });
    this.Cliente = PropertyEntity({ val: composicao.Cliente, text: Localization.Resources.Cargas.Carga.Cliente.getFieldDescription() });
    this.Remetente = PropertyEntity({ val: composicao.Remetente, text: Localization.Resources.Cargas.Carga.Remetente.getFieldDescription() });
};

var DetalheComposicaoFreteDetalhadoModel = function (dados, composicoes) {
    $this = this;

    $this.Numero = PropertyEntity({ val: dados.Numero, text: Localization.Resources.Cargas.Carga.NumerosParenteses.getFieldDescription() });
    $this.Codigo = PropertyEntity({ val: guid() });
    $this.Composicoes = ko.observableArray();

    for (var i = 0; i < composicoes.length; i++)
        $this.Composicoes.push(new DetalheComposicaoFreteModel(composicoes[i]));
};


/*
 * Declaração das Funções de Inicialização
 */

function loadComposicaoFrete() {
    $.get("Content/Static/Carga/ComposicaoFrete.html?dyn=" + guid(), function (data) {
        $("#contentDetalheComposicaoFrete").html(data.replace(/#knoutComposicaoFrete/g, ''));
        $("#contentDetalheComposicaoFreteFilialEmissora").html(data.replace(/#knoutComposicaoFrete/g, 'FilialEmissora'));
    });
}

/*
 * Declaração das Funções Públicas
 */

function DownloadComposicaoFrete(isFilialEmissora) {
    executarDownload("CargaFrete/DownloadDetalhamentoFrete", { Carga: _CodigoCargaDetalhamentoFrete, FilialEmissora: isFilialEmissora });
}

function RecalcularFreteCargasConsolidado() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "O sistema irá recalcular o frete para cargas ainda sem emissao do CT-e, deseja continuar?", function () {
        executarReST("CargaFrete/RecriarFreteRacionalCargasConsolidado", { Carga: _CodigoCargaDetalhamentoFrete }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.Success, "Sucesso, Cargas em calculo de frete.", r.Msg);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function PreencherComposicaoFrete(dados, isFilialEmissora) {

    var idDivAppend = isFilialEmissora ? "FilialEmissora" : "";
    var detalheComposicaoFrete = obterDetalheComposicaoFrete(isFilialEmissora);
    
    if (dados.ComposicaoFreteDocumento != null && dados.ComposicaoFreteDocumento.CTesParaSubcontratacao != null && dados.ComposicaoFreteDocumento.CTesParaSubcontratacao.length > 0) {
        for (var i = 0; i < dados.ComposicaoFreteDocumento.CTesParaSubcontratacao.length; i++) {
            var composicaoFreteCTeSubcontratacao = dados.ComposicaoFreteDocumento.CTesParaSubcontratacao[i];

            detalheComposicaoFrete.ComposicoesCTesSubcontratacao.push(new DetalheComposicaoFreteDetalhadoModel(composicaoFreteCTeSubcontratacao, composicaoFreteCTeSubcontratacao.ComposicaoFrete));
        }

        var tab = bootstrap.Tab.getOrCreateInstance(document.querySelector('a[href="#tabDetalheComposicaoFreteCTeSubcontratacao' + idDivAppend + '"]'));
        tab.show();
    }

    if (dados.ComposicaoFreteDocumento != null && dados.ComposicaoFreteDocumento.NotasFiscais != null && dados.ComposicaoFreteDocumento.NotasFiscais.length > 0) {
        for (var i = 0; i < dados.ComposicaoFreteDocumento.NotasFiscais.length; i++) {
            var composicaoFreteNotaFiscal = dados.ComposicaoFreteDocumento.NotasFiscais[i];

            detalheComposicaoFrete.ComposicoesNotasFiscais.push(new DetalheComposicaoFreteDetalhadoModel(composicaoFreteNotaFiscal, composicaoFreteNotaFiscal.ComposicaoFrete));
        }

        var tab = bootstrap.Tab.getOrCreateInstance(document.querySelector('a[href="#tabDetalheComposicaoFreteNotaFiscal' + idDivAppend + '"]'));
        tab.show();
    }

    if (dados.ComposicaoFreteStage != null && dados.ComposicaoFreteStage.length > 0) {
        for (var i = 0; i < dados.ComposicaoFreteStage.length; i++) {
            var composicaoFreteStage = dados.ComposicaoFreteStage[i];
            detalheComposicaoFrete.ComposicoesStages.push(new DetalheComposicaoFreteDetalhadoModel(composicaoFreteStage, composicaoFreteStage.ComposicaoFrete));
        }

        var tab = bootstrap.Tab.getOrCreateInstance(document.querySelector('a[href="#tabDetalheComposicaoFreteStage' + idDivAppend + '"]'));
        tab.show();
    }

    if (dados.ComposicaoFretePedido != null && dados.ComposicaoFretePedido.length > 0) {
        for (var i = 0; i < dados.ComposicaoFretePedido.length; i++) {
            var composicaoFretePedido = dados.ComposicaoFretePedido[i];
            detalheComposicaoFrete.ComposicoesPedidos.push(new DetalheComposicaoFreteDetalhadoModel(composicaoFretePedido, composicaoFretePedido.ComposicaoFrete));
        }

        var tab = bootstrap.Tab.getOrCreateInstance(document.querySelector('a[href="#tabDetalheComposicaoFretePedido' + idDivAppend + '"]'));
        tab.show();
    }

    if (dados.ComposicaoFreteCarga != null && dados.ComposicaoFreteCarga.length > 0) {
        for (var i = 0; i < dados.ComposicaoFreteCarga.length; i++)
            detalheComposicaoFrete.ComposicoesCarga.push(new DetalheComposicaoFreteModel(dados.ComposicaoFreteCarga[i]));

        var tab = bootstrap.Tab.getOrCreateInstance(document.querySelector('a[href="#tabDetalheComposicaoFreteCarga' + idDivAppend + '"]'));
        tab.show();
    }

    if (dados.ComposicaoFreteCargaSubTrecho != null && dados.ComposicaoFreteCargaSubTrecho.CargasSubTrecho != null && dados.ComposicaoFreteCargaSubTrecho.CargasSubTrecho.length > 0) {
        for (var i = 0; i < dados.ComposicaoFreteCargaSubTrecho.CargasSubTrecho.length; i++) {
            var composicaoFreteCargaSubTrecho = dados.ComposicaoFreteCargaSubTrecho.CargasSubTrecho[i];

            detalheComposicaoFrete.ComposicoesFreteCargaSubTrecho.push(new DetalheComposicaoFreteDetalhadoModel(composicaoFreteCargaSubTrecho, composicaoFreteCargaSubTrecho.ComposicaoFrete));
        }

        var tab = bootstrap.Tab.getOrCreateInstance(document.querySelector('a[href="#tabDetalheComposicaoFreteComposicaoFreteCargaSubTrecho' + idDivAppend + '"]'));
        tab.show();

        $("#btnRecalcularFreteCargasRacionalConsolidado" + idDivAppend).unbind();
        $("#btnRecalcularFreteCargasRacionalConsolidado" + idDivAppend).show();
        $("#btnRecalcularFreteCargasRacionalConsolidado" + idDivAppend).click(function () {
            RecalcularFreteCargasConsolidado();
        });
    }

    //if (dados.ComposicaoFreteCargaSubTrecho != null && dados.ComposicaoFreteCargaSubTrecho.Pedidos != null && dados.ComposicaoFreteCargaSubTrecho.Pedidos.length > 0) {
    //    for (var i = 0; i < dados.ComposicaoFreteCargaSubTrecho.Pedidos.length; i++) {
    //        var composicaoFreteCargaPedidos = dados.ComposicaoFreteCargaSubTrecho.Pedidos[i];

    //        detalheComposicaoFrete.ComposicoesFreteCargaSubTrecho.push(new DetalheComposicaoFreteDetalhadoModel(composicaoFreteCargaPedidos, composicaoFreteCargaPedidos.ComposicaoFrete));
    //    }

    //    var tab = bootstrap.Tab.getOrCreateInstance(document.querySelector('a[href="#tabDetalheComposicaoFreteComposicaoFreteCargaSubTrecho' + idDivAppend + '"]'));
    //    tab.show();
    //}

    $("#liComposicaoFrete" + idDivAppend).show();

    $("#btnDownloadCargaComposicaoFrete" + idDivAppend).unbind();
    $("#btnDownloadCargaComposicaoFrete" + idDivAppend).click(function () {
        DownloadComposicaoFrete(isFilialEmissora);
    });
}

/*
 * Declaração das Funções Privadas
 */

function limparDetalheComposicaoFrete(detalheComposicaoFrete) {
    detalheComposicaoFrete.ComposicoesCarga.removeAll();
    detalheComposicaoFrete.ComposicoesPedidos.removeAll();
    detalheComposicaoFrete.ComposicoesStages.removeAll();
    detalheComposicaoFrete.ComposicoesNotasFiscais.removeAll();
    detalheComposicaoFrete.ComposicoesCTesSubcontratacao.removeAll();
    detalheComposicaoFrete.ComposicoesFreteCargaSubTrecho.removeAll();
}

function obterDetalheComposicaoFrete(isFilialEmissora) {
    return isFilialEmissora ? obterDetalheComposicaoFreteGeralFilialEmissora() : obterDetalheComposicaoFreteGeral();
}

function obterDetalheComposicaoFreteGeral() {
    if (_detalheComposicaoFreteCargaGeral)
        limparDetalheComposicaoFrete(_detalheComposicaoFreteCargaGeral);
    else {
        _detalheComposicaoFreteCargaGeral = new DetalheComposicaoFreteCargaGeral();
        KoBindings(_detalheComposicaoFreteCargaGeral, "contentDetalheComposicaoFrete");

        LocalizeCurrentPage();
    }

    return _detalheComposicaoFreteCargaGeral;
}

function obterDetalheComposicaoFreteGeralFilialEmissora() {
    if (_detalheComposicaoFreteCargaGeralFilialEmissora)
        limparDetalheComposicaoFrete(_detalheComposicaoFreteCargaGeralFilialEmissora);
    else {
        _detalheComposicaoFreteCargaGeralFilialEmissora = new DetalheComposicaoFreteCargaGeral();
        KoBindings(_detalheComposicaoFreteCargaGeralFilialEmissora, "contentDetalheComposicaoFreteFilialEmissora");

        LocalizeCurrentPage();
    }

    return _detalheComposicaoFreteCargaGeralFilialEmissora;
}
