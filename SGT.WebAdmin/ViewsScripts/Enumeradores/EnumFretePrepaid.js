var EnumFretePrepaidHelper = function () {
    this.Todos = -1
    this.Collect = 1;
    this.Prepaid = 2;
    this.PrepaidAbroad = 3;
};

EnumFretePrepaidHelper.prototype.ObterOpcoes = function () {
    return [
        { text: "Collect", value: this.Collect },
        { text: "Prepaid", value: this.Prepaid },
        { text: "Prepaid Abroad", value: this.PrepaidAbroad }
    ];
};

EnumFretePrepaidHelper.prototype.ObterOpcoesPesquisa = function () {
    return [{ text: "Todos", value: -1 }].concat(this.ObterOpcoes());
};

var EnumFretePrepaid = Object.freeze(new EnumFretePrepaidHelper());