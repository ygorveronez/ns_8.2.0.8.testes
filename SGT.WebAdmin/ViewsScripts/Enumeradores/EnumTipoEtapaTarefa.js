let EnumTipoEtapaTarefaHelper = function () {
    this.Todos = "";
    this.QuebrarRequest = 0;
    this.ProcessarRequest = 1;
    this.RetornarIntegracao = 2;
    this.AdicionaRequestParaProcessamento = 3;
    this.GerarCarregamento = 4;
    this.AdicionarPedido = 5;
    this.GerarCarregamentoComRedespachos = 6;
    this.FecharCarga = 7;
    this.EnviarDigitalizacaoCanhoto = 8;
    this.AdicionarAtendimento = 9;
    this.GerarCarregamentoRoterizacao = 10;
};

EnumTipoEtapaTarefaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Quebrar Request", value: this.QuebrarRequest },
            { text: "Processar Request", value: this.ProcessarRequest },
            { text: "Retornar Integração", value: this.RetornarIntegracao },
            { text: "Adicionar Request para Processamento", value: this.AdicionaRequestParaProcessamento },
            { text: "Gerar Carregamento", value: this.GerarCarregamento },
            { text: "Adicionar Pedido", value: this.AdicionarPedido },
            { text: "Gerar Carregamento com Redespachos", value: this.GerarCarregamentoComRedespachos },
            { text: "Fechar Carga", value: this.FecharCarga },
            { text: "Enviar Digitalização de Canhoto", value: this.EnviarDigitalizacaoCanhoto },
            { text: "Adicionar Atendimento", value: this.AdicionarAtendimento },
            { text: "Gerar Carregamento Roteirização", value: this.GerarCarregamentoRoterizacao },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
};

let EnumTipoEtapaTarefa = Object.freeze(new EnumTipoEtapaTarefaHelper());

