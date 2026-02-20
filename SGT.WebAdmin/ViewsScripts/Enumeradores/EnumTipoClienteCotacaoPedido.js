var EnumTipoClienteCotacaoPedidoHelper = function () {
    this.Todos = 0;
    this.ClienteNovo = 1;
    this.ClienteProspect = 2;
    this.ClienteAtivo = 3;
    this.ClienteInativo = 4;
    this.GrupoPessoa = 5;
};

EnumTipoClienteCotacaoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [            
            { text: "Cliente Novo", value: this.ClienteNovo },
            { text: "Cliente Prospect", value: this.ClienteProspect },
            { text: "Cliente Ativo", value: this.ClienteAtivo },
            { text: "Cliente Inativo", value: this.ClienteInativo },
            { text: "Grupo de Pessoa", value: this.GrupoPessoa }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Cliente Novo", value: this.ClienteNovo },
            { text: "Cliente Prospect", value: this.ClienteProspect },
            { text: "Cliente Ativo", value: this.ClienteAtivo },
            { text: "Cliente Inativo", value: this.ClienteInativo },
            { text: "Grupo de Pessoa", value: this.GrupoPessoa }
        ];
    }
};

var EnumTipoClienteCotacaoPedido = Object.freeze(new EnumTipoClienteCotacaoPedidoHelper());