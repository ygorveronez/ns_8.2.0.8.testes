/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _respostaCotacao;
var _pesquisaRespostaCotacao;
var _gridRespostaCotacao;
var _gridProdutos;

var RespostaCotacao = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataPrevisaoEntrega = PropertyEntity({ text: "*Data Previsão Entrega: ", getType: typesKnockout.date, required: true });
    this.CondicaoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Condição de Pagamento:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    this.Produtos = PropertyEntity({ type: types.map, val: GetProdutosToJson, list: new Array() });

    // CRUD
    this.Salvar = PropertyEntity({ eventClick: salvarRetornoClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
};

var ProdutoMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });

    this.Produto = PropertyEntity({ type: types.map, val: "", def: "" });
    this.Marca = PropertyEntity({ type: types.map, val: "", def: "" });
    this.ValorUnitario = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
    this.ValorUnitarioRetorno = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal, configDecimal: { Precision: 4 } });
    this.Quantidade = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
    this.QuantidadeRetorno = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
};

var PesquisaRespostaCotacao = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRespostaCotacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};


//*******EVENTOS*******
function loadRespostaCotacao() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaRespostaCotacao = new PesquisaRespostaCotacao();
    KoBindings(_pesquisaRespostaCotacao, "knockoutPesquisaRespostaCotacao", false, _pesquisaRespostaCotacao.Pesquisar.id);

    // Instancia objeto principal
    _respostaCotacao = new RespostaCotacao();
    KoBindings(_respostaCotacao, "knockoutRespostaCotacao");

    GridProdutos();

    // Inicia busca
    BuscarRespostaCotacao();

    new BuscarCondicaoPagamento(_respostaCotacao.CondicaoPagamento);
}

function salvarRetornoClick(e, sender) {
    Salvar(_respostaCotacao, "RespostaCotacao/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridRespostaCotacao.CarregarGrid();
                LimparCamposRespostaCotacao();
                EsconderTela();
                _pesquisaRespostaCotacao.ExibirFiltros.visibleFade(true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function EditarRespostaCotacaoClick(itemGrid) {
    // Limpa os campos
    LimparCamposRespostaCotacao();

    // Seta o codigo do objeto
    _respostaCotacao.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_respostaCotacao, "RespostaCotacao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaRespostaCotacao.ExibirFiltros.visibleFade(false);

                CarregarProdutos(arg.Data.Produtos);

                ExibirTela();
            } else {
                EsconderTela();
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            EsconderTela();
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarRespostaCotacao() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarRespostaCotacaoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridRespostaCotacao = new GridView(_pesquisaRespostaCotacao.Pesquisar.idGrid, "RespostaCotacao/Pesquisa", _pesquisaRespostaCotacao, menuOpcoes, null);
    _gridRespostaCotacao.CarregarGrid();
}

function EsconderTela() {
    $("#wid-id-4").hide();
}

function ExibirTela() {
    $("#wid-id-4").show();
}

function GridProdutos() {
    var _editableConfig = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.decimal,
        numberMask: ConfigDecimal({ allowZero: true, precision: 4 })
    };
    var _editableConfigString = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.string
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Produto", title: "Produto", width: "35%" },
        { data: "Marca", title: "Marca", width: "35%", editableCell: _editableConfigString },
        { data: "ValorUnitario", title: "Valor Unitário", width: "17%" },
        { data: "ValorUnitarioRetorno", title: "Valor Retorno", width: "17%", editableCell: _editableConfig },
        { data: "Quantidade", title: "Quantidade", width: "17%" },
        { data: "QuantidadeRetorno", title: "Quantidade Retorno", width: "17%", editableCell: _editableConfig }
    ];

    var editarResposta = {
        permite: true,
        callback: SalvarRetornoGrid,
        atualizarRow: true
    };

    _gridProdutos = new BasicDataTable(_respostaCotacao.Produtos.id, header, null, null, null, null, null, null, editarResposta);

    RecarregarGridProdutos();
}

function SalvarRetornoGrid(dataRow) {
    var data = GetProdutos();

    for (var i in data) {
        if (data[i].Codigo.val == dataRow.Codigo) {
            data[i].ValorUnitarioRetorno.val = dataRow.ValorUnitarioRetorno;
            data[i].QuantidadeRetorno.val = dataRow.QuantidadeRetorno;
            data[i].Marca.val = dataRow.Marca;
            break;
        }
    }

    SetProdutos(data);
}

function CarregarProdutos(produtos) {
    var data = produtos.map(function (produto) {
        var koProduto = new ProdutoMap;

        koProduto.Codigo.val = produto.Codigo;
        koProduto.Produto.val = produto.Produto;
        koProduto.ValorUnitario.val = produto.ValorUnitario;
        koProduto.ValorUnitarioRetorno.val = produto.ValorUnitarioRetorno;
        koProduto.Quantidade.val = produto.Quantidade;
        koProduto.QuantidadeRetorno.val = produto.QuantidadeRetorno;
        koProduto.Marca.val = produto.Marca;

        return koProduto;
    });

    SetProdutos(data);
    RecarregarGridProdutos();
}

function RecarregarGridProdutos() {
    var data = GetProdutosToObj();
    _gridProdutos.CarregarGrid(data);
}

function GetProdutos() {
    return _respostaCotacao.Produtos.list.slice();
}

function GetProdutosToJson() {
    var data = GetProdutosToObj();
    return JSON.stringify(data);
}

function GetProdutosToObj() {
    var data = [];

    $.each(GetProdutos(), function (i, produto) {
        var itemGrid = {};

        itemGrid.DT_Enable = true;
        itemGrid.Codigo = produto.Codigo.val;
        itemGrid.Produto = produto.Produto.val;
        itemGrid.ValorUnitario = produto.ValorUnitario.val;
        itemGrid.ValorUnitarioRetorno = produto.ValorUnitarioRetorno.val;
        itemGrid.Quantidade = produto.Quantidade.val;
        itemGrid.QuantidadeRetorno = produto.QuantidadeRetorno.val;
        itemGrid.Marca = produto.Marca.val;

        data.push(itemGrid);
    });

    return data;
}

function SetProdutos(data) {
    return _respostaCotacao.Produtos.list = data.slice();
}

function LimparCamposRespostaCotacao() {
    LimparCampos(_respostaCotacao);
}