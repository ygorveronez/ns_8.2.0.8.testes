/// <reference path="../../Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _fornecedorProduto, _gridFornecedorProduto, _pesquisaFornecedorProduto;

var FornecedorProduto = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.CodigoProduto = PropertyEntity({ text: ko.observable("*Código do Produto no Fornecedor:"), required: true, visible: ko.observable(true) });
    this.FatorConversao = PropertyEntity({ def: "0,00000", val: ko.observable("0,00000"), text: "Fator de Conversão: ", required: false, getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 5, allowZero: true, allowNegative: true }, issue: 1047 });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarFornecedorProdutoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarFornecedorProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirFornecedorProdutoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposFornecedorProduto, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaFornecedorProduto = function () {
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            RecarregarGridFornecedorProduto();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};


//*******EVENTOS*******

function LoadFornecedorProduto() {

    _fornecedorProduto = new FornecedorProduto();
    KoBindings(_fornecedorProduto, "knockoutFornecedorProduto");

    _pesquisaFornecedorProduto = new PesquisaFornecedorProduto();
    KoBindings(_pesquisaFornecedorProduto, "knockoutPesquisaFornecedorProduto");

    new BuscarClientes(_fornecedorProduto.Fornecedor, null, true, [EnumModalidadePessoa.Fornecedor]);
    new BuscarClientes(_pesquisaFornecedorProduto.Fornecedor, null, true, null);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarFornecedorProdutoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoProduto", title: "Código do Produto", width: "15%" },
        { data: "FatorConversao", title: "Fat. Conversão", width: "15%" },
        { data: "Fornecedor", title: "Fornecedor", width: "55%" }
    ];

    _gridFornecedorProduto = new BasicDataTable(_fornecedorProduto.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    RecarregarGridFornecedorProduto();
}

function RecarregarGridFornecedorProduto() {
    var data = new Array();

    var listaFiltrada = _produto.Fornecedores.list;
    if (_pesquisaFornecedorProduto.Fornecedor.codEntity() > 0) {
        listaFiltrada = listaFiltrada.filter(function (arg) {
            return arg.Fornecedor.codEntity.includes(_pesquisaFornecedorProduto.Fornecedor.codEntity());
        });
    }

    $.each(listaFiltrada, function (i, fornecedor) {
        var fornecedorGrid = new Object();

        fornecedorGrid.Codigo = fornecedor.Codigo.val;
        fornecedorGrid.CodigoProduto = fornecedor.CodigoProduto.val;
        fornecedorGrid.FatorConversao = fornecedor.FatorConversao.val;
        fornecedorGrid.Fornecedor = fornecedor.Fornecedor.val;

        data.push(fornecedorGrid);
    });

    _gridFornecedorProduto.CarregarGrid(data);
}

function EditarFornecedorProdutoClick(data) {
    for (var i = 0; i < _produto.Fornecedores.list.length; i++) {
        if (data.Codigo == _produto.Fornecedores.list[i].Codigo.val) {
            var fornecedor = _produto.Fornecedores.list[i];

            _fornecedorProduto.Codigo.val(fornecedor.Codigo.val);
            _fornecedorProduto.CodigoProduto.val(fornecedor.CodigoProduto.val);
            _fornecedorProduto.FatorConversao.val(fornecedor.FatorConversao.val);
            _fornecedorProduto.Fornecedor.val(fornecedor.Fornecedor.val);
            _fornecedorProduto.Fornecedor.codEntity(fornecedor.Fornecedor.codEntity);

            _fornecedorProduto.Adicionar.visible(false);
            _fornecedorProduto.Atualizar.visible(true);
            _fornecedorProduto.Excluir.visible(true);
            _fornecedorProduto.Cancelar.visible(true);
        }
    }
}

function ExcluirFornecedorProdutoClick() {
    for (var i = 0; i < _produto.Fornecedores.list.length; i++) {
        if (_fornecedorProduto.Codigo.val() == _produto.Fornecedores.list[i].Codigo.val) {
            _produto.Fornecedores.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridFornecedorProduto();

    LimparCamposFornecedorProduto();
}

function AdicionarFornecedorProdutoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_fornecedorProduto);

    if (valido) {
        _fornecedorProduto.Codigo.val(guid());

        _produto.Fornecedores.list.push(SalvarListEntity(_fornecedorProduto));

        RecarregarGridFornecedorProduto();

        LimparCamposFornecedorProduto();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function AtualizarFornecedorProdutoClick() {
    var valido = ValidarCamposObrigatorios(_fornecedorProduto);

    if (valido) {

        for (var i = 0; i < _produto.Fornecedores.list.length; i++) {
            if (_fornecedorProduto.Codigo.val() == _produto.Fornecedores.list[i].Codigo.val) {
                _produto.Fornecedores.list[i] = SalvarListEntity(_fornecedorProduto);
                break;
            }
        }

        RecarregarGridFornecedorProduto();

        LimparCamposFornecedorProduto();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposFornecedorProduto() {
    LimparCampos(_fornecedorProduto);
    _fornecedorProduto.Adicionar.visible(true);
    _fornecedorProduto.Atualizar.visible(false);
    _fornecedorProduto.Excluir.visible(false);
    _fornecedorProduto.Cancelar.visible(false);
}

function LimparCamposPesquisaFornecedorProduto() {
    LimparCampos(_pesquisaFornecedorProduto);
    _pesquisaFornecedorProduto.ExibirFiltros.visibleFade(false);
}