var EnumLocalVinculoHistoricoHelper = function () {
    this.Todos = 0;
    this.Pedido = 1;
    this.Carga = 2;
    this.Planejamento = 3;
    this.FilaCarregamento = 4;
};

EnumLocalVinculoHistoricoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pedido", value: this.Pedido },
            { text: "Carga", value: this.Carga },
            { text: "Planejamento de Pedidos", value: this.Planejamento },
            { text: "Fila de Carregamento", value: this.FilaCarregamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumLocalVinculoHistorico = new EnumLocalVinculoHistoricoHelper();
Object.freeze(EnumLocalVinculoHistorico);