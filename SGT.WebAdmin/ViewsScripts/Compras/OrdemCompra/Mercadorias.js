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
var _mercadorias;
var _historicoProduto;
var _gridMercadorias;
var _gridHistoricoProduto;

var Mercadorias = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Produtos = PropertyEntity({ type: types.local });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.VeiculoMercadoria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo Mercadoria:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.Quantidade = PropertyEntity({ text: "*Quantidade: ", getType: typesKnockout.decimal, required: true });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário: ", getType: typesKnockout.decimal, required: true, configDecimal: { precision: 5, allowZero: false, allowNegative: false } });
    this.ValorTotal = PropertyEntity({ text: "*Valor Total: ", getType: typesKnockout.decimal, required: true, configDecimal: { precision: 5, allowZero: false, allowNegative: false } });

    this.CodigoProduto = PropertyEntity({ text: "", getType: typesKnockout.string, val: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarProdutoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirProdutoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarProdutoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

};

var MercadoriasMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: 0 });
    this.VeiculoMercadoria = PropertyEntity({ type: types.entity, codEntity: 0 });
    this.Quantidade = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
    this.ValorUnitario = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
    this.ValorTotal = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
    this.CodigoProduto = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.string });
};

var HistoricoProduto = function () {
    this.Produto = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Historico = PropertyEntity({});
};

//*******EVENTOS*******
function LoadMercadorias() {
    _mercadorias = new Mercadorias();
    KoBindings(_mercadorias, "knockoutMercadorias");

    _historicoProduto = new HistoricoProduto();
    KoBindings(_historicoProduto, "knoutHistoricoProduto");

    _ordemCompra.Codigo.val.subscribe(function (val) {
        _mercadorias.Codigo.val(val);
    });

    GridMercadorias();
    GridHistorico();

    new BuscarProdutoTMS(_mercadorias.Produto, SalvarProduto);
    new BuscarVeiculos(_mercadorias.VeiculoMercadoria);

    $("#" + _mercadorias.ValorUnitario.id + ", #" + _mercadorias.Quantidade.id).focusout(function () {
        ValorTotalMercadoria();
    });

    $("#" + _mercadorias.ValorTotal.id).focusout(function () {
        ValorUnitarioMercadoria();
    });
}

function AdicionarProdutoClick() {

    let valido = ValidarCamposObrigatorios(_mercadorias);

    if (valido) {
        _mercadorias.Codigo.val(guid());

        let data = GetProdutos();
        let item = SalvarListEntity(_mercadorias);

        data.push(item);
        SetProdutos(data);
        SomarTotalizadoresOrdemCompra();

        RecarregarGridProdutos();

        LimparCamposProduto();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function AtualizarProdutoClick() {
    let valido = ValidarCamposObrigatorios(_mercadorias);

    if (valido) {
        let itens = GetProdutos();
        let item = SalvarListEntity(_mercadorias);
        let codigo = _mercadorias.Codigo.val();

        for (let i = 0, s = itens.length; i < s; i++) {
            if (codigo == itens[i].Codigo.val) {
                itens[i] = item;
                break;
            }
        }
        SetProdutos(itens);
        SomarTotalizadoresOrdemCompra();

        RecarregarGridProdutos();
        LimparCamposProduto();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function ExcluirProdutoClick() {
    let itens = GetProdutos();
    let codigo = _mercadorias.Codigo.val();
    for (let i = 0, s = itens.length; i < s; i++) {
        if (codigo == itens[i].Codigo.val) {
            itens.splice(i, 1);
            break;
        }
    }

    SetProdutos(itens);

    RecarregarGridProdutos();
    LimparCamposProduto();
}

function HistoricoProdutoClick(data) {
    _historicoProduto.Produto.val(data.Produto);
    Global.abrirModal('divHistoricoCompra');
    _gridHistoricoProduto.CarregarGrid();
}

function EditarProdutoClick(data) {
    let itens = GetProdutos();
    let produto = null;
    for (let i = 0, s = itens.length; i < s; i++) {
        if (data.Codigo == itens[i].Codigo.val) {
            produto = itens[i];
            break;
        }
    }

    if (produto != null) {
        _mercadorias.Codigo.val(produto.Codigo.val);
        _mercadorias.Produto.val(produto.Produto.val);

        _mercadorias.Produto.codEntity(produto.Produto.codEntity);
        _mercadorias.CodigoProduto.val(produto.CodigoProduto.val);

        _mercadorias.VeiculoMercadoria.val(produto.VeiculoMercadoria.val);
        _mercadorias.VeiculoMercadoria.codEntity(produto.VeiculoMercadoria.codEntity);
        _mercadorias.Quantidade.val(produto.Quantidade.val);
        _mercadorias.ValorUnitario.val(produto.ValorUnitario.val);
        _mercadorias.ValorTotal.val(produto.ValorTotal.val);

        _mercadorias.Adicionar.visible(false);
        _mercadorias.Atualizar.visible(true);
        _mercadorias.Excluir.visible(true);

        DiminuirTotalizadoresOrdemCompra(data);
    }
}

function CancelarProdutoClick() {
    if (_mercadorias.Codigo.val() != 0)
        SomarTotalizadoresOrdemCompra();

    LimparCamposProduto();
}

function SomarTotalizadoresOrdemCompra() {
    let valorTotal = Globalize.parseFloat(_ordemCompra.ValorTotal.val()) || 0;
    let valorTotalItem = Globalize.parseFloat(_mercadorias.ValorTotal.val()) || 0;
    _ordemCompra.ValorTotal.val(Globalize.format(valorTotal + valorTotalItem, "n5"));
}

function DiminuirTotalizadoresOrdemCompra(data) {
    let valorTotal = Globalize.parseFloat(_ordemCompra.ValorTotal.val());
    let valorTotalItem = Globalize.parseFloat(data.ValorTotal);
    _ordemCompra.ValorTotal.val(Globalize.format(valorTotal - valorTotalItem, "n5"));
}


//*******MÉTODOS*******

function GridMercadorias() {
    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [
            { descricao: "Editar", id: guid(), metodo: EditarProdutoClick, visibilidade: VisibilidadeEditarMercadoria },
            { descricao: "Histórico de Compra", id: guid(), metodo: HistoricoProdutoClick },
        ]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "CodigoProduto", title: "Código Produto", width: "12%" },
        { data: "Produto", title: "Produto", width: "35%" },
        { data: "VeiculoMercadoria", title: "Veículo", width: "15%" },
        { data: "Quantidade", title: "Quantidade Solicitada", width: "12%" },
        { data: "ValorUnitario", title: "Valor Unitário", width: "12%" },
        { data: "ValorTotal", title: "Valor Total", width: "12%" },
    ];

    _gridMercadorias = new BasicDataTable(_mercadorias.Produtos.id, header, menuOpcoes);

    RecarregarGridProdutos();
}

function VisibilidadeEditarMercadoria() {
    return _ordemCompra.Situacao.val() == EnumSituacaoOrdemCompra.Aberta;
}

function GridHistorico() {
    _gridHistoricoProduto = new GridView(_historicoProduto.Historico.id, "Produto/HistoricoProduto", _historicoProduto);
}

function CarregarProdutosDaOrdem(produtos) {
    let data = produtos.map(function (produto) {

        let koMercadoria = new MercadoriasMap;

        koMercadoria.Codigo.val = produto.Codigo;
        koMercadoria.CodigoProduto.val = produto.CodigoProduto;
        koMercadoria.Quantidade.val = produto.Quantidade;
        koMercadoria.ValorUnitario.val = produto.ValorUnitario;
        koMercadoria.ValorTotal.val = produto.ValorTotal;
        koMercadoria.Produto.val = produto.Produto.Descricao;
        koMercadoria.Produto.codEntity = produto.Produto.Codigo;
        koMercadoria.VeiculoMercadoria.val = produto.VeiculoMercadoria.Descricao;
        koMercadoria.VeiculoMercadoria.codEntity = produto.VeiculoMercadoria.Codigo;

        return koMercadoria;
    });

    SetProdutos(data);
    RecarregarGridProdutos();
}

function ValorTotalMercadoria() {

    // Desfaz as strings formatadas com vírgulas para decimais e pontos para milhares
    let quantidadeString = _mercadorias.Quantidade.val().replace(/\./g, '').replace(',', '.');
    let valorUnitarioString = _mercadorias.ValorUnitario.val().replace(/\./g, '').replace(',', '.');
    // Convertendo para número
    let quantidade = isNaN(parseFloat(quantidadeString)) ? 0 : parseFloat(quantidadeString);
    let valorUnitario = isNaN(parseFloat(valorUnitarioString)) ? 0 : parseFloat(valorUnitarioString);

    if (quantidade > 0 && valorUnitario > 0) {
        let resultado = quantidade * valorUnitario;
        _mercadorias.ValorTotal.val(Globalize.format(resultado, 'n5'));
    }    
}

function ValorUnitarioMercadoria() {

    // Desfaz as strings formatadas com vírgulas para decimais e pontos para milhares
    let quantidadeString = _mercadorias.Quantidade.val().replace(/\./g, '').replace(',', '.');
    let valorTotalString = _mercadorias.ValorTotal.val().replace(/\./g, '').replace(',', '.');

    // Convertendo para número
    let quantidade = isNaN(parseFloat(quantidadeString)) ? 0 : parseFloat(quantidadeString);
    let valorTotal = isNaN(parseFloat(valorTotalString)) ? 0 : parseFloat(valorTotalString);

    if (quantidade > 0 && valorTotal > 0) {
        let resultado = valorTotal / quantidade;
        _mercadorias.ValorUnitario.val(Globalize.format(resultado, 'n5'));
    }   

}

function GetProdutos() {
    return _ordemCompra.Produtos.list.slice();
}

function SetProdutos(data) {
    return _ordemCompra.Produtos.list = data.slice();
}

function SalvarProduto(data) {
    _mercadorias.CodigoProduto.val(data.CodigoProduto);
    _mercadorias.Produto.val(data.Descricao);
    _mercadorias.Produto.codEntity(data.Codigo);
}

function RecarregarGridProdutos() {
    var data = [];

    $.each(GetProdutos(), function (i, produto) {
        var itemGrid = new Object();

        itemGrid.Codigo = produto.Codigo.val;
        itemGrid.CodigoProduto = produto.CodigoProduto.val;
        itemGrid.Produto = produto.Produto.val;
        itemGrid.Quantidade = produto.Quantidade.val;
        itemGrid.VeiculoMercadoria = produto.VeiculoMercadoria.val;
        itemGrid.ValorUnitario = produto.ValorUnitario.val;
        itemGrid.ValorTotal = produto.ValorTotal.val;

        data.push(itemGrid);
    });

    _gridMercadorias.CarregarGrid(data);
}

function LimparCamposProduto() {
    LimparCampos(_mercadorias);
    _mercadorias.Adicionar.visible(true);
    _mercadorias.Atualizar.visible(false);
    _mercadorias.Excluir.visible(false);
}
