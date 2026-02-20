var SaldoArmazemProduto = function (idKnout) {

    var instancia = this;
    var _gridDetalhesProdutoArmazem;

    this.CodigoProdutoEmbarcador = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoFilial = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Grid = PropertyEntity({ id: guid() });


    this.LoadSaldoArmazemProduto = function (produtoSelecionado) {
        $.get("Content/Static/Filiais/SaldoArmazemProduto.html?dyn=" + guid(), function (html) {
            $(idKnout).html(html);

            KoBindings(instancia, "knoutSaldoArmazemProduto");

            instancia.CodigoProdutoEmbarcador.val(produtoSelecionado.CodigoProduto);
            instancia.CodigoFilial.val(produtoSelecionado.CodigoFilial);

            _gridDetalhesProdutoArmazem = new GridView(instancia.Grid.id, "GestaoArmazem/Pesquisa", instancia, null, null, 5);

            _gridDetalhesProdutoArmazem.CarregarGrid();

            Global.abrirModal("modalSaldoArmazemProduto");
        });
    };
};