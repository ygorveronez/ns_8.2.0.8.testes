var EnumIndicadorPresencaNFeHelper = function () {
    this.Todos = "";
    this.NaoSeAplica = 0;
    this.Presencial = 1;
    this.Internet = 2;
    this.Teleatendimento = 3;
    this.NFCe = 4;
    this.PresencialForaEmpresa = 5;
    this.Outros = 9;
};

EnumIndicadorPresencaNFeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não se aplica", value: this.NaoSeAplica },
            { text: "Operação presencial", value: this.Presencial },
            { text: "Operação não presencial, pela Internet", value: this.Internet },
            { text: "Operação não presencial, Teleatendimento", value: this.Teleatendimento },
            { text: "NFC-e em operação com entrega a domicílio", value: this.NFCe },
            { text: "Operação presencial, fora do estabelecimento", value: this.PresencialForaEmpresa },
            { text: "Operação não presencial, outros", value: this.Outros }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumIndicadorPresencaNFe = Object.freeze(new EnumIndicadorPresencaNFeHelper());