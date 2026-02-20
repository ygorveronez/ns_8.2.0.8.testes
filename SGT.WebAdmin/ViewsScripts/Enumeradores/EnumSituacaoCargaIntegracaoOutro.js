var EnumSituacaoCargaIntegracaoOutrosHelper = function () {
    this.Todos = -1;
    this.AgIntegracao = 0;
    this.Integrado =1
    this.ProblemaIntegracao = 2;
    this.AgRetorno = 3;
};

EnumSituacaoCargaIntegracaoOutrosHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { value: this.AgIntegracao, text: "Ag.Integração" },
            { value: this.Integrado, text: "Integrado" },
            { value: this.ProblemaIntegracao, text: "Problema ao Integrar" },
            { value: this.AgRetorno, text: "Ag.Retorno" },
           
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ value: this.Todos, text: "Todos" }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoCargaIntegracaoOutros = Object.freeze(new EnumSituacaoCargaIntegracaoOutrosHelper());