var EnumUsoMaterialHelper = function () {
    this.Todos = "";
    this.Revenda = 0;
    this.ContinuacaoProcessamento = 1;
    this.Consumo = 2;
    this.Imobilizado = 3;
};

EnumUsoMaterialHelper.prototype = {
    obterDescricao: function (codigoUsoMaterial) {
        switch (codigoUsoMaterial) {
            case this.Revenda: return "Revenda";
            case this.ContinuacaoProcessamento: return "Continuação do Processamento";
            case this.Consumo: return "Consumo";
            case this.Imobilizado: return "Imobilizado";
            default: return ""
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.Revenda), value: this.Revenda },
            { text: this.obterDescricao(this.ContinuacaoProcessamento), value: this.ContinuacaoProcessamento },
            { text: this.obterDescricao(this.Consumo), value: this.Consumo },
            { text: this.obterDescricao(this.Imobilizado), value: this.Imobilizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumUsoMaterial = Object.freeze(new EnumUsoMaterialHelper());
