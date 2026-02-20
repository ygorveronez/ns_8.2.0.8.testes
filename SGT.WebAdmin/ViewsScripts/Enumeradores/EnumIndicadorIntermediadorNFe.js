var EnumIndicadorIntermediadorNFeHelper = function () {
    this.Todos = "";
    this.SemIntermediador = "0";
    this.SitePlataformaTerceiros = "1";
};

EnumIndicadorIntermediadorNFeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem Intermediador", value: this.SemIntermediador },
            { text: "Em Site ou Plataforma de Terceiros", value: this.SitePlataformaTerceiros }
        ];
    },
    obterOpcoesCadastro: function () {
        return [{ text: "Não Definido", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumIndicadorIntermediadorNFe = Object.freeze(new EnumIndicadorIntermediadorNFeHelper());