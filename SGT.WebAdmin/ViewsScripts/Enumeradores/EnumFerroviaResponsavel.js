var EnumFerroviaResponsavelHelper = function () {
    this.Todos = "";
    this.FerroviaOrigem = 1;
    this.FerroviaDestino = 2;
};

EnumFerroviaResponsavelHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.FerroviaResponsavel.FerroviaDeOrigem, value: this.FerroviaOrigem },
            { text: Localization.Resources.Enumeradores.FerroviaResponsavel.FerroviaDeDestino, value: this.FerroviaDestino }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.FerroviaResponsavel.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFerroviaResponsavel = Object.freeze(new EnumFerroviaResponsavelHelper());