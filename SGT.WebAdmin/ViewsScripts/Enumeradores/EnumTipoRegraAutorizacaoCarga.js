var EnumTipoRegraAutorizacaoCargaHelper = function () {
    this.InformadoManualmente = 0;
    this.TabelaFrete = 1;
    this.Ambos = 2;
    this.Outros = 3;    
};

EnumTipoRegraAutorizacaoCargaHelper.prototype.ObterOpcoes = function () {
    return [
        { text: "Valor informado manualmente", value: this.InformadoManualmente },
        { text: "Pela tabela de Frete", value: this.TabelaFrete },
        { text: "Ambos", value: this.Ambos },
        { text: "Outros", value: this.Outros }
    ];
};

EnumTipoRegraAutorizacaoCargaHelper.prototype.ObterOpcoesPesquisa = function () {
    return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
};

var EnumTipoRegraAutorizacaoCarga = Object.freeze(new EnumTipoRegraAutorizacaoCargaHelper());