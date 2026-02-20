var EnumTipoOrdenacaoJanelaCarregamentoHelper = function () {
    this.InicioCarregamento = 0;
    this.Prioridade = 1;
}

EnumTipoOrdenacaoJanelaCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoOrdenacaoJanelaCarregamento.InicioDoCarregamento, value: this.InicioCarregamento },
            { text: Localization.Resources.Enumeradores.TipoOrdenacaoJanelaCarregamento.Prioridade, value: this.Prioridade }
        ];
    }
}

var EnumTipoOrdenacaoJanelaCarregamento = Object.freeze(new EnumTipoOrdenacaoJanelaCarregamentoHelper());