/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="Cotacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cotacaoFornecedor;
var _CRUDCotacaoFornecedor;
var _gridCotacaoFornecedor;

var CotacaoFornecedor = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor: ", idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
};

var CRUDCotacaoFornecedor = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarFornecedorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadCotacaoFornecedor() {
    _cotacaoFornecedor = new CotacaoFornecedor();
    KoBindings(_cotacaoFornecedor, "knockoutCotacaoFornecedor");

    _CRUDCotacaoFornecedor = new CRUDCotacaoFornecedor();
    KoBindings(_CRUDCotacaoFornecedor, "knockoutCRUDCotacaoFornecedor");

    new BuscarClientes(_cotacaoFornecedor.Fornecedor);

    LoadGridCotacaoFornecedor();
}

function AdicionarFornecedorClick() {
    if (!ValidarCamposObrigatorios(_cotacaoFornecedor)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    _cotacaoFornecedor.Codigo.val(guid());
    _cotacao.Fornecedores.list.push(SalvarListEntity(_cotacaoFornecedor));

    LimparCamposCotacaoFornecedor();
}

function ExcluirFornecedorClick(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o fornecedor?", function () {
        for (var i = 0; i < _cotacao.Fornecedores.list.length; i++) {
            if (data.Codigo == _cotacao.Fornecedores.list[i].Codigo.val) {
                _cotacao.Fornecedores.list.splice(i, 1);
                break;
            }
        }

        LimparCamposCotacaoFornecedor();
    });
}

////*******MÉTODOS*******

function LoadGridCotacaoFornecedor() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirFornecedorClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Fornecedor", title: "Fornecedor", width: "80%" }
    ];

    _gridCotacaoFornecedor = new BasicDataTable(_cotacaoFornecedor.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCotacaoFornecedor();
}

function RecarregarGridCotacaoFornecedor() {
    var data = new Array();

    $.each(_cotacao.Fornecedores.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Fornecedor = item.Fornecedor.val;

        data.push(itemGrid);
    });

    _gridCotacaoFornecedor.CarregarGrid(data);
}

function LimparCamposCotacaoFornecedor() {
    LimparCampos(_cotacaoFornecedor);
    _CRUDCotacaoFornecedor.Adicionar.visible(true);

    RecarregarGridCotacaoFornecedor();
}