var EnumTipoCargaJanelaCarregamentoHelper = function () {
    this.Todos = "";
    this.Carregamento = 0;
    this.Descarregamento = 1;
};

EnumTipoCargaJanelaCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCargaJanelaCarregamento.Carregamento, value: this.Carregamento },
            { text: Localization.Resources.Enumeradores.TipoCargaJanelaCarregamento.Descarregamento, value: this.Descarregamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoCargaJanelaCarregamento.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCargaJanelaCarregamento = Object.freeze(new EnumTipoCargaJanelaCarregamentoHelper());
