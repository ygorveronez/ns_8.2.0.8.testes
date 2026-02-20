var EnumCondicaoAutorizacaoHelper = function (isEntidade) {
    this.IgualA = 1;
    this.DiferenteDe = 2;
    this.MaiorQue = 3;
    this.MaiorIgualQue = 4;
    this.MenorQue = 5;
    this.MenorIgualQue = 6;
    this._isEntidade = isEntidade;
}

EnumCondicaoAutorizacaoHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.IgualA: return "Igual a (==)";
            case this.DiferenteDe: return "Diferente de (!=)";
            case this.MaiorIgualQue: return "Maior ou igual que (>=)";
            case this.MaiorQue: return "Maior que (>)";
            case this.MenorIgualQue: return "Menor ou igual que (<=)";
            case this.MenorQue: return "Menor que (<)";
            default: return "";
        }
    },
    obterOpcoes: function () {
        if (this._isEntidade) {
            return [
                { text: this.obterDescricao(this.DiferenteDe), value: this.DiferenteDe },
                { text: this.obterDescricao(this.IgualA), value: this.IgualA }
            ];
        }

        return [
            { text: this.obterDescricao(this.DiferenteDe), value: this.DiferenteDe },
            { text: this.obterDescricao(this.IgualA), value: this.IgualA },
            { text: this.obterDescricao(this.MaiorIgualQue), value: this.MaiorIgualQue },
            { text: this.obterDescricao(this.MaiorQue), value: this.MaiorQue },
            { text: this.obterDescricao(this.MenorIgualQue), value: this.MenorIgualQue },
            { text: this.obterDescricao(this.MenorQue), value: this.MenorQue }
        ];
    }
}

var EnumCondicaoAutorizacao = Object.freeze(new EnumCondicaoAutorizacaoHelper(false));
var EnumCondicaoAutorizacaoEntidade = Object.freeze(new EnumCondicaoAutorizacaoHelper(true));