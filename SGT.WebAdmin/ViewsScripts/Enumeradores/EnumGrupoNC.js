var EnumGrupoNCHelper = function () {
    this.Todos = null;
    this.NaoSelecionado = 0;
    this.Transporte = 1;
    this.NotaFiscal = 2;
    this.Fiscal = 3;
    this.Importacao = 4;
    this.Pedido = 5;
    this.Materiais = 6;
};

EnumGrupoNCHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Transporte", value: this.Transporte },
            { text: "NF-e", value: this.NotaFiscal },
            { text: "Fiscal", value: this.Fiscal },
            { text: "Importação", value: this.Importacao },
            { text: "Pedido", value: this.Pedido },
            { text: "Materiais", value: this.Materiais }
        ];
    },
    obterOpcoesNaoSelecionado: function () {
        return [
            { text: "Transporte", value: this.Transporte },
            { text: "NF-e", value: this.NotaFiscal },
            { text: "Fiscal", value: this.Fiscal },
            { text: "Importação", value: this.Importacao },
            { text: "Pedido", value: this.Pedido },
            { text: "Materiais", value: this.Materiais },
            { text: "Nenhum", value: this.NaoSelecionado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumGrupoNC = Object.freeze(new EnumGrupoNCHelper());