/// <reference path="../../Consultas/Produto.js" />
/// <reference path="Cotacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cotacaoMercadoria;
var _CRUDCotacaoMercadoria;
var _gridCotacaoMercadoria;

var CotacaoMercadoria = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Quantidade = PropertyEntity({ getType: typesKnockout.decimal, text: "*Quantidade:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor Unitário:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true), configDecimal: { precision: 4, allowZero: false, allowNegative: false } });
    this.ValorTotal = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor Total:", val: ko.observable("0,00"), def: "0,00", enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto: ", idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.TotalValorMercadorias = PropertyEntity({ text: "Total: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.Grid = PropertyEntity({ type: types.local });

    this.Quantidade.val.subscribe(function () {
        CalcularValorTotal();
    });
    this.ValorUnitario.val.subscribe(function () {
        CalcularValorTotal();
    });
};

var CRUDCotacaoMercadoria = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarMercadoriaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarMercadoriaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirMercadoriaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadCotacaoMercadoria() {
    _cotacaoMercadoria = new CotacaoMercadoria();
    KoBindings(_cotacaoMercadoria, "knockoutCotacaoMercadoria");

    _CRUDCotacaoMercadoria = new CRUDCotacaoMercadoria();
    KoBindings(_CRUDCotacaoMercadoria, "knockoutCRUDCotacaoMercadoria");

    new BuscarProdutoTMS(_cotacaoMercadoria.Produto);

    LoadGridCotacaoMercadoria();
}

function AdicionarMercadoriaClick() {
    if (!ValidarCamposObrigatorios(_cotacaoMercadoria)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    _cotacaoMercadoria.Codigo.val(guid());
    _cotacao.Mercadorias.list.push(SalvarListEntity(_cotacaoMercadoria));

    LimparCamposCotacaoMercadoria();
}

function AtualizarMercadoriaClick() {
    if (!ValidarCamposObrigatorios(_cotacaoMercadoria)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    for (var i = 0; i < _cotacao.Mercadorias.list.length; i++) {
        if (_cotacaoMercadoria.Codigo.val() == _cotacao.Mercadorias.list[i].Codigo.val) {
            _cotacao.Mercadorias.list[i] = SalvarListEntity(_cotacaoMercadoria);
            break;
        }
    }

    LimparCamposCotacaoMercadoria();
}

function ExcluirMercadoriaClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir a mercadoria?", function () {
        for (var i = 0; i < _cotacao.Mercadorias.list.length; i++) {
            if (_cotacaoMercadoria.Codigo.val() == _cotacao.Mercadorias.list[i].Codigo.val) {
                _cotacao.Mercadorias.list.splice(i, 1);
                break;
            }
        }

        LimparCamposCotacaoMercadoria();
    });
}

////*******MÉTODOS*******

function LoadGridCotacaoMercadoria() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarCotacaoMercadoriaClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Produto", title: "Produto", width: "40%" },
        { data: "Quantidade", title: "Qtd.", width: "10%" },
        { data: "ValorUnitario", title: "Val. Unit.", width: "10%" },
        { data: "ValorTotal", title: "Val. Total", width: "10%" }
    ];

    _gridCotacaoMercadoria = new BasicDataTable(_cotacaoMercadoria.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCotacaoMercadoria();
}

function EditarCotacaoMercadoriaClick(data) {
    for (var i = 0; i < _cotacao.Mercadorias.list.length; i++) {
        if (data.Codigo == _cotacao.Mercadorias.list[i].Codigo.val) {

            var item = _cotacao.Mercadorias.list[i];

            _cotacaoMercadoria.Codigo.val(item.Codigo.val);
            _cotacaoMercadoria.Produto.val(item.Produto.val);
            _cotacaoMercadoria.Produto.codEntity(item.Produto.codEntity);
            _cotacaoMercadoria.Quantidade.val(item.Quantidade.val);
            _cotacaoMercadoria.ValorUnitario.val(item.ValorUnitario.val);
            _cotacaoMercadoria.ValorTotal.val(item.ValorTotal.val);

            _CRUDCotacaoMercadoria.Adicionar.visible(false);
            _CRUDCotacaoMercadoria.Atualizar.visible(true);
            _CRUDCotacaoMercadoria.Excluir.visible(true);

            break;
        }
    }
}

function RecarregarGridCotacaoMercadoria() {
    var data = new Array();

    $.each(_cotacao.Mercadorias.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Produto = item.Produto.val;
        itemGrid.Quantidade = item.Quantidade.val;
        itemGrid.ValorUnitario = item.ValorUnitario.val;
        itemGrid.ValorTotal = item.ValorTotal.val;

        data.push(itemGrid);
    });

    _gridCotacaoMercadoria.CarregarGrid(data);
    CalcularValorTotalMercadorias();
}

function CalcularValorTotal() {
    var quantidade = Globalize.parseFloat(_cotacaoMercadoria.Quantidade.val());
    var valorUnitario = Globalize.parseFloat(_cotacaoMercadoria.ValorUnitario.val());

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;
        _cotacaoMercadoria.ValorTotal.val(Globalize.format(valorTotal, "n2"));
    } else
        _cotacaoMercadoria.ValorTotal.val(Globalize.format(0, "n2"));
}

function CalcularValorTotalMercadorias() {
    var valorTotal = 0;

    $.each(_cotacao.Mercadorias.list, function (i, item) {
        valorTotal += Globalize.parseFloat(item.ValorTotal.val);
    });

    _cotacaoMercadoria.TotalValorMercadorias.val(Globalize.format(valorTotal, "n2"));
}

function LimparCamposCotacaoMercadoria() {
    LimparCampos(_cotacaoMercadoria);
    _CRUDCotacaoMercadoria.Adicionar.visible(true);
    _CRUDCotacaoMercadoria.Atualizar.visible(false);
    _CRUDCotacaoMercadoria.Excluir.visible(false);

    RecarregarGridCotacaoMercadoria();
}