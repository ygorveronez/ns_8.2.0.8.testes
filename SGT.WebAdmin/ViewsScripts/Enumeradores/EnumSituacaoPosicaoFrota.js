var EnumSituacaoPosicaoFrotaHelper = function () {
    this.Ambas = 0;
    this.EmViagem = 1;
    this.SemViagem = 2;
}

EnumSituacaoPosicaoFrotaHelper.prototype = {
    obterTodos: function () {
        return [
            this.EmViagem,
            this.SemViagem
        ];
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoPosicaoFrota.EmViagem, value: this.EmViagem },
            { text: Localization.Resources.Enumeradores.SituacaoPosicaoFrota.SemViagem, value: this.SemViagem }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPosicaoFrota.Ambas, value: this.Ambas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoPosicaoFrota = Object.freeze(new EnumSituacaoPosicaoFrotaHelper());





