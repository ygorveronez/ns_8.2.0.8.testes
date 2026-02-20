var EnumTipoMetaVendaDiretaHelper = function () {
    this.Todos = 0;
    this.Agendamento = 1;
    this.Servico = 2;
    this.Produto = 3;
    this.Valicacao = 4;
};

EnumTipoMetaVendaDiretaHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Todos: return "Todos";
            case this.Agendamento: return "Agendamento";
            case this.Servico: return "Venda de Serviço";
            case this.Produto: return "Venda de Produto";
            case this.Valicacao: return "Validação";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.Agendamento), value: this.Agendamento },
            { text: this.obterDescricao(this.Servico), value: this.Servico },
            { text: this.obterDescricao(this.Produto), value: this.Produto },
            { text: this.obterDescricao(this.Valicacao), value: this.Valicacao }
        ];
    }
};

var EnumTipoMetaVendaDireta = Object.freeze(new EnumTipoMetaVendaDiretaHelper());