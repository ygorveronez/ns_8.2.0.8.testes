var EnumCategoriaProdutoHelper = function () {
    this.Todos = "";
    this.MercadoriaRevenda = 0;
    this.MateriaPrima = 1;
    this.Embalagem = 2;
    this.ProdutoEmProcesso = 3;
    this.ProdutoAcabado = 4;
    this.Subproduto = 5;
    this.ProdutoIntermediario = 6;
    this.MaterialUsoConsumo = 7;
    this.AtivoImobilizado = 8;
    this.Servicos = 9;
    this.OutrosInsumos = 10;
    this.Outras = 99;
};

EnumCategoriaProdutoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Mercadoria para Revenda", value: this.MercadoriaRevenda },
            { text: "Matéria-Prima", value: this.MateriaPrima },
            { text: "Embalagem", value: this.Embalagem },
            { text: "Produto em Processo", value: this.ProdutoEmProcesso },
            { text: "Produto Acabado", value: this.ProdutoAcabado },
            { text: "Subproduto", value: this.Subproduto },
            { text: "Produto Intermediário", value: this.ProdutoIntermediario },
            { text: "Material de Uso e Consumo", value: this.MaterialUsoConsumo },
            { text: "Ativo Imobilizado", value: this.AtivoImobilizado },
            { text: "Serviços", value: this.Servicos },
            { text: "Outros Insumos", value: this.OutrosInsumos },
            { text: "Outros", value: this.Outras }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesComNumero: function () {
        return [
            { text: "00 - Mercadoria para Revenda", value: this.MercadoriaRevenda },
            { text: "01 - Matéria-Prima", value: this.MateriaPrima },
            { text: "02 - Embalagem", value: this.Embalagem },
            { text: "03 - Produto em Processo", value: this.ProdutoEmProcesso },
            { text: "04 - Produto Acabado", value: this.ProdutoAcabado },
            { text: "05 - Subproduto", value: this.Subproduto },
            { text: "06 - Produto Intermediário", value: this.ProdutoIntermediario },
            { text: "07 - Material de Uso e Consumo", value: this.MaterialUsoConsumo },
            { text: "08 - Ativo Imobilizado", value: this.AtivoImobilizado },
            { text: "09 - Serviços", value: this.Servicos },
            { text: "10 - Outros Insumos", value: this.OutrosInsumos },
            { text: "99 - Outros", value: this.Outras }
        ];
    }
};

var EnumCategoriaProduto = Object.freeze(new EnumCategoriaProdutoHelper());