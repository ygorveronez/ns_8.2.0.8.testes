var EnumModalidadePessoaHelper = function () {
    this.Todas = "";
    this.Cliente = 1;
    this.Fornecedor = 2;
    this.TransportadorTerceiro = 3;
};

EnumModalidadePessoaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ModalidadePessoa.Cliente, value: this.Cliente },
            { text: Localization.Resources.Enumeradores.ModalidadePessoa.Fornecedor, value: this.Fornecedor },
            { text: Localization.Resources.Enumeradores.ModalidadePessoa.TransportadorTerceiro, value: this.TransportadorTerceiro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.ModalidadePessoa.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumModalidadePessoa = Object.freeze(new EnumModalidadePessoaHelper());