var EnumHubNonHubHelper = function () {
    this.NonHub = 0;
    this.Hub = 1;
};

EnumHubNonHubHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Non-Hub", value: this.NonHub },
            { text: "Hub", value: this.Hub },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
}

var EnumHubNonHub = Object.freeze(new EnumHubNonHubHelper());