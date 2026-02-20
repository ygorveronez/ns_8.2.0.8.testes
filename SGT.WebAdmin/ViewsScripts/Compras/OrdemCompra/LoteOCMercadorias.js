/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemCompra.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _loteOCMercadorias;
var _gridLoteOCMercadorias;

var LoteOCMercadorias = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Produtos = PropertyEntity({ type: types.local });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.VeiculoMercadoria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo Mercadoria:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.Quantidade = PropertyEntity({ text: "*Quantidade: ", getType: typesKnockout.decimal, required: true });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário: ", getType: typesKnockout.decimal, required: true, configDecimal: { precision: 4, allowZero: false, allowNegative: false } });
    this.ValorTotal = PropertyEntity({ text: "*Valor Total: ", getType: typesKnockout.decimal, required: true, configDecimal: { precision: 2, allowZero: false, allowNegative: false } });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarProdutoLoteOCClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarProdutoLoteOCClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirProdutoLoteOCClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarProdutoLoteOCClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

//*******EVENTOS*******
function LoadLoteOCMercadorias() {
    _loteOCMercadorias = new LoteOCMercadorias();
    KoBindings(_loteOCMercadorias, "knockoutLoteOCMercadorias");

    _loteOrdemCompra.Codigo.val.subscribe(function (val) {
        _loteOCMercadorias.Codigo.val(val);
    });

    GridLoteOCMercadorias();

    new BuscarProdutoTMS(_loteOCMercadorias.Produto);
    new BuscarVeiculos(_loteOCMercadorias.VeiculoMercadoria);

    $("#" + _loteOCMercadorias.ValorUnitario.id + ", #" + _loteOCMercadorias.Quantidade.id).focusout(function () {
        ValorTotalMercadoriaLoteOC();
    });

    $("#" + _loteOCMercadorias.ValorTotal.id).focusout(function () {
        ValorUnitarioMercadoriaLoteOC();
    });
}

function AdicionarProdutoLoteOCClick() {
    var valido = ValidarCamposObrigatorios(_loteOCMercadorias);

    if (valido) {
        _loteOCMercadorias.Codigo.val(guid());

        var data = GetProdutosLoteOC();
        var item = SalvarListEntity(_loteOCMercadorias);

        data.push(item);
        SetProdutosLoteOC(data);
        SomarTotalizadoresLoteOrdemCompra();

        RecarregarGridProdutosLoteOC();

        LimparCamposProdutoLoteOC();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function AtualizarProdutoLoteOCClick() {
    var valido = ValidarCamposObrigatorios(_loteOCMercadorias);

    if (valido) {
        var itens = GetProdutosLoteOC();
        var item = SalvarListEntity(_loteOCMercadorias);
        var codigo = _loteOCMercadorias.Codigo.val();

        for (var i = 0, s = itens.length; i < s; i++) {
            if (codigo == itens[i].Codigo.val) {
                itens[i] = item;
                break;
            }
        }
        SetProdutosLoteOC(itens);
        SomarTotalizadoresLoteOrdemCompra();

        RecarregarGridProdutosLoteOC();
        LimparCamposProdutoLoteOC();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function ExcluirProdutoLoteOCClick() {
    var itens = GetProdutosLoteOC();
    var codigo = _loteOCMercadorias.Codigo.val();
    for (var i = 0, s = itens.length; i < s; i++) {
        if (codigo == itens[i].Codigo.val) {
            itens.splice(i, 1);
            break;
        }
    }

    SetProdutosLoteOC(itens);

    RecarregarGridProdutosLoteOC();
    LimparCamposProdutoLoteOC();
}

function HistoricoProdutoLoteOCClick(data) {
    _historicoProduto.Produto.val(data.Produto);
    Global.abrirModal('divHistoricoCompra');
    _gridHistoricoProduto.CarregarGrid();
}

function EditarProdutoLoteOCClick(data) {
    var itens = GetProdutosLoteOC();
    var produto = null;
    for (var i = 0, s = itens.length; i < s; i++) {
        if (data.Codigo == itens[i].Codigo.val) {
            produto = itens[i];
            break;
        }
    }

    if (produto != null) {
        _loteOCMercadorias.Codigo.val(produto.Codigo.val);
        _loteOCMercadorias.Produto.val(produto.Produto.val);
        _loteOCMercadorias.Produto.codEntity(produto.Produto.codEntity);
        _loteOCMercadorias.VeiculoMercadoria.codEntity(produto.VeiculoMercadoria.codEntity);
        _loteOCMercadorias.Quantidade.val(produto.Quantidade.val);
        _loteOCMercadorias.VeiculoMercadoria.val(produto.VeiculoMercadoria.val);
        _loteOCMercadorias.ValorUnitario.val(produto.ValorUnitario.val);
        _loteOCMercadorias.ValorTotal.val(produto.ValorTotal.val);

        _loteOCMercadorias.Adicionar.visible(false);
        _loteOCMercadorias.Atualizar.visible(true);
        _loteOCMercadorias.Excluir.visible(true);

        DiminuirTotalizadoresLoteOrdemCompra(data);
    }
}

function CancelarProdutoLoteOCClick() {
    if (_loteOCMercadorias.Codigo.val() != 0)
        SomarTotalizadoresLoteOrdemCompra();

    LimparCamposProdutoLoteOC();
}

function SomarTotalizadoresLoteOrdemCompra() {
    var valorTotal = Globalize.parseFloat(_loteOrdemCompra.ValorTotal.val()) || 0;
    var valorTotalItem = Globalize.parseFloat(_loteOCMercadorias.ValorTotal.val()) || 0;

    _loteOrdemCompra.ValorTotal.val(Globalize.format(valorTotal + valorTotalItem, "n2"));
}

function DiminuirTotalizadoresLoteOrdemCompra(data) {
    var valorTotal = Globalize.parseFloat(_loteOrdemCompra.ValorTotal.val());
    var valorTotalItem = Globalize.parseFloat(data.ValorTotal);

    _loteOrdemCompra.ValorTotal.val(Globalize.format(valorTotal - valorTotalItem, "n2"));
}


//*******MÉTODOS*******

function GridLoteOCMercadorias() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [
            { descricao: "Editar", id: guid(), metodo: EditarProdutoLoteOCClick/*, visibilidade: VisibilidadeEditarMercadoria*/ },
            { descricao: "Histórico de Compra", id: guid(), metodo: HistoricoProdutoLoteOCClick },
        ]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Produto", title: "Produto", width: "35%" },
        { data: "VeiculoMercadoria", title: "Veículo", width: "15%" },
        { data: "Quantidade", title: "Quantidade Solicitada", width: "12%" },
        { data: "ValorUnitario", title: "Valor Unitário", width: "12%" },
        { data: "ValorTotal", title: "Valor Total", width: "12%" },
    ];

    _gridLoteOCMercadorias = new BasicDataTable(_loteOCMercadorias.Produtos.id, header, menuOpcoes);

    RecarregarGridProdutosLoteOC();
}

function ValorTotalMercadoriaLoteOC() {
    var qtd = Globalize.parseFloat(_loteOCMercadorias.Quantidade.val()) || 0;
    var valor = Globalize.parseFloat(_loteOCMercadorias.ValorUnitario.val()) || 0;

    _loteOCMercadorias.ValorTotal.val(Globalize.format((qtd * valor), "n2"));
}

function ValorUnitarioMercadoriaLoteOC() {
    var qtd = Globalize.parseFloat(_loteOCMercadorias.Quantidade.val()) || 0;
    var valor = Globalize.parseFloat(_loteOCMercadorias.ValorTotal.val()) || 0;

    if (qtd > 0)
        _loteOCMercadorias.ValorUnitario.val(Globalize.format((valor / qtd), "n4"));
}

function GetProdutosLoteOC() {
    return _loteOrdemCompra.Produtos.list.slice();
}

function SetProdutosLoteOC(data) {
    return _loteOrdemCompra.Produtos.list = data.slice();
}

function RecarregarGridProdutosLoteOC() {
    var data = [];

    $.each(GetProdutosLoteOC(), function (i, produto) {
        var itemGrid = new Object();

        itemGrid.Codigo = produto.Codigo.val;
        itemGrid.Produto = produto.Produto.val;
        itemGrid.Quantidade = produto.Quantidade.val;
        itemGrid.VeiculoMercadoria = produto.VeiculoMercadoria.val;
        itemGrid.ValorUnitario = produto.ValorUnitario.val;
        itemGrid.ValorTotal = produto.ValorTotal.val;

        data.push(itemGrid);
    });

    _gridLoteOCMercadorias.CarregarGrid(data);
}

function LimparCamposProdutoLoteOC() {
    LimparCampos(_loteOCMercadorias);
    _loteOCMercadorias.Adicionar.visible(true);
    _loteOCMercadorias.Atualizar.visible(false);
    _loteOCMercadorias.Excluir.visible(false);
}
