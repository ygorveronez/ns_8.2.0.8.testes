var EnumSituacaoIntegracaoPedidoAguardandoIntegracaoHelper = function () {
    this.AgIntegracao = 0;
    this.Integrado = 1;
    this.ProblemaIntegracao = 2;
    this.AgRetorno = 3;
    this.AgGerarCarga = 4;
    this.ProblemaGerarCarga = 5;
    this.ErroGenerico = 6;
};

EnumSituacaoIntegracaoPedidoAguardandoIntegracaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Integração", value: this.AgIntegracao },
            { text: "Aguardando Retorno", value: this.AgRetorno },
            { text: "Integrado", value: this.Integrado },
            { text: "Falha ao Integrar", value: this.ProblemaIntegracao },
            { text: "Aguardando Gerar Carga", value: this.AgGerarCarga },
            { text: "Problema Geração de Carga", value: this.ProblemaGerarCarga },
            { text: "Erro Genérico", value: this.ErroGenerico },
        ];
    }
};

var EnumSituacaoIntegracaoPedidoAguardandoIntegracao = Object.freeze(new EnumSituacaoIntegracaoPedidoAguardandoIntegracaoHelper());