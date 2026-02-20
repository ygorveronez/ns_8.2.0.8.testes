/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="RequisicaoMercadoria.js" />
/// <reference path="Requisicao.js" />
/// <reference path="../../Consultas/Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _mercadorias;
var _gridMercadorias;

var Mercadorias = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Produtos = PropertyEntity({ type: types.local });

    this.CustoUnitario = PropertyEntity({ text: "Custo Unitário: ", getType: typesKnockout.decimal, required: false, enable: false });
    this.CustoTotal = PropertyEntity({ text: "Custo Total: ", getType: typesKnockout.decimal, required: false, enable: false });

    this.Quantidade = PropertyEntity({ text: "*Quantidade: ", getType: typesKnockout.decimal, required: true });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: true, enable: ko.observable(false), visible: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local de Armazenamento:", idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento) });
    this.Unidade = PropertyEntity({ val: ko.observable("") });
    this.EstoqueAtual = PropertyEntity({ getType: typesKnockout.decimal });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarProdutoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirProdutoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposProduto, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.BuscarEstoqueMinimo = PropertyEntity({ type: types.event, idBtnSearch: guid(), text: "Buscar do Estoque Mínimo", visible: ko.observable(false) });

    this.Quantidade.val.subscribe(function (val) {
        var quantidade = Globalize.parseFloat(_mercadorias.Quantidade.val());
        if (isNaN(quantidade))
            quantidade = 0;
        var custoUnitario = Globalize.parseFloat(_mercadorias.CustoUnitario.val());
        if (isNaN(custoUnitario))
            custoUnitario = 0;
        var custoTotal = Globalize.format((quantidade * custoUnitario), "n2");
        _mercadorias.CustoTotal.val(custoTotal);
    });

};

var MercadoriasMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: 0 });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: 0 });
    this.Quantidade = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
    this.EstoqueAtual = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
    this.CustoUnitario = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
    this.CustoTotal = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
    this.Unidade = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.string });
};

//*******EVENTOS*******

function LoadMercadorias() {
    _mercadorias = new Mercadorias();
    KoBindings(_mercadorias, "knockoutMercadorias");

    _requisicaoMercadoria.Codigo.val.subscribe(function (val) {
        _mercadorias.Codigo.val(val);
    });

    GridMercadorias();

    new BuscarProdutoEstoque(_mercadorias.Produto, RetornoProduto, _requisicao.Filial, true);
    new BuscarProdutoTMSEstoqueMinimo(_mercadorias.BuscarEstoqueMinimo, BuscarEstoqueMinimoRetorno, _requisicao.Filial, _gridMercadorias);
    new BuscarLocalArmazenamentoProduto(_mercadorias.LocalArmazenamento);

}

function AdicionarProdutoClick() {
    var valido = ValidarCamposObrigatorios(_mercadorias);

    if (valido) {
        _mercadorias.Codigo.val(guid());

        var data = GetProdutos();
        var item = SalvarListEntity(_mercadorias);

        data.push(item);
        SetProdutos(data);

        RecarregarGridProdutos();

        LimparCamposProduto();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function AtualizarProdutoClick() {
    var valido = ValidarCamposObrigatorios(_mercadorias);

    if (valido) {
        var itens = GetProdutos();
        var item = SalvarListEntity(_mercadorias);
        var codigo = _mercadorias.Codigo.val();

        for (var i = 0, s = itens.length; i < s; i++) {
            if (codigo == itens[i].Codigo.val) {
                itens[i] = item;
                break;
            }
        }
        SetProdutos(itens);

        RecarregarGridProdutos();
        LimparCamposProduto();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function ExcluirProdutoClick() {
    var itens = GetProdutos();
    var codigo = _mercadorias.Codigo.val();
    for (var i = 0, s = itens.length; i < s; i++) {
        if (codigo == itens[i].Codigo.val) {
            itens.splice(i, 1);
            break;
        }
    }

    SetProdutos(itens);

    RecarregarGridProdutos();
    LimparCamposProduto();
}

function BuscarEstoqueMinimoRetorno(data) {
    if (!$.isArray(data)) data = [data];

    LimparCamposProduto();

    var itensGrid = GetProdutos();
    data.forEach(function (pr) {
        if (ProdutoNaoEstaNaLista(itensGrid, pr.Codigo)) {
            _mercadorias.Codigo.val(guid());
            _mercadorias.Quantidade.val(pr.Diferenca);
            _mercadorias.Produto.val(pr.Descricao);
            _mercadorias.Produto.codEntity(pr.Codigo);
            _mercadorias.EstoqueAtual.val(pr.Quantidade);
            _mercadorias.CustoUnitario.val(pr.UltimoCusto);
            _mercadorias.Unidade.val(pr.Unidade);

            var quantidade = Globalize.parseFloat(pr.Diferenca);
            if (isNaN(quantidade))
                quantidade = 0;
            var custoUnitario = Globalize.parseFloat(pr.UltimoCusto);
            if (isNaN(custoUnitario))
                custoUnitario = 0;
            var custoTotal = Globalize.format((quantidade * custoUnitario), "n2");
            _mercadorias.CustoTotal.val(custoTotal);

            var item = SalvarListEntity(_mercadorias);
            itensGrid.push(item);
        }
    });
    LimparCamposProduto();
    SetProdutos(itensGrid);
    RecarregarGridProdutos();
}

function EditarProdutoClick(data) {
    var itens = GetProdutos();
    var produto = null;
    for (var i = 0, s = itens.length; i < s; i++) {
        if (data.Codigo == itens[i].Codigo.val) {
            produto = itens[i];
            break;
        }
    }

    if (produto != null) {
        _mercadorias.Codigo.val(produto.Codigo.val);
        _mercadorias.Produto.val(produto.Produto.val);
        _mercadorias.Produto.codEntity(produto.Produto.codEntity);
        _mercadorias.Quantidade.val(produto.Quantidade.val);
        _mercadorias.CustoUnitario.val(produto.CustoUnitario.val);
        _mercadorias.CustoTotal.val(produto.CustoTotal.val);
        _mercadorias.EstoqueAtual.val(produto.EstoqueAtual.val);
        _mercadorias.LocalArmazenamento.val(produto.LocalArmazenamento.val);
        _mercadorias.LocalArmazenamento.codEntity(produto.LocalArmazenamento.codEntity);
        _mercadorias.Unidade.val(produto.Unidade.val);

        _mercadorias.Adicionar.visible(false);
        _mercadorias.Atualizar.visible(true);
        _mercadorias.Excluir.visible(true);
    }
}


//*******MÉTODOS*******
function GridMercadorias() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 5, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarProdutoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "LocalArmazenamento", visible: false },
        { data: "CodigoProduto", title: "Código", width: "7%" },
        { data: "Produto", title: "Produto", width: "45%" },
        { data: "Quantidade", title: "Quantidade Solicitada", width: "12%" },
        { data: "Unidade", title: "Unidade", width: "12%" },
        { data: "EstoqueAtual", title: "Estoque Atual", width: "12%" },
        { data: "CustoUnitario", title: "Custo Unitário", width: "12%" },
        { data: "CustoTotal", title: "Custo Total", width: "12%" },
    ];

    _gridMercadorias = new BasicDataTable(_mercadorias.Produtos.id, header, menuOpcoes);

    RecarregarGridProdutos();
}

function ProdutoNaoEstaNaLista(lista, codigo) {
    for (var i = 0, s = lista.length; i < s; i++) {
        if (lista[i].Produto.codEntity == codigo)
            return false;
    }

    return true;
}

function GetProdutos() {
    return _requisicao.Produtos.list.slice();
}

function SetProdutos(data) {
    return _requisicao.Produtos.list = data.slice();
}

function RetornoProduto(produto) {

    _mercadorias.Produto.val(produto.Descricao);
    _mercadorias.Produto.codEntity(produto.CodigoProduto);
    _mercadorias.EstoqueAtual.val(produto.Quantidade);
    _mercadorias.CustoUnitario.val(produto.CustoUnitario);
    _mercadorias.Unidade.val(produto.Unidade);
}

function RecarregarGridProdutos() {
    var data = [];

    $.each(GetProdutos(), function (i, produto) {
        var itemGrid = new Object();

        itemGrid.Codigo = produto.Codigo.val;
        itemGrid.CodigoProduto = produto.Produto.codEntity;
        itemGrid.Produto = produto.Produto.val;
        itemGrid.Quantidade = produto.Quantidade.val;
        itemGrid.EstoqueAtual = produto.EstoqueAtual.val;
        itemGrid.CustoUnitario = produto.CustoUnitario.val;
        itemGrid.CustoTotal = produto.CustoTotal.val;
        itemGrid.LocalArmazenamento = produto.LocalArmazenamento.val;
        itemGrid.Unidade = produto.Unidade.val;

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

function CarregarProdutosDaRequisicao(produtos) {
    var data = produtos.map(function (produto) {
        var koMercadoria = new MercadoriasMap;
        koMercadoria.Codigo.val = produto.Codigo;
        koMercadoria.Produto.val = produto.Produto.Descricao;
        koMercadoria.Produto.codEntity = produto.Produto.Codigo;
        koMercadoria.Quantidade.val = produto.Quantidade;
        koMercadoria.EstoqueAtual.val = produto.EstoqueAtual;
        koMercadoria.Unidade.val = produto.Unidade;
        koMercadoria.CustoUnitario.val = produto.CustoUnitario;
        koMercadoria.CustoTotal.val = produto.CustoTotal;
        koMercadoria.LocalArmazenamento.val = produto.LocalArmazenamento.Descricao;
        koMercadoria.LocalArmazenamento.codEntity = produto.LocalArmazenamento.Codigo;

        return koMercadoria;
    });

    SetProdutos(data);
    RecarregarGridProdutos();

    if (_requisicaoMercadoria.Situacao.val() == EnumSituacaoRequisicaoMercadoria.AgAprovacao || _requisicaoMercadoria.Situacao.val() == EnumSituacaoRequisicaoMercadoria.SemRegra)
        _gridMercadorias.HabilitarOpcoes();
    else
        _gridMercadorias.DesabilitarOpcoes();
}