/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Auditoria.js"/>
/// <reference path="ControleEntrega.js" />

/*
 * Declaração das Classes
 */

var PedidoProduto = function (pedidoProduto) {
    this.Codigo = PropertyEntity({ val: ko.observable(pedidoProduto.Codigo), getType: typesKnockout.int, def: pedidoProduto.Codigo });
    this.CodigoProduto = PropertyEntity({ val: ko.observable(pedidoProduto.CodigoProduto), getType: typesKnockout.int, def: pedidoProduto.CodigoProduto });
    this.PossuiCheckList = PropertyEntity({ val: ko.observable(pedidoProduto.PossuiCheckList), getType: typesKnockout.bool, def: pedidoProduto.PossuiCheckList });
    this.NumeroPedido = PropertyEntity({ val: ko.observable(pedidoProduto.NumeroPedido) });
    this.ProdutoDescricao = PropertyEntity({ val: ko.observable(pedidoProduto.ProdutoDescricao) });
    this.Quantidade = PropertyEntity({ val: ko.observable(pedidoProduto.Quantidade), getType: typesKnockout.decimal, enable: ko.observable(true), def: pedidoProduto.Quantidade, maxlength: 18, text: Localization.Resources.Cargas.ControleEntrega.Quantidade.getFieldDescription() });
    this.QuantidadePlanejada = PropertyEntity({ val: ko.observable(pedidoProduto.QuantidadePlanejada), getType: typesKnockout.decimal, enable: ko.observable(true), def: pedidoProduto.QuantidadePlanejada, maxlength: 18, text: Localization.Resources.Cargas.ControleEntrega.QuantidadePlanejada.getFieldDescription() });

    this.QuantidadeCaixa = PropertyEntity({ val: ko.observable(pedidoProduto.QuantidadeCaixa), getType: typesKnockout.decimal, enable: ko.observable(true), def: pedidoProduto.QuantidadeCaixa, maxlength: 18, text: Localization.Resources.Cargas.ControleEntrega.QuantidadePorCaixa.getFieldDescription() });
    this.QuantidadePorCaixaRealizada = PropertyEntity({ val: ko.observable(pedidoProduto.QuantidadePorCaixaRealizada), getType: typesKnockout.decimal, enable: ko.observable(true), def: pedidoProduto.QuantidadePorCaixaRealizada, maxlength: 18, text: Localization.Resources.Cargas.ControleEntrega.QuantidadePorCaixaRealizada.getFieldDescription() });

    this.QuantidadeCaixasVazias = PropertyEntity({ val: ko.observable(pedidoProduto.QuantidadeCaixasVazias), getType: typesKnockout.decimal, enable: ko.observable(true), def: pedidoProduto.QuantidadeCaixasVazias, maxlength: 18, text: Localization.Resources.Cargas.ControleEntrega.QuantidadePlanejadaDeCaixasVazias.getFieldDescription() });
    this.QuantidadeCaixasVaziasRealizada = PropertyEntity({ val: ko.observable(pedidoProduto.QuantidadeCaixasVaziasRealizada), getType: typesKnockout.decimal, enable: ko.observable(true), def: pedidoProduto.QuantidadeCaixasVaziasRealizada, maxlength: 18, text: Localization.Resources.Cargas.ControleEntrega.QuantidadesDeCaixasVazias.getFieldDescription() });

    this.ImunoPlanejado = PropertyEntity({ val: ko.observable(pedidoProduto.ImunoPlanejado), getType: typesKnockout.int, enable: ko.observable(true), def: pedidoProduto.Imuno, text: Localization.Resources.Cargas.ControleEntrega.ImunoPlanejado.getFieldDescription(), visible: ko.observable(pedidoProduto.ImunoPlanejado != null) });
    this.ImunoRealizado = PropertyEntity({ val: ko.observable(pedidoProduto.ImunoRealizado), getType: typesKnockout.int, enable: ko.observable(true), def: pedidoProduto.Imuno, text: Localization.Resources.Cargas.ControleEntrega.ImunoRealizado.getFieldDescription(), visible: ko.observable(pedidoProduto.ImunoRealizado != null) });
    this.Temperatura = PropertyEntity({ val: ko.observable(pedidoProduto.Temperatura), getType: typesKnockout.decimal, visible: ko.observable(pedidoProduto.ObrigatorioInformarTemperatura), configDecimal: { precision: 2, allowZero: false, allowNegative: true }, def: pedidoProduto.Temperatura, maxlength: 18, text: Localization.Resources.Cargas.ControleEntrega.Temperatura.getFieldDescription() });
    this.JustificativaTemperatura = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.JustificativaDeTemperatura.getFieldDescription(), val: ko.observable(pedidoProduto.JustificativaTemperatura), visible: ko.observable(pedidoProduto.JustificativaTemperatura != ""), def: "" });

    this.DivisoesCapacidade = ko.observableArray();

    //Guarda os mesmos dados do DivisoesCapacidade, mas dividido em Pisos
    this.PisosDivisoesCapacidade = ko.observableArray();

    this.ControlarExibicao = PropertyEntity({ eventClick: controlarExibicaoPedidoProdutoClick, type: types.event });
    this.Auditar = PropertyEntity({ eventClick: auditarPedidoProdutoClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Auditar, visible: ko.observable(true) });
}

var PedidoProdutoDivisaoCapacidade = function (divisaoCapacidade) {
    this.Codigo = PropertyEntity({ val: ko.observable(divisaoCapacidade.Codigo), getType: typesKnockout.int, def: divisaoCapacidade.Codigo });
    this.Quantidade = PropertyEntity({ val: ko.observable(divisaoCapacidade.Quantidade), getType: typesKnockout.decimal, def: "", maxlength: 18, text: divisaoCapacidade.Descricao + ":" });
    this.QuantidadePlanejada = PropertyEntity({ val: ko.observable(divisaoCapacidade.QuantidadePlanejada), def: "" });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function controlarExibicaoPedidoProdutoClick(pedidoProduto) {
    var listaPedidoProduto = $(".controle-entrega-produto-container");

    for (var i = 0; i < listaPedidoProduto.length; i++) {
        var $pedidoProduto = $(listaPedidoProduto[i]);

        if ($pedidoProduto.data("codigo-pedido-produto") == pedidoProduto.Codigo.val())
            $pedidoProduto.toggleClass("controle-entrega-produto-container-ativo");
        else
            $pedidoProduto.removeClass("controle-entrega-produto-container-ativo");
    }
}

/*
 * Declaração das Funções Públicas
 */

function atualizarListaPedidoProduto(entrega) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.DesejaRealmenteAtualizarOsProdutos, function () {
        var dados = {
            Codigo: entrega.Codigo.val(),
            Produtos: JSON.stringify(obterListaPedidoProduto(entrega))
        }

        executarReST("ControleEntregaEntrega/AtualizarProdutos", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.ProdutosAtualizadosComSucesso);
                    atualizarControleEntrega();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

function limparListaPedidoProduto() {
    _entrega.ListaPedidoProduto.removeAll();
}

function preencherListaPedidoProduto(produtos) {
    for (var i = 0; i < produtos.length; i++) {
        var produto = produtos[i];
        var pedidoProduto = new PedidoProduto(produto.Produto);

        _entrega.ListaPedidoProduto.push(pedidoProduto);

        HeaderAuditoria("CargaPedidoProduto", pedidoProduto);

        $("#" + pedidoProduto.Quantidade.id).maskMoney(pedidoProduto.Quantidade.configDecimal);
        $("#" + pedidoProduto.Temperatura.id).maskMoney(pedidoProduto.Temperatura.configDecimal);

        if (produto.DivisoesCapacidade != undefined && produto.DivisoesCapacidade.length > 0) {
            preencherDivisoesCapacidade(pedidoProduto, produto.DivisoesCapacidade);
        }

        if (produto.Produto.PossuiCheckList) {
            possuiCheckList = true;
        }
    }
}

/*
 * Declaração das Funções Privadas
 */

function obterListaDivisaoCapacidade(pedidoProduto) {
    var listaDivisaoCapacidade = pedidoProduto.DivisoesCapacidade().slice();
    var listaDivisaoCapacidadeRetornar = new Array();

    for (var i = 0; i < listaDivisaoCapacidade.length; i++) {
        var divisaoCapacidade = listaDivisaoCapacidade[i];

        listaDivisaoCapacidadeRetornar.push({
            Codigo: divisaoCapacidade.Codigo.val(),
            Quantidade: divisaoCapacidade.Quantidade.val(),
            QuantidadePlanejada: divisaoCapacidade.QuantidadePlanejada.val()
        });
    }

    return listaDivisaoCapacidadeRetornar;
}

function obterListaPedidoProduto(entrega) {
    var listaPedidoProduto = entrega.ListaPedidoProduto().slice();
    var listaPedidoProdutoRetornar = new Array();

    for (var i = 0; i < listaPedidoProduto.length; i++) {
        var pedidoProduto = listaPedidoProduto[i];

        listaPedidoProdutoRetornar.push({
            Codigo: pedidoProduto.Codigo.val(),
            Quantidade: pedidoProduto.Quantidade.val(),
            QuantidadeCaixa: pedidoProduto.QuantidadeCaixa.val(),
            QuantidadeCaixasVaziasRealizada: pedidoProduto.QuantidadeCaixasVaziasRealizada.val(),
            Temperatura: pedidoProduto.Temperatura.val(),
            DivisoesCapacidade: obterListaDivisaoCapacidade(pedidoProduto)
        });
    }

    return listaPedidoProdutoRetornar;
}

var PisosDivisoesCapacidade = function () {
    this.DivisoesCapacidade = ko.observableArray();
    this.NumColunas = PropertyEntity({ val: 0, getType: typesKnockout.int });
}

function preencherDivisoesCapacidade(pedidoProduto, divisoesCapacidade) {
    const numPisos = divisoesCapacidade.map(d => d.Piso ? d.Piso : 0).reduce((max, val) => max > val ? max : val, 1);
    const numColunas = divisoesCapacidade.map(d => d.Coluna ? d.Coluna : 0).reduce((max, val) => max > val ? max : val, 0);
    const qtdPorPiso = divisoesCapacidade.length / numPisos;

    // Para cada piso do caminhão, adicionar o piso na lista
    for (var i = 0; i < numPisos; i++) {
        var pisoDivisaoCapacidade = new PisosDivisoesCapacidade();

        pedidoProduto.PisosDivisoesCapacidade.push(pisoDivisaoCapacidade);
    }

    for (var i = 0; i < divisoesCapacidade.length; i++) {
        const pisoAtual = Math.floor(i / qtdPorPiso);

        var divisaoCapacidade = new PedidoProdutoDivisaoCapacidade(divisoesCapacidade[i]);

        pedidoProduto.DivisoesCapacidade.push(divisaoCapacidade);
        pedidoProduto.PisosDivisoesCapacidade()[pisoAtual].DivisoesCapacidade.push(divisaoCapacidade);
        pedidoProduto.PisosDivisoesCapacidade()[pisoAtual].NumColunas.val = numColunas;
        
        $("#" + divisaoCapacidade.Quantidade.id).maskMoney(divisaoCapacidade.Quantidade.configDecimal);

        divisaoCapacidade.Quantidade.val.subscribe(function () {
            var quantidadeTotal = 0;

            for (var j = 0; j < pedidoProduto.DivisoesCapacidade().length; j++) {
                var divisaoCapacidadeQuantidade = pedidoProduto.DivisoesCapacidade()[j].Quantidade.val() || "0";

                quantidadeTotal += Globalize.parseFloat(divisaoCapacidadeQuantidade);
            }

            pedidoProduto.Quantidade.val(Globalize.format(quantidadeTotal, "n2"));
        });
    }
}

function auditarPedidoProdutoClick(e) {
    var data = { Codigo: e.Codigo.val() };
    var closureAuditoria = OpcaoAuditoria("CargaPedidoProduto", null, e);

    closureAuditoria(data);
}