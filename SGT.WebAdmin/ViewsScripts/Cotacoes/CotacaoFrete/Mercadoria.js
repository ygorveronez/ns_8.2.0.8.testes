/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/GrupoProduto.js" />
/// <reference path="CotacaoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _mercadoria;
var _CRUDMercadoria;
var _gridMercadoria;

var Mercadoria = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Quantidade = PropertyEntity({ getType: typesKnockout.int, text: "*Quantidade:", val: ko.observable(""), def: "", required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Produto: "), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoProduto = PropertyEntity({ val: ko.observable(false), options: Global.ObterOpcoesBooleano("Simulado", "Cadastrado"), def: false, text: "Tipo Produto: ", required: false, visible: ko.observable(true) });
    this.TipoProduto.val.subscribe(tipoProdutoChange);

    this.Peso = PropertyEntity({ getType: typesKnockout.decimal, text: "Peso:", val: ko.observable(""), def: "", required: false, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 3, allowZero: true, allowNegative: false } });
    this.Largura = PropertyEntity({ getType: typesKnockout.decimal, text: "Largura (MT):", val: ko.observable(""), def: "", required: false, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 3, allowZero: true, allowNegative: false } });
    this.Comprimento = PropertyEntity({ getType: typesKnockout.decimal, text: "Comprimento (MT):", val: ko.observable(""), def: "", required: false, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 3, allowZero: true, allowNegative: false } });
    this.Altura = PropertyEntity({ getType: typesKnockout.decimal, text: "Altura (MT):", val: ko.observable(""), def: "", required: false, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 3, allowZero: true, allowNegative: false } });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Grupo Produto: "), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });

    this.Grid = PropertyEntity({ type: types.local });
};

var CRUDMercadoria = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarMercadoriaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarMercadoriaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirMercadoriaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadMercadoria() {
    _mercadoria = new Mercadoria();
    KoBindings(_mercadoria, "knockoutMercadoria");

    _CRUDMercadoria = new CRUDMercadoria();
    KoBindings(_CRUDMercadoria, "knockoutCRUDMercadoria");

    new BuscarProdutos(_mercadoria.Produto, null, null, null, _cotacaoFrete.GrupoProduto);
    new BuscarGruposProdutos(_mercadoria.GrupoProduto);

    LoadGridMercadoria();
}

function AdicionarMercadoriaClick() {
    if (!ValidarCamposObrigatorios(_mercadoria)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    _mercadoria.Codigo.val(guid());
    _cotacaoFrete.Mercadorias.list.push(SalvarListEntity(_mercadoria));

    LimparCamposMercadoria();
}

function AtualizarMercadoriaClick() {
    if (!ValidarCamposObrigatorios(_mercadoria)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    for (var i = 0; i < _cotacaoFrete.Mercadorias.list.length; i++) {
        if (_mercadoria.Codigo.val() == _cotacaoFrete.Mercadorias.list[i].Codigo.val) {
            _cotacaoFrete.Mercadorias.list[i] = SalvarListEntity(_mercadoria);
            break;
        }
    }

    LimparCamposMercadoria();
}

function ExcluirMercadoriaClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir a mercadoria?", function () {
        for (var i = 0; i < _cotacaoFrete.Mercadorias.list.length; i++) {
            if (_mercadoria.Codigo.val() == _cotacaoFrete.Mercadorias.list[i].Codigo.val) {
                _cotacaoFrete.Mercadorias.list.splice(i, 1);
                break;
            }
        }

        LimparCamposMercadoria();
    });
}

////*******MÉTODOS*******

function LoadGridMercadoria() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarMercadoriaClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoProduto", visible: false },
        { data: "Produto", title: "Produto", width: "40%" },
        { data: "Quantidade", title: "Quantidade", width: "20%" },
        { data: "Peso", title: "Peso", width: "20%" },
        { data: "Largura", title: "Largura", width: "20%" },
        { data: "Comprimento", title: "Comprimento", width: "20%" },
        { data: "Altura", title: "Altura", width: "20%" },
        { data: "GrupoProduto", title: "Grupo Produto", width: "20%" },
    ];

    _gridMercadoria = new BasicDataTable(_mercadoria.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridMercadoria();
}

function EditarMercadoriaClick(data) {
    for (var i = 0; i < _cotacaoFrete.Mercadorias.list.length; i++) {
        if (data.Codigo == _cotacaoFrete.Mercadorias.list[i].Codigo.val) {

            var item = _cotacaoFrete.Mercadorias.list[i];

            _mercadoria.Codigo.val(item.Codigo.val);
            _mercadoria.TipoProduto.val(item.TipoProduto.val);
            _mercadoria.Produto.val(item.Produto.val);
            _mercadoria.Produto.codEntity(item.Produto.codEntity);
            _mercadoria.Quantidade.val(item.Quantidade.val);
            _mercadoria.Peso.val(item.Peso.val);
            _mercadoria.Largura.val(item.Largura.val);
            _mercadoria.Comprimento.val(item.Comprimento.val);
            _mercadoria.Altura.val(item.Altura.val);
            _mercadoria.GrupoProduto.val(item.GrupoProduto.val);

            _CRUDMercadoria.Adicionar.visible(false);
            _CRUDMercadoria.Atualizar.visible(true);
            _CRUDMercadoria.Excluir.visible(true);

            break;
        }
    }
}

function RecarregarGridMercadoria() {
    var data = new Array();

    $.each(_cotacaoFrete.Mercadorias.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.TipoProduto = item.TipoProduto.val;
        itemGrid.Produto = item.Produto.val;
        itemGrid.Quantidade = item.Quantidade.val;
        itemGrid.Peso = item.Peso.val;
        itemGrid.Largura = item.Largura.val;
        itemGrid.Comprimento = item.Comprimento.val;
        itemGrid.Altura = item.Altura.val;
        itemGrid.GrupoProduto = item.GrupoProduto.val;

        data.push(itemGrid);
    });

    _gridMercadoria.CarregarGrid(data);
}

function LimparCamposMercadoria() {
    LimparCampos(_mercadoria);
    _CRUDMercadoria.Adicionar.visible(true);
    _CRUDMercadoria.Atualizar.visible(false);
    _CRUDMercadoria.Excluir.visible(false);

    RecarregarGridMercadoria();
}

function tipoProdutoChange() {
    var produtoSimulado = _mercadoria.TipoProduto.val();

    _mercadoria.Produto.enable(!produtoSimulado);
    _mercadoria.Produto.required(!produtoSimulado);

    if (!produtoSimulado)
        _mercadoria.Produto.text("*Produto");
    else {
        _mercadoria.Produto.text("Produto");
        _mercadoria.Produto.val("");
        _mercadoria.Produto.codEntity(0);
    }

    _mercadoria.Peso.visible(produtoSimulado);
    _mercadoria.Largura.visible(produtoSimulado);
    _mercadoria.Comprimento.visible(produtoSimulado);
    _mercadoria.Altura.visible(produtoSimulado);
    _mercadoria.GrupoProduto.visible(produtoSimulado);
}