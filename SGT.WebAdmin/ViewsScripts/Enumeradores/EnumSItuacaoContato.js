var EnumSituacaoContatoHelper = function () {
    this.Ativo = true;
    this.Inativo = false;
};

EnumSituacaoContatoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoContato.Ativo, value: this.Ativo },
            { text: Localization.Resources.Enumeradores.SituacaoContato.Inativo, value: this.Inativo },
        ];
    },
}

var EnumSituacaoContato = Object.freeze(new EnumSituacaoContatoHelper());