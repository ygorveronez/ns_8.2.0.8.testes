var EnumTipoOrdemServicoVendaHelper = function () {
    this.Todos = "";
    this.OrdemServicoInterna = 1;
    this.CentroDeCusto = 2;
};

EnumTipoOrdemServicoVendaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "OI - Ordem Serviço Interna", value: this.OrdemServicoInterna },
            { text: "CC - Centro de Custo", value: this.CentroDeCusto }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesSelecione: function () {
        return [{ text: "Selecione", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoOrdemServicoVenda = Object.freeze(new EnumTipoOrdemServicoVendaHelper());