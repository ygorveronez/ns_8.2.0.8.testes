/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="VincularProdutosFornecedorEmbarcadorPorNFe.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Filial.js" />

var _vincularProdutoFornecedor;
var _crudVincularProdutoFornecedor;

var VincularProdutoFornecedor = function () {
    this.Codigo = PropertyEntity({ });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor: ", idBtnSearch: guid(), enable: false });
    this.CodigoInterno = PropertyEntity({ text: "Código interno: ", val: ko.observable(""), def: "", enable: false });
    this.Filiais = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.CodigoProdutoEmbarcador = PropertyEntity({ val: ko.observable(""), def: "" });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto Embarcador:", idBtnSearch: guid(), required: true });
};

var ProdutosNFe = function () {
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor: ", idBtnSearch: guid() });
    this.Produtos = PropertyEntity({ val: ko.observable(new Array()), def: new Array() });
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
};

var CRUDVincularProdutoFornecedor = function () {
    this.Salvar = PropertyEntity({ type: types.event, eventClick: salvarVinculoClick, text: "Salvar", visible: true });
};

function loadVincularProdutoFornecedor() {
    _vincularProdutoFornecedor = new VincularProdutoFornecedor();
    KoBindings(_vincularProdutoFornecedor, "knockoutVincularProdutoFornecedor");

    _produtosNFe = new ProdutosNFe();
    KoBindings(_produtosNFe, "knockoutProdutosNFe");

    _crudVincularProdutoFornecedor = new CRUDVincularProdutoFornecedor();
    KoBindings(_crudVincularProdutoFornecedor, "knockoutCRUDVincularProdutoFornecedor");

    $("#modalVincularProdutoFornecedor")
        .on('hidden.bs.modal', function () { LimparCampos(_vincularProdutoFornecedor); });

    new BuscarProdutos(_vincularProdutoFornecedor.ProdutoEmbarcador, callbackProdutoEmbarcador);
    new BuscarFilial(_vincularProdutoFornecedor.Filiais);

    loadGridProdutos();
}

function callbackProdutoEmbarcador(data) {
    _vincularProdutoFornecedor.ProdutoEmbarcador.codEntity(data.Codigo);
    _vincularProdutoFornecedor.ProdutoEmbarcador.val(data.Descricao);
    _vincularProdutoFornecedor.CodigoProdutoEmbarcador.val(data.CodigoProdutoEmbarcador);
}

function abrirModalVincularProduto(data) {
    LimparCampos(_vincularProdutoFornecedor);
    
    _vincularProdutoFornecedor.Codigo.val(data.Codigo);
    _vincularProdutoFornecedor.Fornecedor.codEntity(_produtosNFe.Fornecedor.codEntity());
    _vincularProdutoFornecedor.Fornecedor.val(_produtosNFe.Fornecedor.val());
    _vincularProdutoFornecedor.CodigoInterno.val(data.CodigoProdutoFornecedor);
    _vincularProdutoFornecedor.ProdutoEmbarcador.codEntity(data.ProdutoEmbarcadorCodigo);
    _vincularProdutoFornecedor.ProdutoEmbarcador.val(data.DescricaoProdutoEmbarcador);
    _vincularProdutoFornecedor.CodigoProdutoEmbarcador.val(data.CodigoProdutoEmbarcador);
    _vincularProdutoFornecedor.Filiais.multiplesEntities(data.Filiais);

    Global.abrirModal("modalVincularProdutoFornecedor");
}

function salvarVinculoClick() {
    if (!ValidarCamposObrigatorios(_vincularProdutoFornecedor)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Necessário informar os campos obrigatórios");
        return;
    }

    dados = RetornarObjetoPesquisa(_vincularProdutoFornecedor);

    executarReST("VincularProdutosFornecedorEmbarcadorPorNFe/VincularProduto", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                Global.fecharModal("modalVincularProdutoFornecedor");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function abrirModalProdutosNota(dados) {
    executarReST("AutorizacaoNaoConformidade/ObterDadosProdutos", { Codigo: dados.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_produtosNFe, r);
                _gridProdutos.CarregarGrid(r.Data.Produtos);
                _produtosNFe.Produtos.val(r.Data.Produtos);
                _produtosNFe.Grid.visible(true);
                Global.abrirModal("divModalProdutosNota");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function loadGridProdutos() {
    var opcaoVincular = { descricao: "Vincular", id: guid(), metodo: vincularProdutoClick, icone: "", visibilidade: visibilidadeVincularProduto };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarProdutoClick, icone: "", visibilidade: visibilidadeEditarProduto };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5, opcoes: [opcaoVincular, opcaoEditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Incluso", visible: false },
        { data: "ProdutoEmbarcadorCodigo", visible: false },
        { data: "Filiais", visible: false },
        { data: "CodigoProdutoFornecedor", title: "Cód. Fornecedor", width: "10%", className: "text-align-left" },
        { data: "DescricaoProdutoFornecedor", title: "Descrição", width: "20%", className: "text-align-left" },
        { data: "Status", title: "Status", width: "8%", className: "text-align-left" },
        { data: "CodigoProdutoEmbarcador", title: "Cód. Embarcador", width: "12%", className: "text-align-left" },
        { data: "DescricaoProdutoEmbarcador", title: "Descrição Embarcador", width: "20%", className: "text-align-left" },
        { data: "DescricoesFiliais", title: "Filial", width: "20%", className: "text-align-left" }
    ];

    _gridProdutos = new BasicDataTable(_produtosNFe.Grid.idGrid, header, menuOpcoes, null, null, 10);
    _gridProdutos.CarregarGrid([]);
}


function visibilidadeVincularProduto(data) {
    return !data.Incluso;
}

function visibilidadeEditarProduto(data) {
    return data.Incluso;
}

function vincularProdutoClick(data) {
    abrirModalVincularProduto(data);
}

function editarProdutoClick(data) {
    abrirModalVincularProduto(data);
}