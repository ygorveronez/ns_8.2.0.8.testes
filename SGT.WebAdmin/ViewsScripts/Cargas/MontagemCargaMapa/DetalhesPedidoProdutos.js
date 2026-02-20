/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _detalhesPedidoProdutos ;
var _gridDetalhesPedidoProdutos;
var dataRowProdutosEditado = [];

// Será apresentado nos detalhes do pedido sem carregamento ainda
var DetalhesPedidoProdutos = function () {
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ id: guid() });
}
//*******EVENTOS*******

function permiteEditarProduto(somenteVizualizacao, dataRow) {
    var tipoEdicao = _sessaoRoteirizador.TipoEdicaoPalletProdutoMontagemCarregamento.val();
    return !somenteVizualizacao && (tipoEdicao == EnumTipoEdicaoPalletProdutoMontagemCarregamento.EdicaoPermitida ||
        (tipoEdicao == EnumTipoEdicaoPalletProdutoMontagemCarregamento.ControlePalletAbertoFechado && dataRow.PalletFechado == Localization.Resources.Gerais.Geral.Sim));
}

function loadDetalhesPedidoProdutos() {
    _detalhesPedidoProdutos = new DetalhesPedidoProdutos();
    KoBindings(_detalhesPedidoProdutos, "knoutDetalhesPedidoProdutos");    

    const somenteVizualizacao = !_CONFIGURACAO_TMS.MontagemCarga.PermitirEditarPedidosAtravesTelaMontagemCargaMapa;

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
        { data: "QuantidadePallets", title: "Pallets", width: "4%" },
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

function carregarDetalhesPedidoProdutos(codigoPedido, callback) {
    _detalhesPedidoProdutos.Pedido.val(codigoPedido);
    //_gridDetalhesPedidoProdutos.CarregarGrid(callback);
    obterProdutosPedido();
}

function carregarSaldoArmazemProduto(produtoSelecionado) {
    new SaldoArmazemProduto("#modalSaldoGestaoArmazem").LoadSaldoArmazemProduto(produtoSelecionado);
}

function obterProdutosPedido() {
    executarReST("GestaoPedido/DetalhesPedidoProdutos", { Codigo: _detalhesPedidoProdutos.Pedido.val() }, function (retorno) {
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

function callbackEditarColuna(rowData, row, head, callbackTabPress, table) {
    rowData.PesoTotal = calcularPesoTotalProduto(rowData);
    rowData.QuantidadePallets = calcularQuantidadePallets(rowData);

    var produtoInicial = dataRowProdutosEditado.find(_dataRow => _dataRow.Codigo == rowData.Codigo);

    let confirmarDesmembramento = true;

    if (rowData.Quantidade > produtoInicial?.Quantidade) {
        rowData.Quantidade = produtoInicial.Quantidade;
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
