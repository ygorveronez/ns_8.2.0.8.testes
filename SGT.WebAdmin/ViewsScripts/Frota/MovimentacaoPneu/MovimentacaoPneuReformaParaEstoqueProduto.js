/// <reference path="MovimentacaoPneuReformaParaEstoque.js" />
/// <reference path="../../Consultas/Produto.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridProdutos;
var _movimentacaoPneuReformaParaEstoqueProduto;

/*
 * Declaração das Classes
 */

var MovimentacaoPneuReformaParaEstoqueProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoFinalidadeProduto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: true });
    this.FinalidadeProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Finalidade:", idBtnSearch: guid(), required: false });
    this.Quantidade = PropertyEntity({ text: "*Quantidade:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true });
    this.Valor = PropertyEntity({ text: "*Valor Unit.:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMovimentacaoPneuReformaParaEstoqueProdutoClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarMovimentacaoPneuReformaParaEstoqueProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarMovimentacaoPneuReformaParaEstoqueProdutoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirMovimentacaoPneuReformaParaEstoqueProdutoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridProdutos() {
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarMovimentacaoPneuReformaParaEstoqueProdutoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoProduto", visible: false },
        { data: "CodigoFinalidadeProduto", visible: false },
        { data: "Produto", title: "Produto", width: "30%" },
        { data: "Quantidade", title: "Quantidade", width: "20%" },
        { data: "Valor", title: "Valor Unit.", width: "20%" },
        { data: "FinalidadeProduto", title: "Finalidade", width: "20%" }
    ];

    _gridProdutos = new BasicDataTable("grid-reforma-para-estoque-produto", header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridProdutos.CarregarGrid([]);
}

function loadMovimentacaoPneuReformaParaEstoqueProduto() {
    _movimentacaoPneuReformaParaEstoqueProduto = new MovimentacaoPneuReformaParaEstoqueProduto();
    KoBindings(_movimentacaoPneuReformaParaEstoqueProduto, "knockoutMovimentacaoPneuReformaParaEstoqueProduto");

    new BuscarProdutoTMS(_movimentacaoPneuReformaParaEstoqueProduto.Produto);
    new BuscarFinalidadeProdutoOrdemServico(_movimentacaoPneuReformaParaEstoqueProduto.FinalidadeProduto);

    loadGridProdutos();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMovimentacaoPneuReformaParaEstoqueProdutoClick() {
    if (ValidarCamposObrigatorios(_movimentacaoPneuReformaParaEstoqueProduto)) {
        var produtos = _gridProdutos.BuscarRegistros();

        produtos.push({
            Codigo: guid(),
            CodigoProduto: _movimentacaoPneuReformaParaEstoqueProduto.Produto.codEntity(),
            CodigoFinalidadeProduto: _movimentacaoPneuReformaParaEstoqueProduto.FinalidadeProduto.codEntity(),
            Produto: _movimentacaoPneuReformaParaEstoqueProduto.Produto.val(),
            FinalidadeProduto: _movimentacaoPneuReformaParaEstoqueProduto.FinalidadeProduto.val(),
            Quantidade: _movimentacaoPneuReformaParaEstoqueProduto.Quantidade.val(),
            Valor: _movimentacaoPneuReformaParaEstoqueProduto.Valor.val()
        });

        _gridProdutos.CarregarGrid(produtos);

        fecharModalMovimentacaoPneuReformaParaEstoqueProduto();
    }
    else
        exibirMensagemCamposObrigatorio();
}

function atualizarMovimentacaoPneuReformaParaEstoqueProdutoClick() {
    if (ValidarCamposObrigatorios(_movimentacaoPneuReformaParaEstoqueProduto)) {
        var produtos = _gridProdutos.BuscarRegistros();

        for (var i = 0; i < produtos.length; i++) {
            var produto = produtos[i];

            if (produto.Codigo == _movimentacaoPneuReformaParaEstoqueProduto.Codigo.val()) {
                produto.CodigoProduto = _movimentacaoPneuReformaParaEstoqueProduto.Produto.codEntity();
                produto.CodigoFinalidadeProduto = _movimentacaoPneuReformaParaEstoqueProduto.FinalidadeProduto.codEntity();
                produto.Produto = _movimentacaoPneuReformaParaEstoqueProduto.Produto.val();
                produto.FinalidadeProduto = _movimentacaoPneuReformaParaEstoqueProduto.FinalidadeProduto.val();
                produto.Quantidade = _movimentacaoPneuReformaParaEstoqueProduto.Quantidade.val();
                produto.Valor = _movimentacaoPneuReformaParaEstoqueProduto.Valor.val();

                break;
            }
        }

        _gridProdutos.CarregarGrid(produtos);

        fecharModalMovimentacaoPneuReformaParaEstoqueProduto();
    }
    else
        exibirMensagemCamposObrigatorio();
}

function cancelarMovimentacaoPneuReformaParaEstoqueProdutoClick() {
    fecharModalMovimentacaoPneuReformaParaEstoqueProduto();
}

function editarMovimentacaoPneuReformaParaEstoqueProdutoClick(produtoSelecionado) {
    var isEdicao = true;

    _movimentacaoPneuReformaParaEstoqueProduto.Codigo.val(produtoSelecionado.Codigo);
    _movimentacaoPneuReformaParaEstoqueProduto.Produto.codEntity(produtoSelecionado.CodigoProduto);
    _movimentacaoPneuReformaParaEstoqueProduto.Produto.val(produtoSelecionado.Produto);
    _movimentacaoPneuReformaParaEstoqueProduto.FinalidadeProduto.codEntity(produtoSelecionado.CodigoFinalidadeProduto);
    _movimentacaoPneuReformaParaEstoqueProduto.FinalidadeProduto.val(produtoSelecionado.FinalidadeProduto);
    _movimentacaoPneuReformaParaEstoqueProduto.Quantidade.val(produtoSelecionado.Quantidade);
    _movimentacaoPneuReformaParaEstoqueProduto.Valor.val(produtoSelecionado.Valor);

    controlarBotoesHabilitadosMovimentacaoPneuReformaParaEstoqueProduto(isEdicao);
    exibirModalMovimentacaoPneuReformaParaEstoqueProduto();
}

function excluirMovimentacaoPneuReformaParaEstoqueProdutoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o produto " + _movimentacaoPneuReformaParaEstoqueProduto.Produto.val() + "?", function () {
        var produtos = _gridProdutos.BuscarRegistros();

        for (var i = 0; i < produtos.length; i++) {
            var produto = produtos[i];

            if (produto.Codigo == _movimentacaoPneuReformaParaEstoqueProduto.Codigo.val()) {
                produtos.splice(i, 1);

                break;
            }
        }

        _gridProdutos.CarregarGrid(produtos);

        fecharModalMovimentacaoPneuReformaParaEstoqueProduto();
    });
}

/*
 * Declaração das Funções Públicas
 */

function adicionarMovimentacaoPneuReformaParaEstoqueProduto() {
    var isEdicao = false;

    controlarBotoesHabilitadosMovimentacaoPneuReformaParaEstoqueProduto(isEdicao);
    exibirModalMovimentacaoPneuReformaParaEstoqueProduto();
}

function exibirModalMovimentacaoPneuReformaParaEstoqueProduto() {
    Global.abrirModal('divModalMovimentacaoPneuReformaParaEstoqueProduto');
    $("#divModalMovimentacaoPneuReformaParaEstoqueProduto").one('hidden.bs.modal', function () {
        LimparCampos(_movimentacaoPneuReformaParaEstoqueProduto);
    });
}

function limparProdutos() {
    _gridProdutos.CarregarGrid([]);
}

/*
 * Declaração das Funções Privadas
 */

function controlarBotoesHabilitadosMovimentacaoPneuReformaParaEstoqueProduto(isEdicao) {
    _movimentacaoPneuReformaParaEstoqueProduto.Atualizar.visible(isEdicao);
    _movimentacaoPneuReformaParaEstoqueProduto.Excluir.visible(isEdicao);
    _movimentacaoPneuReformaParaEstoqueProduto.Cancelar.visible(isEdicao);
    _movimentacaoPneuReformaParaEstoqueProduto.Adicionar.visible(!isEdicao);
}

function fecharModalMovimentacaoPneuReformaParaEstoqueProduto() {
    Global.fecharModal('divModalMovimentacaoPneuReformaParaEstoqueProduto');
}

function obterMovimentacaoPneuReformaParaEstoqueProdutos() {
    var produtos = _gridProdutos.BuscarRegistros();

    return JSON.stringify(produtos);
}
