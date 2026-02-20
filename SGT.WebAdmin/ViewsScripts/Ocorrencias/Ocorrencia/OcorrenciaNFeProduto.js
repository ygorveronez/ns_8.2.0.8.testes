/// <reference path="Ocorrencia.js" />

var _gridOcorrenciaNFeProduto;
var _ocorrenciaNFeProduto;
var _listaSelecionados

var OcorrenciaNFeProduto = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.ListaOcorrenciaNFeProduto = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

function loadOcorrenciaNFeProduto() {
    _ocorrenciaNFeProduto = new OcorrenciaNFeProduto();
    KoBindings(_ocorrenciaNFeProduto, "knockoutOcorrenciaNFeProduto");

    CarregarGridNFeProduto();
}

function CarregarGridNFeProduto() {
    var editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: AtualizarGridProduto,
    };

    var editable = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.decimal,
        numberMask: ConfigDecimal(),
    };

    var editableIntConfig = {
        editable: false,
        type: EnumTipoColunaEditavelGrid.decimal,
        numberMask: ConfigDecimal()
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCTe", visible: false },
        { data: "CodigoXMLNotaFiscal", visible: false },
        { data: "Nota", title: "Nota", width: "10%" },
        { data: "CodigoProdutoEmbarcador", title: "Código", width: "10%" },
        { data: "Produto", title: "Produto", width: "30%" },
        { data: "Quantidade", title: "Quantidade", width: "7%", editableCell: editableIntConfig },
        { data: "QuantidadeDevolucao", title: "Qtd. Devolver", width: "7%", editableCell: editable },
        { data: "ValorProduto", title: "Valor Unitário", width: "10%", visible: false },
        { data: "ValorTotal", title: "Total", width: "10%", visible: false },
        { data: "ValorTotalDevolver", title: "Total Devolver", width: "10%", visible: false },
        { data: "NomeDestinatario", title: "Cliente", width: "20%" }
    ];

    _gridOcorrenciaNFeProduto = new BasicDataTable(_ocorrenciaNFeProduto.Grid.id, header, null, { column: 1, dir: orderDir.asc }, null, 5, null, null, editarColuna);
    RecarregarGridNFeProduto();
}

function RecarregarGridNFeProduto(codigoCTe) {
    var data = new Array();
    $.each(_ocorrenciaNFeProduto.ListaOcorrenciaNFeProduto.list, function (i, produto) {
        if (codigoCTe != null && produto.CodigoCTe != codigoCTe)
            return;

        var parametroGrid = new Object();

        parametroGrid.DT_Enable = true;
        parametroGrid.Codigo = produto.Codigo;
        parametroGrid.CodigoCTe = produto.CodigoCTe;
        parametroGrid.CodigoXMLNotaFiscal = produto.CodigoXMLNotaFiscal;
        parametroGrid.Nota = produto.Nota;
        parametroGrid.CodigoProdutoEmbarcador = produto.CodigoProdutoEmbarcador;
        parametroGrid.Produto = produto.Produto;
        parametroGrid.Quantidade = produto.Quantidade;
        parametroGrid.QuantidadeDevolucao = produto.QuantidadeDevolucao;
        parametroGrid.ValorProduto = produto.ValorProduto;
        parametroGrid.ValorTotal = produto.ValorTotal;
        parametroGrid.ValorTotalDevolver = produto.ValorTotalDevolver;
        parametroGrid.NomeDestinatario = produto.NomeDestinatario;

        if (produto.QuantidadeDevolucao > 0)
            parametroGrid.DT_FontColor = "#e08506";

        data.push(parametroGrid);
    });
    _gridOcorrenciaNFeProduto.CarregarGrid(data);
}

function devolverProdutosClick(data) {
    obterOcorrenciasProdutosNotas(data);
    Global.abrirModal('divModalDevolverProdutosOcorrencia');
}

function obterOcorrenciasProdutosNotas(data) {
    if (_ocorrenciaNFeProduto.ListaOcorrenciaNFeProduto.list != null &&
        _ocorrenciaNFeProduto.ListaOcorrenciaNFeProduto.list.length > 0 &&
        _ocorrenciaNFeProduto.ListaOcorrenciaNFeProduto.list.some((produto) => produto.CodigoCTe == data.CodigoCTE)) {
        RecarregarGridNFeProduto(data.CodigoCTE);
        return;
    }

    executarReST("Ocorrencia/ObterProdutosNotasCTesOcorrencia", { CodigoCTe: data.CodigoCTE }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                CarregarGridProdutos(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function AtualizarGridProduto(dataRow, row, head, callbackTabPress, table) {
    var data = GetNFeProdutos();

    var quantidade = Globalize.parseFloat(dataRow.Quantidade);
    var quantidadeDevolucao = Globalize.parseFloat(dataRow.QuantidadeDevolucao);

    if (quantidadeDevolucao % 1 != 0)
        quantidadeDevolucao = Math.trunc(quantidadeDevolucao);

    if (quantidadeDevolucao > quantidade)
        quantidadeDevolucao = quantidade;

    if (quantidadeDevolucao > 0)
        dataRow.DT_FontColor = "#e08506";
    else 
        dataRow.DT_FontColor = "#000000";

    for (var i in data) {
        if (dataRow.Codigo == data[i].Codigo) {
            data[i].QuantidadeDevolucao = quantidadeDevolucao;
            dataRow.QuantidadeDevolucao = quantidadeDevolucao;
            AtualizarDataRow(table, row, dataRow, callbackTabPress);
        }
    }

    SetNFeProdutos(data);
};

function CarregarGridProdutos(produtosNotas) {
    if (produtosNotas != null) {
        $.each(produtosNotas, function (i, produto) {
            _ocorrenciaNFeProduto.ListaOcorrenciaNFeProduto.list.push(produto);
        });
    } else {
        _ocorrenciaNFeProduto.ListaOcorrenciaNFeProduto.list = new Array();
    }

    RecarregarGridNFeProduto();
}

function GetNFeProdutos() {
    if (_ocorrenciaNFeProduto.ListaOcorrenciaNFeProduto.list.length > 0) {
        return _ocorrenciaNFeProduto.ListaOcorrenciaNFeProduto.list.slice();
    } else {
        return new Array();
    }
}

function SetNFeProdutos(data) {
    return _ocorrenciaNFeProduto.ListaOcorrenciaNFeProduto.list = data.slice();
}

function preencherListaNFeProdutos() {
    _ocorrencia.ListaOcorrenciaNFeProduto.val(JSON.stringify(_ocorrenciaNFeProduto.ListaOcorrenciaNFeProduto.list));
}