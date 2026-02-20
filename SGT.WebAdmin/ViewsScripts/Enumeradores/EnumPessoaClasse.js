var EnumPessoaClasseHelper = function () {
    this.Todas = 0;
    this.Um = 1;
    this.Dois = 2;
    this.Tres = 3;
    this.Quatro = 4;
    this.Cinco = 5;
};

EnumPessoaClasseHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.PessoaClasse.Classe1, value: this.Um },
            { text: Localization.Resources.Enumeradores.PessoaClasse.Classe2, value: this.Dois },
            { text: Localization.Resources.Enumeradores.PessoaClasse.Classe3, value: this.Tres },
            { text: Localization.Resources.Enumeradores.PessoaClasse.Classe4, value: this.Quatro },
            { text: Localization.Resources.Enumeradores.PessoaClasse.Classe5, value: this.Cinco }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.PessoaClasse.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumPessoaClasse = Object.freeze(new EnumPessoaClasseHelper());