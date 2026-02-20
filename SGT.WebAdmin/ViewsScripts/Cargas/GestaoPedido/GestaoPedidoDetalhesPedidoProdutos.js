/// <reference path="../../Enumeradores/EnumTipoColunaEditavelGrid.js" />

var _detalhesPedidoProdutos;
var _gridDetalhesPedidoProdutos;
var dataRowProdutosEditado = [];

var DetalhesPedidoProdutos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ id: guid() });
}

function loadGestaoPedidosDetalhesPedidoProdutos() {
    _detalhesPedidoProdutos = new DetalhesPedidoProdutos();
    KoBindings(_detalhesPedidoProdutos, "knoutDetalhesPedidoProdutos");
    _gridDetalhesPedidoProdutos = new GridView(_detalhesPedidoProdutos.Grid.id, "GestaoPedido/DetalhesPedidoProdutos", _detalhesPedidoProdutos, null);
}

function permiteEditarProduto(somenteVizualizacao, dataRow) {
    return !somenteVizualizacao && (dataRow.TipoEdicaoPalletProdutoMontagemCarregamento == EnumTipoEdicaoPalletProdutoMontagemCarregamento.EdicaoPermitida ||
        (dataRow.TipoEdicaoPalletProdutoMontagemCarregamento == EnumTipoEdicaoPalletProdutoMontagemCarregamento.ControlePalletAbertoFechado && dataRow.PalletFechado == Localization.Resources.Gerais.Geral.Sim));
}

function carregarDetalhesPedidoProdutos(codigoPedido, somenteVizualizacao) {
    _detalhesPedidoProdutos.Codigo.val(codigoPedido);
    
    obterProdutosPedido();

    const opcaoConsultarEstoque = { descricao: "Consultar estoque", id: guid(), metodo: carregarSaldoArmazemProduto, icone: "", visibilidade: true };

    const menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 4, opcoes: [opcaoConsultarEstoque] };

    var configuracaoEdicaoColuna = {
        permite: true,
        atualizarRow: true,
        callback: callbackEditarColuna
    };

    var editableQuantidade = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        mask: "",
        precision: 0,
        numberMask: ConfigInt({ thousands: "" }),
        numberMask: { precision: 0 },
        maxlength: 10,
        conditions: function (dataRow) {
            var index = dataRowProdutosEditado.findIndex(_dataRow => _dataRow.Codigo == dataRow.Codigo);

            if (index == -1) dataRowProdutosEditado.push(structuredClone(dataRow));

            return permiteEditarProduto(somenteVizualizacao, dataRow);
        },
    };

    var editablePeso = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.decimal,
        numberMask: ConfigDecimal({ allowZero: true, precision: 3 }),
        conditions: function (dataRow) {
            return permiteEditarProduto(somenteVizualizacao, dataRow);
        }
    };

    const editablePallet = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        numberMask: ConfigInt({ thousands: "" }),
        conditions: function (dataRow) {
            var index = dataRowProdutosEditado.findIndex(_dataRow => _dataRow.Codigo == dataRow.Codigo);

            if (index == -1) dataRowProdutosEditado.push(structuredClone(dataRow));

            return permiteEditarProduto(somenteVizualizacao, dataRow);
        },
    };

    const header = [
        { data: "CodigoProduto", visible: false },
        { data: "CodigoFilial", visible: false },
        { data: "Desmembrar", visible: false },
        { data: "CodigoIntegracao", title: "Cód. Integração", width: "6%" },
        { data: "Descricao", title: "Descrição", width: "10%" },
        { data: "Quantidade", title: "Quantidade", width: "5%", className: "text-align-right", editableCell: editableQuantidade },
        { data: "Estoque", title: "Estoque", width: "5%" },
        { data: "CodigoIntegracaoArmazem", title: "Cód. Armazém", width: "6%" },
        { data: "PesoUnitario", title: "Peso Unitário", width: "6%", editableCell: editablePeso },
        { data: "PesoTotal", title: "Peso Total", width: "5%" },
        { data: "QuantiadeCaixasPorPallet", title: "Cx. por Pallet", width: "6%" },
        { data: "QuantidadePallets", title: "Pallets", width: "4%", editableCell: editablePallet },
        { data: "PalletFechado", title: "Pallet Fechado", width: "6%" },
    ];

    _gridDetalhesPedidoProdutos = new BasicDataTable(
        _detalhesPedidoProdutos.Grid.id,
        header,
        menuOpcoes,
        { column: 1, dir: "asc" },
        null,
        null,
        null,
        null,
        configuracaoEdicaoColuna
    );
}

function callbackEditarColuna(rowData, row, head, callbackTabPress, table) {
    if (head.name == "QuantidadePallets") {
        rowData.Quantidade = calcularQuantidade(rowData);
        rowData.PesoTotal = calcularPesoTotalProduto(rowData);
    } else {
        rowData.PesoTotal = calcularPesoTotalProduto(rowData);
        rowData.QuantidadePallets = calcularQuantidadePallets(rowData);
    }
  
    var produtoInicial = dataRowProdutosEditado.find(_dataRow => _dataRow.Codigo == rowData.Codigo);

    let confirmarDesmembramento = true;

    if (rowData.Quantidade > produtoInicial?.Quantidade) {
        rowData.Quantidade = produtoInicial.Quantidade;
        rowData.QuantidadePallets = produtoInicial.QuantidadePallets;
        rowData.PesoTotal = produtoInicial.PesoTotal;
        confirmarDesmembramento = false;
        exibirMensagem(tipoMensagem.aviso, "Atenção", `A quantidade informada não pode ser maior que ${produtoInicial.Quantidade}`);
    }
    
    _gridDetalhesPedidoProdutos.AtualizarDataRow(row, rowData);

    if (!rowData.Desmembrar && confirmarDesmembramento) {
        exibirConfirmacao("Confirmação", "Deseja Desmembrar o pedido?", function () {
            rowData.Desmembrar = true;

            _gridDetalhesPedidoProdutos.AtualizarDataRow(row, rowData, callbackTabPress);
        });
    }
}

function calcularPesoTotalProduto(rowData) {
    var quantidade = parseInt(rowData.Quantidade);
    var pesoUnitario = Globalize.parseFloat(rowData.PesoUnitario);

    return Globalize.format(pesoUnitario * quantidade, "n3");
}

function calcularQuantidadePallets(rowData) {
    var quantidade = parseInt(rowData.Quantidade);
    var quantidadeCaixas = parseInt(rowData.QuantiadeCaixasPorPallet);

    return Globalize.format(quantidade / quantidadeCaixas, "n3");
}

function calcularQuantidade(rowData) {   
    var quantidadePallet = parseInt(rowData.QuantidadePallets);
    var quantidadeCaixas = parseInt(rowData.QuantiadeCaixasPorPallet);
    return (quantidadePallet * quantidadeCaixas);
}

function removerItemClick(dataRow) {
    var produtosPedido = _gridDetalhesPedidoProdutos.BuscarRegistros();

    if (produtosPedido?.length > 0) {
        var novoProdutosPedido = produtosPedido.filter(produtoPedido => produtoPedido.Codigo !== dataRow.Codigo);

        _gridDetalhesPedidoProdutos.CarregarGrid(novoProdutosPedido);
    }
}

function obterProdutosPedido() {
    executarReST("GestaoPedido/DetalhesPedidoProdutos", { Codigo: _detalhesPedidoProdutos.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _gridDetalhesPedidoProdutos.CarregarGrid(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function carregarSaldoArmazemProduto(produtoSelecionado) {
    new SaldoArmazemProduto("#modalSaldoGestaoArmazem").LoadSaldoArmazemProduto(produtoSelecionado);
} 