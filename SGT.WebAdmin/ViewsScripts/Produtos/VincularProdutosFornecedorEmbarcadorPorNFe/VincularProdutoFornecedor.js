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

var CRUDVincularProdutoFornecedor = function () {
    this.Salvar = PropertyEntity({ type: types.event, eventClick: salvarVinculoClick, text: "Salvar", visible: true });
};

function loadVincularProdutoFornecedor() {
    _vincularProdutoFornecedor = new VincularProdutoFornecedor();
    KoBindings(_vincularProdutoFornecedor, "knockoutVincularProdutoFornecedor");

    _crudVincularProdutoFornecedor = new CRUDVincularProdutoFornecedor();
    KoBindings(_crudVincularProdutoFornecedor, "knockoutCRUDVincularProdutoFornecedor");

    $("#modalVincularProdutoFornecedor")
        .on('hidden.bs.modal', function () { LimparCampos(_vincularProdutoFornecedor); });

    new BuscarProdutos(_vincularProdutoFornecedor.ProdutoEmbarcador, callbackProdutoEmbarcador);
    new BuscarFilial(_vincularProdutoFornecedor.Filiais);
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
                atualizarProdutoVinculado();
                Global.fecharModal("modalVincularProdutoFornecedor");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function atualizarProdutoVinculado() {
    _produtosNFe.Produtos.val().forEach(produto => {
        if (produto.Codigo == _vincularProdutoFornecedor.Codigo.val()) {
            produto.ProdutoEmbarcadorCodigo = _vincularProdutoFornecedor.ProdutoEmbarcador.codEntity();
            produto.DescricaoProdutoEmbarcador = _vincularProdutoFornecedor.ProdutoEmbarcador.val();
            produto.CodigoProdutoEmbarcador = _vincularProdutoFornecedor.CodigoProdutoEmbarcador.val();
            produto.Filiais = _vincularProdutoFornecedor.Filiais.multiplesEntities();
            produto.DescricoesFiliais = _vincularProdutoFornecedor.Filiais.multiplesEntities().map(x => x.Descricao);
            produto.Incluso = true;
            produto.Status = "Já cadastrado";
        }
    });

    _gridProdutos.CarregarGrid(_produtosNFe.Produtos.val());
}