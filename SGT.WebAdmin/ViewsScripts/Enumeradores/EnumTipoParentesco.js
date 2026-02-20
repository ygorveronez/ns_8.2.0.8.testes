var EnumTipoParentescoHelper = function () {
    this.Nenhum = 0;
    this.Outro = 1;
    this.Pai = 2;
    this.Mae = 3;
    this.Filhos = 4;
    this.Irmao = 5;
    this.Avo = 6;
    this.Neto = 7;
    this.Tio = 8;
    this.Sobrinho = 9;
    this.Bisavo = 10;
    this.Bisneto = 11;
    this.Primo = 12;
    this.Trisavo = 13;
    this.Trineto = 14;
    this.TioAvo = 15;
    this.SobrinhoNeto = 16;
    this.Esposo = 17;
};

EnumTipoParentescoHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Nenhum: return Localization.Resources.Enumeradores.TipoParentesco.Nenhum;
            case this.Outro: return Localization.Resources.Enumeradores.TipoParentesco.Outro;
            case this.Pai: return Localization.Resources.Enumeradores.TipoParentesco.Pai;
            case this.Mae: return Localization.Resources.Enumeradores.TipoParentesco.Mae;
            case this.Filhos: return Localization.Resources.Enumeradores.TipoParentesco.Filhos;
            case this.Irmao: return Localization.Resources.Enumeradores.TipoParentesco.Irmao;
            case this.Avo: return Localization.Resources.Enumeradores.TipoParentesco.Avo;
            case this.Neto: return Localization.Resources.Enumeradores.TipoParentesco.Neto;
            case this.Tio: return Localization.Resources.Enumeradores.TipoParentesco.Tio;
            case this.Sobrinho: return Localization.Resources.Enumeradores.TipoParentesco.Sobrinho;
            case this.Bisavo: return Localization.Resources.Enumeradores.TipoParentesco.Bisavo;
            case this.Bisneto: return Localization.Resources.Enumeradores.TipoParentesco.Bisneto;
            case this.Primo: return Localization.Resources.Enumeradores.TipoParentesco.Primo;
            case this.Trisavo: return Localization.Resources.Enumeradores.TipoParentesco.Trisavo;
            case this.Trineto: return Localization.Resources.Enumeradores.TipoParentesco.Trineto;
            case this.TioAvo: return Localization.Resources.Enumeradores.TipoParentesco.TioAvo;
            case this.SobrinhoNeto: return Localization.Resources.Enumeradores.TipoParentesco.SobrinhoNeto;
            case this.Esposo: return Localization.Resources.Enumeradores.TipoParentesco.Esposo;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoParentesco.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Outro, value: this.Outro },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Pai, value: this.Pai },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Mae, value: this.Mae },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Filhos, value: this.Filhos },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Irmao, value: this.Irmao },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Avo, value: this.Avo },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Neto, value: this.Neto },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Tio, value: this.Tio },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Sobrinho, value: this.Sobrinho },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Bisavo, value: this.Bisavo },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Bisneto, value: this.Bisneto },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Primo, value: this.Primo },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Trisavo, value: this.Trisavo },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Trineto, value: this.Trineto },
            { text: Localization.Resources.Enumeradores.TipoParentesco.TioAvo, value: this.TioAvo },
            { text: Localization.Resources.Enumeradores.TipoParentesco.SobrinhoNeto, value: this.SobrinhoNeto },
            { text: Localization.Resources.Enumeradores.TipoParentesco.Esposo, value: this.Esposo }
        ];
    }
};

var EnumTipoParentesco = Object.freeze(new EnumTipoParentescoHelper());