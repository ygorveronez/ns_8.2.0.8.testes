var EnumRegiaoHelper = function () {
    this.NaoSelecionado = "";
    this.Norte = 1;
    this.Nordeste = 2;
    this.Sul = 3;
    this.Sudeste = 4;
    this.CentroOeste = 5;
};

EnumRegiaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Norte", value: this.Norte },
            { text: "Nordeste", value: this.Nordeste },
            { text: "Sul", value: this.Sul },
            { text: "Sudeste", value: this.Sudeste },
            { text: "Centro Oeste", value: this.CentroOeste },
        ];
    },
    obterOpcoesPorParametro: function (estadoNorte, estadoSul, estadoNordeste, estadoCentroeste, estadoSudeste) {
        var listaEnum = [];
        if (estadoNorte)
            listaEnum.push(PropertyEntity({ text: "Norte", value: this.Norte }));

        if (estadoSul)
            listaEnum.push(PropertyEntity({ text: "Sul", value: this.Sul }));

        if (estadoNordeste)
            listaEnum.push(PropertyEntity({ text: "Nordeste", value: this.Nordeste }));

        if (estadoCentroeste)
            listaEnum.push(PropertyEntity({ text: "Centro Oeste", value: this.CentroOeste }));

        if (estadoSudeste)
            listaEnum.push(PropertyEntity({ text: "Sudeste", value: this.Sudeste }));

        return listaEnum;
    }
};

var EnumRegiao = Object.freeze(new EnumRegiaoHelper());